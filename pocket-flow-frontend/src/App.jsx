import React from 'react';
import { Routes, Route } from 'react-router-dom';
import RepoInputForm from './components/RepoInputForm';
import ChatWindow from './components/ChatWindow';

function App() {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<RepoInputForm />} />
        <Route path="/chat/:sessionId" element={<ChatWindow />} />
      </Routes>
    </div>
  );
}

export default App;
