---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// useEffect + fetch - Getting data from APIs

console.log('=== useEffect for Data Fetching ===\n');

// Simulate React component lifecycle
class ComponentLifecycle {
  constructor(name) {
    this.name = name;
    this.effects = [];
    this.state = {};
  }
  
  // Simulate useEffect
  useEffect(callback, dependencies) {
    console.log(`[${this.name}] Registering effect with dependencies:`, dependencies || 'none');
    this.effects.push({ callback, dependencies });
  }
  
  // Simulate component mount
  mount() {
    console.log(`\n[${this.name}] Component mounting...`);
    this.effects.forEach(effect => {
      if (!effect.dependencies || effect.dependencies.length === 0) {
        console.log(`[${this.name}] Running effect (runs on mount)`);
        effect.callback();
      }
    });
  }
  
  // Simulate state update
  setState(newState) {
    console.log(`\n[${this.name}] State updating:`, newState);
    let oldState = { ...this.state };
    this.state = { ...this.state, ...newState };
    
    this.effects.forEach(effect => {
      if (effect.dependencies) {
        let changed = effect.dependencies.some(dep => {
          return oldState[dep] !== this.state[dep];
        });
        if (changed) {
          console.log(`[${this.name}] Dependency changed, re-running effect`);
          effect.callback();
        }
      }
    });
  }
}

// Example 1: Fetch data on component mount
console.log('=== Example 1: Fetch Users on Mount ===');

let UserList = new ComponentLifecycle('UserList');

UserList.useEffect(() => {
  console.log('[UserList] Fetching users from API...');
  console.log('[UserList] fetch("http://localhost:4000/api/users")');
  
  // Simulate API response
  setTimeout(() => {
    let users = [
      { id: 1, name: 'Alice' },
      { id: 2, name: 'Bob' }
    ];
    console.log('[UserList] Received:', users.length, 'users');
    console.log('[UserList] setUsers(data) → triggers re-render');
  }, 100);
}, []); // Empty array = run once on mount

UserList.mount();

// Example 2: Fetch when dependency changes
setTimeout(() => {
  console.log('\n\n=== Example 2: Fetch User Details When ID Changes ===');
  
  let UserProfile = new ComponentLifecycle('UserProfile');
  UserProfile.state = { userId: null };
  
  UserProfile.useEffect(() => {
    if (UserProfile.state.userId) {
      console.log(`[UserProfile] Fetching user ${UserProfile.state.userId}...`);
      console.log(`[UserProfile] fetch("http://localhost:4000/api/users/${UserProfile.state.userId}")`);
      
      setTimeout(() => {
        let user = { id: UserProfile.state.userId, name: 'Alice', email: 'alice@example.com' };
        console.log('[UserProfile] Received:', user);
      }, 100);
    }
  }, ['userId']); // Re-run when userId changes
  
  UserProfile.mount();
  
  // Simulate user clicking different profiles
  setTimeout(() => {
    console.log('\n[User] Clicks on user 1');
    UserProfile.setState({ userId: 1 });
  }, 200);
  
  setTimeout(() => {
    console.log('\n[User] Clicks on user 2');
    UserProfile.setState({ userId: 2 });
  }, 400);
}, 300);

// Example 3: Real fetch pattern
setTimeout(() => {
  console.log('\n\n=== Example 3: Complete Fetch Pattern ===\n');
  
  console.log('// React component with useEffect\n');
  console.log('function UserList() {');
  console.log('  const [users, setUsers] = useState([]);');
  console.log('  const [loading, setLoading] = useState(true);');
  console.log('  const [error, setError] = useState(null);\n');
  
  console.log('  useEffect(() => {');
  console.log('    async function fetchUsers() {');
  console.log('      try {');
  console.log('        setLoading(true);');
  console.log('        const response = await fetch("http://localhost:4000/api/users");');
  console.log('        ');
  console.log('        if (!response.ok) {');
  console.log('          throw new Error(`HTTP error! status: ${response.status}`);');
  console.log('        }');
  console.log('        ');
  console.log('        const data = await response.json();');
  console.log('        setUsers(data);');
  console.log('      } catch (err) {');
  console.log('        setError(err.message);');
  console.log('      } finally {');
  console.log('        setLoading(false);');
  console.log('      }');
  console.log('    }\n');
  console.log('    fetchUsers();');
  console.log('  }, []); // Run once on mount\n');
  
  console.log('  if (loading) return <div>Loading...</div>;');
  console.log('  if (error) return <div>Error: {error}</div>;');
  
  console.log('  return (');
  console.log('    <ul>');
  console.log('      {users.map(user => (');
  console.log('        <li key={user.id}>{user.name}</li>');
  console.log('      ))}');
  console.log('    </ul>');
  console.log('  );');
  console.log('}');
}, 800);

// Dependency array explanation
setTimeout(() => {
  console.log('\n\n=== useEffect Dependency Array ===\n');
  
  let cases = [
    {
      code: 'useEffect(() => { ... });',
      dependencies: 'NONE',
      when: 'Every render (usually a mistake!)'
    },
    {
      code: 'useEffect(() => { ... }, []);',
      dependencies: '[] (empty)',
      when: 'Once on mount only'
    },
    {
      code: 'useEffect(() => { ... }, [userId]);',
      dependencies: '[userId]',
      when: 'On mount + whenever userId changes'
    },
    {
      code: 'useEffect(() => { ... }, [userId, page]);',
      dependencies: '[userId, page]',
      when: 'On mount + when userId OR page changes'
    }
  ];
  
  cases.forEach(c => {
    console.log(`${c.code}`);
    console.log(`  Dependencies: ${c.dependencies}`);
    console.log(`  Runs: ${c.when}\n`);
  });
}, 1100);
```
