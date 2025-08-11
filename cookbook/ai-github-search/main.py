import os
import sys
from typing import Dict, Any, Optional
from fastapi import FastAPI, Query
from fastapi.responses import HTMLResponse, JSONResponse
from fastapi.staticfiles import StaticFiles
from pocketflow import Flow

# Add the current directory to Python path for imports
current_dir = os.path.dirname(os.path.abspath(__file__))
if current_dir not in sys.path:
    sys.path.insert(0, current_dir)

from flow import create_search_flow

# Try to load .env if python-dotenv is available
try:
    from dotenv import load_dotenv
    load_dotenv()
except ImportError:
    pass

app = FastAPI(title="AI GitHub Search")
static_dir = os.path.join(os.path.dirname(__file__), "static")
app.mount("/static", StaticFiles(directory=static_dir), name="static")


@app.get("/", response_class=HTMLResponse)
def index():
    with open(os.path.join(static_dir, "index.html"), "r", encoding="utf-8") as f:
        return HTMLResponse(f.read())


@app.get("/api/search")
def api_search(
    q: str = Query("", min_length=1),
    language: Optional[str] = Query(None, description="Programming language filter"),
    min_stars: Optional[int] = Query(None, ge=0, description="Minimum stars"),
    max_stars: Optional[int] = Query(None, ge=0, description="Maximum stars"),
    updated_after: Optional[str] = Query(None, description="Updated after date (YYYY-MM-DD)"),
    license: Optional[str] = Query(None, description="License type"),
):
    shared: Dict[str, Any] = {"nl_query": q}
    
    # Build filters dict
    filters = {}
    if language:
        filters["language"] = language
    if min_stars is not None:
        filters["min_stars"] = min_stars
    if max_stars is not None:
        filters["max_stars"] = max_stars
    if updated_after:
        filters["updated_after"] = updated_after
    if license:
        filters["license"] = license
    
    if filters:
        shared["filters"] = filters
    
    flow: Flow = create_search_flow()
    flow.run(shared)
    
    return JSONResponse({
        "query": q,
        "filters": filters,
        "results": shared.get("results", []),
        "gh_queries": shared.get("gh_queries", []),
        "search_strategies": shared.get("search_strategies", []),
    })


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=int(os.getenv("PORT", 8000)))


