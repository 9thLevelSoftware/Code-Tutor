---
type: "EXAMPLE"
title: "Deploying to Railway"
---

Railway deployment for Bun/Hono APIs with Postgres.

```bash
# Railway Deployment Guide for Bun/Hono

# ============================================
# Step 1: Prepare Your Project
# ============================================

# package.json
{
  "name": "my-api",
  "scripts": {
    "start": "bun run src/index.ts",
    "dev": "bun --watch src/index.ts",
    "build": "bun build src/index.ts --outdir=dist"
  }
}

# Procfile (optional but recommended)
web: bun run src/index.ts

# ============================================
# Step 2: Railway CLI Setup
# ============================================

# Install Railway CLI
npm install -g @railway/cli

# Login
railway login

# Initialize project
railway init

# Link to existing project
railway link

# ============================================
# Step 3: Add Database (One Command!)
# ============================================

# Add Postgres
railway add --plugin postgres

# This creates:
# - Managed Postgres instance
# - DATABASE_URL automatically set
# - Connected to your service

# ============================================
# Step 4: Configure Environment
# ============================================

# Set environment variables
railway variables set NODE_ENV=production
railway variables set JWT_SECRET=your-secret-here
railway variables set CORS_ORIGIN=https://your-frontend.vercel.app

# View all variables
railway variables

# ============================================
# Step 5: Deploy
# ============================================

# Deploy current directory
railway up

# Or connect to GitHub for auto-deploys
# Railway Dashboard -> Settings -> Connect GitHub

# ============================================
# Step 6: View Logs & Status
# ============================================

# View logs
railway logs

# Open dashboard
railway open

# Get deployment URL
railway domain

# ============================================
# railway.json (Optional Configuration)
# ============================================
{
  "$schema": "https://railway.app/railway.schema.json",
  "build": {
    "builder": "NIXPACKS"
  },
  "deploy": {
    "startCommand": "bun run src/index.ts",
    "healthcheckPath": "/health",
    "restartPolicyType": "ON_FAILURE"
  }
}
```
