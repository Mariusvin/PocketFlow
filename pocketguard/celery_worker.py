import os
from celery import Celery
from github import Github
from pocketguard.flow import SecretScannerAgent
from pocketguard.main import get_installation_access_token

celery_app = Celery(
    "pocketguard",
    broker=os.getenv("CELERY_BROKER_URL", "redis://localhost:6379/0"),
    backend=os.getenv("CELery_RESULT_BACKEND", "redis://localhost:6379/0"),
)

from pocketguard.flow import FileFetcherAgent, CodeAnalysisAgent, GitHubCommenterAgent

@celery_app.task
def run_code_analysis(installation_id: int, repo_name: str, pr_number: int, pr_head_sha: str):
    """
    A Celery task to run the code analysis agent pipeline.
    """
    token = get_installation_access_token(installation_id)
    github_client = Github(token)

    # 1. Fetch files
    file_fetcher = FileFetcherAgent()
    file_fetcher_result = file_fetcher.run(
        github_client=github_client,
        repo_name=repo_name,
        pr_number=pr_number,
        pr_head_sha=pr_head_sha
    )

    # 2. Analyze code
    code_analyzer = CodeAnalysisAgent()
    analysis_result = code_analyzer.run(files=file_fetcher_result["files"])

    # 3. Post comments
    commenter = GitHubCommenterAgent()
    commenter.run(
        github_client=github_client,
        repo_name=repo_name,
        pr_number=pr_number,
        pr_head_sha=pr_head_sha,
        comments=analysis_result["comments"]
    )

    return {"status": "completed"}
