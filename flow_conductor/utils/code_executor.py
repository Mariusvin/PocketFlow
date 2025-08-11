import subprocess
import sys

def execute_code(code_string: str) -> tuple[bool, str]:
    """
    Executes a string of Python code in a separate process and captures its output.

    Args:
        code_string: A string containing the Python code to execute.

    Returns:
        A tuple containing:
        - A boolean indicating whether the execution was successful (True) or not (False).
        - A string containing the captured stdout and stderr.
    """
    try:
        # We use the same Python executable that is running this script.
        python_executable = sys.executable

        # Run the code using subprocess.
        # We pass the code as a string to the python interpreter via the -c flag.
        result = subprocess.run(
            [python_executable, "-c", code_string],
            capture_output=True,
            text=True,
            timeout=30  # Add a timeout to prevent long-running code
        )

        # Check the return code to see if the process exited successfully.
        if result.returncode == 0:
            return True, result.stdout
        else:
            return False, result.stderr

    except subprocess.TimeoutExpired:
        return False, "Execution timed out after 30 seconds."
    except Exception as e:
        return False, f"An unexpected error occurred: {str(e)}"

if __name__ == '__main__':
    # Example usage for testing the utility function.

    # Test case 1: Successful execution
    success_code = "print('Hello, World!')"
    success, output = execute_code(success_code)
    print(f"--- Test 1 (Success) ---")
    print(f"Success: {success}")
    print(f"Output:\n{output}")
    assert success is True
    assert "Hello, World!" in output

    print("-" * 20)

    # Test case 2: Execution with an error
    error_code = "print(x)" # x is not defined
    success, output = execute_code(error_code)
    print(f"--- Test 2 (Error) ---")
    print(f"Success: {success}")
    print(f"Output:\n{output}")
    assert success is False
    assert "NameError: name 'x' is not defined" in output

    print("-" * 20)

    # Test case 3: Successful execution with unittest
    unittest_code = """
import unittest

class TestMyFunction(unittest.TestCase):
    def test_addition(self):
        self.assertEqual(1 + 1, 2)

if __name__ == '__main__':
    # Creating a TestSuite
    suite = unittest.TestSuite()
    suite.addTest(unittest.makeSuite(TestMyFunction))

    # Running the tests
    runner = unittest.TextTestRunner()
    result = runner.run(suite)

    # Check if the tests were successful
    if result.wasSuccessful():
        print("All tests passed!")
    else:
        print("Tests failed.")
        # To make the subprocess fail, we can exit with a non-zero code
        exit(1)
"""
    success, output = execute_code(unittest_code)
    print(f"--- Test 3 (Unittest) ---")
    print(f"Success: {success}")
    print(f"Output:\n{output}")
    assert success is True
    assert "All tests passed!" in output
