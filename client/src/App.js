import React, { useState, useEffect } from 'react';
import './App.css';

function App() {
  const [keywords, setKeywords] = useState('');
  const [domains, setDomains] = useState([]);
  const [loading, setLoading] = useState(false);
  const [loadingMessage, setLoadingMessage] = useState('');
  const [error, setError] = useState('');

  const handleGenerate = async () => {
    if (!keywords.trim()) {
      setError('Please enter keywords to generate domains.');
      return;
    }
    setError('');
    setDomains([]); // Clear previous results
    setLoading(true);

    try {
      // Step 1: Generate suggestions from the AI
      setLoadingMessage('Generating creative suggestions with AI...');
      const suggestionsResponse = await fetch('http://localhost:3001/api/generate-suggestions', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ keywords }),
      });

      if (!suggestionsResponse.ok) {
        const errorData = await suggestionsResponse.json();
        throw new Error(errorData.error || 'Failed to get suggestions from the AI.');
      }

      const { suggestions } = await suggestionsResponse.json();

      if (!suggestions || suggestions.length === 0) {
        throw new Error('The AI did not return any suggestions. Try different keywords.');
      }

      // Step 2: Check availability of the generated suggestions
      setLoadingMessage('Checking domain availability...');
      const domainsWithPrice = suggestions.map(name => ({
        name,
        price: `$${(Math.random() * 10 + 5).toFixed(2)}` // Keep price placeholder for now
      }));

      const availabilityResponse = await fetch('http://localhost:3001/api/check-domains', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ domains: domainsWithPrice }),
      });

      if (!availabilityResponse.ok) {
        const errorData = await availabilityResponse.json();
        throw new Error(errorData.error || 'Failed to check domain availability.');
      }

      const results = await availabilityResponse.json();
      setDomains(results);

    } catch (error) {
      console.error('An error occurred during the generation process:', error);
      setError(error.message || 'An unexpected error occurred. Please try again.');
    } finally {
      setLoading(false);
      setLoadingMessage('');
    }
  };

  useEffect(() => {
    if (keywords.trim()) {
      setError('');
    }
  }, [keywords]);

  return (
    <div className="App">
      <header className="App-header">
        <h1>Domain Name Generator</h1>
      </header>
      <main>
        <div className="input-section">
          <input
            type="text"
            value={keywords}
            onChange={(e) => setKeywords(e.target.value)}
            placeholder="Enter keywords (e.g., 'tech startup')"
          />
          <button onClick={handleGenerate} disabled={loading}>
            {loading ? 'Generating...' : 'Generate'}
          </button>
        </div>
        {error && <p className="error">{error}</p>}
        <div className="results-section">
          {loading ? (
            <p className="loading-message">{loadingMessage}</p>
          ) : (
            domains.map((domain, index) => (
              <div key={index} className="domain-result">
                <span className="domain-name">{domain.name}</span>
                <span className={`status ${domain.available ? 'available' : 'unavailable'}`}>
                  {domain.available ? 'Available' : 'Unavailable'}
                </span>
                <span className="price">{domain.price}</span>
              </div>
            ))
          )}
        </div>
      </main>
    </div>
  );
}

export default App;
