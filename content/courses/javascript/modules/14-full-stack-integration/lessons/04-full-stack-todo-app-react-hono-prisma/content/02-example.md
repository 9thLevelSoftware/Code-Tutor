---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// Complete Full-Stack Todo App Simulation

console.log('=== Full-Stack Todo Application ===\n');

// DATABASE LAYER (Prisma + PostgreSQL)
let database = {
  todos: [
    { id: 1, title: 'Learn React', completed: false, userId: 1 },
    { id: 2, title: 'Build API', completed: true, userId: 1 },
    { id: 3, title: 'Deploy app', completed: false, userId: 1 }
  ],
  
  // Simulate Prisma queries
  async findMany(filter = {}) {
    console.log('[Database] SELECT * FROM todos WHERE userId =', filter.userId || 'ALL');
    let results = filter.userId 
      ? this.todos.filter(t => t.userId === filter.userId)
      : this.todos;
    return results;
  },
  
  async create(data) {
    console.log('[Database] INSERT INTO todos', data);
    let newTodo = {
      id: this.todos.length + 1,
      completed: false,
      ...data
    };
    this.todos.push(newTodo);
    return newTodo;
  },
  
  async update(id, data) {
    console.log(`[Database] UPDATE todos SET ... WHERE id = ${id}`);
    let todo = this.todos.find(t => t.id === id);
    if (todo) {
      Object.assign(todo, data);
      return todo;
    }
    return null;
  },
  
  async delete(id) {
    console.log(`[Database] DELETE FROM todos WHERE id = ${id}`);
    let index = this.todos.findIndex(t => t.id === id);
    if (index !== -1) {
      let deleted = this.todos.splice(index, 1)[0];
      return deleted;
    }
    return null;
  }
};

// BACKEND LAYER (Hono API)
let backend = {
  corsEnabled: true,
  
  async handleGetTodos(userId) {
    console.log('[Backend] GET /api/todos');
    console.log('[Backend] Checking CORS... ' + (this.corsEnabled ? 'Allowed' : 'Blocked'));
    
    if (!this.corsEnabled) {
      return { status: 403, error: 'CORS error' };
    }
    
    let todos = await database.findMany({ userId });
    console.log(`[Backend] Found ${todos.length} todos`);
    return { status: 200, data: todos };
  },
  
  async handleCreateTodo(title, userId) {
    console.log('[Backend] POST /api/todos', { title, userId });
    
    // Validation
    if (!title || title.trim().length === 0) {
      console.log('[Backend] Validation failed: title required');
      return { status: 400, error: 'Title is required' };
    }
    
    let newTodo = await database.create({ title, userId });
    console.log('[Backend] Created todo:', newTodo.id);
    return { status: 201, data: newTodo };
  },
  
  async handleUpdateTodo(id, updates) {
    console.log(`[Backend] PATCH /api/todos/${id}`, updates);
    let updated = await database.update(id, updates);
    
    if (!updated) {
      console.log('[Backend] Todo not found');
      return { status: 404, error: 'Todo not found' };
    }
    
    console.log('[Backend] Updated todo:', updated);
    return { status: 200, data: updated };
  },
  
  async handleDeleteTodo(id) {
    console.log(`[Backend] DELETE /api/todos/${id}`);
    let deleted = await database.delete(id);
    
    if (!deleted) {
      return { status: 404, error: 'Todo not found' };
    }
    
    console.log('[Backend] Deleted todo');
    return { status: 200, data: { message: 'Deleted successfully' } };
  }
};

