---
type: "EXAMPLE"
title: "GitHub Actions + Fly.io"
---

Automated deployment from GitHub Actions:

```yaml
# .github/workflows/deploy.yml
name: Deploy to Fly.io

on:
  push:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - uses: actions/setup-java@v4
        with:
          java-version: '25'
          distribution: 'temurin'
          cache: 'maven'
          cache-dependency-path: '**/pom.xml'
      
      - name: Run tests
        run: ./mvnw verify

  deploy:
    needs: test
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Install Fly.io CLI
        run: |
          curl -L https://fly.io/install.sh | sh
          echo "$HOME/.fly/bin" >> $GITHUB_PATH
      
      - name: Deploy to Fly.io
        run: fly deploy --remote-only
        env:
          FLY_API_TOKEN: ${{ secrets.FLY_API_TOKEN }}

# How to get FLY_API_TOKEN:
# 1. Run: fly tokens create deploy -x 999d
# 2. Copy the token
# 3. Add to GitHub repo secrets as FLY_API_TOKEN
```

**Note:** Railway discontinued their free tier in 2024. This example now uses Fly.io which offers a generous free tier with 256MB RAM and 3GB transfer per month.
