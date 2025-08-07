import Head from 'next/head';
import { useEffect, useState } from 'react';
import { getWorkflows, createWorkflow, deleteWorkflow } from '../lib/api';

export default function Dashboard() {
  const [workflows, setWorkflows] = useState([]);
  const [error, setError] = useState(null);

  const fetchWorkflows = async () => {
    try {
      const data = await getWorkflows();
      setWorkflows(data);
    } catch (err) {
      setError('Failed to fetch workflows. Is the backend running?');
    }
  };

  useEffect(() => {
    fetchWorkflows();
  }, []);

  const handleCreateWorkflow = async () => {
    const name = prompt('Enter new workflow name:');
    if (name) {
      try {
        await createWorkflow({ name });
        fetchWorkflows(); // Refresh the list
      } catch (err) {
        setError('Failed to create workflow.');
      }
    }
  };

  const handleDeleteWorkflow = async (id) => {
    if (window.confirm('Are you sure you want to delete this workflow?')) {
      try {
        await deleteWorkflow(id);
        fetchWorkflows(); // Refresh the list
      } catch (err) {
        setError('Failed to delete workflow.');
      }
    }
  };

  return (
    <div>
      <Head>
        <title>My Dashboard - Flow Studio</title>
      </Head>

      <main style={{ fontFamily: 'sans-serif', padding: '2rem', maxWidth: '800px', margin: 'auto' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h1 style={{ fontSize: '2rem' }}>My Workflows</h1>
          <button
            onClick={handleCreateWorkflow}
            style={{
              padding: '0.75rem 1.5rem',
              fontSize: '1rem',
              backgroundColor: '#0070f3',
              color: 'white',
              border: 'none',
              borderRadius: '0.5rem',
              cursor: 'pointer',
            }}
          >
            + New Workflow
          </button>
        </div>

        {error && <p style={{ color: 'red' }}>{error}</p>}

        <div style={{ marginTop: '2rem', border: '1px solid #eaeaea', borderRadius: '0.5rem' }}>
          {workflows.length === 0 ? (
            <p style={{ padding: '2rem', textAlign: 'center', color: '#666' }}>
              You don't have any workflows yet. Create one to get started!
            </p>
          ) : (
            <ul style={{ listStyle: 'none', margin: 0, padding: 0 }}>
              {workflows.map((wf) => (
                <li key={wf.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '1rem', borderBottom: '1px solid #eaeaea' }}>
                  <span>{wf.name}</span>
                  <button onClick={() => handleDeleteWorkflow(wf.id)} style={{ background: 'none', border: 'none', color: 'red', cursor: 'pointer' }}>
                    Delete
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>
      </main>
    </div>
  );
}
