import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { askQuestion } from '../services/api';
import StatusDisplay from './StatusDisplay';
import Message from './Message';

const ChatWindow = () => {
  const { sessionId } = useParams();
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isIndexing, setIsIndexing] = useState(true);

  const handleIndexingReady = () => {
    setIsIndexing(false);
    setMessages([{
      sender: 'ai',
      content: {
        type: 'multi-part',
        content: [{ type: 'markdown', text: 'Repository ready! What would you like to know?' }]
      }
    }]);
  };

  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (!input.trim()) return;

    const userMessage = { sender: 'user', content: { type: 'multi-part', content: [{ type: 'markdown', text: input }] } };
    setMessages((prev) => [...prev, userMessage]);
    setInput('');
    setIsLoading(true);

    try {
      const response = await askQuestion(sessionId, input);
      const aiMessage = { sender: 'ai', content: response.answer };
      setMessages((prev) => [...prev, aiMessage]);
    } catch (error) {
      console.error('Failed to send message:', error);
      const errorMessage = {
        sender: 'ai',
        content: {
          type: 'multi-part',
          content: [{ type: 'markdown', text: 'Sorry, I encountered an error. Please try again.' }]
        }
      };
      setMessages((prev) => [...prev, errorMessage]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex flex-col h-screen bg-gray-100">
      <header className="p-4 bg-white border-b">
        <h1 className="text-xl font-bold text-gray-800">Codebase Q&A</h1>
      </header>

      <main className="flex-1 p-4 overflow-y-auto">
        <div className="max-w-3xl mx-auto">
          <StatusDisplay sessionId={sessionId} onReady={handleIndexingReady} />

          <div className="mt-4 space-y-4">
            {messages.map((msg, index) => (
              <Message key={index} message={msg} />
            ))}
            {isLoading && <Message message={{ sender: 'ai', isLoading: true }} />}
          </div>
        </div>
      </main>

      {!isIndexing && (
        <footer className="p-4 bg-white border-t">
          <div className="max-w-3xl mx-auto">
            <form onSubmit={handleSendMessage} className="flex items-center">
              <input
                type="text"
                value={input}
                onChange={(e) => setInput(e.target.value)}
                className="flex-1 px-3 py-2 border border-gray-300 rounded-l-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="Ask a question about the codebase..."
                disabled={isLoading}
              />
              <button
                type="submit"
                className="px-4 py-2 text-white bg-indigo-600 rounded-r-md hover:bg-indigo-700 disabled:bg-indigo-300"
                disabled={isLoading}
              >
                Send
              </button>
            </form>
          </div>
        </footer>
      )}
    </div>
  );
};

export default ChatWindow;
