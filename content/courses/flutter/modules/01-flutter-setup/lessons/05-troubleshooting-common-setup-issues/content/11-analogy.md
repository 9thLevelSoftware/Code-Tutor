---
type: "ANALOGY"
title: "Problem 9: Pod Install Fails (iOS/Mac)"
---


### Error message:
`Error: CocoaPods not installed` or `pod install failed`

CocoaPods is a dependency manager for iOS. Flutter uses it to manage iOS-specific libraries.

### Solution:
Install CocoaPods and set up your iOS project dependencies:




```bash
# Install CocoaPods
sudo gem install cocoapods

# Set up pods
pod setup

# Then from your project:
cd ios
pod install
cd ..

# Try running again
flutter run
```
