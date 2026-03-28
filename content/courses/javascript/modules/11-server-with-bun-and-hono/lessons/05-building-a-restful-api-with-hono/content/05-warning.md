---
type: "WARNING"
title: "Common Pitfalls"
---

Common RESTful API mistakes (applies to both Hono and Express):

1. **Using verbs in URLs**:
   ```javascript
   // Wrong!
   app.get('/api/getUsers', ...);
   app.post('/api/createUser', ...);
   
   // Correct!
   app.get('/api/users', ...);
   app.post('/api/users', ...);
   ```
   The HTTP method IS the verb!

2. **Forgetting async for body parsing in Hono**:
   ```javascript
   // Wrong!
   app.post('/api/books', (c) => {
     const body = c.req.json();  // This is a Promise!
   });
   
   // Correct!
   app.post('/api/books', async (c) => {
     const body = await c.req.json();  // Await it!
   });
   ```

3. **Wrong status codes**:
   ```javascript
   // Wrong in Hono
   return c.json({ error: 'Not found' });  // Still 200!
   
   // Correct
   return c.json({ error: 'Not found' }, 404);
   ```

4. **Forgetting to return in Hono**:
   ```javascript
   // Wrong - missing return!
   app.get('/api/books', (c) => {
     c.json(books);
   });
   
   // Correct
   app.get('/api/books', (c) => {
     return c.json(books);
   });
   ```

5. **Not validating input**:
   ```javascript
   // Always validate!
   app.post('/api/books', async (c) => {
     const body = await c.req.json();
     
     if (!body.title || !body.author) {
       return c.json({ error: 'Invalid input' }, 400);
     }
     // ... safe to create book
   });
   ```

6. **Forgetting to handle not found**:
   ```javascript
   app.get('/api/books/:id', (c) => {
     const book = books.find(...);
     if (!book) {
       return c.json({ error: 'Book not found' }, 404);
     }
     return c.json(book);
   });
   ```