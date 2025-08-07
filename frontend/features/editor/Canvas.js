import React from 'react';
import { MiniMap, Controls, Background } from 'reactflow';

// The Canvas component is now just for rendering the controls and background.
// The main ReactFlow provider is in the editor page.
export default function Canvas() {
  return (
    <>
      <Controls />
      <MiniMap />
      <Background variant="dots" gap={12} size={1} />
    </>
  );
}
