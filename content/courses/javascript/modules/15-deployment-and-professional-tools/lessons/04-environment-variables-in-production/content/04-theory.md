---
type: "THEORY"
title: "Secrets Management Overview"
---

Handling secrets securely in production:

1. **Types of Secrets**:
   ```
   - Database passwords
   - API keys (Stripe, SendGrid, etc.)
   - JWT signing secrets
   - OAuth client secrets
   - Encryption keys
   - Service account credentials
   ```

2. **Secrets Management Options**:
   ```
   Level 1: Platform Environment Variables (Good Start)
   - Render, Railway, Vercel dashboards
   - Encrypted at rest
   - Limited access control

   Level 2: Secret Management Services (Better)
   - Doppler (developer-friendly)
   - HashiCorp Vault (enterprise)
   - AWS Secrets Manager
   - Automatic rotation
   - Audit logs

   Level 3: Cloud Provider Secrets (Enterprise)
   - AWS Parameter Store / Secrets Manager
   - Google Secret Manager
   - Azure Key Vault
   ```

3. **Doppler Example** (recommended for teams):
   ```bash
   # Install Doppler CLI
   brew install dopplerhq/cli/doppler

   # Login and setup
   doppler login
   doppler setup

   # Run with injected secrets
   doppler run -- bun start

   # CI/CD integration
   doppler run -- bun test
   ```

4. **Never Do This**:
   ```typescript
   // NEVER hardcode secrets
   const JWT_SECRET = 'my-secret-key-123'; // WRONG!

   // NEVER commit .env files with real secrets
   // .gitignore MUST include:
   .env
   .env.local
   .env.production

   // NEVER log secrets
   console.log('Connecting with:', process.env.DATABASE_URL); // WRONG!
   console.log('Connecting to database...'); // CORRECT
   ```

5. **Secrets Rotation**:
   ```
   Good practices:
   - Rotate secrets quarterly (at minimum)
   - Rotate immediately if compromised
   - Use short-lived tokens where possible
   - Have a rotation procedure documented
   ```