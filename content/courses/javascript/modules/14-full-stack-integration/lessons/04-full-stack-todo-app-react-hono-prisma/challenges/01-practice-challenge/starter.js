// Simplified blog system

let database = {
  posts: [
    { id: 1, title: 'First Post', content: 'Hello world!' },
    { id: 2, title: 'Second Post', content: 'Learning full-stack!' }
  ],
  
  findAll() {
    console.log('[DB] SELECT * FROM posts');
    return [...this.posts];
  },
  
  create(data) {
    console.log('[DB] INSERT INTO posts', data);
    let post = { id: this.posts.length + 1, ...data };
    this.posts.push(post);
    return post;
  }
};

let backend = {
  handleGetPosts() {
    console.log('[API] GET /api/posts');
    let posts = database.findAll();
    return { status: 200, data: posts };
  },
  
  handleCreatePost(title, content) {
    console.log('[API] POST /api/posts');
    let post = database.create({ title, content });
    return { status: 201, data: post };
  }
};

let BlogApp = {
  state: { posts: [], loading: false },
  
  setState(updates) {
    this.state = { ...this.state, ...updates };
    console.log('[App State]', this.state);
  },
  
  async mount() {
    console.log('[App] Mounting...');
    await this.fetchPosts();
  },
  
  async fetchPosts() {
    console.log('[App] Fetching posts...');
    this.setState({ loading: true });
    
    let response = backend.handleGetPosts();
    this.setState({ posts: response.data, loading: false });
  },
  
  async createPost(title, content) {
    console.log(`[App] Creating post: ${title}`);
    let response = backend.handleCreatePost(title, content);
    console.log('[App] Post created, refreshing...');
    await this.fetchPosts();
  }
};

// Test
console.log('=== Blog App Test ===\n');
BlogApp.mount().then(() => {
  console.log('\n[User] Clicks "New Post"');
  BlogApp.createPost('Third Post', 'Full-stack is awesome!');
});