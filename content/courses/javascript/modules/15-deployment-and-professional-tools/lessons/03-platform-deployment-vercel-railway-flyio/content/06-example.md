---
type: "EXAMPLE"
title: "Deploying to Fly.io"
---

Fly.io deployment for Docker containers with global distribution.

```bash
# Fly.io Deployment Guide for Docker Apps

# ============================================
# Step 1: Install Fly CLI
# ============================================

# macOS
brew install flyctl

# Windows
powershell -Command "iwr https://fly.io/install.ps1 -useb | iex"

# Linux
curl -L https://fly.io/install.sh | sh

# Login
fly auth login

# ============================================
# Step 2: Launch App
# ============================================

# In your project directory
fly launch

# This creates fly.toml and Dockerfile if needed

# ============================================
# fly.toml Configuration
# ============================================

# fly.toml
app = "my-api"
primary_region = "iad"  # US East

[build]
  dockerfile = "Dockerfile"

[env]
  NODE_ENV = "production"
  PORT = "3000"

[http_service]
  internal_port = 3000
  force_https = true
  auto_stop_machines = true
  auto_start_machines = true
  min_machines_running = 1

  [http_service.concurrency]
    type = "connections"
    hard_limit = 25
    soft_limit = 20

[[http_service.checks]]
  grace_period = "10s"
  interval = "30s"
  method = "GET"
  path = "/health"
  timeout = "5s"

# ============================================
# Step 3: Set Secrets
# ============================================

# Set secrets (encrypted, not in fly.toml!)
fly secrets set DATABASE_URL="postgres://..."
fly secrets set JWT_SECRET="your-secret-here"

# List secrets
fly secrets list

# ============================================
# Step 4: Deploy
# ============================================

# Deploy (builds and pushes Docker image)
fly deploy

# Deploy from GitHub Actions
fly deploy --remote-only

# ============================================
# Step 5: Scale & Monitor
# ============================================

# Scale to multiple regions
fly scale count 2 --region iad,lhr

# View running machines
fly status

# View logs
fly logs

# SSH into machine
fly ssh console

# Open in browser
fly open

# ============================================
# Step 6: Add Postgres (Optional)
# ============================================

# Create Fly Postgres cluster
fly postgres create --name my-api-db

# Attach to app (sets DATABASE_URL automatically)
fly postgres attach my-api-db

# Connect to database
fly postgres connect -a my-api-db
```
