---
type: "WARNING"
title: "Common Pitfalls"
---

Common React deployment mistakes:

1. **Wrong environment variable prefix**:
   ```javascript
   // WRONG! Won't work in Vite
   const API_URL = process.env.REACT_APP_API_URL;  // ✗
   const API_URL = process.env.API_URL;             // ✗
   
   // CORRECT for Vite!
   const API_URL = import.meta.env.VITE_API_URL;   // ✓
   ```

2. **Hardcoded API URL**:
   ```javascript
   // WRONG! Won't work after deployment
   fetch('http://localhost:3000/api/users');  // ✗
   
   // CORRECT!
   const API_URL = import.meta.env.VITE_API_URL;
   fetch(`${API_URL}/api/users`);  // ✓
   ```

3. **Forgot to add env vars in Vercel dashboard**:
   ```
   Error: import.meta.env.VITE_API_URL is undefined
   
   Fix:
   1. Go to Vercel project settings
   2. Environment Variables
   3. Add: VITE_API_URL = https://my-api.onrender.com
   4. Redeploy
   ```

4. **Backend CORS not updated**:
   ```javascript
   // Error in browser console:
   // "Access to fetch at 'https://api.com' from origin 'https://my-app.vercel.app' 
   // has been blocked by CORS policy"
   
   // Fix in Express backend:
   const allowedOrigins = [
     'https://my-app.vercel.app',  // Add this!
     'http://localhost:5173'
   ];
   
   app.use(cors({ origin: allowedOrigins }));
   ```

5. **Build folder committed to Git**:
   ```bash
   # .gitignore should include:
   dist/
   build/
   .vercel/
   
   # If accidentally committed:
   git rm -r --cached dist
   git commit -m "Remove dist folder"
   ```

6. **Wrong build output directory**:
   ```
   # Vercel settings:
   Build Command: npm run build
   Output Directory: dist    ← Must match Vite's output!
   
   # Vite outputs to dist/ by default
   # If you changed it in vite.config.js, update Vercel settings
   ```

7. **Client-side routing 404 errors**:
   ```
   Problem: Refreshing /about gives 404 error
   
   Solution: Add vercel.json:
   {
     "rewrites": [
       { "source": "/(.*)", "destination": "/index.html" }
     ]
   }
   
   This tells Vercel to serve index.html for all routes
   (React Router handles routing on the client)
   ```

8. **Mixed content warning (HTTP/HTTPS)**:
   ```javascript
   // WRONG! Frontend is HTTPS, API is HTTP
   VITE_API_URL=http://my-api.com  // ✗ Browser blocks this!
   
   // CORRECT! Both must be HTTPS
   VITE_API_URL=https://my-api.com  // ✓
   ```

9. **Environment variables not rebuilding**:
   ```
   Changed VITE_API_URL but still seeing old value?
   
   Fix:
   1. Environment vars are baked into build at build time
   2. Must trigger new deployment after changing them
   3. In Vercel: Deployments → Redeploy
   ```

10. **Forgot to test production build locally**:
    ```bash
    # Always test before deploying!
    npm run build      # Build for production
    npm run preview    # Preview the build locally
    
    # Open http://localhost:4173 and test everything
    # Make sure API calls work!
    ```