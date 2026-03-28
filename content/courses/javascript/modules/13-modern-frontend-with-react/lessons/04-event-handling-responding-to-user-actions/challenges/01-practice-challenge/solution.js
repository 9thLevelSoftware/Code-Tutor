// Complete interactive app with event handling

let CounterApp = {
  state: {
    count: 0,
    history: [],
    lastAction: null
  },
  
  handleIncrement() {
    console.log('[Event] onClick={handleIncrement}');
    this.state.count++;
    this.state.history.push({ action: '+1', timestamp: Date.now() });
    this.state.lastAction = 'increment';
    this.render();
  },
  
  handleDecrement() {
    console.log('[Event] onClick={handleDecrement}');
    this.state.count--;
    this.state.history.push({ action: '-1', timestamp: Date.now() });
    this.state.lastAction = 'decrement';
    this.render();
  },
  
  handleReset() {
    console.log('[Event] onClick={handleReset}');
    this.state.count = 0;
    this.state.history = [];
    this.state.lastAction = 'reset';
    this.render();
  },
  
  handleIncrementBy(amount) {
    console.log(`[Event] onClick={() => handleIncrementBy(${amount})}`);
    this.state.count += amount;
    this.state.history.push({ action: `+${amount}`, timestamp: Date.now() });
    this.render();
  },
  
  render() {
    console.log('\n[React] Re-rendering component...');
    console.log('┌────────────────────────────┐');
    console.log(`│ Count: ${String(this.state.count).padEnd(19)} │`);
    console.log('├────────────────────────────┤');
    console.log('│ [ -1 ]  [ +1 ]  [ +5 ]     │');
    console.log('│         [Reset]            │');
    console.log('├────────────────────────────┤');
    console.log(`│ Actions: ${String(this.state.history.length).padEnd(17)} │`);
    if (this.state.history.length > 0) {
      let recent = this.state.history.slice(-3).map(h => h.action).join(', ');
      console.log(`│ Recent: ${recent.padEnd(18)} │`);
    }
    console.log('└────────────────────────────┘\n');
  }
};

// Form with event handling
let LoginForm = {
  state: {
    email: '',
    password: '',
    submitted: false,
    errors: []
  },
  
  handleEmailChange(event) {
    console.log(`[Event] onChange={handleEmailChange}`);
    this.state.email = event.target.value;
    console.log(`  Email: "${this.state.email}"`);
  },
  
  handlePasswordChange(event) {
    console.log(`[Event] onChange={handlePasswordChange}`);
    this.state.password = event.target.value;
    console.log(`  Password: "${'*'.repeat(this.state.password.length)}"`);
  },
  
  handleSubmit(event) {
    console.log(`[Event] onSubmit={handleSubmit}`);
    event.preventDefault();
    
    // Validation
    this.state.errors = [];
    if (!this.state.email.includes('@')) {
      this.state.errors.push('Invalid email');
    }
    if (this.state.password.length < 6) {
      this.state.errors.push('Password too short');
    }
    
    if (this.state.errors.length === 0) {
      this.state.submitted = true;
      console.log('  ✓ Form valid! Logging in...');
    } else {
      console.log('  ✗ Form errors:', this.state.errors.join(', '));
    }
  },
  
  render() {
    console.log('\n[Render] Login Form');
    console.log('  Email:', this.state.email || '(empty)');
    console.log('  Password:', '*'.repeat(this.state.password.length) || '(empty)');
    if (this.state.errors.length > 0) {
      console.log('  Errors:', this.state.errors.join(', '));
    }
    if (this.state.submitted) {
      console.log('  Status: ✓ Logged in!');
    }
    console.log('');
  }
};

// Run simulations
console.log('=== Counter App Simulation ===\n');

CounterApp.render();

console.log('User clicks +1:');
CounterApp.handleIncrement();

console.log('User clicks +1 again:');
CounterApp.handleIncrement();

console.log('User clicks +5:');
CounterApp.handleIncrementBy(5);

console.log('User clicks -1:');
CounterApp.handleDecrement();

console.log('User clicks reset:');
CounterApp.handleReset();

console.log('\n=== Login Form Simulation ===\n');

LoginForm.render();

console.log('User types email:');
LoginForm.handleEmailChange({ target: { value: 'a' } });
LoginForm.handleEmailChange({ target: { value: 'al' } });
LoginForm.handleEmailChange({ target: { value: 'alice@example.com' } });
LoginForm.render();

console.log('User types password:');
LoginForm.handlePasswordChange({ target: { value: 'pass' } });
LoginForm.render();

console.log('User clicks submit (invalid):');
LoginForm.handleSubmit({ preventDefault: () => {} });
LoginForm.render();

console.log('User fixes password:');
LoginForm.handlePasswordChange({ target: { value: 'password123' } });
LoginForm.render();

console.log('User clicks submit (valid):');
LoginForm.handleSubmit({ preventDefault: () => {} });
LoginForm.render();

console.log('=== Key Takeaways ===\n');
let takeaways = [
  '✓ Event handlers respond to user actions',
  '✓ Use onClick, onChange, onSubmit, etc.',
  '✓ Pass function reference, not call: onClick={handleClick}',
  '✓ Access event with parameter: (e) => ...',
  '✓ Use e.preventDefault() to prevent default behavior',
  '✓ Controlled inputs: value={state} onChange={setState}',
  '✓ Event updates state → state update triggers re-render'
];
takeaways.forEach(t => console.log(t));