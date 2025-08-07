const express = require('express');
const app = express();
const port = 3001; // Port for the backend server

app.get('/api', (req, res) => {
  res.send({ message: 'Hello from the Flow Studio Backend!' });
});

app.listen(port, () => {
  console.log(`Backend server listening at http://localhost:${port}`);
});
