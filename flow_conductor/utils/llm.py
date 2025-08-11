import time

def call_llm(prompt: str) -> str:
    """
    A dummy function to simulate a call to a large language model.
    """
    print(f"--- LLM PROMPT ---\n{prompt}\n--------------------")
    time.sleep(2) # Simulate network latency and processing time
    if "outline" in prompt:
        return """
```yaml
sections:
    - Introduction to PocketFlow
    - Core Concepts
    - Getting Started
```"""
    elif "Write a short paragraph" in prompt:
        return "This is a paragraph about the section."
    elif "Rewrite the following draft" in prompt:
        return "This is the final, styled article."
    else:
        return "This is a dummy response from the LLM."
