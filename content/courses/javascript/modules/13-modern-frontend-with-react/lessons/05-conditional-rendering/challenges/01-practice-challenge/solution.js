// Complete solution with all conditional rendering patterns

let AuthApp = {
  state: {
    isLoggedIn: false,
    user: null,
    loading: false,
    error: null
  },
  
  render() {
    console.log('\n┌────────────────────────────────────────┐');
    console.log('│          Authentication App            │');
    console.log('├────────────────────────────────────────┤');
    
    // Loading state
    if (this.state.loading) {
      console.log('│  ⏳ Loading...                         │');
      console.log('└────────────────────────────────────────┘\n');
      return;
    }
    
    // Error state
    if (this.state.error) {
      console.log(`│  ❌ Error: ${this.state.error.padEnd(25)} │`);
      console.log('│  [Try Again]                           │');
      console.log('└────────────────────────────────────────┘\n');
      return;
    }
    
    // Logged in state
    if (this.state.isLoggedIn) {
      console.log(`│  Welcome, ${this.state.user}! ${' '.repeat(26 - this.state.user.length)} │`);
      console.log('│  ─────────────────────────────────────│');
      console.log('│  Dashboard                             │');
      console.log('│  Profile                               │');
      console.log('│  Settings                              │');
      console.log('│  ─────────────────────────────────────│');
      console.log('│  [Logout]                              │');
    } else {
      // Logged out state
      console.log('│  Please log in to continue            │');
      console.log('│  ─────────────────────────────────────│');
      console.log('│  [Login]                               │');
      console.log('│  [Sign Up]                             │');
    }
    
    console.log('└────────────────────────────────────────┘\n');
  },
  
  async login(username, password) {
    console.log(`[Action] Login as "${username}"`);
    this.state.loading = true;
    this.state.error = null;
    this.render();
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 800));
    
    // Simulate authentication
    if (password === 'wrong') {
      this.state.loading = false;
      this.state.error = 'Invalid credentials';
      this.render();
      return;
    }
    
    this.state.loading = false;
    this.state.isLoggedIn = true;
    this.state.user = username;
    this.render();
  },
  
  logout() {
    console.log('[Action] Logout');
    this.state.isLoggedIn = false;
    this.state.user = null;
    this.state.error = null;
    this.render();
  }
};

// Todo List with conditional rendering
let TodoApp = {
  state: {
    todos: [
      { id: 1, text: 'Learn conditional rendering', completed: true },
      { id: 2, text: 'Build a todo app', completed: false },
      { id: 3, text: 'Master React', completed: false }
    ],
    filter: 'all'  // 'all', 'active', 'completed'
  },
  
  render() {
    console.log('\n┌────────────────────────────────────────┐');
    console.log('│             Todo List                  │');
    console.log('├────────────────────────────────────────┤');
    
    // Filter todos based on current filter
    let filteredTodos;
    if (this.state.filter === 'completed') {
      filteredTodos = this.state.todos.filter(t => t.completed);
    } else if (this.state.filter === 'active') {
      filteredTodos = this.state.todos.filter(t => !t.completed);
    } else {
      filteredTodos = this.state.todos;
    }
    
    // Empty state
    if (filteredTodos.length === 0) {
      console.log('│  No todos to show                      │');
    } else {
      filteredTodos.forEach(todo => {
        let checkbox = todo.completed ? '[✓]' : '[ ]';
        let text = todo.completed ? `${todo.text} (done)` : todo.text;
        let line = `│  ${checkbox} ${text}`.padEnd(41) + '│';
        console.log(line);
      });
    }
    
    console.log('├────────────────────────────────────────┤');
    
    // Filter buttons with active state
    let allActive = this.state.filter === 'all' ? '*' : ' ';
    let activeActive = this.state.filter === 'active' ? '*' : ' ';
    let completedActive = this.state.filter === 'completed' ? '*' : ' ';
    
    console.log(`│  [${allActive}]All [${activeActive}]Active [${completedActive}]Completed      │`);
    console.log('└────────────────────────────────────────┘\n');
  },
  
  setFilter(filter) {
    console.log(`[Action] Set filter: ${filter}`);
    this.state.filter = filter;
    this.render();
  }
};

// Run demonstrations
async function runDemo() {
  console.log('=== Authentication Demo ===\n');
  
  AuthApp.render();
  
  console.log('User attempts login with wrong password:');
  await AuthApp.login('Alice', 'wrong');
  
  await new Promise(resolve => setTimeout(resolve, 1000));
  
  console.log('User attempts login with correct password:');
  await AuthApp.login('Alice', 'correct');
  
  await new Promise(resolve => setTimeout(resolve, 1000));
  
  console.log('User logs out:');
  AuthApp.logout();
  
  await new Promise(resolve => setTimeout(resolve, 500));
  
  console.log('\n=== Todo List Demo ===\n');
  
  TodoApp.render();
  
  await new Promise(resolve => setTimeout(resolve, 500));
  console.log('Show only active todos:');
  TodoApp.setFilter('active');
  
  await new Promise(resolve => setTimeout(resolve, 500));
  console.log('Show only completed todos:');
  TodoApp.setFilter('completed');
  
  await new Promise(resolve => setTimeout(resolve, 500));
  console.log('Show all todos:');
  TodoApp.setFilter('all');
}

runDemo();