---
type: "LEGACY_COMPARISON"
title: "Node.js/Express Equivalent"
---

Hono and Express are similar, but Hono is simpler and more modern. The main difference is Hono's unified context object (c) vs Express's separate req/res.

```javascript
// Express way (Node.js)
import express from 'express';
const app = express();

app.use(express.json()); // Need middleware for JSON!

// Two separate objects: req and res
app.get('/api/users', (req, res) => {
  res.json([{ id: 1, name: 'Alice' }]);
});

app.post('/api/users', (req, res) => {
  const body = req.body;  // Already parsed
  res.status(201).json({ message: 'Created', user: body });
});

app.listen(3000, () => {
  console.log('Server on port 3000');
});

// vs Hono way (cleaner!)
// import { Hono } from 'hono';
// const app = new Hono();
// 
// app.get('/api/users', (c) => c.json([...]));
// app.post('/api/users', async (c) => {
//   const body = await c.req.json();
//   return c.json({ message: 'Created' }, 201);
// });
// 
// export default app;  // That's it!
```
