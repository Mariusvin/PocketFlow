import unittest
from unittest.mock import patch, MagicMock
from fastapi.testclient import TestClient
from pocketguard.main import app

class TestMain(unittest.TestCase):

    def setUp(self):
        self.client = TestClient(app)

    @patch('pocketguard.main.open', new_callable=unittest.mock.mock_open, read_data='test key')
    @patch('pocketguard.main.verify_signature')
    @patch('pocketguard.main.get_installation_access_token')
    @patch('pocketguard.flow.SecretScannerAgent')
    def test_handle_webhook_pull_request_opened(self, mock_agent, mock_get_token, mock_verify_signature, mock_open):
        mock_verify_signature.return_value = None
        mock_get_token.return_value = "test_token"
        mock_agent_instance = mock_agent.return_value
        mock_agent_instance.run.return_value = {"status": "completed"}

        payload = {
            "action": "opened",
            "installation": {"id": 123},
            "repository": {"full_name": "test/repo"},
            "pull_request": {"number": 1}
        }

        response = self.client.post("/webhooks", json=payload, headers={"x-hub-signature-256": "sha256=test"})

        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json(), {"status": "ok"})
        mock_verify_signature.assert_called_once()
        mock_get_token.assert_called_once_with(123)
        mock_agent.assert_called_once()
        mock_agent_instance.run.assert_called_once()

    @patch('pocketguard.main.verify_signature')
    def test_handle_webhook_ping_event(self, mock_verify_signature):
        mock_verify_signature.return_value = None

        payload = {"zen": "Keep it simple."}
        response = self.client.post("/webhooks", json=payload, headers={"x-hub-signature-256": "sha256=test"})

        self.assertEqual(response.status_code, 200)

if __name__ == "__main__":
    unittest.main()
