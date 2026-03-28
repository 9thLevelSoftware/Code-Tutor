---
type: "THEORY"
title: "Deployment Options"
---

Where can you run your Spring Boot application?

TRADITIONAL HOSTING (VPS):
- DigitalOcean Droplets, AWS EC2, Linode
- You manage the server, OS, Java, etc.
- Full control, full responsibility
- Good for: Learning Linux, complex setups

PLATFORM AS A SERVICE (PaaS):
- Fly.io, Heroku, Render
- Upload code, platform handles the rest
- Less control, less work
- Good for: Getting started, MVPs, side projects

CONTAINER PLATFORMS:
- AWS ECS/EKS, Google Cloud Run, Azure Container Apps
- Run Docker containers at scale
- More control than PaaS, less than VPS
- Good for: Production workloads

SERVERLESS:
- AWS Lambda, Google Cloud Functions
- Pay only when code runs
- Limited to short-lived functions
- Good for: Event-driven, low-traffic apps

FOR LEARNING:
We'll use Fly.io because:
- Generous free tier (256MB RAM, 3GB transfer/month)
- Docker-based and Git-based deployments
- Automatic HTTPS
- Easy PostgreSQL support
- No credit card required to start
- Note: Railway discontinued their free tier in 2024. This lesson now uses Fly.io.