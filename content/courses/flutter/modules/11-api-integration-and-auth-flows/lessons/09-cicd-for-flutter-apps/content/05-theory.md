---
type: "THEORY"
title: "Section 3: Advanced GitHub Actions Workflows"
---


### Workflow with Test Coverage Enforcement


### Workflow with Firebase Test Lab


### Workflow for Automatic Deployment to Stores




```yaml
name: Deploy to Stores

on:
  push:
    tags:
      - 'v*'  # Trigger on version tags like v1.0.0

jobs:
  deploy-android:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.38.0'

      - uses: actions/setup-java@v4
        with:
          distribution: 'zulu'
          java-version: '17'

      - name: Build Android App Bundle
        run: flutter build appbundle --release

      - name: Sign APK
        uses: r0adkll/sign-android-release@v1
        with:
          releaseDirectory: build/app/outputs/bundle/release
          signingKeyBase64: ${{ secrets.SIGNING_KEY }}
          alias: ${{ secrets.KEY_ALIAS }}
          keyStorePassword: ${{ secrets.KEY_STORE_PASSWORD }}
          keyPassword: ${{ secrets.KEY_PASSWORD }}

      - name: Deploy to Google Play
        uses: r0adkll/upload-google-play@v1
        with:
          serviceAccountJsonPlainText: ${{ secrets.GOOGLE_PLAY_SERVICE_ACCOUNT }}
          packageName: com.yourcompany.yourapp
          releaseFiles: build/app/outputs/bundle/release/*.aab
          track: internal  # or: alpha, beta, production

  deploy-ios:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v4

      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.38.0'

      - name: Build iOS app
        run: flutter build ios --release --no-codesign

      - name: Build and sign with Xcode
        run: |
          cd ios
          xcodebuild -workspace Runner.xcworkspace \
            -scheme Runner \
            -configuration Release \
            -archivePath $PWD/build/Runner.xcarchive \
            archive

          xcodebuild -exportArchive \
            -archivePath $PWD/build/Runner.xcarchive \
            -exportPath $PWD/build \
            -exportOptionsPlist ExportOptions.plist

      - name: Upload to TestFlight
        uses: apple-actions/upload-testflight-build@v1
        with:
          app-path: 'ios/build/Runner.ipa'
          issuer-id: ${{ secrets.APPSTORE_ISSUER_ID }}
          api-key-id: ${{ secrets.APPSTORE_API_KEY_ID }}
          api-private-key: ${{ secrets.APPSTORE_API_PRIVATE_KEY }}
```
