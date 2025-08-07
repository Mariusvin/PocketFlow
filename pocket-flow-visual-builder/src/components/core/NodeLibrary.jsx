import React from 'react';
import { availableNodes } from '../../data/available-nodes.js';

const NodeLibrary = () => {
  const onDragStart = (event, nodeType) => {
    event.dataTransfer.setData('application/reactflow', nodeType);
    event.dataTransfer.effectAllowed = 'move';
  };

  return (
    <aside>
      <h2 className="text-lg font-semibold mb-4">Node Library</h2>
      <div className="space-y-2">
        {availableNodes.map((node) => (
          <div
            key={node.type}
            className="p-3 border-2 border-dashed border-gray-600 rounded-md cursor-grab bg-gray-700 hover:bg-gray-600 hover:border-blue-500"
            onDragStart={(event) => onDragStart(event, node.type)}
            draggable
          >
            {node.label}
          </div>
        ))}
      </div>
    </aside>
  );
};

export default NodeLibrary;
