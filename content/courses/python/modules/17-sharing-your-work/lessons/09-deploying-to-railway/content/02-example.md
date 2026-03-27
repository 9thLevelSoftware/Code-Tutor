---
type: "EXAMPLE"
title: "Railway CLI Setup and Deployment"
---

**The Railway CLI is the fastest way to deploy:**

```bash
# Step 1: Install Railway CLI
# On macOS/Linux:
curl -fsSL https://railway.app/install.sh | sh

# On Windows (PowerShell):
iwr https://railway.app/install.ps1 -useb | iex

# Or via npm (cross-platform):
npm install -g @railway/cli

# Step 2: Login to Railway
railway login
# Opens browser for authentication

# Step 3: Initialize project (from your app directory)
cd finance-tracker
railway init
# Creates new Railway project or links to existing

# Step 4: Add PostgreSQL database
railway add --database postgresql
# Provisions database and sets DATABASE_URL automatically

# Step 5: Set environment variables
railway variables set SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
railway variables set ENVIRONMENT="production"
railway variables set DEBUG="false"

# Step 6: Deploy
railway up
# Builds and deploys your application

# Step 7: Open in browser
railway open

# Useful commands:
railway logs          # View logs
railway status        # Check deployment status
railway run <cmd>     # Run command in Railway environment
railway connect postgresql  # Connect to database
railway domain        # Get or set custom domain
```
