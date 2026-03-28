// Complete state management simulation

let Counter = {
  state: { count: 0 },
  listeners: [],
  
  setCount(newValue) {
    let oldValue = this.state.count;
    this.state.count = newValue;
    console.log(`[State Update] count: ${oldValue} → ${newValue}`);
    console.log('[React] Re-rendering component...');
    this.notifyListeners();
  },
  
  increment() {
    console.log('[Action] Increment');
    this.setCount(this.state.count + 1);
  },
  
  decrement() {
    console.log('[Action] Decrement');
    this.setCount(this.state.count - 1);
  },
  
  incrementBy(amount) {
    console.log(`[Action] Increment by ${amount}`);
    this.setCount(this.state.count + amount);
  },
  
  reset() {
    console.log('[Action] Reset');
    this.setCount(0);
  },
  
  getCount() {
    return this.state.count;
  },
  
  // Subscribe to changes
  onChange(callback) {
    this.listeners.push(callback);
  },
  
  notifyListeners() {
    this.listeners.forEach(fn => fn(this.state.count));
  },
  
  render() {
    console.log('\n[Render] Counter UI:');
    console.log(`┌─────────────────┐`);
    console.log(`│  Count: ${String(this.state.count).padEnd(6)} │`);
    console.log(`├─────────────────┤`);
    console.log(`│  [ - ] [ + ]    │`);
    console.log(`│  [ Reset ]      │`);
    console.log(`└─────────────────┘\n`);
  }
};

// Advanced: TodoList with state
let TodoList = {
  state: {
    todos: [],
    nextId: 1
  },
  
  setTodos(newTodos) {
    this.state.todos = newTodos;
    console.log('[State] todos updated:', newTodos.length, 'items');
  },
  
  addTodo(text) {
    console.log(`[Action] Add todo: "${text}"`);
    let newTodo = {
      id: this.state.nextId++,
      text: text,
      completed: false
    };
    // Must create NEW array (don't mutate!)
    this.setTodos([...this.state.todos, newTodo]);
  },
  
  toggleTodo(id) {
    console.log(`[Action] Toggle todo ${id}`);
    this.setTodos(
      this.state.todos.map(todo =>
        todo.id === id
          ? { ...todo, completed: !todo.completed }  // New object
          : todo
      )
    );
  },
  
  deleteTodo(id) {
    console.log(`[Action] Delete todo ${id}`);
    this.setTodos(
      this.state.todos.filter(todo => todo.id !== id)
    );
  },
  
  render() {
    console.log('\n[Render] Todo List:');
    if (this.state.todos.length === 0) {
      console.log('  No todos yet!');
    } else {
      this.state.todos.forEach(todo => {
        let checkbox = todo.completed ? '[✓]' : '[ ]';
        let text = todo.completed ? `(${todo.text})` : todo.text;
        console.log(`  ${checkbox} ${text}`);
      });
    }
    console.log('');
  }
};

// Run simulations
console.log('=== Counter Simulation ===\n');

Counter.render();

console.log('User clicks +1 three times:');
Counter.increment();
Counter.increment();
Counter.increment();
Counter.render();

console.log('User clicks -1:');
Counter.decrement();
Counter.render();

console.log('User clicks reset:');
Counter.reset();
Counter.render();

console.log('\n=== TodoList Simulation ===\n');

TodoList.render();

TodoList.addTodo('Learn useState');
TodoList.addTodo('Build a counter app');
TodoList.addTodo('Master React');
TodoList.render();

console.log('User completes first todo:');
TodoList.toggleTodo(1);
TodoList.render();

console.log('User deletes second todo:');
TodoList.deleteTodo(2);
TodoList.render();

console.log('=== Key Takeaways ===\n');
let takeaways = [
  '✓ useState gives components memory',
  '✓ State updates trigger re-renders',
  '✓ Never mutate state directly',
  '✓ Always create new objects/arrays',
  '✓ Use setCount(prev => prev + 1) for updates based on previous',
  '✓ Can have multiple state variables',
  '✓ Each state is independent'
];
takeaways.forEach(t => console.log(t));