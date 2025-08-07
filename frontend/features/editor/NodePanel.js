import React from 'react';

const DraggableNode = ({ type, label }) => {
  const onDragStart = (event, nodeType) => {
    event.dataTransfer.setData('application/reactflow', nodeType);
    event.dataTransfer.effectAllowed = 'move';
  };

  return (
    <div
      style={{
        padding: '10px',
        border: '1px solid #ddd',
        borderRadius: '5px',
        marginBottom: '10px',
        cursor: 'grab',
        textAlign: 'center',
      }}
      onDragStart={(event) => onDragStart(event, type)}
      draggable
    >
      {label}
    </div>
  );
};

export default function NodePanel() {
  return (
    <aside style={{
      width: '250px',
      padding: '1rem',
      borderRight: '1px solid #eee',
      background: '#f8f8f8',
    }}>
      <h2 style={{ marginTop: 0 }}>Nodes</h2>
      <DraggableNode type="input" label="Input Node" />
      <DraggableNode type="default" label="Default Node" />
      <DraggableNode type="output" label="Output Node" />
    </aside>
  );
}
