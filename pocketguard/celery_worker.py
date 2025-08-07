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

@celery_app.task
def run_code_analysis(installation_id: int, repo_name: str, pr_number: int):
    """
    A Celery task to run the code analysis agent.
    """
    token = get_installation_access_token(installation_id)
    github_client = Github(token)

    agent = SecretScannerAgent()
    agent.run(github_client=github_client, repo_name=repo_name, pr_number=pr_number)

    return {"status": "completed"}
