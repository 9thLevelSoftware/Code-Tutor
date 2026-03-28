// Simulate API
function fetchUsers() {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve([
        { id: 1, name: 'Alice', email: 'alice@example.com' },
        { id: 2, name: 'Bob', email: 'bob@example.com' },
        { id: 3, name: 'Charlie', email: 'charlie@example.com' }
      ]);
    }, 1000);
  });
}

// Simulate React component
let UserListComponent = {
  state: {
    users: [],
    loading: false,
    error: null
  },
  
  setState(updates) {
    this.state = { ...this.state, ...updates };
    console.log('[State Updated]', this.state);
  },
  
  async mount() {
    console.log('[Component] Mounting...');
    console.log('[useEffect] Running effect (fetch users)');
    
    this.setState({ loading: true });
    
    try {
      console.log('[Fetch] Calling API...');
      let users = await fetchUsers();
      console.log('[Fetch] Received', users.length, 'users');
      this.setState({ users, loading: false });
    } catch (err) {
      console.log('[Fetch] Error:', err.message);
      this.setState({ error: err.message, loading: false });
    }
  }
};

// Test
console.log('=== UserList Component ===\n');
UserListComponent.mount();