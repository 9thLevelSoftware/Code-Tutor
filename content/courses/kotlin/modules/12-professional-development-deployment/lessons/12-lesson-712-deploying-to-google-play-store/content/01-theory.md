---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 40 minutes

The Google Play Store is the primary distribution channel for Android apps. Understanding the publishing process, store listing optimization, and release management ensures your KMP Android app reaches users effectively.

**Play Console Setup**

1. Create a Google Play Developer account ($25 one-time fee)
2. Set up your app in Play Console
3. Configure store listing (title, description, screenshots)
4. Set up content ratings and pricing

**Release Tracks**

| Track | Purpose | Audience |
|-------|---------|----------|
| Internal | Quick testing | Up to 100 testers |
| Closed | Beta testing | Selected testers |
| Open | Public beta | Anyone can join |
| Production | Full release | All Play Store users |

**Publishing with Gradle**

```kotlin
// build.gradle.kts (androidApp)
plugins {
    id("com.github.triplet.play") version "3.8.4"
}

play {
    serviceAccountCredentials.set(
        file("play-store-credentials.json")
    )
    track.set("internal") // or "alpha", "beta", "production"
    userFraction.set(0.2) // 20% rollout for staged releases
}
```

**Automated Publishing**

```yaml
# .github/workflows/deploy.yml
- name: Publish to Play Store
  env:
    SERVICE_ACCOUNT_JSON: ${{ secrets.PLAY_STORE_JSON }}
  run: |
    echo $SERVICE_ACCOUNT_JSON | base64 -d > service-account.json
    ./gradlew :androidApp:publishBundle
```

**App Bundle vs APK**

Always use Android App Bundle (AAB):
- Smaller download sizes (Google optimizes for each device)
- Required for new apps since 2021
- Easier to manage feature modules

```bash
# Generate AAB
./gradlew :androidApp:bundleRelease

# Upload via Play Console or API
```

**Store Listing Optimization**

```
Title (50 chars): MyApp - Task Manager
Short description (80 chars): Organize tasks effortlessly
Full description (4000 chars): Detailed features...

Screenshots:
- Phone: 1080x1920 or 1920x1080
- 7-inch tablet: 1080x1920
- 10-inch tablet: 2560x1600
```

**Release Checklist**

- [ ] App signed with release keystore
- [ ] Version code incremented
- [ ] ProGuard/R8 enabled for release
- [ ] Privacy policy URL provided
- [ ] Content rating questionnaire completed
- [ ] Store listing assets uploaded
- [ ] Release notes written

**Post-Release**

- Monitor crash reports in Play Console
- Review user ratings and feedback
- Analyze install and retention metrics
- Plan staged rollouts for risky updates

The Play Store is your gateway to billions of Android users - invest time in a polished store presence.
