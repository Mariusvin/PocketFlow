import re
from github import Github

class Agent:
    """A mock agent class to simulate the behavior of The-Pocket/Flow."""
    def __init__(self, name):
        self.name = name

    def run(self, **kwargs):
        """Simulates running the agent."""
        print(f"Agent {self.name} is running with inputs: {kwargs}")
        return {}

class SecretScannerAgent(Agent):
    """An agent that scans for hardcoded secrets in a pull request."""

    def __init__(self):
        super().__init__(name="SecretScannerAgent")

    def run(self, github_client: Github, repo_name: str, pr_number: int):
        """
        Scans for hardcoded secrets in a pull request and posts comments.
        """
        print(f"Running SecretScannerAgent on {repo_name} PR #{pr_number}")
        repo = github_client.get_repo(repo_name)
        pr = repo.get_pull(pr_number)
        files = pr.get_files()

        secret_pattern = re.compile(r'API_KEY\s*=\s*["\']\w+["\']')

        for file in files:
            content = repo.get_contents(file.filename, ref=pr.head.sha).decoded_content.decode("utf-8")
            lines = content.split('\n')
            for i, line in enumerate(lines):
                if secret_pattern.search(line):
                    print(f"Found secret in {file.filename} on line {i+1}")
                    pr.create_review_comment(
                        body="Hardcoded secret found. Please remove it.",
                        commit_id=pr.head.sha,
                        path=file.filename,
                        line=i + 1,
                    )

        return {"status": "completed"}
