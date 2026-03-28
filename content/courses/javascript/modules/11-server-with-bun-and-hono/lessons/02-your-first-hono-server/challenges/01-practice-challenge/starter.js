// Hono app simulation
class HonoApp {
  constructor() {
    this.routes = [];
  }
  
  get(path, handler) {
    this.routes.push({ method: 'GET', path, handler });
  }
  
  post(path, handler) {
    this.routes.push({ method: 'POST', path, handler });
  }
  
  simulateRequest(method, path) {
    let route = this.routes.find(r => r.method === method && r.path === path);
    if (route) {
      let c = {
        req: { method, path },
        json: function(data, status = 200) {
          console.log(`[${status}]`, JSON.stringify(data));
          return { status, body: data };
        }
      };
      route.handler(c);
    }
  }
}

let app = new HonoApp();

// Sample todo data
let todos = [
  { id: 1, task: 'Learn Hono', completed: false },
  { id: 2, task: 'Build an API', completed: true }
];

// TODO: Add your routes here using the Hono pattern
// Remember: (c) => { return c.json(data); }

// Test your routes
app.simulateRequest('GET', '/api/todos');
app.simulateRequest('POST', '/api/todos');
app.simulateRequest('GET', '/api/todos/completed');