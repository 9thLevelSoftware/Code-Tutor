---
type: "THEORY"
title: "Configuration Validation at Startup"
---

Validate environment variables before your app starts:

1. **Why Validate at Startup?**:
   ```
   Without validation:
   - App starts
   - User makes request
   - Request tries to use DATABASE_URL
   - DATABASE_URL is undefined
   - Error! App crashes
   - Bad user experience

   With validation:
   - App checks all required vars
   - Missing DATABASE_URL detected
   - App refuses to start
   - Error caught in deployment
   - Fix before users affected
   ```

2. **Manual Validation**:
   ```typescript
   // config.ts - Validate on import
   const requiredEnvVars = [
     'DATABASE_URL',
     'JWT_SECRET',
     'NODE_ENV'
   ] as const;

   function validateEnv() {
     const missing: string[] = [];

     for (const envVar of requiredEnvVars) {
       if (!process.env[envVar]) {
         missing.push(envVar);
       }
     }

     if (missing.length > 0) {
       console.error('Missing required environment variables:');
       missing.forEach(v => console.error(`  - ${v}`));
       process.exit(1);
     }

     console.log('Environment validated successfully');
   }

   validateEnv();

   // Export validated config
   export const config = {
     databaseUrl: process.env.DATABASE_URL!,
     jwtSecret: process.env.JWT_SECRET!,
     nodeEnv: process.env.NODE_ENV as 'development' | 'staging' | 'production',
     port: parseInt(process.env.PORT || '3000', 10)
   };
   ```

3. **Using Zod for Validation** (type-safe):
   ```typescript
   import { z } from 'zod';

   const envSchema = z.object({
     NODE_ENV: z.enum(['development', 'staging', 'production']),
     DATABASE_URL: z.string().url(),
     JWT_SECRET: z.string().min(32, 'JWT secret must be at least 32 characters'),
     PORT: z.string().transform(Number).default('3000'),
     LOG_LEVEL: z.enum(['debug', 'info', 'warn', 'error']).default('info'),

     // Optional with defaults
     CORS_ORIGIN: z.string().default('*'),
     RATE_LIMIT_MAX: z.string().transform(Number).default('100'),
   });

   // Validate and export
   export const env = envSchema.parse(process.env);

   // Now TypeScript knows the exact types!
   // env.PORT is number
   // env.NODE_ENV is 'development' | 'staging' | 'production'
   ```

4. **Usage Pattern**:
   ```typescript
   // index.ts
   import { env } from './config';

   // Type-safe access
   const app = new Hono();

   app.get('/health', (c) => c.json({
     status: 'ok',
     environment: env.NODE_ENV,
     port: env.PORT
   }));

   export default {
     port: env.PORT,
     fetch: app.fetch
   };
   ```

5. **Example Startup Output**:
   ```
   $ bun start

   Validating environment...
   Environment: production
   Database: connected
   Server listening on port 3000

   # Or if validation fails:
   $ bun start

   Environment validation failed:
   - DATABASE_URL: Required
   - JWT_SECRET: String must be at least 32 characters

   Process exited with code 1
   ```