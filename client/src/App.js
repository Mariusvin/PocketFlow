import React, { useState, useEffect } from 'react';
import './App.css';

function App() {
  const [keywords, setKeywords] = useState('');
  const [domains, setDomains] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const generateDomains = (keywords) => {
    return new Promise(resolve => {
      setTimeout(() => {
        // Placeholder for AI-powered domain generation
        const suffixes = ['.com', '.net', '.org', '.io'];
        const generatedDomains = [];
        suffixes.forEach(suffix => {
          generatedDomains.push({ name: `${keywords.replace(/\s+/g, '')}${suffix}`, available: Math.random() > 0.5, price: `$${(Math.random() * 10 + 5).toFixed(2)}` });
          generatedDomains.push({ name: `${keywords.replace(/\s+/g, 'hq')}${suffix}`, available: Math.random() > 0.5, price: `$${(Math.random() * 10 + 5).toFixed(2)}` });
        });
        resolve(generatedDomains);
      }, 1000);
    });
  };

  const handleGenerate = async () => {
    if (!keywords.trim()) {
      setError('Please enter keywords to generate domains.');
      return;
    }
    setError('');
    setLoading(true);
    const newDomains = await generateDomains(keywords);
    setDomains(newDomains);
    setLoading(false);
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
            <p>Loading...</p>
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
