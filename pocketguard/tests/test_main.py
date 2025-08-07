import unittest
from unittest.mock import patch, MagicMock
from fastapi.testclient import TestClient
from pocketguard.main import app

class TestMain(unittest.TestCase):

    def setUp(self):
        self.client = TestClient(app)

    @patch('pocketguard.main.verify_signature')
    @patch('pocketguard.celery_worker.run_code_analysis.delay')
    def test_handle_webhook_pull_request_opened(self, mock_delay, mock_verify_signature):
        mock_verify_signature.return_value = None

        payload = {
            "action": "opened",
            "installation": {"id": 123},
            "repository": {"full_name": "test/repo"},
            "pull_request": {"number": 1, "head": {"sha": "test_sha"}}
        }

        response = self.client.post("/webhooks", json=payload, headers={"x-hub-signature-256": "sha256=test"})

        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json(), {"status": "ok"})
        mock_verify_signature.assert_called_once()
        mock_delay.assert_called_once_with(
            installation_id=123,
            repo_name="test/repo",
            pr_number=1,
            pr_head_sha="test_sha"
        )

    @patch('pocketguard.main.verify_signature')
    def test_handle_webhook_ping_event(self, mock_verify_signature):
        mock_verify_signature.return_value = None

        payload = {"zen": "Keep it simple."}
        response = self.client.post("/webhooks", json=payload, headers={"x-hub-signature-256": "sha256=test"})

        self.assertEqual(response.status_code, 200)

    def test_github_auth(self):
        response = self.client.get("/auth/github")
        self.assertEqual(response.status_code, 200)
        self.assertIn("https://github.com/login/oauth/authorize", response.json()["url"])

    @patch('requests.post')
    def test_github_auth_callback(self, mock_post):
        mock_post.return_value.status_code = 200
        mock_post.return_value.json.return_value = {"access_token": "test_token"}

        response = self.client.get("/auth/github/callback?code=test_code")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()["access_token"], "test_token")

    def test_get_user_me(self):
        response = self.client.get("/api/user/me", headers={"Authorization": "Bearer test_token"})
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()["username"], "testuser")

    def test_get_user_repositories(self):
        response = self.client.get("/api/user/repositories", headers={"Authorization": "Bearer test_token"})
        self.assertEqual(response.status_code, 200)
        self.assertEqual(len(response.json()), 2)

if __name__ == "__main__":
    unittest.main()
