import React, { useState, useEffect } from 'react';
import { getRepoStatus } from '../services/api';

const StatusDisplay = ({ sessionId, onReady }) => {
  const [status, setStatus] = useState('Initializing...');

  useEffect(() => {
    if (!sessionId) return;

    setStatus('Cloning repository...'); // Initial status

    const intervalId = setInterval(async () => {
      try {
        const response = await getRepoStatus(sessionId);
        setStatus(response.status);
        if (response.status === 'Ready') {
          clearInterval(intervalId);
          if (onReady) {
            onReady();
          }
        }
      } catch (error) {
        console.error('Failed to get repo status:', error);
        setStatus('Error fetching status.');
        clearInterval(intervalId);
      }
    }, 3000); // Poll every 3 seconds

    return () => clearInterval(intervalId); // Cleanup on unmount
  }, [sessionId, onReady]);

  return (
    <div className="p-4 bg-gray-100 rounded-md">
      <p className="text-sm font-medium text-gray-800">
        Indexing Status: <span className="font-bold text-indigo-600">{status}</span>
      </p>
    </div>
  );
};

export default StatusDisplay;
