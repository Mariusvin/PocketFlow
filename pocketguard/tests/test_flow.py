import unittest
from unittest.mock import MagicMock, patch
from pocketguard.flow import SecretScannerAgent

class TestFlow(unittest.TestCase):

    def test_secret_scanner_agent_run(self):
        mock_github_client = MagicMock()
        mock_repo = MagicMock()
        mock_pr = MagicMock()
        mock_file = MagicMock()
        mock_contents = MagicMock()

        mock_github_client.get_repo.return_value = mock_repo
        mock_repo.get_pull.return_value = mock_pr
        mock_pr.get_files.return_value = [mock_file]
        mock_file.filename = "config.py"
        mock_pr.head.sha = "test_sha"

        secret_code = 'API_KEY = "my_super_secret_key"'
        mock_repo.get_contents.return_value.decoded_content.decode.return_value = secret_code

        agent = SecretScannerAgent()
        agent.run(github_client=mock_github_client, repo_name="test/repo", pr_number=1)

        mock_github_client.get_repo.assert_called_once_with("test/repo")
        mock_repo.get_pull.assert_called_once_with(1)
        mock_pr.get_files.assert_called_once()
        mock_repo.get_contents.assert_called_once_with("config.py", ref="test_sha")
        mock_pr.create_review_comment.assert_called_once_with(
            body="Hardcoded secret found. Please remove it.",
            commit_id="test_sha",
            path="config.py",
            line=1
        )

if __name__ == "__main__":
    unittest.main()
