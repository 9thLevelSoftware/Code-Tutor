---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 40 minutes

TestFlight is Apple's platform for beta testing iOS apps before App Store release. It allows you to distribute your KMP iOS app to internal testers and external beta users for feedback.

**TestFlight Overview**

TestFlight provides two testing groups:
- **Internal Testing**: Up to 100 team members, immediate distribution
- **External Testing**: Up to 10,000 testers, requires App Store review

**Uploading to TestFlight**

1. **Via Xcode**
   - Product → Archive
   - Distribute App → App Store Connect
   - Upload

2. **Via Command Line (CI/CD)**

```bash
# Build and archive
xcodebuild -workspace MyApp.xcworkspace \
  -scheme MyApp \
  -configuration Release \
  -destination 'generic/platform=iOS' \
  archive -archivePath build/MyApp.xcarchive

# Upload to TestFlight
xcodebuild -exportArchive \
  -archivePath build/MyApp.xcarchive \
  -exportOptionsPlist exportOptions.plist \
  -exportPath build/Output

# Use altool or Transporter for upload
xcrun altool --upload-app \
  --type ios \
  --file build/Output/MyApp.ipa \
  --apiKey $APP_STORE_KEY \
  --apiIssuer $APP_STORE_ISSUER
```

**exportOptions.plist**

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" ...>
<plist version="1.0">
<dict>
    <key>method</key>
    <string>app-store</string>
    <key>teamID</key>
    <string>YOUR_TEAM_ID</string>
    <key>signingStyle</key>
    <string>automatic</string>
</dict>
</plist>
```

**Fastlane Integration**

```ruby
# Fastfile
lane :beta do
  build_app(workspace: "MyApp.xcworkspace", scheme: "MyApp")
  upload_to_testflight(
    skip_waiting_for_build_processing: true,
    notify_external_testers: false
  )
end
```

**Managing Testers**

```
App Store Connect → My App → TestFlight
├── Internal Testing
│   └── Add iTunes Connect users
└── External Testing
    ├── Create group (e.g., "Beta Testers")
    ├── Add testers by email
    └── Submit for Beta App Review
```

**TestFlight Features**

- Automatic updates for testers
- In-app feedback screenshots
- Crash reporting
- Usage analytics (sessions, devices)

**Best Practices**

1. Use internal testing for daily builds during development
2. Create multiple external groups for different user segments
3. Provide clear test instructions in the "What to Test" field
4. Respond to tester feedback promptly
5. Use TestFlight before every App Store submission

TestFlight bridges development and production, letting you catch issues before they reach the App Store.
