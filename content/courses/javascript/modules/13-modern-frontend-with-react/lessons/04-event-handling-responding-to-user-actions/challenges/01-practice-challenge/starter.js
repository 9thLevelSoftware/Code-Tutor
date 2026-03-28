// Interactive Counter App

let CounterApp = {
  state: {
    count: 0,
    history: []
  },
  
  handleIncrement() {
    this.state.count++;
    this.state.history.push('+1');
    console.log('[Event] Increment → count:', this.state.count);
  },
  
  handleDecrement() {
    this.state.count--;
    this.state.history.push('-1');
    console.log('[Event] Decrement → count:', this.state.count);
  },
  
  handleReset() {
    this.state.count = 0;
    this.state.history = [];
    console.log('[Event] Reset → count:', this.state.count);
  },
  
  render() {
    console.log('\n[Render]');
    console.log('  Current Count:', this.state.count);
    console.log('  History:', this.state.history.join(', ') || 'none');
    console.log('');
  }
};

// Simulate user interactions
console.log('=== Counter App ===\n');

CounterApp.render();

console.log('User clicks +1 button:');
CounterApp.handleIncrement();
CounterApp.render();

console.log('User clicks +1 button again:');
CounterApp.handleIncrement();
CounterApp.render();

console.log('User clicks -1 button:');
CounterApp.handleDecrement();
CounterApp.render();

console.log('User clicks reset button:');
CounterApp.handleReset();
CounterApp.render();