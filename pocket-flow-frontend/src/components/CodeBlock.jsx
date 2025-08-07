import React, { useState } from 'react';
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter';
import { atomDark } from 'react-syntax-highlighter/dist/esm/styles/prism';

const CodeBlock = ({ language, code }) => {
  const [isCopied, setIsCopied] = useState(false);

  const handleCopy = () => {
    navigator.clipboard.writeText(code);
    setIsCopied(true);
    setTimeout(() => setIsCopied(false), 2000);
  };

  return (
    <div className="relative my-4 rounded-md bg-gray-800">
      <div className="flex items-center justify-between px-4 py-2 text-xs text-gray-400 border-b border-gray-700">
        <span>{language}</span>
        <button
          onClick={handleCopy}
          className="px-2 py-1 text-xs font-semibold text-white bg-gray-600 rounded-md hover:bg-gray-500"
        >
          {isCopied ? 'Copied!' : 'Copy'}
        </button>
      </div>
      <SyntaxHighlighter language={language} style={atomDark} customStyle={{ margin: 0, borderRadius: '0 0 0.375rem 0.375rem' }}>
        {code}
      </SyntaxHighlighter>
    </div>
  );
};

export default CodeBlock;
