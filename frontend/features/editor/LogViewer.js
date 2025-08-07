import React from 'react';

export default function LogViewer({ logs = [] }) {
  return (
    <div style={{
      height: '200px',
      background: '#222',
      color: '#eee',
      fontFamily: 'monospace',
      padding: '1rem',
      overflowY: 'auto',
      borderTop: '1px solid #444',
    }}>
      <h3 style={{ marginTop: 0, borderBottom: '1px solid #555', paddingBottom: '0.5rem' }}>Logs</h3>
      {logs.length === 0 ? (
        <p style={{ color: '#888' }}>No logs to display. Run a workflow to see output.</p>
      ) : (
        logs.map((log, index) => (
          <div key={index}>{log}</div>
        ))
      )}
    </div>
  );
}
