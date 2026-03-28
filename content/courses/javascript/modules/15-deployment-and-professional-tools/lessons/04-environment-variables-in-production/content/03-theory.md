---
type: "THEORY"
title: "Development vs Staging vs Production"
---

Managing environments in real-world applications:

1. **Environment Overview**:
   ```
   DEVELOPMENT (localhost)
   - Your laptop
   - Local database (Docker Postgres)
   - Relaxed security
   - Detailed error messages
   - Hot reloading enabled

   STAGING (preview environment)
   - Cloud-hosted (Render, Railway)
   - Copy of production architecture
   - Test before going live
   - May use production database copy
   - Accessible to team only

   PRODUCTION (live)
   - Real users
   - Real data
   - Maximum security
   - Minimal error details
   - Performance optimized
   ```

2. **Environment-Specific Variables**:
   ```bash
   # .env.development (checked into git, safe defaults)
   NODE_ENV=development
   DATABASE_URL=postgres://localhost:5432/myapp_dev
   API_URL=http://localhost:3000
   LOG_LEVEL=debug
   DEBUG=true

   # .env.staging (NOT in git - set in hosting platform)
   NODE_ENV=staging
   DATABASE_URL=postgres://staging-db.internal/myapp_staging
   API_URL=https://staging-api.myapp.com
   LOG_LEVEL=info
   DEBUG=false

   # Production (NEVER in git - always in hosting platform)
   NODE_ENV=production
   DATABASE_URL=postgres://prod-db.aws.com/myapp
   API_URL=https://api.myapp.com
   LOG_LEVEL=error
   DEBUG=false
   ```

3. **Loading Environment Files** (Bun does this automatically!):
   ```typescript
   // Bun loads .env automatically based on NODE_ENV
   // .env                 (always loaded)
   // .env.local           (always loaded, git-ignored)
   // .env.development     (when NODE_ENV=development)
   // .env.production      (when NODE_ENV=production)

   const env = process.env.NODE_ENV; // 'development' | 'staging' | 'production'
   const dbUrl = process.env.DATABASE_URL;
   ```

4. **Conditional Logic by Environment**:
   ```typescript
   const isDev = process.env.NODE_ENV === 'development';
   const isProd = process.env.NODE_ENV === 'production';

   // Show detailed errors only in development
   app.onError((err, c) => {
     console.error(err);
     if (isDev) {
       return c.json({ error: err.message, stack: err.stack }, 500);
     }
     return c.json({ error: 'Internal server error' }, 500);
   });

   // Enable debug logging in development
   if (isDev) {
     app.use('*', logger());
   }
   ```