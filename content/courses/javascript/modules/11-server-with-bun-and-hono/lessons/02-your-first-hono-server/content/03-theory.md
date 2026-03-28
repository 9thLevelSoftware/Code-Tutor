---
type: "THEORY"
title: "Breaking Down the Syntax"
---

Understanding Hono fundamentals:

1. **Creating a Hono App**:
   ```javascript
   import { Hono } from 'hono';
   const app = new Hono();
   ```
   - `new Hono()` creates a new application
   - `app` is your main server object

2. **Route Methods** (HTTP verbs):
   - `app.get()` - Read data
   - `app.post()` - Create data
   - `app.put()` - Update data (replace)
   - `app.patch()` - Update data (partial)
   - `app.delete()` - Delete data

3. **The Context Object (c)**:
   Unlike Express with separate req/res, Hono uses ONE context object:
   ```javascript
   app.get('/hello', (c) => {
     // c has everything!
     return c.text('Hello!');
   });
   ```

4. **Accessing Request Data**:
   - `c.req.param('id')` - URL parameters (`/users/:id`)
   - `c.req.query('search')` - Query strings (`?search=hello`)
   - `await c.req.json()` - POST/PUT body (async!)
   - `c.req.header('Authorization')` - HTTP headers

5. **Response Helpers**:
   - `c.text('Hello')` - Send text
   - `c.json({ data })` - Send JSON
   - `c.json(data, 201)` - JSON with status code
   - `c.html('<h1>Hi</h1>')` - Send HTML
   - `c.redirect('/other')` - Redirect

6. **Status Codes**:
   - Pass as second argument: `c.json(data, 201)`
   - 200: OK (success)
   - 201: Created (new resource)
   - 400: Bad Request (client error)
   - 404: Not Found
   - 500: Internal Server Error

7. **Running the Server (Bun)**:
   ```javascript
   // app.ts
   import { Hono } from 'hono';
   const app = new Hono();
   
   app.get('/', (c) => c.text('Hello!'));
   
   export default app;  // Bun auto-serves!
   ```
   Run with: `bun run app.ts`