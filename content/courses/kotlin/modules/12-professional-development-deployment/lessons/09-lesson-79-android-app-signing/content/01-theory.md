---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 40 minutes

Signing your Android app is required for distribution. Understanding the signing process, keystore management, and Play Store requirements ensures smooth app publication.

**Understanding App Signing**

Android requires all APKs and App Bundles to be digitally signed with a certificate. The signature:
- Verifies your identity as the developer
- Ensures app updates come from the same source
- Enables app integrity checks

**Creating a Keystore**

```bash
# Generate a new keystore
keytool -genkey -v \
  -keystore myapp.keystore \
  -alias myapp \
  -keyalg RSA \
  -keysize 2048 \
  -validity 10000

# Generate upload key for Play Store signing
keytool -genkey -v \
  -keystore upload.keystore \
  -alias upload \
  -keyalg RSA \
  -keysize 2048 \
  -validity 10000
```

**Gradle Configuration**

```kotlin
// build.gradle.kts (androidApp)
android {
    signingConfigs {
        create("release") {
            storeFile = file("myapp.keystore")
            storePassword = System.getenv("STORE_PASSWORD")
            keyAlias = "myapp"
            keyPassword = System.getenv("KEY_PASSWORD")
        }
    }
    
    buildTypes {
        release {
            signingConfig = signingConfigs.getByName("release")
            isMinifyEnabled = true
            isShrinkResources = true
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }
}
```

**Play Store Signing**

Google Play offers app signing by Google Play (recommended):

1. **App Signing by Play**: Google manages and protects your signing key
2. **Upload Key**: You keep this - used to upload new versions

Setup process:
```
1. Create upload keystore locally
2. Generate .pem from keystore
3. Upload .pem to Play Console
4. Opt in to app signing by Google Play
5. Sign with upload key for all future releases
```

**CI/CD Signing**

```yaml
# GitHub Actions
- name: Decode keystore
  env:
    ENCODED_KEYSTORE: ${{ secrets.KEYSTORE_BASE64 }}
  run: |
    echo $ENCODED_KEYSTORE | base64 -d > androidApp/myapp.keystore

- name: Build release
  env:
    STORE_PASSWORD: ${{ secrets.STORE_PASSWORD }}
    KEY_PASSWORD: ${{ secrets.KEY_PASSWORD }}
  run: ./gradlew :androidApp:assembleRelease
```

**Verifying Signing**

```bash
# Verify APK is signed
jarsigner -verify -verbose -certs myapp.apk

# Get certificate info
keytool -printcert -jarfile myapp.apk
```

**Keystore Security Best Practices**

1. Never commit keystores to version control
2. Store passwords in CI/CD secrets or environment variables
3. Back up your keystore securely (cloud + offline)
4. Use different keystores for debug and release
5. Set long validity periods (25+ years)
6. Document the keystore location and passwords securely

Losing your keystore means you cannot update your app - treat it as a critical asset.
