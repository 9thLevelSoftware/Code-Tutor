---
type: "LEGACY_COMPARISON"
title: "Node.js/Express Equivalent"
---

The REST principles are identical between Express and Hono. The main differences are in syntax: Hono uses a unified context object and passes status as a second argument.

```javascript
// Express way
import express from 'express';
const app = express();
app.use(express.json());  // Need middleware!

app.get('/api/books/:id', (req, res) => {
  const id = req.params.id;              // Object property
  const book = books.find(b => b.id === id);
  if (!book) {
    return res.status(404).json({ error: 'Not found' });
  }
  res.json(book);
});

app.post('/api/books', (req, res) => {
  const body = req.body;                 // Already parsed
  res.status(201).json({ book: body });
});

app.listen(3000);

// Hono way (cleaner!)
import { Hono } from 'hono';
const app = new Hono();
// No body parsing middleware needed!

app.get('/api/books/:id', (c) => {
  const id = c.req.param('id');          // Method call
  const book = books.find(b => b.id === id);
  if (!book) {
    return c.json({ error: 'Not found' }, 404);
  }
  return c.json(book);
});

app.post('/api/books', async (c) => {
  const body = await c.req.json();       // Async!
  return c.json({ book: body }, 201);
});

export default app;  // That's it!
```
