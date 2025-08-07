// This file will contain all the functions for making API calls to the backend.

export const submitRepository = async (url) => {
  console.log(`Submitting repository: ${url}`);
  // Simulate API call
  return { sessionId: 'mock-session-id', status: 'PENDING' };
};

export const getRepoStatus = async (sessionId) => {
  console.log(`Getting status for session: ${sessionId}`);
  // Simulate API call
  return { status: 'Ready' };
};

export const askQuestion = async (sessionId, message) => {
  console.log(`Asking question for session ${sessionId}: ${message}`);
  // Simulate API call
  return {
    answer: {
      type: 'multi-part',
      content: [
        {
          type: 'markdown',
          text: 'This is a mock answer from the backend.',
        },
      ],
    },
  };
};
