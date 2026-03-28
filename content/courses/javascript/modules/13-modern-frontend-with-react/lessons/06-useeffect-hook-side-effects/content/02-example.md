---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// useEffect Hook - Side Effects

console.log('=== useEffect Hook ===\n');

// SIDE EFFECT = Code that affects something outside the component
// Examples: API calls, timers, DOM manipulation, subscriptions

// Simulating component lifecycle
class Component {
  constructor(name) {
    this.name = name;
    this.effects = [];
    this.mounted = false;
  }
  
  useEffect(effect, dependencies) {
    this.effects.push({ effect, dependencies });
  }
  
  mount() {
    console.log(`[${this.name}] Mounting...`);
    this.mounted = true;
    
    // Run all effects
    this.effects.forEach(({ effect, dependencies }) => {
      console.log(`[${this.name}] Running effect (dependencies: ${dependencies || 'none'})`);
      let cleanup = effect();
      if (cleanup) {
        console.log(`[${this.name}] Effect registered cleanup function`);
      }
    });
  }
  
  unmount() {
    console.log(`\n[${this.name}] Unmounting...`);
    console.log(`[${this.name}] Running cleanup functions`);
    this.mounted = false;
  }
}

// Example 1: Effect runs once on mount
let TitleComponent = new Component('TitleComponent');

TitleComponent.useEffect(() => {
  console.log('  Setting document title to: "My React App"');
  // In real React: document.title = 'My React App';
}, []); // Empty array = run once on mount

TitleComponent.mount();

// Example 2: Effect with cleanup
console.log('\n--- Timer Component ---');
let TimerComponent = new Component('TimerComponent');

TimerComponent.useEffect(() => {
  console.log('  Starting timer (setInterval)');
  let intervalId = 123;
  
  // Cleanup function (returned)
  return () => {
    console.log('  Stopping timer (clearInterval)');
  };
}, []);

TimerComponent.mount();
setTimeout(() => TimerComponent.unmount(), 1000);

// Example 3: Effect runs when dependency changes
setTimeout(() => {
  console.log('\n--- User Profile Component ---');
  
  let ProfileComponent = new Component('ProfileComponent');
  let userId = 1;
  
  ProfileComponent.useEffect(() => {
    console.log(`  Fetching data for user ${userId}...`);
    console.log(`  fetch('/api/users/${userId}')`);
  }, [userId]); // Re-run when userId changes
  
  ProfileComponent.mount();
  
  // Simulate prop change
  setTimeout(() => {
    console.log('\n[Props Changed] userId: 1 → 2');
    userId = 2;
    console.log('[ProfileComponent] Re-running effects with new userId');
    console.log(`  Fetching data for user ${userId}...`);
  }, 500);
}, 1500);

// DEPENDENCY ARRAY PATTERNS
setTimeout(() => {
  console.log('\n\n=== useEffect Dependency Patterns ===\n');
  
  let patterns = [
    {
      code: 'useEffect(() => { ... });',
      deps: 'NO array',
      runs: 'Every render (usually a mistake!)'
    },
    {
      code: 'useEffect(() => { ... }, []);',
      deps: 'Empty []',
      runs: 'Once on mount only'
    },
    {
      code: 'useEffect(() => { ... }, [count]);',
      deps: '[count]',
      runs: 'On mount + when count changes'
    },
    {
      code: 'useEffect(() => { ... }, [a, b, c]);',
      deps: '[a, b, c]',
      runs: 'On mount + when a, b, OR c changes'
    }
  ];
  
  patterns.forEach(p => {
    console.log(`${p.code}`);
    console.log(`  Dependencies: ${p.deps}`);
    console.log(`  Runs: ${p.runs}\n`);
  });
}, 2500);

// COMMON USE CASES
setTimeout(() => {
  console.log('=== Common useEffect Use Cases ===\n');
  
  console.log('1. FETCHING DATA:');
  console.log('useEffect(() => {');
  console.log('  fetch("/api/users")');
  console.log('    .then(res => res.json())');
  console.log('    .then(data => setUsers(data));');
  console.log('}, []); // Fetch once on mount\n');
  
  console.log('2. SETTING DOCUMENT TITLE:');
  console.log('useEffect(() => {');
  console.log('  document.title = `Count: ${count}`;');
  console.log('}, [count]); // Update when count changes\n');
  
  console.log('3. SUBSCRIBING TO EVENTS:');
  console.log('useEffect(() => {');
  console.log('  function handleResize() {');
  console.log('    setWidth(window.innerWidth);');
  console.log('  }');
  console.log('  ');
  console.log('  window.addEventListener("resize", handleResize);');
  console.log('  ');
  console.log('  return () => {  // Cleanup!');
  console.log('    window.removeEventListener("resize", handleResize);');
  console.log('  };');
  console.log('}, []); // Set up once\n');
  
  console.log('4. TIMERS:');
  console.log('useEffect(() => {');
  console.log('  const timer = setInterval(() => {');
  console.log('    setCount(c => c + 1);');
  console.log('  }, 1000);');
  console.log('  ');
  console.log('  return () => clearInterval(timer); // Cleanup!');
  console.log('}, []); // Start timer once\n');
  
  console.log('5. LOCAL STORAGE:');
  console.log('useEffect(() => {');
  console.log('  localStorage.setItem("theme", theme);');
  console.log('}, [theme]); // Save when theme changes');
}, 2600);
```
