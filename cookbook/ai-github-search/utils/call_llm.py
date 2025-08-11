import os
from typing import List, Dict, Union


def _provider() -> str:
    return os.getenv("LLM_PROVIDER", "openai").lower()


def call_llm(prompt_or_messages: Union[str, List[Dict[str, str]]]) -> str:
    """Very small LLM wrapper with provider switch.

    - Supports: OpenAI, Google (Gemini), DeepSeek via OpenAI client
    - If no API key found, returns a heuristic rewrite of the input
    """

    provider = _provider()

    # Fallback: no keys, return the last user content or prompt
    def _fallback() -> str:
        if isinstance(prompt_or_messages, str):
            return prompt_or_messages
        # messages-style
        for m in reversed(prompt_or_messages):
            if m.get("role") == "user":
                return m.get("content", "")
        return ""

    if provider == "openai":
        api_key = os.getenv("OPENAI_API_KEY")
        if not api_key:
            return _fallback()
        try:
            from openai import OpenAI

            client = OpenAI(api_key=api_key)
            if isinstance(prompt_or_messages, str):
                messages = [{"role": "user", "content": prompt_or_messages}]
            else:
                messages = prompt_or_messages
            r = client.chat.completions.create(
                model=os.getenv("OPENAI_MODEL", "gpt-4o-mini"),
                messages=messages,
                temperature=0.3,
            )
            return r.choices[0].message.content or ""
        except Exception:
            return _fallback()

    if provider == "gemini":
        api_key = os.getenv("GEMINI_API_KEY")
        if not api_key:
            return _fallback()
        try:
            from google import genai

            client = genai.Client(api_key=api_key)
            if isinstance(prompt_or_messages, str):
                content = prompt_or_messages
            else:
                # Flatten messages
                content = "\n".join([m.get("content", "") for m in prompt_or_messages])
            resp = client.models.generate_content(
                model=os.getenv("GEMINI_MODEL", "gemini-2.0-flash-001"),
                contents=content,
            )
            return getattr(resp, "text", "")
        except Exception:
            return _fallback()

    if provider == "deepseek":
        api_key = os.getenv("DEEPSEEK_API_KEY")
        if not api_key:
            return _fallback()
        try:
            from openai import OpenAI

            client = OpenAI(api_key=api_key, base_url="https://api.deepseek.com")
            if isinstance(prompt_or_messages, str):
                messages = [{"role": "user", "content": prompt_or_messages}]
            else:
                messages = prompt_or_messages
            r = client.chat.completions.create(
                model=os.getenv("DEEPSEEK_MODEL", "deepseek-chat"),
                messages=messages,
                temperature=0.3,
            )
            return r.choices[0].message.content or ""
        except Exception:
            return _fallback()

    return _fallback()


if __name__ == "__main__":
    print(call_llm("Rewrite this as a short GitHub search query for Next.js todo app using Tailwind."))


