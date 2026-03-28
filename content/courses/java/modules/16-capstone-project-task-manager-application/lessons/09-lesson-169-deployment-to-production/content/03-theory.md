---
type: "THEORY"
title: "GitHub Actions CI/CD Pipeline"
---

BOTH PATHS (with differences)

GitHub Actions automates testing and deployment. Every push triggers tests, and merges to main trigger deployment.

**Thymeleaf path:** You only need the `test` and `build` jobs. Remove the `test-frontend` job and the `frontend` Docker image build step entirely.

**React path:** You need all four jobs: backend tests, frontend tests, Docker build for both images, and deploy.

The workflow below shows the full React pipeline. Thymeleaf users: delete the `test-frontend` job, remove it from the `needs` array in the `build` job, and remove the "Build and push Frontend image" step.

Create the workflow file:
```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  # Run tests
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:17
        env:
          POSTGRES_DB: testdb
          POSTGRES_USER: test
          POSTGRES_PASSWORD: test
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v4

      - name: Set up JDK 25
        uses: actions/setup-java@v4
        with:
          java-version: '25'
          distribution: 'temurin'
          cache: 'gradle'

      - name: Run tests
        env:
          SPRING_DATASOURCE_URL: jdbc:postgresql://localhost:5432/testdb
          SPRING_DATASOURCE_USERNAME: test
          SPRING_DATASOURCE_PASSWORD: test
        run: ./gradlew test

      - name: Upload test results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: test-results
          path: build/reports/tests/

  # Test frontend
  test-frontend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: ./frontend

    steps:
      - uses: actions/checkout@v4

      - name: Set up Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: frontend/package-lock.json

      - name: Install dependencies
        run: npm ci

      - name: Run tests
        run: npm test

      - name: Build
        run: npm run build

  # Build and push Docker images
  build:
    needs: [test, test-frontend]
    runs-on: ubuntu-latest
    if: github.event_name == 'push' && github.ref == 'refs/heads/main'
    
    permissions:
      contents: read
      packages: write

    steps:
      - uses: actions/checkout@v4

      - name: Log in to Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build and push API image
        uses: docker/build-push-action@v6
        with:
          context: .
          push: true
          tags: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}-api:latest

      - name: Build and push Frontend image
        uses: docker/build-push-action@v6
        with:
          context: ./frontend
          push: true
          tags: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}-frontend:latest

  # Deploy to Fly.io
  deploy:
    needs: build
    runs-on: ubuntu-latest
    if: github.event_name == 'push' && github.ref == 'refs/heads/main'

    steps:
      - uses: actions/checkout@v4

      - name: Install Fly.io CLI
        run: |
          curl -L https://fly.io/install.sh | sh
          echo "$HOME/.fly/bin" >> $GITHUB_PATH

      - name: Deploy to Fly.io
        env:
          FLY_API_TOKEN: ${{ secrets.FLY_API_TOKEN }}
        run: fly deploy --remote-only
```

This pipeline:
1. Runs backend tests with PostgreSQL service
2. Runs frontend tests and build
3. Builds Docker images on main branch
4. Pushes to GitHub Container Registry
5. Deploys to Fly.io

Add secrets in GitHub repository settings:
- FLY_API_TOKEN: Your Fly.io deployment token (get it with: fly tokens create deploy -x 999d)

**Note:** Railway discontinued their free tier in 2024. This lesson now uses Fly.io which offers a generous free tier with 256MB RAM and 3GB transfer per month.