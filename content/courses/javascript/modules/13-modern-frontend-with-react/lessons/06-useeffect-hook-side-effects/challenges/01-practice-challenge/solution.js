// Complete useEffect simulation

let ClockComponent = {
  state: { time: new Date().toLocaleTimeString(), ticks: 0 },
  timerId: null,
  mounted: false,
  
  mount() {
    console.log('┌────────────────────────────────┐');
    console.log('│     Clock Component Mount      │');
    console.log('└────────────────────────────────┘\n');
    
    this.mounted = true;
    
    console.log('[useEffect] Starting side effects...');
    console.log('[Effect 1] Setting document title');
    // document.title = 'Clock App';
    
    console.log('[Effect 2] Starting interval timer\n');
    this.timerId = setInterval(() => {
      if (this.mounted) {
        this.state.time = new Date().toLocaleTimeString();
        this.state.ticks++;
        console.log(`⏰ ${this.state.time} (tick #${this.state.ticks})`);
      }
    }, 1000);
    
    console.log(`✓ Timer ID: ${this.timerId}`);
    console.log('✓ Cleanup function registered\n');
  },
  
  unmount() {
    console.log('\n┌────────────────────────────────┐');
    console.log('│    Clock Component Unmount     │');
    console.log('└────────────────────────────────┘\n');
    
    this.mounted = false;
    
    console.log('[Cleanup] Running effect cleanup...');
    
    if (this.timerId) {
      clearInterval(this.timerId);
      console.log(`✓ Timer ${this.timerId} cleared`);
      this.timerId = null;
    }
    
    console.log(`✓ Total ticks: ${this.state.ticks}`);
    console.log('✓ Component unmounted cleanly\n');
  }
};

// Fetch component with cleanup
let DataFetcher = {
  state: { data: null, loading: false },
  controller: null,
  
  async mount(userId) {
    console.log('┌────────────────────────────────┐');
    console.log('│   DataFetcher Mount (user:${userId})   │'.replace('${userId}', userId));
    console.log('└────────────────────────────────┘\n');
    
    console.log('[useEffect] Running with [userId] dependency');
    console.log(`  userId = ${userId}\n`);
    
    // Simulate AbortController for fetch cancellation
    this.controller = { aborted: false };
    
    this.state.loading = true;
    console.log('[Fetch] Starting request...');
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    if (!this.controller.aborted) {
      this.state.data = { id: userId, name: `User ${userId}` };
      this.state.loading = false;
      console.log('[Fetch] Success:', this.state.data);
    } else {
      console.log('[Fetch] Aborted');
    }
    
    console.log('\n✓ Cleanup function registered');
  },
  
  unmount() {
    console.log('\n[Cleanup] Aborting fetch if in progress...');
    if (this.controller) {
      this.controller.aborted = true;
      console.log('✓ Fetch aborted');
    }
  }
};

// Run demonstrations
async function runDemo() {
  console.log('═══ useEffect Demo ═══\n');
  
  // Demo 1: Clock
  console.log('DEMO 1: Clock with Timer\n');
  ClockComponent.mount();
  
  await new Promise(resolve => setTimeout(resolve, 3500));
  
  ClockComponent.unmount();
  
  await new Promise(resolve => setTimeout(resolve, 500));
  
  // Demo 2: Data fetching
  console.log('\nDEMO 2: Data Fetching with Cleanup\n');
  console.log('Fetching user 1...');
  let fetch1 = DataFetcher.mount(1);
  
  await new Promise(resolve => setTimeout(resolve, 500));
  
  console.log('\n[Props Change] userId: 1 → 2');
  console.log('Cleaning up old effect...');
  DataFetcher.unmount();
  
  console.log('\nFetching user 2...');
  await DataFetcher.mount(2);
  
  console.log('\n\n=== useEffect Best Practices ===\n');
  let practices = [
    '✓ Always include dependency array ([], [dep], etc.)',
    '✓ Return cleanup function when needed',
    '✓ Don\'t call async functions directly in useEffect',
    '✓ Clean up timers, subscriptions, event listeners',
    '✓ Use separate useEffect for unrelated logic',
    '✓ Put all dependencies in the array',
    '✓ Cleanup prevents memory leaks'
  ];
  
  practices.forEach(p => console.log(p));
}

runDemo();