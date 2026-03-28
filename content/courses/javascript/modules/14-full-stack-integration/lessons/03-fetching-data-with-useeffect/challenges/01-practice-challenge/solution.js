// Complete data fetching simulation

// Mock API with various endpoints
let mockAPI = {
  users: [
    { id: 1, name: 'Alice', email: 'alice@example.com', role: 'Admin' },
    { id: 2, name: 'Bob', email: 'bob@example.com', role: 'User' },
    { id: 3, name: 'Charlie', email: 'charlie@example.com', role: 'User' }
  ],
  
  fetchUsers(delay = 1000) {
    console.log(`[API] GET /api/users (simulating ${delay}ms network delay)`);
    return new Promise((resolve) => {
      setTimeout(() => {
        console.log('[API] Responding with', this.users.length, 'users');
        resolve([...this.users]);
      }, delay);
    });
  },
  
  fetchUserById(id, delay = 800) {
    console.log(`[API] GET /api/users/${id}`);
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        let user = this.users.find(u => u.id === id);
        if (user) {
          console.log('[API] Found user:', user.name);
          resolve({ ...user });
        } else {
          console.log('[API] 404 Not Found');
          reject(new Error('User not found'));
        }
      }, delay);
    });
  },
  
  createUser(userData, delay = 1000) {
    console.log('[API] POST /api/users', userData);
    return new Promise((resolve) => {
      setTimeout(() => {
        let newUser = {
          id: this.users.length + 1,
          ...userData
        };
        this.users.push(newUser);
        console.log('[API] Created user:', newUser);
        resolve(newUser);
      }, delay);
    });
  }
};

// Component 1: UserList (fetch all users on mount)
let UserListComponent = {
  state: { users: [], loading: false, error: null },
  
  setState(updates) {
    this.state = { ...this.state, ...updates };
    console.log('[UserList State]', this.state);
  },
  
  async mount() {
    console.log('[UserList] Component mounting...');
    console.log('[UserList] useEffect(() => { fetchUsers() }, [])');
    
    try {
      this.setState({ loading: true, error: null });
      let users = await mockAPI.fetchUsers();
      this.setState({ users, loading: false });
      console.log('[UserList] Render with', users.length, 'users\n');
    } catch (err) {
      this.setState({ error: err.message, loading: false });
      console.log('[UserList] Render error state\n');
    }
  }
};

// Component 2: UserProfile (fetch user when ID changes)
let UserProfileComponent = {
  state: { userId: null, user: null, loading: false, error: null },
  
  setState(updates) {
    let oldUserId = this.state.userId;
    this.state = { ...this.state, ...updates };
    console.log('[UserProfile State]', this.state);
    
    // Simulate useEffect with [userId] dependency
    if ('userId' in updates && updates.userId !== oldUserId) {
      console.log('[UserProfile] userId changed → running effect');
      this.fetchUser();
    }
  },
  
  async fetchUser() {
    if (!this.state.userId) {
      console.log('[UserProfile] No userId, skipping fetch');
      return;
    }
    
    console.log(`[UserProfile] useEffect(() => { fetchUser(${this.state.userId}) }, [userId])`);
    
    try {
      this.state.loading = true;
      this.state.error = null;
      let user = await mockAPI.fetchUserById(this.state.userId);
      this.state.user = user;
      this.state.loading = false;
      console.log('[UserProfile] Render with user:', user.name, '\n');
    } catch (err) {
      this.state.error = err.message;
      this.state.loading = false;
      console.log('[UserProfile] Render error\n');
    }
  },
  
  mount() {
    console.log('[UserProfile] Component mounting...');
    console.log('[UserProfile] useEffect registered with [userId] dependency\n');
  }
};

// Run simulation
async function runSimulation() {
  console.log('=== Full-Stack Data Fetching Simulation ===\n');
  
  // Scenario 1: Fetch all users on mount
  console.log('--- Scenario 1: UserList Component ---\n');
  await UserListComponent.mount();
  
  // Scenario 2: Fetch specific user when ID changes
  console.log('--- Scenario 2: UserProfile Component ---\n');
  UserProfileComponent.mount();
  
  await new Promise(resolve => setTimeout(resolve, 100));
  console.log('[User Action] Clicks on user 1');
  UserProfileComponent.setState({ userId: 1 });
  
  await new Promise(resolve => setTimeout(resolve, 1000));
  console.log('[User Action] Clicks on user 2');
  UserProfileComponent.setState({ userId: 2 });
  
  // Scenario 3: Create new user
  await new Promise(resolve => setTimeout(resolve, 1000));
  console.log('\n--- Scenario 3: Create User ---\n');
  let newUser = await mockAPI.createUser({ 
    name: 'Diana', 
    email: 'diana@example.com',
    role: 'User'
  });
  
  console.log('\n--- Final State ---');
  console.log('Total users in database:', mockAPI.users.length);
  console.log('Users:', mockAPI.users.map(u => u.name).join(', '));
}

runSimulation();