// FRONTEND LAYER (React Component)
let TodoApp = {
  state: {
    todos: [],
    loading: false,
    error: null,
    newTodoTitle: '',
    currentUserId: 1
  },
  
  setState(updates) {
    this.state = { ...this.state, ...updates };
    console.log('[TodoApp State]', {
      todos: this.state.todos.length + ' todos',
      loading: this.state.loading,
      error: this.state.error
    });
  },
  
  // useEffect - fetch todos on mount
  async componentDidMount() {
    console.log('\n[TodoApp] Component mounted');
    console.log('[TodoApp] useEffect(() => { fetchTodos() }, [])');
    await this.fetchTodos();
  },
  
  async fetchTodos() {
    console.log('[TodoApp] Fetching todos from API...');
    console.log('[TodoApp] fetch("http://localhost:4000/api/todos")');
    
    this.setState({ loading: true, error: null });
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 300));
    
    let response = await backend.handleGetTodos(this.state.currentUserId);
    
    if (response.status === 200) {
      this.setState({ todos: response.data, loading: false });
      console.log('[TodoApp] Rendering', response.data.length, 'todos');
    } else {
      this.setState({ error: response.error, loading: false });
    }
  },
  
  async addTodo(title) {
    console.log(`\n[TodoApp] User types "${title}" and clicks Add`);
    console.log('[TodoApp] handleAddTodo()');
    console.log('[TodoApp] fetch("http://localhost:4000/api/todos", { method: "POST", ... })');
    
    await new Promise(resolve => setTimeout(resolve, 200));
    
    let response = await backend.handleCreateTodo(title, this.state.currentUserId);
    
    if (response.status === 201) {
      console.log('[TodoApp] Todo created! Refreshing list...');
      await this.fetchTodos();
    } else {
      this.setState({ error: response.error });
    }
  },
  
  async toggleTodo(id) {
    console.log(`\n[TodoApp] User clicks checkbox for todo ${id}`);
    let todo = this.state.todos.find(t => t.id === id);
    console.log(`[TodoApp] Toggling completed: ${todo.completed} → ${!todo.completed}`);
    console.log(`[TodoApp] fetch("http://localhost:4000/api/todos/${id}", { method: "PATCH", ... })`);
    
    await new Promise(resolve => setTimeout(resolve, 200));
    
    let response = await backend.handleUpdateTodo(id, { completed: !todo.completed });
    
    if (response.status === 200) {
      console.log('[TodoApp] Updated! Refreshing...');
      await this.fetchTodos();
    }
  },
  
  async deleteTodo(id) {
    console.log(`\n[TodoApp] User clicks delete for todo ${id}`);
    console.log(`[TodoApp] fetch("http://localhost:4000/api/todos/${id}", { method: "DELETE" })`);
    
    await new Promise(resolve => setTimeout(resolve, 200));
    
    let response = await backend.handleDeleteTodo(id);
    
    if (response.status === 200) {
      console.log('[TodoApp] Deleted! Refreshing...');
      await this.fetchTodos();
    }
  }
};

// RUN SIMULATION
async function runFullStackSimulation() {
  console.log('=== Simulating Full-Stack Todo App ===\n');
  
  // 1. App loads
  await TodoApp.componentDidMount();
  
  await new Promise(resolve => setTimeout(resolve, 500));
  
  // 2. User adds new todo
  await TodoApp.addTodo('Master full-stack development');
  
  await new Promise(resolve => setTimeout(resolve, 500));
  
  // 3. User toggles todo
  await TodoApp.toggleTodo(1);
  
  await new Promise(resolve => setTimeout(resolve, 500));
  
  // 4. User deletes todo
  await TodoApp.deleteTodo(2);
  
  await new Promise(resolve => setTimeout(resolve, 500));
  
  // 5. Final state
  console.log('\n=== Final Application State ===');
  console.log('\nDatabase:');
  database.todos.forEach(todo => {
    console.log(`  [${todo.completed ? '✓' : ' '}] ${todo.id}. ${todo.title}`);
  });
  
  console.log('\nFrontend State:');
  console.log('  Todos displayed:', TodoApp.state.todos.length);
  console.log('  Loading:', TodoApp.state.loading);
  console.log('  Error:', TodoApp.state.error || 'none');
}

runFullStackSimulation();
```
