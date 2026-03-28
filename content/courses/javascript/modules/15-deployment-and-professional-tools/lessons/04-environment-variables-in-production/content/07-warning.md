---
type: "WARNING"
title: "Common Pitfalls"
---

Common environment variable mistakes:

1. **Committing .env to Git** (MAJOR security issue!):
   ```bash
   # Check if .env is tracked:
   git ls-files | grep .env
   
   # If it returns .env, you've committed secrets!
   # Fix immediately:
   git rm --cached .env
   echo ".env" >> .gitignore
   git commit -m "Remove .env from git"
   
   # If already pushed to GitHub:
   # 1. Rotate all secrets immediately!
   # 2. Consider the secrets compromised
   ```

2. **Forgot to load dotenv**:
   ```javascript
   // Error: process.env.DATABASE_URL is undefined
   
   // Fix: Load dotenv at the very top
   import 'dotenv/config';  // Must be first!
   import express from 'express';
   
   // or
   require('dotenv').config();  // Must be first!
   const express = require('express');
   ```

3. **Wrong variable names** (typos):
   ```javascript
   // .env
   DATABASE_URL=postgres://...
   
   // server.js
   const db = process.env.DATABSE_URL;  // ✗ Typo! Returns undefined
   const db = process.env.DATABASE_URL;  // ✓ Correct
   
   // Tip: Use constants to avoid typos
   const REQUIRED_VARS = ['DATABASE_URL', 'JWT_SECRET'];
   ```

4. **Not updating production env vars**:
   ```
   Changed .env locally but forgot to update Render!
   
   Fix:
   1. Update .env → works locally
   2. Update Render Environment Variables → works in production
   3. Must update BOTH places!
   ```

5. **Hardcoded fallbacks for secrets**:
   ```javascript
   // WRONG! Production will use insecure fallback
   const JWT_SECRET = process.env.JWT_SECRET || 'default-secret';  // ✗
   
   // CORRECT! Fail if secret is missing
   const JWT_SECRET = process.env.JWT_SECRET;
   if (!JWT_SECRET) {
     throw new Error('JWT_SECRET environment variable is required!');
   }
   ```

6. **Type confusion** (everything is a string!):
   ```javascript
   // WRONG! process.env values are always strings
   const PORT = process.env.PORT;  // "3000" (string)
   app.listen(PORT);  // Works but technically wrong type
   
   const MAX = process.env.MAX_ITEMS;  // "10" (string)
   if (items.length > MAX) // ✗ String comparison!
   
   // CORRECT! Convert types explicitly
   const PORT = parseInt(process.env.PORT || '3000', 10);  // number
   const MAX = parseInt(process.env.MAX_ITEMS || '10', 10);  // number
   const DEBUG = process.env.DEBUG === 'true';  // boolean
   ```

7. **Exposing env vars in frontend** (React/Vite):
   ```javascript
   // Backend (.env) - These are SECRET!
   DATABASE_URL=postgres://...
   JWT_SECRET=super-secret
   STRIPE_SECRET_KEY=sk_live_...
   
   // Frontend (.env) - These are PUBLIC!
   VITE_API_URL=https://api.com
   VITE_STRIPE_PUBLIC_KEY=pk_live_...  // Note: PUBLIC key
   
   // Remember: VITE_ vars are bundled into JavaScript
   // Anyone can see them in browser!
   // NEVER put secrets in VITE_ variables!
   ```

8. **Missing .env.example**:
   ```bash
   # Create .env.example (commit this!)
   # Other developers copy this to .env
   
   # .env.example
   NODE_ENV=development
   PORT=3000
   DATABASE_URL=postgres://localhost/myapp_dev
   JWT_SECRET=your-secret-here
   STRIPE_SECRET_KEY=sk_test_your-key
   
   # Instructions for new developers:
   # 1. Copy .env.example to .env
   # 2. Fill in real values
   # 3. Never commit .env!
   ```

9. **Environment variables not rebuilding**:
   ```
   React/Vite issue:
   - Changed VITE_API_URL in Vercel
   - Still seeing old value!
   
   Why: Env vars are baked into build at build time
   
   Fix:
   1. Change env var in Vercel dashboard
   2. Trigger new deployment (Deployments → Redeploy)
   3. Env vars from build time are used, not runtime!
   ```

10. **Different formats on different platforms**:
    ```bash
    # .env file format (local)
    DATABASE_URL=postgres://localhost/db
    NODE_ENV=development
    
    # Render format (same!)
    DATABASE_URL = postgres://render.com/db
    NODE_ENV = production
    
    # Note: Render adds spaces around =, both work fine
    # Just be consistent in your .env file
    ```