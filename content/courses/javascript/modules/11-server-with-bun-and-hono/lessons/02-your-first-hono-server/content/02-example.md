---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// Hono - The Ultra-Fast Web Framework (2025)
// Note: This is simulated Hono code for learning

// BASIC HONO SERVER STRUCTURE

// In real Bun:
// import { Hono } from 'hono';
// const app = new Hono();

// For this demo, we'll simulate Hono behavior:
class HonoApp {
  constructor() {
    this.routes = [];
  }
  
  get(path, handler) {
    this.routes.push({ method: 'GET', path, handler });
    console.log(`Route registered: GET ${path}`);
  }
  
  post(path, handler) {
    this.routes.push({ method: 'POST', path, handler });
    console.log(`Route registered: POST ${path}`);
  }
  
  simulateRequest(method, path, body = null) {
    let route = this.routes.find(r => r.method === method && r.path === path);
    if (route) {
      // Hono uses a Context object (c) instead of req/res
      let c = {
        req: {
          method,
          path,
          query: (key) => null,
          param: (key) => null,
          json: async () => body || {}
        },
        text: function(data, status = 200) {
          console.log(`Response [${status}]:`, data);
          return { status, body: data };
        },
        json: function(data, status = 200) {
          console.log(`Response [${status}]:`, JSON.stringify(data));
          return { status, body: data };
        }
      };
      route.handler(c);
    } else {
      console.log(`404 Not Found: ${method} ${path}`);
    }
  }
}

let app = new HonoApp();

// ROUTE DEFINITIONS - Notice the simpler 'c' context pattern!

// 1. Simple GET route - Homepage
app.get('/', (c) => {
  return c.text('Welcome to Hono!');
});

// 2. GET route returning JSON
app.get('/api/status', (c) => {
  return c.json({
    status: 'online',
    version: '1.0.0',
    framework: 'Hono',
    timestamp: Date.now()
  });
});

// 3. GET route - List of users
app.get('/api/users', (c) => {
  let users = [
    { id: 1, name: 'Alice', email: 'alice@example.com' },
    { id: 2, name: 'Bob', email: 'bob@example.com' }
  ];
  return c.json(users);
});

// 4. POST route - Create new user
app.post('/api/users', async (c) => {
  // In real Hono: const body = await c.req.json();
  let newUser = { id: 3, name: 'Charlie', email: 'charlie@example.com' };
  
  return c.json({
    message: 'User created successfully',
    user: newUser
  }, 201);  // Status code as second argument!
});

// 5. Error response
app.get('/api/error', (c) => {
  return c.json({
    error: 'Something went wrong!',
    message: 'Internal server error'
  }, 500);
});

// In real Bun, export the app:
// export default app;
// Then run: bun run app.ts

console.log('\nHono server routes registered!');
console.log('In real Bun, export default app and run with: bun run app.ts');

// SIMULATE REQUESTS
console.log('\n--- Simulating HTTP Requests ---\n');

app.simulateRequest('GET', '/');
app.simulateRequest('GET', '/api/status');
app.simulateRequest('GET', '/api/users');
app.simulateRequest('POST', '/api/users');
app.simulateRequest('GET', '/api/error');
app.simulateRequest('GET', '/api/notfound');
```
