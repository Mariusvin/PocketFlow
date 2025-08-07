import re
import os
from github import Github

class Agent:
    """Base class for all agents in the pipeline."""
    def __init__(self, name):
        self.name = name

    def run(self, **kwargs):
        raise NotImplementedError

class FileFetcherAgent(Agent):
    """Fetches changed files from a pull request."""
    def __init__(self):
        super().__init__(name="FileFetcherAgent")

    def run(self, github_client: Github, repo_name: str, pr_number: int, pr_head_sha: str):
        print(f"Running {self.name} on {repo_name} PR #{pr_number}")
        repo = github_client.get_repo(repo_name)
        pr = repo.get_pull(pr_number)
        files = pr.get_files()

        file_objects = []
        for file in files:
            content = repo.get_contents(file.filename, ref=pr_head_sha).decoded_content.decode("utf-8")
            file_objects.append({"filename": file.filename, "content": content})

        return {"files": file_objects}

import subprocess
import tempfile

class PythonLinterAgent(Agent):
    """Lints Python files using flake8."""
    def __init__(self):
        super().__init__(name="PythonLinterAgent")

    def run(self, files: list):
        print(f"Running {self.name}")
        comments = []
        for file in files:
            if file["filename"].endswith(".py"):
                with tempfile.NamedTemporaryFile(mode="w", suffix=".py", delete=False) as temp_file:
                    temp_file.write(file["content"])
                    temp_filepath = temp_file.name

                try:
                    result = subprocess.run(
                        ["flake8", "--isolated", temp_filepath],
                        capture_output=True,
                        text=True,
                    )
                    for line in result.stdout.splitlines():
                        parts = line.split(":")
                        if len(parts) >= 4:
                            comments.append({
                                "filename": file["filename"],
                                "line": int(parts[1]),
                                "body": f"flake8: {parts[3].strip()}"
                            })
                finally:
                    os.remove(temp_filepath)
        return {"comments": comments}

class CodeAnalysisAgent(Agent):
    """Analyzes code for issues."""
    def __init__(self):
        super().__init__(name="CodeAnalysisAgent")
        self.linters = [PythonLinterAgent(), SecretScannerAgent()]

    def run(self, files: list):
        print(f"Running {self.name}")
        all_comments = []
        for linter in self.linters:
            result = linter.run(files=files)
            all_comments.extend(result.get("comments", []))
        return {"comments": all_comments}

class SecretScannerAgent(Agent):
    """Scans for hardcoded secrets."""
    def __init__(self):
        super().__init__(name="SecretScannerAgent")

    def run(self, files: list):
        print(f"Running {self.name}")
        comments = []
        secret_pattern = re.compile(r'API_KEY\s*=\s*["\']\w+["\']')

        for file in files:
            lines = file["content"].split('\n')
            for i, line in enumerate(lines):
                if secret_pattern.search(line):
                    comments.append({
                        "filename": file["filename"],
                        "line": i + 1,
                        "body": "Hardcoded secret found. Please remove it."
                    })
        return {"comments": comments}

class GitHubCommenterAgent(Agent):
    """Posts comments to a GitHub pull request."""
    def __init__(self):
        super().__init__(name="GitHubCommenterAgent")

    def run(self, github_client: Github, repo_name: str, pr_number: int, pr_head_sha: str, comments: list):
        print(f"Running {self.name} on {repo_name} PR #{pr_number}")
        repo = github_client.get_repo(repo_name)
        pr = repo.get_pull(pr_number)

        for comment in comments:
            pr.create_review_comment(
                body=comment["body"],
                commit_id=pr_head_sha,
                path=comment["filename"],
                line=comment["line"],
            )
        return {"status": "completed"}
