---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// useState Hook - Component Memory

console.log('=== React useState Hook ===\n');

// WHY WE NEED STATE
console.log('--- Without State (Doesn\'t Work!) ---\n');

function simulateComponentWithoutState() {
  let count = 0;  // Regular variable
  
  console.log('[Component] Initial render: count =', count);
  
  // User clicks button
  console.log('[User] Clicks +1 button');
  count = count + 1;
  console.log('[Component] count =', count, '(but component doesn\'t re-render!)');
  
  // Component renders again (for some reason)
  count = 0;  // RESETS! Regular variables don't persist
  console.log('[Component] Re-render: count =', count, '(LOST the value!)');
}

simulateComponentWithoutState();

// WITH STATE (Works!)
console.log('\n--- With useState (Works!) ---\n');

function simulateComponentWithState() {
  // useState hook - PERSISTS between renders
  let state = {
    count: 0,
    setCount: function(newValue) {
      this.count = newValue;
      console.log('[State Update] count changed to:', newValue);
      console.log('[React] Re-rendering component...');
    }
  };
  
  console.log('[Component] Initial render: count =', state.count);
  
  // User clicks button
  console.log('\n[User] Clicks +1 button');
  state.setCount(state.count + 1);
  console.log('[Component] Re-render: count =', state.count, '(PERSISTED!)');
  
  // User clicks again
  console.log('\n[User] Clicks +1 button again');
  state.setCount(state.count + 1);
  console.log('[Component] Re-render: count =', state.count);
}

simulateComponentWithState();

// REAL useState SYNTAX
console.log('\n\n=== useState Syntax ===\n');

console.log('// Import from React');
console.log('import { useState } from "react";\n');

console.log('function Counter() {');
console.log('  // useState returns [currentValue, setterFunction]');
console.log('  const [count, setCount] = useState(0);');
console.log('  //      ^^^^^  ^^^^^^^^          ^^^');
console.log('  //      value  updater          initial value\n');

console.log('  function increment() {');
console.log('    setCount(count + 1);  // Update state');
console.log('  }\n');

console.log('  return (');
console.log('    <div>');
console.log('      <p>Count: {count}</p>');
console.log('      <button onClick={increment}>+1</button>');
console.log('    </div>');
console.log('  );');
console.log('}\n');

// MULTIPLE STATE VARIABLES
console.log('=== Multiple State Variables ===\n');

console.log('function UserProfile() {');
console.log('  const [name, setName] = useState("Alice");');
console.log('  const [age, setAge] = useState(25);');
console.log('  const [isLoggedIn, setIsLoggedIn] = useState(false);\n');

console.log('  // Each state is independent!');
console.log('  setName("Bob");        // Only updates name');
console.log('  setAge(30);           // Only updates age');
console.log('  setIsLoggedIn(true);  // Only updates isLoggedIn');
console.log('}\n');

// STATE WITH OBJECTS
console.log('=== State with Objects ===\n');

let userState = {
  user: { name: 'Alice', age: 25, email: 'alice@example.com' },
  setUser: function(newUser) {
    // MUST create new object (don't mutate!)
    this.user = { ...this.user, ...newUser };
    console.log('[State] Updated user:', this.user);
  }
};

console.log('Initial user:', userState.user);

console.log('\nUpdating age:');
userState.setUser({ age: 26 });  // Spread syntax preserves other fields

console.log('\n// WRONG way (mutation):');
console.log('user.age = 26;        // ✗ Don\'t mutate directly!');
console.log('setUser(user);       // ✗ React won\'t detect change!\n');

console.log('// CORRECT way (new object):');
console.log('setUser({ ...user, age: 26 });  // ✓ Creates new object');

// STATE WITH ARRAYS
console.log('\n\n=== State with Arrays ===\n');

let todosState = {
  todos: ['Learn React', 'Build app'],
  setTodos: function(newTodos) {
    this.todos = newTodos;
    console.log('[State] Updated todos:', this.todos);
  }
};

console.log('Initial todos:', todosState.todos);

console.log('\nAdding todo:');
todosState.setTodos([...todosState.todos, 'Deploy app']);

console.log('\nRemoving first todo:');
todosState.setTodos(todosState.todos.slice(1));

console.log('\n--- Array State Patterns ---');
console.log('Add item:    setTodos([...todos, newItem])');
console.log('Remove item: setTodos(todos.filter(t => t.id !== id))');
console.log('Update item: setTodos(todos.map(t => t.id === id ? updated : t))');
```
