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
          console.log(`[${s}]`, JSON.stringify(d));
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
  { id: 1, title: 'Learn Hono', completed: false }
];
let nextId = 2;

// TODO: Implement your routes here using Hono patterns
// Remember: (c) => { return c.json(data); }
// For POST/PUT: async (c) => { const body = await c.req.json(); }

// Test
app.simulateRequest('GET', '/api/tasks');
app.simulateRequest('POST', '/api/tasks', { title: 'Build API' });
app.simulateRequest('GET', '/api/tasks/1');
app.simulateRequest('DELETE', '/api/tasks/1');