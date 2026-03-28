---
type: "WARNING"
title: "Common Pitfalls"
---

Common Hono mistakes:

1. **Forgetting to return the response**:
   ```javascript
   app.get('/api/users', (c) => {
     c.json({ data: 'hello' });  // Missing return!
   });
   
   // Correct:
   app.get('/api/users', (c) => {
     return c.json({ data: 'hello' });  // Always return!
   });
   ```

2. **Forgetting async for body parsing**:
   ```javascript
   // Wrong - req.json() is async!
   app.post('/api/users', (c) => {
     const body = c.req.json();  // This is a Promise!
   });
   
   // Correct:
   app.post('/api/users', async (c) => {
     const body = await c.req.json();  // Await it!
   });
   ```

3. **Using Express patterns by mistake**:
   ```javascript
   // Express style (won't work in Hono!):
   app.get('/users', (req, res) => {
     res.json({ data });
   });
   
   // Hono style:
   app.get('/users', (c) => {
     return c.json({ data });
   });
   ```

4. **Route order still matters**:
   ```javascript
   app.get('/api/users/active', ...);  // Specific first
   app.get('/api/users/:id', ...);     // Generic after
   ```

5. **Not exporting for Bun**:
   ```javascript
   // For Bun to serve your app:
   export default app;  // Don't forget this!
   
   // Or with custom port:
   export default {
     port: 3000,
     fetch: app.fetch
   };
   ```

6. **Status code position**:
   ```javascript
   // Hono: status is SECOND argument
   return c.json({ error: 'Not found' }, 404);
   
   // NOT like Express: res.status(404).json(...)
   ```