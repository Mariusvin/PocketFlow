const express = require('express');
const router = express.Router();
const db = require('../lib/db');

// GET /api/workflows - List all workflows
router.get('/', async (req, res) => {
  try {
    // Assuming a user_id is available on req.user (from auth middleware)
    // For now, we'll use a hardcoded owner_id for the query.
    const ownerId = 'user123';
    const { rows } = await db.query('SELECT * FROM workflows WHERE owner_id = $1 ORDER BY updated_at DESC', [ownerId]);
    res.json(rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// POST /api/workflows - Create a new workflow
router.post('/', express.json(), async (req, res) => {
  const { name } = req.body;
  if (!name) {
    return res.status(400).json({ error: 'Workflow name is required.' });
  }
  try {
    // Hardcoded owner_id for now
    const ownerId = 'user123';
    const { rows } = await db.query(
      'INSERT INTO workflows (name, owner_id, flow_data) VALUES ($1, $2, $3) RETURNING *',
      [name, ownerId, { nodes: [], edges: [] }]
    );
    res.status(201).json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// GET /api/workflows/:id - Get a single workflow
router.get('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    const { rows } = await db.query('SELECT * FROM workflows WHERE id = $1', [id]);
    if (rows.length > 0) {
      res.json(rows[0]);
    } else {
      res.status(404).json({ error: 'Workflow not found.' });
    }
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// PUT /api/workflows/:id - Update a workflow
router.put('/:id', express.json(), async (req, res) => {
  const { id } = req.params;
  const { name, description, flow_data } = req.body;

  try {
    // We only update fields that are provided in the request body.
    const { rows } = await db.query('SELECT * FROM workflows WHERE id = $1', [id]);
    if (rows.length === 0) {
      return res.status(404).json({ error: 'Workflow not found.' });
    }

    const currentWorkflow = rows[0];
    const newName = name ?? currentWorkflow.name;
    const newDescription = description ?? currentWorkflow.description;
    const newFlowData = flow_data ?? currentWorkflow.flow_data;

    const { rows: updatedRows } = await db.query(
      'UPDATE workflows SET name = $1, description = $2, flow_data = $3, updated_at = NOW() WHERE id = $4 RETURNING *',
      [newName, newDescription, newFlowData, id]
    );
    res.json(updatedRows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Internal server error' });
  }
});


// DELETE /api/workflows/:id - Delete a workflow
router.delete('/:id', async (req, res) => {
  try {
    const { id } = req.params;
    const result = await db.query('DELETE FROM workflows WHERE id = $1', [id]);
    if (result.rowCount > 0) {
      res.status(204).send(); // No Content
    } else {
      res.status(404).json({ error: 'Workflow not found.' });
    }
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// POST /api/workflows/:id/run - Run a workflow
router.post('/:id/run', async (req, res) => {
  const { id } = req.params;

  try {
    const { rows } = await db.query('SELECT * FROM workflows WHERE id = $1', [id]);
    if (rows.length === 0) {
      return res.status(404).json({ error: 'Workflow not found.' });
    }
    const workflow = rows[0];
    const flowData = workflow.flow_data;

    // This is a placeholder for a more robust Python execution environment.
    // In a real production system, you'd use a dedicated process queue (like Celery)
    // to run Python code safely and scalably. For now, we use `exec` for simplicity.
    const pythonScript = `
import io
import sys
from contextlib import redirect_stdout
# Assuming pocketflow is installed and accessible in the environment
from pocketflow import Node, Flow

# Generic Node that represents any node from the frontend
class GenericPocketflowNode(Node):
    def __init__(self, id, data):
        super().__init__()
        self.node_id = id
        self.node_data = data

    def exec(self, *args, **kwargs):
        # The actual logic would depend on the node's type and data.
        # For now, we just log its execution.
        print(f"Executing node {self.node_data.get('label', self.node_id)}")
        return f"Result from {self.node_id}"

# --- Execution Logic ---
flow_data = ${JSON.stringify(flowData)}
nodes_map = {node['id']: GenericPocketflowNode(node['id'], node.get('data', {})) for node in flow_data.get('nodes', [])}

if not nodes_map:
    print("No nodes in workflow.")
    sys.exit()

edges = flow_data.get('edges', [])
target_nodes = {edge['target'] for edge in edges}
start_nodes = [nodes_map[nid] for nid in nodes_map if nid not in target_nodes]

if not start_nodes:
    print("Error: No start node found (a node with no incoming edges).")
    sys.exit()

for edge in edges:
    source_node = nodes_map.get(edge['source'])
    target_node = nodes_map.get(edge['target'])
    if source_node and target_node:
        source_node >> target_node

# Run the flow and capture output
f = io.StringIO()
with redirect_stdout(f):
    try:
        flow = Flow(start=start_nodes[0])
        flow.run({})
        print("Workflow execution finished.")
    except Exception as e:
        print(f"Error during workflow execution: {e}")

print("---LOG_CAPTURE_END---")
print(f.getvalue())
    `;

    const { exec } = require('child_process');
    exec(`python3 -c "${pythonScript.replace(/"/g, '\\"')}"`, (error, stdout, stderr) => {
      if (error) {
        console.error(`exec error: ${error}`);
        return res.status(500).json({ error: 'Failed to execute workflow', details: stderr });
      }

      const outputParts = stdout.split("---LOG_CAPTURE_END---");
      const logs = (outputParts[1] || '').trim().split('\\n');

      res.json({ success: true, logs });
    });

  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Internal server error' });
  }
});

module.exports = router;
