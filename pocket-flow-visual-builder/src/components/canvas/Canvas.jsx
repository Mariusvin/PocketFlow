import React, { useRef } from 'react';
import ReactFlow, { ReactFlowProvider } from 'react-flow';
import useStore from '../../store/useStore';

import 'react-flow/dist/style.css';

const selector = (state) => ({
  nodes: state.nodes,
  edges: state.edges,
  onNodesChange: state.onNodesChange,
  onEdgesChange: state.onEdgesChange,
  addNode: state.addNode,
});

const CanvasContent = () => {
  const reactFlowWrapper = useRef(null);
  const { nodes, edges, onNodesChange, onEdgesChange, addNode } = useStore(selector);

  const onDragOver = (event) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
  };

  const onDrop = (event) => {
    event.preventDefault();

    const type = event.dataTransfer.getData('application/reactflow');
    if (typeof type === 'undefined' || !type) {
      return;
    }

    // We need to project the client position to the react-flow pane
    const position = {
      x: event.clientX - reactFlowWrapper.current.getBoundingClientRect().left,
      y: event.clientY - reactFlowWrapper.current.getBoundingClientRect().top,
    };

    addNode(type, position);
  };

  return (
    <div className="reactflow-wrapper" ref={reactFlowWrapper} style={{ height: '100%', width: '100%' }}>
      <ReactFlow
        nodes={nodes}
        edges={edges}
        onNodesChange={onNodesChange}
        onEdgesChange={onEdgesChange}
        onDragOver={onDragOver}
        onDrop={onDrop}
        fitView
      >
      </ReactFlow>
    </div>
  );
};

// We need to wrap with ReactFlowProvider to use hooks like useReactFlow
const Canvas = () => (
  <ReactFlowProvider>
    <CanvasContent />
  </ReactFlowProvider>
);


export default Canvas;
