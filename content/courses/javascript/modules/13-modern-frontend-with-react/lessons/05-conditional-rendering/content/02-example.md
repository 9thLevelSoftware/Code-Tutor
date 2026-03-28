---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// Conditional Rendering in React

console.log('=== Conditional Rendering ===\n');

// METHOD 1: if/else (outside JSX)
function renderGreeting(isLoggedIn) {
  if (isLoggedIn) {
    return '<h1>Welcome back!</h1>';
  } else {
    return '<h1>Please log in.</h1>';
  }
}

console.log('User logged in:');
console.log(renderGreeting(true));
console.log('\nUser NOT logged in:');
console.log(renderGreeting(false));

// METHOD 2: Ternary operator (? :)
function renderStatus(isOnline) {
  return `<p>Status: ${isOnline ? 'Online' : 'Offline'}</p>`;
}

console.log('\nTernary operator:');
console.log(renderStatus(true));
console.log(renderStatus(false));

// METHOD 3: Logical AND (&&)
function renderNotifications(hasNotifications, count) {
  return `
    <div>
      <h2>Dashboard</h2>
      ${hasNotifications && `<p>You have ${count} new notifications!</p>` || ''}
    </div>
  `;
}

console.log('\nLogical AND (&&):');
console.log(renderNotifications(true, 5));
console.log(renderNotifications(false, 0));

// REAL-WORLD EXAMPLE: Login/Logout Button
let app = {
  state: { isLoggedIn: false, username: null },
  
  render() {
    console.log('\n[Render] App UI:\n');
    
    if (this.state.isLoggedIn) {
      console.log(`Welcome, ${this.state.username}!`);
      console.log('[Logout Button]');
    } else {
      console.log('Welcome, Guest!');
      console.log('[Login Button]');
    }
  },
  
  login(username) {
    this.state.isLoggedIn = true;
    this.state.username = username;
    this.render();
  },
  
  logout() {
    this.state.isLoggedIn = false;
    this.state.username = null;
    this.render();
  }
};

console.log('\n=== Login/Logout Example ===');
app.render();

console.log('\nUser clicks login:');
app.login('Alice');

console.log('\nUser clicks logout:');
app.logout();

// LOADING STATES
let dataFetcher = {
  state: { loading: true, data: null, error: null },
  
  render() {
    console.log('\n[Render] Data View:\n');
    
    if (this.state.loading) {
      console.log('⏳ Loading...');
    } else if (this.state.error) {
      console.log(`❌ Error: ${this.state.error}`);
    } else if (this.state.data) {
      console.log('✓ Data:', this.state.data);
    } else {
      console.log('No data yet.');
    }
  },
  
  async simulateFetch(shouldFail = false) {
    this.state.loading = true;
    this.state.error = null;
    this.state.data = null;
    this.render();
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 500));
    
    if (shouldFail) {
      this.state.loading = false;
      this.state.error = 'Network error';
    } else {
      this.state.loading = false;
      this.state.data = { users: ['Alice', 'Bob', 'Charlie'] };
    }
    this.render();
  }
};

console.log('\n=== Loading State Example ===');
setTimeout(async () => {
  dataFetcher.render();
  
  console.log('\nFetching data (success):');
  await dataFetcher.simulateFetch(false);
  
  console.log('\nFetching data (failure):');
  await dataFetcher.simulateFetch(true);
}, 100);

// CONDITIONAL CSS CLASSES
setTimeout(() => {
  console.log('\n\n=== Conditional Styling ===\n');
  
  function renderButton(isActive) {
    let className = isActive ? 'btn-active' : 'btn-inactive';
    let text = isActive ? 'Active' : 'Inactive';
    return `<button class="${className}">${text}</button>`;
  }
  
  console.log('Active button:');
  console.log(renderButton(true));
  console.log('\nInactive button:');
  console.log(renderButton(false));
  
  // Multiple conditions
  function renderAlert(type, message) {
    let className = type === 'success' ? 'alert-success' :
                    type === 'error' ? 'alert-error' :
                    type === 'warning' ? 'alert-warning' :
                    'alert-info';
    
    let icon = type === 'success' ? '✓' :
               type === 'error' ? '✗' :
               type === 'warning' ? '⚠' :
               'ℹ';
    
    return `<div class="${className}">${icon} ${message}</div>`;
  }
  
  console.log('\nConditional alerts:');
  console.log(renderAlert('success', 'Saved successfully!'));
  console.log(renderAlert('error', 'Failed to save!'));
  console.log(renderAlert('warning', 'Are you sure?'));
  console.log(renderAlert('info', 'New update available'));
}, 1200);

// LIST RENDERING WITH CONDITIONS
setTimeout(() => {
  console.log('\n\n=== Conditional List Rendering ===\n');
  
  let todos = [
    { id: 1, text: 'Learn React', completed: true },
    { id: 2, text: 'Build app', completed: false },
    { id: 3, text: 'Deploy', completed: false }
  ];
  
  console.log('All todos:');
  todos.forEach(todo => {
    let checkbox = todo.completed ? '[✓]' : '[ ]';
    let style = todo.completed ? '(completed)' : '';
    console.log(`${checkbox} ${todo.text} ${style}`);
  });
  
  console.log('\nCompleted only:');
  todos.filter(todo => todo.completed).forEach(todo => {
    console.log(`[✓] ${todo.text}`);
  });
  
  console.log('\nIncomplete only:');
  todos.filter(todo => !todo.completed).forEach(todo => {
    console.log(`[ ] ${todo.text}`);
  });
}, 1300);

// REACT SYNTAX PATTERNS
setTimeout(() => {
  console.log('\n\n=== React Conditional Rendering Patterns ===\n');
  
  console.log('// Pattern 1: Ternary operator');
  console.log('{isLoggedIn ? <Dashboard /> : <Login />}\n');
  
  console.log('// Pattern 2: Logical AND (show/hide)');
  console.log('{hasError && <ErrorMessage />}');
  console.log('{count > 0 && <p>You have {count} items</p>}\n');
  
  console.log('// Pattern 3: If/else (before return)');
  console.log('if (loading) return <Spinner />;');
  console.log('if (error) return <Error message={error} />;');
  console.log('return <Data data={data} />;\n');
  
  console.log('// Pattern 4: Switch for multiple conditions');
  console.log('switch(status) {');
  console.log('  case "loading": return <Spinner />;');
  console.log('  case "error": return <Error />;');
  console.log('  case "success": return <Data />;');
  console.log('  default: return null;');
  console.log('}\n');
  
  console.log('// Pattern 5: Null for hiding');
  console.log('{!shouldShow && null}  // Renders nothing');
  console.log('{shouldShow ? <Component /> : null}');
}, 1400);
```
