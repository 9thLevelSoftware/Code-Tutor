---
type: "KEY_POINT"
title: "Fly.io Basics"
---

Fly.io is a modern platform that makes deployment simple:

KEY FEATURES:
- Deploy from GitHub or Docker images
- Built-in PostgreSQL, Redis, object storage
- Environment variables management
- Automatic HTTPS certificates
- Logs and metrics
- Private networking between services

PRICING (as of 2025):
- Free tier: 256MB RAM, 3GB transfer/month
- Hobby: Pay-as-you-go
- Pro: Usage-based pricing

DEPLOYMENT METHODS:

1. Fly.io CLI (recommended):
   - fly launch (creates config)
   - fly deploy
   - Auto-deploy on push if configured

2. GitHub Actions:
   - Deploy via CI/CD pipeline
   - Uses FLY_API_TOKEN secret

3. Docker Image:
   - Push to container registry
   - Fly.io pulls and runs

FLY.IO DETECTS:
- fly.toml configuration file
- Dockerfile (uses it)
- pom.xml or package.json (builds automatically)