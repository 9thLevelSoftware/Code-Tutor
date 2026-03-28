---
type: "EXAMPLE"
title: "Complete CI/CD Workflow"
---

Full GitHub Actions workflow for a Spring Boot application:

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

# When to run
on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

# Environment variables available to all jobs
env:
  JAVA_VERSION: '25'
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  # Job 1: Build and Test
  test:
    name: Build and Test
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Set up Java
        uses: actions/setup-java@v4
        with:
          java-version: ${{ env.JAVA_VERSION }}
          distribution: 'temurin'
          cache: 'maven'
          cache-dependency-path: '**/pom.xml'
      
      - name: Run tests
        run: ./mvnw verify
      
      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: test-results
          path: target/surefire-reports/

  # Job 2: Build and Push Docker Image
  build:
    name: Build Docker Image
    needs: test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    
    permissions:
      contents: read
      packages: write
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Log in to Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}
      
      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=sha,prefix=
            type=raw,value=latest
      
      - name: Build and push
        uses: docker/build-push-action@v6
        with:
          context: .
          push: true
          tags: ${{ steps.meta.outputs.tags }}

  # Job 3: Deploy to Fly.io
  deploy:
    name: Deploy to Fly.io
    needs: build
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Install Fly.io CLI
        run: |
          curl -L https://fly.io/install.sh | sh
          echo "$HOME/.fly/bin" >> $GITHUB_PATH
      
      - name: Deploy to Fly.io
        run: fly deploy --remote-only
        env:
          FLY_API_TOKEN: ${{ secrets.FLY_API_TOKEN }}
```
