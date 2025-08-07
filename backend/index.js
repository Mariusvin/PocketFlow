const express = require('express');
const app = express();
const port = 3001; // Port for the backend server

app.use(express.json()); // Middleware to parse JSON bodies

// Import and use the workflows router
const workflowsRouter = require('./routes/workflows');
app.use('/api/workflows', workflowsRouter);

app.get('/api', (req, res) => {
  res.send({ message: 'Hello from the Flow Studio Backend!' });
});

app.listen(port, () => {
  console.log(`Backend server listening at http://localhost:${port}`);
});
