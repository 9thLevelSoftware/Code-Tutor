---
type: "THEORY"
title: "Breaking Down the Syntax"
---

React deployment configuration:

1. **Environment Variables** (VITE_ prefix required!):
   ```javascript
   // .env.local (development)
   VITE_API_URL=http://localhost:3000
   VITE_DEBUG=true
   
   // In Vercel dashboard (production)
   VITE_API_URL=https://my-api.onrender.com
   VITE_DEBUG=false
   ```
   
   ```javascript
   // Using in React code
   const API_URL = import.meta.env.VITE_API_URL;
   
   fetch(`${API_URL}/api/users`)
     .then(res => res.json())
     .then(data => console.log(data));
   ```

2. **vite.config.js** (usually default is fine):
   ```javascript
   import { defineConfig } from 'vite';
   import react from '@vitejs/plugin-react';
   
   export default defineConfig({
     plugins: [react()],
     build: {
       outDir: 'dist',  // Output folder
       sourcemap: false  // Don't include source maps in production
     }
   });
   ```

3. **Build Command**:
   ```bash
   # Test build locally first!
   npm run build
   
   # Creates dist/ folder with:
   # - index.html
   # - assets/index-[hash].js
   # - assets/index-[hash].css
   
   # Preview build locally
   npm run preview
   # Opens http://localhost:4173
   ```

4. **package.json Scripts**:
   ```json
   {
     "scripts": {
       "dev": "vite",
       "build": "vite build",
       "preview": "vite preview"
     }
   }
   ```

5. **Connecting to Backend API**:
   ```javascript
   // src/config/api.js
   const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3000';
   
   export async function fetchUsers() {
     const response = await fetch(`${API_URL}/api/users`);
     return response.json();
   }
   
   export async function createUser(userData) {
     const response = await fetch(`${API_URL}/api/users`, {
       method: 'POST',
       headers: { 'Content-Type': 'application/json' },
       body: JSON.stringify(userData)
     });
     return response.json();
   }
   ```

6. **Update Backend CORS** (CRITICAL!):
   ```javascript
   // Express backend server.js
   const allowedOrigins = [
     'https://my-app.vercel.app',      // Your Vercel URL
     'https://my-app-git-*.vercel.app', // Preview deployments
     'http://localhost:5173'            // Local dev
   ];
   
   app.use(cors({
     origin: allowedOrigins,
     credentials: true
   }));
   ```

7. **Vercel Configuration** (optional vercel.json):
   ```json
   {
     "buildCommand": "npm run build",
     "outputDirectory": "dist",
     "rewrites": [
       {
         "source": "/(.*)",
         "destination": "/index.html"
       }
     ]
   }
   ```
   The `rewrites` rule handles client-side routing (React Router).

8. **.gitignore** (don't commit build folder!):
   ```
   node_modules/
   dist/
   .env
   .env.local
   .env.production
   .vercel
   ```