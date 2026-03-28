---
type: "WARNING"
title: "Common Pitfalls"
---

Common full-stack integration mistakes:

1. **Forgetting to enable CORS**:
   ```javascript
   // Hono backend missing:
   app.use('*', cors());  // Add this!
   ```

2. **Wrong API URL**:
   ```jsx
   // Wrong!
   fetch('localhost:4000/api/todos')  // Missing http://
   
   // Correct!
   fetch('http://localhost:4000/api/todos')
   
   // Best!
   const API_URL = import.meta.env.VITE_API_URL;
   fetch(`${API_URL}/api/todos`);
   ```

3. **Not refreshing data after mutations**:
   ```jsx
   // Wrong! (UI doesn't update)
   async function addTodo() {
     await fetch('/api/todos', { method: 'POST', ... });
     // Forgot to refresh!
   }
   
   // Correct!
   async function addTodo() {
     await fetch('/api/todos', { method: 'POST', ... });
     fetchTodos();  // Refresh the list!
   }
   ```

4. **Not validating on backend (Hono)**:
   ```javascript
   // NEVER trust frontend data!
   app.post('/api/todos', async (c) => {
     const { title } = await c.req.json();
     
     // Validate!
     if (!title || title.trim().length === 0) {
       return c.json({ error: 'Title required' }, 400);
     }
     
     // Now safe to create
     const todo = await prisma.todo.create({ data: { title } });
     return c.json(todo);
   });
   ```

5. **Hardcoded user IDs** (security issue!):
   ```javascript
   // Wrong! (any user can access any todo)
   app.get('/api/todos', async (c) => {
     const todos = await prisma.todo.findMany();
     return c.json(todos);
   });
   
   // Correct! (filter by authenticated user)
   app.get('/api/todos', async (c) => {
     const todos = await prisma.todo.findMany({
       where: { userId: c.get('userId') }  // From auth middleware
     });
     return c.json(todos);
   });
   ```