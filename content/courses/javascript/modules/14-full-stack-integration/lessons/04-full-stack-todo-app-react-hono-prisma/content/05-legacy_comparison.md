---
type: "LEGACY_COMPARISON"
title: "Express Backend Equivalent"
---

If you encounter older codebases using Express, here's the same Todo API. The main differences are: Express uses (req, res) instead of (c), res.json() instead of return c.json(), and requires a separate cors package.

```javascript
// Express + Prisma Todo API
import express from 'express';
import cors from 'cors';  // Separate package!
import { PrismaClient } from '@prisma/client';

const app = express();
const prisma = new PrismaClient();

app.use(cors());
app.use(express.json());  // Required for req.body!

// GET all todos
app.get('/api/todos', async (req, res) => {
  const todos = await prisma.todo.findMany({
    where: { userId: req.user.id },
    orderBy: { createdAt: 'desc' }
  });
  res.json(todos);
});

// POST new todo
app.post('/api/todos', async (req, res) => {
  const { title } = req.body;
  
  if (!title) {
    return res.status(400).json({ error: 'Title required' });
  }
  
  const todo = await prisma.todo.create({
    data: { title, userId: req.user.id }
  });
  
  res.status(201).json(todo);
});

// PATCH update todo
app.patch('/api/todos/:id', async (req, res) => {
  const { id } = req.params;
  const { completed, title } = req.body;
  
  const todo = await prisma.todo.update({
    where: { id: parseInt(id) },
    data: { completed, title }
  });
  
  res.json(todo);
});

// DELETE todo
app.delete('/api/todos/:id', async (req, res) => {
  const { id } = req.params;
  
  await prisma.todo.delete({
    where: { id: parseInt(id) }
  });
  
  res.json({ message: 'Deleted' });
});

app.listen(4000, () => {
  console.log('API running on http://localhost:4000');
});
```
