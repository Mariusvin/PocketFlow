import { useRouter } from 'next/router';
import Head from 'next/head';
import { useCallback, useEffect, useState, useRef } from 'react';
import ReactFlow, { useNodesState, useEdgesState, addEdge } from 'reactflow';
import Canvas from '../../features/editor/Canvas';
import NodePanel from '../../features/editor/NodePanel';
import { getWorkflow, updateWorkflow } from '../../lib/api';
import 'reactflow/dist/style.css';

let id = 0;
const getId = () => `dndnode_${id++}`;

const WorkflowEditor = ({ workflowId }) => {
  const reactFlowWrapper = useRef(null);
  const [nodes, setNodes, onNodesChange] = useNodesState([]);
  const [edges, setEdges, onEdgesChange] = useEdgesState([]);
  const [reactFlowInstance, setReactFlowInstance] = useState(null);
  const [workflowName, setWorkflowName] = useState('Loading...');
  const [error, setError] = useState(null);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    if (workflowId) {
      getWorkflow(workflowId)
        .then((data) => {
          setWorkflowName(data.name);
          if (data.flow_data && data.flow_data.nodes) {
            setNodes(data.flow_data.nodes);
            setEdges(data.flow_data.edges || []);
          }
        })
        .catch(() => setError('Failed to load workflow.'));
    }
  }, [workflowId, setNodes, setEdges]);

  const onConnect = useCallback((params) => setEdges((eds) => addEdge(params, eds)), [setEdges]);

  const handleSave = async () => {
    if (!reactFlowInstance) return;
    setIsSaving(true);
    setError(null);
    try {
      const flow_data = reactFlowInstance.toObject();
      await updateWorkflow(workflowId, { flow_data });
    } catch (err) {
      setError('Failed to save workflow.');
    } finally {
      setIsSaving(false);
    }
  };

  const onDragOver = useCallback((event) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
  }, []);

  const onDrop = useCallback(
    (event) => {
      event.preventDefault();

      const type = event.dataTransfer.getData('application/reactflow');
      if (typeof type === 'undefined' || !type) {
        return;
      }

      const position = reactFlowInstance.screenToFlowPosition({
        x: event.clientX,
        y: event.clientY,
      });
      const newNode = {
        id: getId(),
        type,
        position,
        data: { label: `${type} node` },
      };

      setNodes((nds) => nds.concat(newNode));
    },
    [reactFlowInstance, setNodes]
  );


  return (
    <div style={{ height: '100vh', display: 'flex', flexDirection: 'column' }} ref={reactFlowWrapper}>
      <Head>
        <title>Editing {workflowName} - Flow Studio</title>
      </Head>
      <header style={{ padding: '0.5rem 1rem', borderBottom: '1px solid #eee', display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: 'white', zIndex: 10 }}>
        <h1>{workflowName}</h1>
        <div>
          {error && <span style={{ color: 'red', marginRight: '1rem' }}>{error}</span>}
          <button onClick={handleSave} disabled={isSaving} style={{ padding: '0.5rem 1rem', cursor: 'pointer' }}>
            {isSaving ? 'Saving...' : 'Save'}
          </button>
        </div>
      </header>
      <div style={{ flex: 1, display: 'flex' }}>
        <NodePanel />
        <div style={{ flex: 1 }} >
          <ReactFlow
            nodes={nodes}
            edges={edges}
            onNodesChange={onNodesChange}
            onEdgesChange={onEdgesChange}
            onConnect={onConnect}
            onInit={setReactFlowInstance}
            onDrop={onDrop}
            onDragOver={onDragOver}
            fitView
          >
            <Canvas />
          </ReactFlow>
        </div>
      </div>
    </div>
  );
};

const EditorPage = () => {
  const router = useRouter();
  const { workflowId } = router.query;

  if (!router.isReady) {
    return <div>Loading...</div>;
  }

  return <WorkflowEditor workflowId={workflowId} />;
};

export default EditorPage;
