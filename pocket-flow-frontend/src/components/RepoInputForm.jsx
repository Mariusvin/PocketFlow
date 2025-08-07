import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { submitRepository } from '../services/api';

const RepoInputForm = () => {
  const [url, setUrl] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      // Basic URL validation
      if (!url || !url.startsWith('https://github.com/')) {
        throw new Error('Please enter a valid GitHub repository URL.');
      }

      const response = await submitRepository(url);
      if (response && response.sessionId) {
        navigate(`/chat/${response.sessionId}`);
      } else {
        throw new Error('Failed to start session. Please try again.');
      }
    } catch (err) {
      setError(err.message || 'An unexpected error occurred.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="w-full max-w-md p-8 space-y-6 bg-white rounded-lg shadow-md">
        <h1 className="text-2xl font-bold text-center text-gray-900">Codebase Q&A</h1>
        <p className="text-center text-gray-600">
          Enter a GitHub repository URL to start asking questions about its codebase.
        </p>
        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="repo-url" className="sr-only">
              GitHub Repository URL
            </label>
            <input
              id="repo-url"
              name="url"
              type="url"
              value={url}
              onChange={(e) => setUrl(e.target.value)}
              required
              className="w-full px-3 py-2 placeholder-gray-500 border border-gray-300 rounded-md appearance-none focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              placeholder="https://github.com/user/repo"
              disabled={isLoading}
            />
          </div>
          {error && <p className="text-sm text-red-600">{error}</p>}
          <div>
            <button
              type="submit"
              disabled={isLoading}
              className="relative flex justify-center w-full px-4 py-2 text-sm font-medium text-white bg-indigo-600 border border-transparent rounded-md group hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:bg-indigo-300"
            >
              {isLoading ? 'Processing...' : 'Start Q&A'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default RepoInputForm;
