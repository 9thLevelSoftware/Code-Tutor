---
type: "THEORY"
title: "Breaking Down the Syntax"
---

Environment variables in practice:

1. **Development (.env file)**:
   ```bash
   # .env (local development only, never commit!)
   NODE_ENV=development
   PORT=3000
   DATABASE_URL=postgres://localhost/myapp_dev
   JWT_SECRET=dev-secret-simple-is-ok
   STRIPE_SECRET_KEY=sk_test_abc123
   FRONTEND_URL=http://localhost:5173
   ```

2. **Loading .env file** (using dotenv):
   ```javascript
   // At the very top of server.js
   import 'dotenv/config';
   // or
   require('dotenv').config();
   
   // Now process.env has your variables!
   console.log(process.env.DATABASE_URL);
   ```

3. **Using environment variables**:
   ```javascript
   // Good pattern with fallbacks
   const PORT = process.env.PORT || 3000;
   const NODE_ENV = process.env.NODE_ENV || 'development';
   
   // Required variables (no fallback)
   const DATABASE_URL = process.env.DATABASE_URL;
   if (!DATABASE_URL) {
     throw new Error('DATABASE_URL environment variable is required!');
   }
   
   // Type conversion
   const MAX_CONNECTIONS = parseInt(process.env.MAX_CONNECTIONS || '10', 10);
   const ENABLE_DEBUG = process.env.ENABLE_DEBUG === 'true';
   ```

4. **Validation helper**:
   ```javascript
   function requireEnv(name) {
     const value = process.env[name];
     if (!value) {
       throw new Error(`Missing required environment variable: ${name}`);
     }
     return value;
   }
   
   // Usage
   const DATABASE_URL = requireEnv('DATABASE_URL');
   const JWT_SECRET = requireEnv('JWT_SECRET');
   ```

5. **.env.example** (commit this!):
   ```bash
   # .env.example - Template for other developers
   # Copy this to .env and fill in real values
   
   NODE_ENV=development
   PORT=3000
   DATABASE_URL=postgres://localhost/myapp_dev
   JWT_SECRET=your-secret-here
   STRIPE_SECRET_KEY=sk_test_your-key
   ```

6. **.gitignore** (CRITICAL!):
   ```
   # Never commit these!
   .env
   .env.local
   .env.development
   .env.production
   .env.test
   
   # DO commit this:
   # .env.example
   ```

7. **Setting in Render (Backend)**:
   ```
   Render Dashboard:
   1. Go to your web service
   2. Click "Environment"
   3. Add variables:
      DATABASE_URL = postgres://...
      JWT_SECRET = your-production-secret
      NODE_ENV = production
   4. Save (triggers redeploy)
   ```

8. **Setting in Vercel (Frontend)**:
   ```
   Vercel Dashboard:
   1. Project Settings → Environment Variables
   2. Add variables (must start with VITE_):
      VITE_API_URL = https://api.myapp.com
      VITE_STRIPE_PUBLIC_KEY = pk_live_...
   3. Redeploy to apply changes
   ```

9. **Environment-specific logic**:
   ```javascript
   const isDevelopment = process.env.NODE_ENV === 'development';
   const isProduction = process.env.NODE_ENV === 'production';
   
   if (isDevelopment) {
     // Detailed logging
     console.log('Full error:', error.stack);
     
     // Allow all CORS in dev
     app.use(cors({ origin: '*' }));
   }
   
   if (isProduction) {
     // Hide error details
     console.log('Error occurred');
     
     // Strict CORS
     app.use(cors({ origin: process.env.FRONTEND_URL }));
   }
   ```

10. **Configuration module pattern**:
    ```javascript
    // config/index.js
    export const config = {
      env: process.env.NODE_ENV || 'development',
      port: parseInt(process.env.PORT || '3000', 10),
      
      database: {
        url: process.env.DATABASE_URL,
        poolSize: parseInt(process.env.DB_POOL_SIZE || '10', 10)
      },
      
      jwt: {
        secret: process.env.JWT_SECRET,
        expiresIn: process.env.JWT_EXPIRES || '7d'
      },
      
      stripe: {
        secretKey: process.env.STRIPE_SECRET_KEY,
        webhookSecret: process.env.STRIPE_WEBHOOK_SECRET
      },
      
      cors: {
        origin: process.env.CORS_ORIGIN || 'http://localhost:5173'
      }
    };
    
    // Validate on import
    const required = ['DATABASE_URL', 'JWT_SECRET'];
    required.forEach(key => {
      if (!process.env[key]) {
        throw new Error(`Missing required env var: ${key}`);
      }
    });
    ```