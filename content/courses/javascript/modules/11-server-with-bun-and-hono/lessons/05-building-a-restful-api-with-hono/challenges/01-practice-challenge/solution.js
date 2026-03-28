class HonoApp {
  constructor() { this.routes = []; }
  get(p, h) { this.routes.push({ method: 'GET', path: p, handler: h }); }
  post(p, h) { this.routes.push({ method: 'POST', path: p, handler: h }); }
  put(p, h) { this.routes.push({ method: 'PUT', path: p, handler: h }); }
  delete(p, h) { this.routes.push({ method: 'DELETE', path: p, handler: h }); }
  async simulateRequest(method, path, body = null) {
    console.log(`\n${method} ${path}`);
    let id = path.match(/\d+$/)?.[0];
    let route = this.routes.find(r => {
      if (r.method !== method) return false;
      let pattern = r.path.replace(':id', '([^/]+)');
      return new RegExp('^' + pattern + '$').test(path);
    });
    if (route) {
      let c = {
        req: {
          param: (key) => key === 'id' ? id : null,
          json: async () => body || {}
        },
        json: function(d, s = 200) {
          console.log(`[${s}]`, JSON.stringify(d, null, 2));
          return { status: s, body: d };
        }
      };
      await route.handler(c);
    }
  }
}

let app = new HonoApp();

// Task storage
let tasks = [
  { id: 1, title: 'Learn Hono', completed: false },
  { id: 2, title: 'Build API', completed: true }
];
let nextId = 3;

// GET /api/tasks - List all
app.get('/api/tasks', (c) => {
  return c.json({ count: tasks.length, tasks });
});

// GET /api/tasks/:id - Get one
app.get('/api/tasks/:id', (c) => {
  const task = tasks.find(t => t.id === parseInt(c.req.param('id')));
  if (!task) {
    return c.json({ error: 'Task not found' }, 404);
  }
  return c.json(task);
});

// POST /api/tasks - Create
app.post('/api/tasks', async (c) => {
  const body = await c.req.json();
  
  if (!body.title) {
    return c.json({ error: 'Title is required' }, 400);
  }
  
  const newTask = {
    id: nextId++,
    title: body.title,
    completed: false
  };
  tasks.push(newTask);
  
  return c.json({ message: 'Task created', task: newTask }, 201);
});

// PUT /api/tasks/:id - Update
app.put('/api/tasks/:id', async (c) => {
  const id = parseInt(c.req.param('id'));
  const body = await c.req.json();
  const index = tasks.findIndex(t => t.id === id);
  
  if (index === -1) {
    return c.json({ error: 'Task not found' }, 404);
  }
  
  tasks[index] = {
    id: id,
    title: body.title,
    completed: body.completed
  };
  
  return c.json({ message: 'Task updated', task: tasks[index] });
});

// DELETE /api/tasks/:id - Delete
app.delete('/api/tasks/:id', (c) => {
  const id = parseInt(c.req.param('id'));
  const index = tasks.findIndex(t => t.id === id);
  
  if (index === -1) {
    return c.json({ error: 'Task not found' }, 404);
  }
  
  const deleted = tasks.splice(index, 1)[0];
  return c.json({ message: 'Task deleted', task: deleted });
});

// Tests
console.log('=== Task API Demo (Hono) ===');
app.simulateRequest('GET', '/api/tasks');
app.simulateRequest('GET', '/api/tasks/1');
app.simulateRequest('POST', '/api/tasks', { title: 'Deploy app' });
app.simulateRequest('PUT', '/api/tasks/1', { title: 'Learn Hono', completed: true });
app.simulateRequest('DELETE', '/api/tasks/2');
app.simulateRequest('GET', '/api/tasks');