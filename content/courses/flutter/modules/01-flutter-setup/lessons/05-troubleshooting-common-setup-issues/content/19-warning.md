---
type: "WARNING"
title: "Impeller Rendering Issues on Android"
---


**What is Impeller?**
Impeller is Flutter's modern rendering engine, replacing the older Skia renderer. It's enabled by default on iOS (since Flutter 3.29, with no Skia fallback) and Android API 29+ (since Flutter 3.27).

**Why You Might See Issues:**
Most devices work perfectly, but some Android devices have GPU driver bugs that cause:
- Visual glitches or artifacts
- Unexpected stuttering
- Blank screens

**Quick Fix - Disable Impeller Temporarily:**
```bash
flutter run --no-enable-impeller
```

**If Issues Persist:**
See the full troubleshooting guide in `TROUBLESHOOTING.md` section A.1.

**Good News:** Flutter automatically falls back to OpenGL on devices with known issues (like some Exynos chips). Most users never encounter problems.

