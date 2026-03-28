// Clock Component with useEffect

let ClockComponent = {
  state: { time: new Date().toLocaleTimeString() },
  timerId: null,
  
  mount() {
    console.log('[Clock] Mounting...');
    console.log('[Clock] useEffect(() => { ... }, [])');
    
    // Effect: Start timer
    this.timerId = setInterval(() => {
      this.state.time = new Date().toLocaleTimeString();
      console.log('[Clock] Tick:', this.state.time);
    }, 1000);
    
    console.log('[Clock] Timer started (ID:', this.timerId, ')');
    console.log('[Clock] Registered cleanup function\n');
  },
  
  unmount() {
    console.log('\n[Clock] Unmounting...');
    console.log('[Clock] Running cleanup function');
    
    // Cleanup: Stop timer
    if (this.timerId) {
      clearInterval(this.timerId);
      console.log('[Clock] Timer stopped');
    }
  }
};

// Test
async function testClock() {
  console.log('=== Clock Component Test ===\n');
  
  ClockComponent.mount();
  
  // Wait 3 seconds
  await new Promise(resolve => setTimeout(resolve, 3000));
  
  ClockComponent.unmount();
}

testClock();