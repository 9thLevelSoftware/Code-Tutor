---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// Hono - RESTful API Example (2025)

// Simulated Hono app
class HonoApp {
  constructor() { this.routes = []; }
  get(path, handler) { this.routes.push({ method: 'GET', path, handler }); }
  post(path, handler) { this.routes.push({ method: 'POST', path, handler }); }
  put(path, handler) { this.routes.push({ method: 'PUT', path, handler }); }
  delete(path, handler) { this.routes.push({ method: 'DELETE', path, handler }); }
  
  async simulateRequest(method, path, body = null) {
    console.log(`\n=== ${method} ${path} ===`);
    if (body) console.log('Body:', JSON.stringify(body));
    
    let route = this.routes.find(r => {
      if (r.method !== method) return false;
      let routePattern = r.path.replace(':id', '([^/]+)');
      return new RegExp('^' + routePattern + '$').test(path);
    });
    
    if (route) {
      let id = path.match(/\d+$/)?.[0];
      // Hono context object
      let c = {
        req: {
          param: (key) => key === 'id' ? id : null,
          json: async () => body || {}
        },
        json: function(data, status = 200) {
          console.log(`Status: ${status}`);
          console.log('Response:', JSON.stringify(data, null, 2));
          return { status, body: data };
        }
      };
      await route.handler(c);
    } else {
      console.log('Status: 404');
      console.log('Response: Not Found');
    }
  }
}

let app = new HonoApp();

// IN-MEMORY DATABASE (simulated)
let books = [
  { id: 1, title: '1984', author: 'George Orwell', year: 1949, available: true },
  { id: 2, title: 'To Kill a Mockingbird', author: 'Harper Lee', year: 1960, available: true },
  { id: 3, title: 'The Great Gatsby', author: 'F. Scott Fitzgerald', year: 1925, available: false }
];

let nextId = 4;

// RESTful API ROUTES with Hono

// 1. GET /api/books - List all books
app.get('/api/books', (c) => {
  return c.json({
    count: books.length,
    books: books
  });
});

// 2. GET /api/books/:id - Get one book
app.get('/api/books/:id', (c) => {
  const id = parseInt(c.req.param('id'));
  const book = books.find(b => b.id === id);
  
  if (!book) {
    return c.json({ 
      error: 'Book not found',
      id: id 
    }, 404);
  }
  
  return c.json(book);
});

// 3. POST /api/books - Create new book
app.post('/api/books', async (c) => {
  const body = await c.req.json();
  
  // Validation
  if (!body.title || !body.author) {
    return c.json({
      error: 'Validation failed',
      message: 'Title and author are required'
    }, 400);
  }
  
  const newBook = {
    id: nextId++,
    title: body.title,
    author: body.author,
    year: body.year,
    available: true
  };
  
  books.push(newBook);
  
  return c.json({
    message: 'Book created successfully',
    book: newBook
  }, 201);
});

// 4. PUT /api/books/:id - Update entire book (replace)
app.put('/api/books/:id', async (c) => {
  const id = parseInt(c.req.param('id'));
  const body = await c.req.json();
  const index = books.findIndex(b => b.id === id);
  
  if (index === -1) {
    return c.json({ error: 'Book not found' }, 404);
  }
  
  // Replace entire book (keeping the ID)
  books[index] = {
    id: id,
    title: body.title,
    author: body.author,
    year: body.year,
    available: body.available
  };
  
  return c.json({
    message: 'Book updated successfully',
    book: books[index]
  });
});

// 5. DELETE /api/books/:id - Delete a book
app.delete('/api/books/:id', (c) => {
  const id = parseInt(c.req.param('id'));
  const index = books.findIndex(b => b.id === id);
  
  if (index === -1) {
    return c.json({ error: 'Book not found' }, 404);
  }
  
  const deletedBook = books.splice(index, 1)[0];
  
  return c.json({
    message: 'Book deleted successfully',
    book: deletedBook
  });
});

// TEST THE API

console.log('\nRESTful API Demo - Book Library System (Hono)\n');

// List all books
app.simulateRequest('GET', '/api/books');

// Get specific book
app.simulateRequest('GET', '/api/books/1');

// Get non-existent book
app.simulateRequest('GET', '/api/books/999');

// Create new book
app.simulateRequest('POST', '/api/books', {
  title: 'The Hobbit',
  author: 'J.R.R. Tolkien',
  year: 1937
});

// Update book
app.simulateRequest('PUT', '/api/books/1', {
  title: '1984',
  author: 'George Orwell',
  year: 1949,
  available: false
});

// Delete book
app.simulateRequest('DELETE', '/api/books/2');

// List all books after changes
app.simulateRequest('GET', '/api/books');
```
