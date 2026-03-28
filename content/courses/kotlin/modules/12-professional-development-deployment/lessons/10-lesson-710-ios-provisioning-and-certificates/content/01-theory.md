---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes

iOS app distribution requires understanding Apple's provisioning and certificate system. While complex, mastering this process enables you to deploy apps to devices, TestFlight, and the App Store.

**Apple Developer Program Requirements**

To distribute iOS apps, you need:
- Apple Developer Program membership ($99/year)
- D-U-N-S Number (for organization accounts)
- Legal entity documentation

**Certificate Types**

| Certificate | Purpose | Validity |
|-------------|---------|----------|
| Development | Run on test devices | 1 year |
| Distribution (App Store) | Submit to App Store | 1 year |
| Distribution (Enterprise) | Internal company distribution | 1 year |
| Apple Distribution | New unified certificate | 1 year |

**Creating Certificates**

1. **Via Xcode (Recommended)**
   - Xcode → Preferences → Accounts
   - Select team → Manage Certificates
   - Click + to add certificate

2. **Via Apple Developer Portal**
   - Certificates, Identifiers & Profiles
   - Create Certificate Signing Request (CSR) on Mac
   - Upload CSR, download certificate

**Provisioning Profiles**

A provisioning profile combines:
- App ID (bundle identifier)
- Certificates (who can sign)
- Device list (for development)
- Entitlements (app capabilities)

```
Development Profile:
- App ID: com.example.myapp
- Certificate: Your dev certificate
- Devices: iPhone 1, iPhone 2, iPad 1
- Enables: Push Notifications

Distribution Profile:
- App ID: com.example.myapp
- Certificate: Distribution cert
- Devices: All App Store devices
- Enables: Push, In-App Purchase
```

**Entitlements and Capabilities**

Configure app capabilities in Xcode:

```xml
<!-- MyApp.entitlements -->
<dict>
    <key>aps-environment</key>
    <string>production</string>
    <key>com.apple.developer.associated-domains</key>
    <array>
        <string>applinks:example.com</string>
    </array>
</dict>
```

Common capabilities:
- Push Notifications
- In-App Purchase
- iCloud
- Associated Domains (Universal Links)
- Sign in with Apple

**KMP iOS Configuration**

```kotlin
// build.gradle.kts (shared)
kotlin {
    iosTarget {
        binaries.framework {
            baseName = "Shared"
            export("...")
            
            // Embed in app bundle
            isStatic = false
        }
    }
}
```

**Build and Archive**

```bash
# Build for device
xcodebuild -workspace MyApp.xcworkspace \
  -scheme MyApp \
  -configuration Release \
  -destination 'generic/platform=iOS' \
  archive -archivePath build/MyApp.xcarchive

# Export IPA
xcodebuild -exportArchive \
  -archivePath build/MyApp.xcarchive \
  -exportPath build/Output \
  -exportOptionsPlist exportOptions.plist
```

**Common Issues and Solutions**

| Issue | Solution |
|-------|----------|
| "No matching provisioning profile" | Check bundle ID matches |
| "Certificate not found" | Download certificates in Xcode |
| "Device not in profile" | Add device UDID to portal |
| "Entitlements mismatch" | Regenerate provisioning profile |

Proper provisioning setup is essential - errors here block all distribution.
