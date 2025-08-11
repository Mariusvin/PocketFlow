import yaml
from pocketflow import Node, BatchNode
from utils.llm import call_llm
from utils.code_executor import execute_code

# --- Article Generation Workflow Nodes ---

class GenerateOutline(Node):
    def prep(self, shared):
        return shared["topic"]

    def exec(self, topic):
        prompt = f"""
Create a simple outline for an article about {topic}.
Include at most 3 main sections (no subsections).

Output the sections in YAML format as shown below:

```yaml
sections:
    - First section title
    - Second section title
    - Third section title
```"""
        response = call_llm(prompt)
        yaml_str = response.split("```yaml")[1].split("```")[0].strip()
        structured_result = yaml.safe_load(yaml_str)
        return structured_result

    def post(self, shared, prep_res, exec_res):
        sections = exec_res["sections"]
        shared["sections"] = sections

        # Send progress update via SSE queue
        progress_msg = {"step": "outline", "progress": 33, "data": {"sections": sections}}
        shared["sse_queue"].put_nowait(progress_msg)

        return "default"

class WriteContent(BatchNode):
    def prep(self, shared):
        # Store sections and sse_queue for use in exec
        self.sections = shared.get("sections", [])
        self.sse_queue = shared["sse_queue"]
        return self.sections

    def exec(self, section):
        prompt = f"""
Write a short paragraph (MAXIMUM 100 WORDS) about this section:

{section}

Requirements:
- Explain the idea in simple, easy-to-understand terms
- Use everyday language, avoiding jargon
- Keep it very concise (no more than 100 words)
- Include one brief example or analogy
"""
        content = call_llm(prompt)

        # Send progress update for this section
        current_section_index = self.sections.index(section) if section in self.sections else 0
        total_sections = len(self.sections)

        # Progress from 33% (after outline) to 66% (before styling)
        # Each section contributes (66-33)/total_sections = 33/total_sections percent
        section_progress = 33 + ((current_section_index + 1) * 33 // total_sections)

        progress_msg = {
            "step": "content",
            "progress": section_progress,
            "data": {
                "section": section,
                "completed_sections": current_section_index + 1,
                "total_sections": total_sections
            }
        }
        self.sse_queue.put_nowait(progress_msg)

        return f"## {section}\n\n{content}\n"

    def post(self, shared, prep_res, exec_res_list):
        draft = "\n".join(exec_res_list)
        shared["draft"] = draft
        return "default"

class ApplyStyle(Node):
    def prep(self, shared):
        return shared["draft"]

    def exec(self, draft):
        prompt = f"""
Rewrite the following draft in a conversational, engaging style:

{draft}

Make it:
- Conversational and warm in tone
- Include rhetorical questions that engage the reader
- Add analogies and metaphors where appropriate
- Include a strong opening and conclusion
"""
        return call_llm(prompt)

    def post(self, shared, prep_res, exec_res):
        shared["final_article"] = exec_res

        # Send completion update via SSE queue
        progress_msg = {"step": "complete", "progress": 100, "data": {"final_article": exec_res}}
        shared["sse_queue"].put_nowait(progress_msg)

        return "default"

# --- Code Generation Workflow Nodes ---

class GenerateTests(Node):
    def prep(self, shared):
        return shared["problem_description"]

    def exec(self, problem_description):
        prompt = f"""
Based on the following problem description, write a suite of unit tests in Python using the `unittest` framework.
The tests should cover the main functionality, edge cases, and potential error conditions.
The code should be a single block that can be executed.

Problem:
{problem_description}

```python
import unittest

# Your test class and methods here

```"""
        response = call_llm(prompt)
        # Simple extraction of the code block
        try:
            code = response.split("```python")[1].split("```")[0].strip()
        except IndexError:
            code = response # Assume the whole response is code if parsing fails
        return code

    def post(self, shared, prep_res, exec_res):
        shared["generated_tests"] = exec_res
        progress_msg = {"step": "tests_generated", "progress": 33, "data": {"tests": exec_res}}
        shared["sse_queue"].put_nowait(progress_msg)
        return "default"

class ImplementSolution(Node):
    def prep(self, shared):
        return shared["problem_description"], shared["generated_tests"]

    def exec(self, inputs):
        problem_description, generated_tests = inputs
        prompt = f"""
Based on the following problem description and unit tests, write the Python code for the solution.
The solution should be a single function or class that passes the provided tests.

Problem:
{problem_description}

Tests:
{generated_tests}

```python
# Your solution code here

```"""
        response = call_llm(prompt)
        try:
            code = response.split("```python")[1].split("```")[0].strip()
        except IndexError:
            code = response
        return code

    def post(self, shared, prep_res, exec_res):
        shared["generated_solution"] = exec_res
        progress_msg = {"step": "solution_implemented", "progress": 66, "data": {"solution": exec_res}}
        shared["sse_queue"].put_nowait(progress_msg)
        return "default"

class RunAndValidate(Node):
    def prep(self, shared):
        return shared["generated_solution"], shared["generated_tests"]

    def exec(self, inputs):
        solution_code, test_code = inputs
        # Combine the solution and test code into a single script for execution
        full_code = f"{solution_code}\n\n{test_code}"
        success, output = execute_code(full_code)
        return {"success": success, "output": output}

    def post(self, shared, prep_res, exec_res):
        shared["validation_result"] = exec_res
        progress_msg = {"step": "complete", "progress": 100, "data": exec_res}
        shared["sse_queue"].put_nowait(progress_msg)
        return "default"
