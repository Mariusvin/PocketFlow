from typing import Any, Dict, List, Optional, Tuple
import os
import httpx
from pocketflow import Node
from utils.call_llm import call_llm


class BuildSearchQuery(Node):
    """Turns a natural language prompt into a set of GitHub code/search queries.

    Shared inputs:
      - shared["nl_query"]: str

    Writes:
      - shared["gh_queries"]: List[str]
    """

    def prep(self, shared: Dict[str, Any]):
        return shared.get("nl_query", "")

    def exec(self, nl_query: str) -> List[str]:
        if not nl_query:
            return []
        prompt = f"""
Convert this user request into up to 3 precise GitHub search queries (for the code search bar).
Keep them short and include key frameworks, languages, and minimal keywords.
Return as one query per line, no numbering.

User request: {nl_query}
"""
        text = call_llm(prompt)
        lines = [l.strip() for l in text.splitlines() if l.strip()]
        # Heuristic fallback if model returns paragraph
        if len(lines) == 1 and " " in lines[0] and len(lines[0]) > 120:
            lines = [nl_query]
        return lines[:3]

    def post(self, shared: Dict[str, Any], prep_res: str, exec_res: List[str]):
        shared["gh_queries"] = exec_res


class SearchGitHub(Node):
    """Calls GitHub search API for the queries, merges and normalizes results.

    Inputs in shared:
      - gh_queries: List[str]
      - filters: Dict (optional) with language, min_stars, max_stars, updated_after, license

    Writes:
      - shared["results_raw"]: List[Dict]
    """

    def prep(self, shared: Dict[str, Any]):
        queries = shared.get("gh_queries", [])
        filters = shared.get("filters", {})
        return queries, filters

    def exec(self, inputs: Tuple[List[str], Dict[str, Any]]) -> List[Dict[str, Any]]:
        queries, filters = inputs
        if not queries:
            return []
        
        token = os.getenv("GITHUB_TOKEN", "")
        headers = {
            "Accept": "application/vnd.github+json",
            "User-Agent": "PocketFlow-AI-GitHub-Search",
        }
        if token:
            headers["Authorization"] = f"Bearer {token}"

        results: List[Dict[str, Any]] = []
        
        # Build base search parameters
        base_params = {
            "sort": "stars",
            "order": "desc",
            "per_page": 30,  # Increased for better filtering
        }
        
        # Apply filters
        if filters.get("language"):
            base_params["language"] = filters["language"]
        
        if filters.get("min_stars"):
            base_params["stars"] = f">={filters['min_stars']}"
        
        if filters.get("max_stars"):
            if "stars" in base_params:
                base_params["stars"] = f"{filters['min_stars']}..{filters['max_stars']}"
            else:
                base_params["stars"] = f"<={filters['max_stars']}"
        
        if filters.get("updated_after"):
            base_params["pushed"] = f">{filters['updated_after']}"
        
        if filters.get("license"):
            base_params["license"] = filters["license"]

        with httpx.Client(timeout=20) as client:
            for q in queries:
                # Combine user query with filters
                search_query = q
                if filters.get("language"):
                    search_query += f" language:{filters['language']}"
                if filters.get("min_stars"):
                    search_query += f" stars:>={filters['min_stars']}"
                if filters.get("max_stars"):
                    search_query += f" stars:<={filters['max_stars']}"
                if filters.get("updated_after"):
                    search_query += f" pushed:>{filters['updated_after']}"
                if filters.get("license"):
                    search_query += f" license:{filters['license']}"
                
                params = {**base_params, "q": search_query}
                r = client.get("https://api.github.com/search/repositories", params=params, headers=headers)
                
                if r.status_code == 200:
                    items = r.json().get("items", [])
                    results.extend(items)
                elif r.status_code == 422:  # Validation error, try without problematic filters
                    # Fallback to basic search
                    params = {**base_params, "q": q}
                    r = client.get("https://api.github.com/search/repositories", params=params, headers=headers)
                    if r.status_code == 200:
                        items = r.json().get("items", [])
                        results.extend(items)

        # Deduplicate by full_name
        seen = set()
        unique = []
        for it in results:
            key = it.get("full_name")
            if key and key not in seen:
                seen.add(key)
                unique.append(it)
        
        return unique

    def post(self, shared: Dict[str, Any], prep_res: Tuple[List[str], Dict[str, Any]], exec_res: List[Dict[str, Any]]):
        shared["results_raw"] = exec_res


class RankAndFormat(Node):
    """Optionally LLM-ranks results and returns a simplified list for UI."""

    def prep(self, shared: Dict[str, Any]):
        return shared.get("results_raw", [])

    def exec(self, items: List[Dict[str, Any]]):
        if not items:
            return []
        
        # Create a compact text for LLM ranking, but also support no-key mode
        rows = []
        for it in items[:50]:  # Increased limit for better ranking
            rows.append(
                {
                    "name": it.get("full_name", ""),
                    "description": it.get("description", "") or "",
                    "stars": it.get("stargazers_count", 0),
                    "forks": it.get("forks_count", 0),
                    "url": it.get("html_url", ""),
                    "language": it.get("language", ""),
                    "license": it.get("license", {}).get("name", "") if it.get("license") else "",
                    "updated_at": it.get("updated_at", ""),
                    "open_issues": it.get("open_issues_count", 0),
                    "size": it.get("size", 0),
                }
            )

        api_key_missing = not (
            os.getenv("OPENAI_API_KEY") or os.getenv("GEMINI_API_KEY") or os.getenv("DEEPSEEK_API_KEY")
        )
        if api_key_missing:
            # Enhanced sorting by multiple factors
            rows.sort(key=lambda x: (
                x.get("stars", 0) * 2 +  # Stars are most important
                x.get("forks", 0) +      # Forks add value
                (1 if x.get("language") else 0)  # Prefer repos with language specified
            ), reverse=True)
            return rows[:25]

        # Ask LLM to rank primarily by match and stars
        prompt = "Rank these repositories for the given search intent, return top 25 as JSON list with fields name,url,description,language,stars,forks,license,updated_at,open_issues,size.\n" \
                 "The intent: "
        prompt += "<intent/>\n"
        prompt += "Repo candidates (JSON lines):\n"
        for r in rows:
            prompt += f"{r}\n"

        text = call_llm(prompt)
        # Robust JSON extraction fallback
        import json

        def _strip_code_fence(s: str) -> str:
            if "```" in s:
                # take the first fenced block
                parts = s.split("```")
                if len(parts) >= 3:
                    body = parts[1]
                    # remove optional language tag like json\n
                    if body.lower().startswith("json\n"):
                        body = body.split("\n", 1)[1] if "\n" in body else ""
                    return body.strip()
            return s

        def _slice_first_list(s: str) -> Optional[str]:
            try:
                start = s.index('[')
                end = s.rindex(']')
                return s[start:end+1]
            except ValueError:
                return None

        candidates: List[str] = []
        stripped = _strip_code_fence(text)
        candidates.append(stripped)
        sliced = _slice_first_list(text)
        if sliced:
            candidates.append(sliced)

        for cand in candidates:
            try:
                data = json.loads(cand)
                if isinstance(data, list):
                    return data[:25]
            except Exception:
                continue
        
        # Fallback to enhanced sorting
        rows.sort(key=lambda x: (
            x.get("stars", 0) * 2 +
            x.get("forks", 0) +
            (1 if x.get("language") else 0)
        ), reverse=True)
        return rows[:25]

    def post(self, shared: Dict[str, Any], prep_res: List[Dict[str, Any]], exec_res: List[Dict[str, Any]]):
        shared["results"] = exec_res


