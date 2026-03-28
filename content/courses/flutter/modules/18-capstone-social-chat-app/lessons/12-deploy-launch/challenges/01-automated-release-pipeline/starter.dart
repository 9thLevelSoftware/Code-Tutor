# .github/workflows/release.yml
name: Release Pipeline

on:
  push:
    tags:
      - 'v*.*.*'

env:
  FLUTTER_VERSION: '3.38.0'

jobs:
  validate:
    runs-on: ubuntu-latest
    outputs:
      version: ${{ steps.version.outputs.value }}
      changelog: ${{ steps.changelog.outputs.content }}
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Extract version from tag
        id: version
        run: |
          # Extract version from GITHUB_REF
          # Validate version format
          # Compare with pubspec.yaml version
          echo "value=" >> $GITHUB_OUTPUT

      - name: Generate changelog
        id: changelog
        run: |
          # Get commits since last tag
          # Group by type (feat, fix, etc.)
          # Format as markdown
          echo "content=" >> $GITHUB_OUTPUT

  test:
    needs: validate
    runs-on: ubuntu-latest
    steps:
      # Run unit tests
      # Run integration tests
      # Check code coverage threshold
      # Fail if tests don't pass

  build-android:
    needs: test
    runs-on: ubuntu-latest
    steps:
      # Build release app bundle
      # Sign with release keystore
      # Upload debug symbols
      # Save artifact

  build-ios:
    needs: test
    runs-on: macos-latest
    steps:
      # Install certificates
      # Build release IPA
      # Upload dSYMs
      # Save artifact

  deploy-beta:
    needs: [build-android, build-ios]
    runs-on: ubuntu-latest
    steps:
      # Deploy Android to Internal track
      # Deploy iOS to TestFlight

  create-release:
    needs: deploy-beta
    runs-on: ubuntu-latest
    steps:
      # Create GitHub release
      # Attach artifacts
      # Add changelog

  promote-production:
    needs: create-release
    runs-on: ubuntu-latest
    environment: production  # Requires manual approval
    steps:
      # Promote Android to Production (staged)
      # Submit iOS for review

  notify:
    needs: [deploy-beta, promote-production]
    if: always()
    runs-on: ubuntu-latest
    steps:
      # Send Slack notification with results