import { useRouter } from 'next/router';
import Head from 'next/head';
import { useCallback, useEffect, useState } from 'react';
import { useNodesState, useEdgesState, addEdge } from 'reactflow';
import Canvas from '../../features/editor/Canvas'; // Corrected path
import { getWorkflow, updateWorkflow } from '../../lib/api'; // Corrected path
import 'reactflow/dist/style.css';

const WorkflowEditor = ({ workflowId }) => {
  const [nodes, setNodes, onNodesChange] = useNodesState([]);
  const [edges, setEdges, onEdgesChange] = useEdgesState([]);
  const [workflowName, setWorkflowName] = useState('Loading...');
  const [error, setError] = useState(null);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    if (workflowId) {
      getWorkflow(workflowId)
        .then((data) => {
          setWorkflowName(data.name);
          // react-flow expects an array, ensure flow_data has nodes and edges
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
    setIsSaving(true);
    setError(null);
    try {
      const flow_data = { nodes, edges };
      await updateWorkflow(workflowId, { flow_data });
    } catch (err) {
      setError('Failed to save workflow.');
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div style={{ height: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Head>
        <title>Editing {workflowName} - Flow Studio</title>
      </Head>
      <header style={{ padding: '0.5rem 1rem', borderBottom: '1px solid #eee', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h1>{workflowName}</h1>
        <div>
          {error && <span style={{ color: 'red', marginRight: '1rem' }}>{error}</span>}
          <button onClick={handleSave} disabled={isSaving} style={{ padding: '0.5rem 1rem', cursor: 'pointer' }}>
            {isSaving ? 'Saving...' : 'Save'}
          </button>
        </div>
      </header>
      <div style={{ flex: 1 }}>
        <Canvas
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onConnect={onConnect}
        />
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
