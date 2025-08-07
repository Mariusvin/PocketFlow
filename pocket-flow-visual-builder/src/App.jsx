import React from 'react';
import Sidebar from './components/layout/Sidebar';
import Canvas from './components/canvas/Canvas';
import PropertiesPanel from './components/layout/PropertiesPanel';
import LogPanel from './components/core/LogPanel';

function App() {
  return (
    <div className="flex flex-col h-screen bg-gray-900 text-white">
      <header className="bg-gray-800 p-3 shadow-md z-10">
        <h1 className="text-xl font-bold">Pocket Flow Visual Builder</h1>
      </header>
      <main className="flex flex-grow">
        <div className="w-1/5 bg-gray-800 p-4 overflow-y-auto">
          <Sidebar />
        </div>
        <div className="flex-grow bg-gray-700">
          <Canvas />
        </div>
        <div className="w-1/4 bg-gray-800 p-4 overflow-y-auto">
          <PropertiesPanel />
        </div>
      </main>
      <footer className="bg-gray-800 p-2 h-48 resize-y overflow-y-auto">
        <LogPanel />
      </footer>
    </div>
  );
}

export default App;
