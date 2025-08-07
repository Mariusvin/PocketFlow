const express = require('express');
const router = express.Router();
const db = require('../lib/db'); // Using our mock db
const { randomUUID } = require('crypto');

// A temporary in-memory store for workflows to simulate a database
let mockWorkflows = [
  { id: randomUUID(), name: 'My First Workflow', description: 'A sample workflow.', owner_id: 'user123', created_at: new Date() },
  { id: randomUUID(), name: 'Web Scraper', description: 'Scrapes a website for data.', owner_id: 'user123', created_at: new Date() },
];

// GET /api/workflows - List all workflows
router.get('/', async (req, res) => {
  // In a real app, you'd query the database:
  // const { rows } = await db.query('SELECT * FROM workflows WHERE owner_id = $1', [req.user.id]);
  res.json(mockWorkflows);
});

// POST /api/workflows - Create a new workflow
router.post('/', express.json(), async (req, res) => {
  const { name } = req.body;
  if (!name) {
    return res.status(400).json({ error: 'Workflow name is required.' });
  }
  const newWorkflow = {
    id: randomUUID(),
    name,
    description: '',
    flow_data: {},
    owner_id: 'user123', // Mock owner
    created_at: new Date(),
    updated_at: new Date(),
  };
  mockWorkflows.push(newWorkflow);
  // In a real app, you'd insert into the database
  res.status(201).json(newWorkflow);
});

// GET /api/workflows/:id - Get a single workflow
router.get('/:id', (req, res) => {
  const { id } = req.params;
  const workflow = mockWorkflows.find(wf => wf.id === id);
  if (workflow) {
    res.json(workflow);
  } else {
    res.status(404).json({ error: 'Workflow not found.' });
  }
});

// PUT /api/workflows/:id - Update a workflow
router.put('/:id', express.json(), (req, res) => {
  const { id } = req.params;
  const { name, description, flow_data } = req.body;

  const workflowIndex = mockWorkflows.findIndex(wf => wf.id === id);

  if (workflowIndex === -1) {
    return res.status(404).json({ error: 'Workflow not found.' });
  }

  const updatedWorkflow = {
    ...mockWorkflows[workflowIndex],
    name: name ?? mockWorkflows[workflowIndex].name,
    description: description ?? mockWorkflows[workflowIndex].description,
    flow_data: flow_data ?? mockWorkflows[workflowIndex].flow_data,
    updated_at: new Date(),
  };

  mockWorkflows[workflowIndex] = updatedWorkflow;

  res.json(updatedWorkflow);
});


// DELETE /api/workflows/:id - Delete a workflow
router.delete('/:id', (req, res) => {
  const { id } = req.params;
  const initialLength = mockWorkflows.length;
  mockWorkflows = mockWorkflows.filter(wf => wf.id !== id);

  if (mockWorkflows.length === initialLength) {
    return res.status(404).json({ error: 'Workflow not found.' });
  }

  // In a real app, you'd delete from the database
  res.status(204).send(); // No Content
});

module.exports = router;
