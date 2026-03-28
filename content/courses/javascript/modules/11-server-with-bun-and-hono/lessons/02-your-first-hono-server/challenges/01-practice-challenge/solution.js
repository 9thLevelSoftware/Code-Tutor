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
  { id: 2, task: 'Build an API', completed: true },
  { id: 3, task: 'Deploy app', completed: true }
];

// Route 1: Get all todos (Hono pattern)
app.get('/api/todos', (c) => {
  return c.json(todos);
});

// Route 2: Create new todo (with 201 status)
app.post('/api/todos', (c) => {
  let newTodo = {
    id: todos.length + 1,
    task: 'New task',
    completed: false
  };
  todos.push(newTodo);
  
  return c.json({
    message: 'Todo created',
    todo: newTodo
  }, 201);  // Status as second argument!
});

// Route 3: Get completed todos only
app.get('/api/todos/completed', (c) => {
  let completedTodos = todos.filter(t => t.completed);
  return c.json(completedTodos);
});

// Test the routes
console.log('All todos:');
app.simulateRequest('GET', '/api/todos');

console.log('\nCreate todo:');
app.simulateRequest('POST', '/api/todos');

console.log('\nCompleted todos:');
app.simulateRequest('GET', '/api/todos/completed');