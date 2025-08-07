import React from 'react';
import ReactMarkdown from 'react-markdown';
import CodeBlock from './CodeBlock';

const Message = ({ message }) => {
  const { sender, content, isLoading } = message;

  const renderContent = (part, index) => {
    if (part.type === 'markdown') {
      return <ReactMarkdown key={index}>{part.text}</ReactMarkdown>;
    }
    if (part.type === 'code') {
      return <CodeBlock key={index} language={part.language} code={part.code} />;
    }
    return null;
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-start">
        <div className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg">
          <span className="inline-block w-3 h-3 mr-1 bg-gray-400 rounded-full animate-pulse"></span>
          <span className="inline-block w-3 h-3 mr-1 bg-gray-400 rounded-full animate-pulse delay-75"></span>
          <span className="inline-block w-3 h-3 bg-gray-400 rounded-full animate-pulse delay-150"></span>
        </div>
      </div>
    );
  }

  const messageClass = sender === 'user'
    ? 'bg-indigo-500 text-white self-end'
    : 'bg-white text-gray-800 self-start';

  return (
    <div className={`flex ${sender === 'user' ? 'justify-end' : 'justify-start'}`}>
      <div className={`max-w-xl px-4 py-2 rounded-lg shadow-md ${messageClass}`}>
        <div className="prose max-w-none">
          {content && content.content.map(renderContent)}
        </div>
      </div>
    </div>
  );
};

export default Message;
