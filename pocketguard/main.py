import os
import time
import hmac
import hashlib
import jwt
import requests
from fastapi import FastAPI, Request, HTTPException
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
        installation_id = body["installation"]["id"]
        token = get_installation_access_token(installation_id)
        print(f"Generated installation token: {token[:10]}...")

        from github import Github
        from pocketguard.flow import SecretScannerAgent

        github_client = Github(token)
        repo_name = body["repository"]["full_name"]
        pr_number = body["pull_request"]["number"]

        agent = SecretScannerAgent()
        agent.run(github_client=github_client, repo_name=repo_name, pr_number=pr_number)

    return {"status": "ok"}

@app.get("/")
async def root():
    return {"message": "PocketGuard is running!"}
