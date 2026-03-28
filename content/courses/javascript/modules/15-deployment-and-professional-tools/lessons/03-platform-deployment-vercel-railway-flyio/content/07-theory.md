---
type: "THEORY"
title: "Environment Variables by Platform"
---

Setting environment variables on each platform:

1. **Vercel** (Dashboard):
   ```
   Settings -> Environment Variables

   Name: VITE_API_URL
   Value: https://api.example.com
   Environments: Production, Preview, Development

   Important: Prefix with VITE_ for client-side access!
   ```

2. **Railway** (CLI or Dashboard):
   ```bash
   # CLI
   railway variables set KEY=value
   railway variables set DATABASE_URL=postgres://...

   # Dashboard
   Service -> Variables -> Add Variable

   # Reference other variables
   DATABASE_URL=${{Postgres.DATABASE_URL}}
   ```

3. **Render** (Dashboard):
   ```
   Service -> Environment -> Add Environment Variable

   Key: DATABASE_URL
   Value: postgres://...

   # Render auto-injects for managed DBs
   DATABASE_URL is automatic when you add Postgres
   ```

4. **Fly.io** (Secrets):
   ```bash
   # Set secrets (encrypted)
   fly secrets set DATABASE_URL=postgres://...
   fly secrets set JWT_SECRET=your-secret

   # fly.toml for non-sensitive env vars
   [env]
     NODE_ENV = "production"
     LOG_LEVEL = "info"

   # Never put secrets in fly.toml!
   ```

5. **Best Practices**:
   ```
   DO:
   - Use platform's secret storage
   - Set different values per environment
   - Document required variables
   - Validate at startup

   DON'T:
   - Commit secrets to git
   - Use same secrets in dev/prod
   - Hardcode in Dockerfile
   - Log secret values
   ```