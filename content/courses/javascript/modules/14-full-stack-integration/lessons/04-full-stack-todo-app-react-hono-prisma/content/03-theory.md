---
type: "THEORY"
title: "Breaking Down the Syntax"
---

Complete full-stack implementation:

**1. Database (Prisma Schema)**:
```prisma
// schema.prisma
model Todo {
  id        Int      @id @default(autoincrement())
  title     String
  completed Boolean  @default(false)
  userId    Int
  createdAt DateTime @default(now())
}
```

**2. Backend (Hono + Prisma)**:
```javascript
// server.js
import { Hono } from 'hono';
import { cors } from 'hono/cors';
import { serve } from '@hono/node-server';
import { PrismaClient } from '@prisma/client';

const app = new Hono();
const prisma = new PrismaClient();

app.use('*', cors());  // Enable CORS!

// GET all todos
app.get('/api/todos', async (c) => {
  const todos = await prisma.todo.findMany({
    where: { userId: c.get('userId') },
    orderBy: { createdAt: 'desc' }
  });
  return c.json(todos);
});

// POST new todo
app.post('/api/todos', async (c) => {
  const { title } = await c.req.json();
  
  if (!title) {
    return c.json({ error: 'Title required' }, 400);
  }
  
  const todo = await prisma.todo.create({
    data: { title, userId: c.get('userId') }
  });
  
  return c.json(todo, 201);
});

// PATCH update todo
app.patch('/api/todos/:id', async (c) => {
  const id = parseInt(c.req.param('id'));
  const { completed, title } = await c.req.json();
  
  const todo = await prisma.todo.update({
    where: { id },
    data: { completed, title }
  });
  
  return c.json(todo);
});

// DELETE todo
app.delete('/api/todos/:id', async (c) => {
  const id = parseInt(c.req.param('id'));
  
  await prisma.todo.delete({
    where: { id }
  });
  
  return c.json({ message: 'Deleted' });
});

serve(app, (info) => {
  console.log(`API running on http://localhost:${info.port}`);
});
```

**3. Frontend (React)**:
```jsx
// TodoApp.jsx
import { useState, useEffect } from 'react';

const API_URL = 'http://localhost:3000';

function TodoApp() {
  const [todos, setTodos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [newTitle, setNewTitle] = useState('');
  
  // Fetch todos on mount
  useEffect(() => {
    fetchTodos();
  }, []);
  
  async function fetchTodos() {
    const res = await fetch(`${API_URL}/api/todos`);
    const data = await res.json();
    setTodos(data);
    setLoading(false);
  }
  
  async function addTodo(e) {
    e.preventDefault();
    
    await fetch(`${API_URL}/api/todos`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ title: newTitle })
    });
    
    setNewTitle('');
    fetchTodos();
  }
  
  async function toggleTodo(id, completed) {
    await fetch(`${API_URL}/api/todos/${id}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ completed: !completed })
    });
    
    fetchTodos();
  }
  
  async function deleteTodo(id) {
    await fetch(`${API_URL}/api/todos/${id}`, {
      method: 'DELETE'
    });
    
    fetchTodos();
  }
  
  if (loading) return <div>Loading...</div>;
  
  return (
    <div>
      <h1>My Todos</h1>
      
      <form onSubmit={addTodo}>
        <input
          value={newTitle}
          onChange={(e) => setNewTitle(e.target.value)}
          placeholder="New todo..."
        />
        <button type="submit">Add</button>
      </form>
      
      <ul>
        {todos.map(todo => (
          <li key={todo.id}>
            <input
              type="checkbox"
              checked={todo.completed}
              onChange={() => toggleTodo(todo.id, todo.completed)}
            />
            <span style={{ textDecoration: todo.completed ? 'line-through' : 'none' }}>
              {todo.title}
            </span>
            <button onClick={() => deleteTodo(todo.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
```