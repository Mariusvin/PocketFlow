const express = require('express');
const cors = require('cors');
const axios = require('axios');
const { GoogleGenerativeAI } = require('@google/generative-ai');

const app = express();
const port = 3001;

// IMPORTANT: In a production environment, this key should be stored securely,
// for example, in an environment variable.
const GEMINI_API_KEY = "AIzaSyDWWBEkX9nYVpBj3sHOGP87SnKumidmBik";

const genAI = new GoogleGenerativeAI(GEMINI_API_KEY);

app.use(cors());
app.use(express.json());

app.get('/', (req, res) => {
  res.send('Hello from the backend server!');
});

app.post('/api/generate-suggestions', async (req, res) => {
  const { keywords } = req.body;

  if (!keywords) {
    return res.status(400).json({ error: 'Keywords are required.' });
  }

  try {
    const model = genAI.getGenerativeModel({ model: "gemini-1.5-flash" }); // Using a fast model

    const prompt = `Generate a list of 10 creative and brandable domain name ideas based on the following keywords: "${keywords}". The domain names should be a mix of .com, .io, and .ai suffixes. Return the list as a JSON array of strings. For example: ["domain1.com", "domain2.io"]`;

    const result = await model.generateContent(prompt);
    const response = await result.response;
    const text = await response.text();

    // Clean the response to get only the JSON part
    const jsonString = text.match(/\[.*\]/s)[0];
    const suggestions = JSON.parse(jsonString);

    res.json({ suggestions });
  } catch (error) {
    console.error('Error generating suggestions with Gemini:', error);
    res.status(500).json({ error: 'Failed to generate domain suggestions.' });
  }
});

app.post('/api/check-domains', async (req, res) => {
  const { domains } = req.body;

  if (!domains || !Array.isArray(domains)) {
    return res.status(400).json({ error: 'Invalid request body. "domains" should be an array.' });
  }

  const apiKey = 'demokey'; // Using the demo key provided by WhoAPI

  try {
    const availabilityPromises = domains.map(domain => {
      const url = `http://api.whoapi.com/?domain=${domain.name}&r=taken&apikey=${apiKey}`;
      return axios.get(url);
    });

    const responses = await Promise.all(availabilityPromises);

    const results = responses.map((response, index) => {
      const isTaken = response.data.taken === '1';
      return {
        ...domains[index],
        available: !isTaken,
      };
    });

    res.json(results);
  } catch (error) {
    console.error('Error checking domain availability:', error);
    res.status(500).json({ error: 'Failed to check domain availability.' });
  }
});

app.listen(port, () => {
  console.log(`Server listening at http://localhost:${port}`);
});
