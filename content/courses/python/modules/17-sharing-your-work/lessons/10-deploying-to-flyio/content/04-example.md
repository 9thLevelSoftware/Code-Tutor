---
type: "EXAMPLE"
title: "Managing Secrets and Environment Variables"
---

**Secrets are encrypted and available to your app:**

```bash
# Set secrets (encrypted, not shown in fly.toml)
fly secrets set SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
fly secrets set API_KEY="external-api-key"

# Set multiple secrets at once
fly secrets set SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret") JWT_SECRET="key2" API_TOKEN="token"

# List all secrets (names only, not values)
fly secrets list

# Remove a secret
fly secrets unset API_KEY

# Import from .env file
cat .env.production | fly secrets import

# Secrets are available in your app as environment variables:
import os

secret_key = os.environ["SECRET_KEY"]
database_url = os.environ["DATABASE_URL"]  # Set by fly postgres attach

# View deployed app's environment
fly ssh console -C "env | grep -E '(SECRET|DATABASE)'"
```
