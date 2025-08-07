import time

def call_llm(prompt: str) -> str:
    """
    Mocks a call to a Large Language Model.
    In a real application, this would contain an API call to a service like OpenAI.
    """
    print(f"\\n--- Calling LLM with prompt: '{prompt[:50]}...' ---\\n")
    time.sleep(1) # Simulate network latency

    # Mock response based on prompt content
    if "welcome" in prompt.lower():
        response = "Welcome to the future of website creation, powered by mFlow and PocketFlow!"
    elif "features" in prompt.lower():
        response = "Our AI can generate text, suggest layouts, and create beautiful images for your site."
    else:
        response = f"This is a mock AI response to the prompt: '{prompt}'"

    print(f"\\n--- LLM Response: '{response[:50]}...' ---\\n")
    return response

if __name__ == "__main__":
    print("Testing llm_helper.py...")
    test_prompt_1 = "Generate a welcome message for our new website."
    print(f"Input: {test_prompt_1}")
    print(f"Output: {call_llm(test_prompt_1)}\\n")

    test_prompt_2 = "Tell me about the features."
    print(f"Input: {test_prompt_2}")
    print(f"Output: {call_llm(test_prompt_2)}\\n")
