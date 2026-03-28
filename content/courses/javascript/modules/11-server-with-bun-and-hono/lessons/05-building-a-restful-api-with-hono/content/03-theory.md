---
type: "THEORY"
title: "Breaking Down the Syntax"
---

RESTful API design principles (same for Hono and Express):

1. **HTTP Methods Map to CRUD**:
   - CREATE -> POST
   - READ -> GET
   - UPDATE -> PUT (complete) or PATCH (partial)
   - DELETE -> DELETE

2. **RESTful URL Patterns**:
   ```
   GET    /api/resources      -> List all
   GET    /api/resources/:id  -> Get one
   POST   /api/resources      -> Create new
   PUT    /api/resources/:id  -> Update (replace)
   PATCH  /api/resources/:id  -> Update (partial)
   DELETE /api/resources/:id  -> Delete
   ```

3. **Hono Response Patterns**:
   ```javascript
   // Success with default 200
   return c.json({ data: books });
   
   // Created - 201
   return c.json({ message: 'Created' }, 201);
   
   // Error - 404
   return c.json({ error: 'Not found' }, 404);
   
   // Validation error - 400
   return c.json({ error: 'Invalid input' }, 400);
   ```

4. **Status Codes**:
   - 200 OK - Successful GET, PUT, PATCH
   - 201 Created - Successful POST
   - 204 No Content - Successful DELETE (no body)
   - 400 Bad Request - Validation error
   - 404 Not Found - Resource doesn't exist
   - 500 Internal Server Error - Server problem

5. **Accessing Data in Hono**:
   ```javascript
   // URL parameter
   const id = c.req.param('id');
   
   // Request body (async!)
   const body = await c.req.json();
   
   // Query parameter
   const search = c.req.query('search');
   ```

6. **Validation**:
   ```javascript
   app.post('/api/books', async (c) => {
     const body = await c.req.json();
     
     if (!body.title) {
       return c.json({ error: 'Title is required' }, 400);
     }
     // ... create book
   });
   ```

7. **Real Hono + Bun Server**:
   ```javascript
   import { Hono } from 'hono';
   const app = new Hono();
   
   app.get('/api/books', (c) => c.json(books));
   
   export default app;  // Bun serves this!
   ```