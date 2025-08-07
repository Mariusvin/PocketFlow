import unittest
from unittest.mock import MagicMock, patch
from pocketguard.celery_worker import run_code_analysis
from pocketguard.flow import PythonLinterAgent, SecretScannerAgent

class TestFlow(unittest.TestCase):

    @patch('pocketguard.flow.subprocess.run')
    def test_python_linter_agent(self, mock_subprocess_run):
        mock_subprocess_run.return_value.stdout = "test.py:1:1: F401 'os' imported but unused"
        agent = PythonLinterAgent()
        files = [{"filename": "test.py", "content": "import os"}]
        result = agent.run(files=files)
        self.assertEqual(len(result["comments"]), 1)
        self.assertEqual(result["comments"][0]["body"], "flake8: F401 'os' imported but unused")

    def test_secret_scanner_agent(self):
        agent = SecretScannerAgent()
        files = [{"filename": "config.py", "content": 'API_KEY = "my_secret"'}]
        result = agent.run(files=files)
        self.assertEqual(len(result["comments"]), 1)
        self.assertEqual(result["comments"][0]["body"], "Hardcoded secret found. Please remove it.")

    @patch('pocketguard.celery_worker.get_installation_access_token')
    @patch('pocketguard.celery_worker.Github')
    @patch('pocketguard.flow.FileFetcherAgent.run')
    @patch('pocketguard.flow.CodeAnalysisAgent.run')
    @patch('pocketguard.flow.GitHubCommenterAgent.run')
    def test_run_code_analysis_pipeline(self, mock_commenter_run, mock_analysis_run, mock_fetcher_run, mock_github, mock_get_token):
        mock_get_token.return_value = "test_token"
        mock_github_client = MagicMock()
        mock_github.return_value = mock_github_client

        mock_fetcher_run.return_value = {"files": [{"filename": "test.py", "content": "import os"}]}
        mock_analysis_run.return_value = {"comments": [{"filename": "test.py", "line": 1, "body": "test comment"}]}

        run_code_analysis(installation_id=123, repo_name="test/repo", pr_number=1, pr_head_sha="test_sha")

        mock_get_token.assert_called_once_with(123)
        mock_github.assert_called_once_with("test_token")
        mock_fetcher_run.assert_called_once()
        mock_analysis_run.assert_called_once()
        mock_commenter_run.assert_called_once()

if __name__ == "__main__":
    unittest.main()
