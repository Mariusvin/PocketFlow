import os
import time
import hmac
import hashlib
import jwt
import requests
from fastapi import FastAPI, Request, HTTPException, Depends
from dotenv import load_dotenv

load_dotenv()

app = FastAPI()

GITHUB_APP_ID = os.getenv("GITHUB_APP_ID")
GITHUB_WEBHOOK_SECRET = os.getenv("GITHUB_WEBHOOK_SECRET")
GITHUB_PRIVATE_KEY_PATH = os.getenv("GITHUB_PRIVATE_KEY_PATH")


def verify_signature(request_body: bytes, signature_header: str):
    """Verify the GitHub webhook signature."""
    if not signature_header:
        raise HTTPException(status_code=400, detail="X-Hub-Signature-256 header is missing!")

    hash_object = hmac.new(GITHUB_WEBHOOK_SECRET.encode('utf-8'), msg=request_body, digestmod=hashlib.sha256)
    expected_signature = "sha256=" + hash_object.hexdigest()

    if not hmac.compare_digest(expected_signature, signature_header):
        raise HTTPException(status_code=400, detail="Request signature does not match!")

def get_installation_access_token(installation_id: int):
    """Generate an installation access token."""
    with open(GITHUB_PRIVATE_KEY_PATH, "r") as f:
        private_key = f.read()

    now = int(time.time())
    payload = {
        "iat": now,
        "exp": now + (10 * 60),  # 10 minutes
        "iss": GITHUB_APP_ID,
    }
    encoded_jwt = jwt.encode(payload, private_key, algorithm="RS256")

    headers = {
        "Authorization": f"Bearer {encoded_jwt}",
        "Accept": "application/vnd.github.v3+json",
    }

    url = f"https://api.github.com/app/installations/{installation_id}/access_tokens"
    response = requests.post(url, headers=headers)
    response.raise_for_status()

    return response.json()["token"]


@app.post("/webhooks")
async def handle_webhook(request: Request):
    """Handles incoming GitHub webhooks."""
    signature_header = request.headers.get("x-hub-signature-256")
    request_body = await request.body()
    verify_signature(request_body, signature_header)

    body = await request.json()
    print("Webhook verified and received!")

    if "installation" in body and body.get("action") in ["opened", "reopened", "synchronize"]:
        from pocketguard.celery_worker import run_code_analysis

        installation_id = body["installation"]["id"]
        repo_name = body["repository"]["full_name"]
        pr_number = body["pull_request"]["number"]
        pr_head_sha = body["pull_request"]["head"]["sha"]

        run_code_analysis.delay(
            installation_id=installation_id,
            repo_name=repo_name,
            pr_number=pr_number,
            pr_head_sha=pr_head_sha
        )
        print(f"Queued analysis for {repo_name} PR #{pr_number}")

    return {"status": "ok"}

@app.get("/")
async def root():
    return {"message": "PocketGuard is running!"}


@app.get("/auth/github")
async def github_auth():
    """
    Redirects the user to GitHub for authentication.
    """
    github_client_id = os.getenv("GITHUB_CLIENT_ID")
    return {
        "message": "Redirecting to GitHub...",
        "url": f"https://github.com/login/oauth/authorize?client_id={github_client_id}"
    }

@app.get("/auth/github/callback")
async def github_auth_callback(code: str):
    """
    Handles the callback from GitHub after authentication.
    """
    github_client_id = os.getenv("GITHUB_CLIENT_ID")
    github_client_secret = os.getenv("GITHUB_CLIENT_SECRET")

    params = {
        "client_id": github_client_id,
        "client_secret": github_client_secret,
        "code": code
    }
    headers = {"Accept": "application/json"}

    response = requests.post("https://github.com/login/oauth/access_token", params=params, headers=headers)
    response.raise_for_status()

    access_token = response.json()["access_token"]

    # Here you would typically use the access token to get user info,
    # create a user record in your database, and create a session.

    return {"message": "Authentication successful!", "access_token": access_token}


# Placeholder for an authentication dependency
def get_current_user(request: Request):
    # In a real application, this would verify a session token or JWT
    # and return the authenticated user's data.
    auth_header = request.headers.get("Authorization")
    if not auth_header or not auth_header.startswith("Bearer "):
        raise HTTPException(status_code=401, detail="Not authenticated")

    # Mock user data
    return {"id": 1, "github_id": 99, "username": "testuser"}

@app.get("/api/user/me")
async def get_user_me(user: dict = Depends(get_current_user)):
    """
    Returns the authenticated user's profile.
    """
    return user

@app.get("/api/user/repositories")
async def get_user_repositories(user: dict = Depends(get_current_user)):
    """
    Returns a list of repositories for the authenticated user.
    """
    # Mock data
    return [
        {"id": 123, "name": "test/repo", "enabled": True},
        {"id": 456, "name": "test/another-repo", "enabled": False},
    ]
