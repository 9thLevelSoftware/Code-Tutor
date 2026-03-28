---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes

Fastlane is an automation tool that simplifies iOS and Android deployment. It handles code signing, screenshot generation, and app store uploads - turning hours of manual work into a single command.

**Why Fastlane for KMP**

- Automates repetitive deployment tasks
- Handles code signing headaches
- Generates localized screenshots
- Works with CI/CD pipelines
- Supports both iOS and Android

**Setup**

```bash
# Install Fastlane
gem install fastlane -NV

# Initialize in your project
cd iosApp && fastlane init
cd ../androidApp && fastlane init
```

**iOS Fastfile**

```ruby
# iosApp/fastlane/Fastfile
default_platform(:ios)

platform :ios do
  desc "Run tests"
  lane :test do
    scan(scheme: "MyApp")
  end

  desc "Build for TestFlight"
  lane :beta do
    # Sync signing certificates
    match(type: "appstore")
    
    # Build and upload
    build_app(workspace: "MyApp.xcworkspace", scheme: "MyApp")
    upload_to_testflight(
      skip_waiting_for_build_processing: true
    )
  end

  desc "Deploy to App Store"
  lane :release do
    match(type: "appstore")
    build_app(workspace: "MyApp.xcworkspace", scheme: "MyApp")
    upload_to_app_store(
      submit_for_review: true,
      automatic_release: false
    )
  end
end
```

**Android Fastfile**

```ruby
# androidApp/fastlane/Fastfile
default_platform(:android)

platform :android do
  desc "Run tests"
  lane :test do
    gradle(task: "test")
  end

  desc "Build release AAB"
  lane :build do
    gradle(task: "bundleRelease")
  end

  desc "Deploy to Play Store"
  lane :deploy do
    gradle(task: "bundleRelease")
    upload_to_play_store(
      track: "internal",
      release_status: "draft"
    )
  end
end
```

**Match - Code Signing Simplified**

```ruby
# Store certificates in private Git repo
match(type: "development")
match(type: "appstore")
```

Match stores your certificates and profiles in a Git repository, sharing them securely with your team.

**Automated Screenshots**

```ruby
lane :screenshots do
  capture_screenshots(
    workspace: "MyApp.xcworkspace",
    scheme: "MyAppUITests",
    languages: ["en-US", "de-DE", "fr-FR"],
    devices: ["iPhone 15 Pro", "iPhone SE"]
  )
  upload_to_app_store(skip_binary_upload: true)
end
```

**CI/CD Integration**

```yaml
# .github/workflows/deploy.yml
- name: Deploy iOS Beta
  env:
    MATCH_PASSWORD: ${{ secrets.MATCH_PASSWORD }}
    FASTLANE_PASSWORD: ${{ secrets.FASTLANE_PASSWORD }}
  run: |
    cd iosApp && fastlane beta
```

**Best Practices**

1. Use `match` for team code signing
2. Separate lanes for different environments (dev, staging, prod)
3. Never commit API keys - use environment variables
4. Version your lanes: `beta`, `release`, `hotfix`
5. Add preconditions: `ensure_git_branch`, `ensure_git_status_clean`

Fastlane transforms deployment from a dreaded manual process into an automated, reliable pipeline.
