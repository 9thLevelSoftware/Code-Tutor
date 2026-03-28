// Counter with state

let Counter = {
  state: { count: 0 },
  
  setCount(newValue) {
    this.state.count = newValue;
    console.log('[State] count =', this.state.count);
  },
  
  increment() {
    this.setCount(this.state.count + 1);
  },
  
  decrement() {
    this.setCount(this.state.count - 1);
  },
  
  reset() {
    this.setCount(0);
  },
  
  getCount() {
    return this.state.count;
  }
};

// Test
console.log('Initial count:', Counter.getCount());

Counter.increment();
Counter.increment();
Counter.increment();
Counter.decrement();
Counter.reset();

console.log('Final count:', Counter.getCount());