// Authentication App with Conditional Rendering

let AuthApp = {
  state: {
    isLoggedIn: false,
    user: null,
    loading: false
  },
  
  render() {
    console.log('\n[Render] Auth UI:');
    console.log('─'.repeat(40));
    
    if (this.state.loading) {
      console.log('⏳ Loading...');
    } else if (this.state.isLoggedIn) {
      console.log(`Welcome, ${this.state.user}!`);
      console.log('[Logout Button]');
    } else {
      console.log('Please log in to continue');
      console.log('[Login Button]');
    }
    
    console.log('─'.repeat(40) + '\n');
  },
  
  async login(username) {
    console.log(`[Action] Login as ${username}`);
    this.state.loading = true;
    this.render();
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 500));
    
    this.state.loading = false;
    this.state.isLoggedIn = true;
    this.state.user = username;
    this.render();
  },
  
  logout() {
    console.log('[Action] Logout');
    this.state.isLoggedIn = false;
    this.state.user = null;
    this.render();
  }
};

// Test all states
async function testAuthApp() {
  console.log('=== Auth App Test ===');
  
  AuthApp.render();
  
  await AuthApp.login('Alice');
  
  await new Promise(resolve => setTimeout(resolve, 500));
  
  AuthApp.logout();
}

testAuthApp();