import time

# --- State for simulating self-correction ---
# This is a simple global state for testing purposes.
# In a real application, this logic would not exist.
solution_attempts = 0

def call_llm(prompt: str) -> str:
    """
    A dummy function to simulate a call to a large language model.
    This version is specifically tailored to test the self-correction loop.
    """
    global solution_attempts
    print(f"--- LLM PROMPT ---\n{prompt}\n--------------------")
    time.sleep(1) # Faster sleep for testing

    # --- Test-specific logic for self-correction ---
    if "write a suite of unit tests" in prompt and "factorial" in prompt:
        return """
```python
import unittest

# The function to be tested, 'factorial', is expected to be defined
# in the same scope before this test suite is run.

class TestFactorial(unittest.TestCase):
    def test_zero(self):
        self.assertEqual(factorial(0), 1)

    def test_positive(self):
        self.assertEqual(factorial(1), 1)
        self.assertEqual(factorial(5), 120)
        self.assertEqual(factorial(10), 3628800)

    def test_negative(self):
        with self.assertRaises(ValueError):
            factorial(-1)

# This part is crucial for running the tests and getting a failure exit code
if __name__ == '__main__':
    unittest.main()
```"""
    elif "write the Python code for the solution" in prompt and solution_attempts == 0:
        solution_attempts += 1
        return """
```python
# Incorrect factorial function (off-by-one error)
def factorial(n):
    if n < 0:
        raise ValueError("Factorial is not defined for negative numbers")
    if n == 0:
        return 1
    res = 1
    for i in range(1, n): # Incorrect: should be range(1, n + 1)
        res *= i
    return res
```"""
    elif "debug and provide a corrected version" in prompt:
        return """
```python
# Correct factorial function
def factorial(n):
    if n < 0:
        raise ValueError("Factorial is not defined for negative numbers")
    if n == 0:
        return 1
    res = 1
    for i in range(1, n + 1):
        res *= i
    return res
```"""

    # --- Generic article generation logic ---
    elif "outline" in prompt:
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

    # Fallback for any other prompt
    else:
        return "This is a dummy response from the LLM."
