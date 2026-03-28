---
type: "KEY_POINT"
title: "Secrets Management"
---

NEVER PUT SECRETS IN WORKFLOW FILES!

# BAD - exposed in version control
env:
  DATABASE_PASSWORD: supersecret123

# GOOD - use GitHub Secrets
env:
  DATABASE_PASSWORD: ${{ secrets.DATABASE_PASSWORD }}

SETTING UP SECRETS:

1. Go to repository Settings
2. Click 'Secrets and variables' > 'Actions'
3. Click 'New repository secret'
4. Add name (e.g., FLY_API_TOKEN) and value

COMMON SECRETS:
- FLY_API_TOKEN: For Fly.io deployment
- DOCKER_USERNAME, DOCKER_PASSWORD: For Docker Hub
- AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY: For AWS
- GITHUB_TOKEN: Automatically provided by GitHub!

BUILT-IN VARIABLES:

${{ github.repository }}     # owner/repo
${{ github.actor }}          # Username who triggered
${{ github.sha }}            # Full commit SHA
${{ github.ref }}            # refs/heads/main
${{ github.event_name }}     # push, pull_request, etc.
${{ secrets.GITHUB_TOKEN }}  # Auto-generated token

USING OUTPUTS BETWEEN STEPS:

- name: Get version
  id: version
  run: echo "version=$(cat VERSION)" >> $GITHUB_OUTPUT

- name: Use version
  run: echo "Version is ${{ steps.version.outputs.version }}"