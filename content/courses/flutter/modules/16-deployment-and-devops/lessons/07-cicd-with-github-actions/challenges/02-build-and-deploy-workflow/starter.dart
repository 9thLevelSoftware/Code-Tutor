name: Build & Deploy Android

on:
  push:
    tags:
      - '___'  # Match version tags

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-java@v4
        with:
          distribution: 'zulu'
          java-version: '17'

      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.38.0'
          cache: true

      - run: flutter pub get

      # Decode keystore from secret
      - name: Decode keystore
        run: echo "${{ secrets.___ }}" | base64 -d > android/app/keystore.jks

      # Build signed APK
      - name: Build APK
        env:
          KEYSTORE_PASSWORD: ${{ secrets.KEYSTORE_PASSWORD }}
          KEY_ALIAS: ${{ secrets.KEY_ALIAS }}
          KEY_PASSWORD: ${{ secrets.KEY_PASSWORD }}
        run: flutter build apk ___

      # Upload artifact
      - name: Upload APK
        uses: actions/upload-artifact@v4
        with:
          name: android-apk
          path: ___
          retention-days: ___

      # Deploy to Play Store
      - name: Deploy to Play Store
        uses: r0adkll/upload-google-play@v1
        with:
          serviceAccountJsonPlainText: ${{ secrets.GOOGLE_PLAY_SERVICE_ACCOUNT }}
          packageName: com.example.myapp
          releaseFiles: build/app/outputs/flutter-apk/app-release.apk
          track: ___