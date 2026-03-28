---
type: "THEORY"
title: "Tools of the Trade"
---

CONTAINERIZATION:
- Docker: Package app + environment together
- Docker Compose: Define multi-container apps

CI/CD PLATFORMS:
- GitHub Actions: Built into GitHub, free for public repos
- GitLab CI: Built into GitLab
- Jenkins: Self-hosted, highly customizable
- CircleCI, Travis CI: Cloud-based options

CLOUD PLATFORMS:
- Fly.io: Simple deployment, great for learning (generous free tier)
- Heroku: Easy PaaS (Platform as a Service)
- AWS, GCP, Azure: Full cloud providers
- DigitalOcean, Render: Developer-friendly options

MONITORING:
- Prometheus + Grafana: Metrics and dashboards
- ELK Stack: Log aggregation
- Datadog, New Relic: All-in-one monitoring

IN THIS MODULE:
We'll focus on the most practical tools:
1. Docker - containerize your Spring Boot app
2. Docker Compose - add PostgreSQL database
3. GitHub Actions - automate testing and deployment
4. Fly.io - deploy to the cloud for free (Note: Railway discontinued their free tier in 2024, so we use Fly.io instead)

By the end, every push to main = automatic deployment.