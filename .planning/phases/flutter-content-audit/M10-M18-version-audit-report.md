# Flutter M10-M18 Content Version Audit Report

**Scope:** C:\Users\dasbl\Downloads\Code-Tutor\content\courses\flutter\modules\{10,11,12,13,14,15,16,17,18}
**Current Flutter Stable:** 3.41.0 (as of March 2026)
**Target Reference:** 3.38.x / 3.41.x

## Summary

Found **27 files** with potentially outdated version references across Flutter SDK, Dart SDK, Firebase packages, and other dependencies.

---

## Flutter SDK Version References

### Outdated: 3.24.0 (Should be 3.38.0 or 3.41.0)

| File Path | Current Version | Recommended | Context |
|-----------|----------------|-------------|---------|
| `11-api-integration-and-auth-flows/lessons/09-cicd-for-flutter-apps/content/01-theory.md` | 3.24.0 | 3.38.0+ | GitHub Actions workflow - flutter-action |
| `11-api-integration-and-auth-flows/lessons/09-cicd-for-flutter-apps/content/04-theory.md` | 3.24.0 | 3.38.0+ | GitHub Actions workflow - flutter-action |
| `11-api-integration-and-auth-flows/lessons/09-cicd-for-flutter-apps/content/05-theory.md` (line 34) | 3.24.0 | 3.38.0+ | GitHub Actions workflow - Android build |
| `11-api-integration-and-auth-flows/lessons/09-cicd-for-flutter-apps/content/05-theory.md` (line 68) | 3.24.0 | 3.38.0+ | GitHub Actions workflow - iOS build |
| `11-api-integration-and-auth-flows/lessons/09-cicd-for-flutter-apps/content/07-example.md` | 3.24.0 | 3.38.0+ | GitHub Actions workflow |
| `11-api-integration-and-auth-flows/lessons/09-cicd-for-flutter-apps/content/08-theory.md` | 3.22.0, 3.24.0 | 3.38.0+ | Matrix testing - both versions outdated |
| `11-api-integration-and-auth-flows/lessons/09-cicd-for-flutter-apps/content/10-example.md` | 3.24.0 | 3.38.0+ | GitHub Actions workflow |
| `11-api-integration-and-auth-flows/lessons/10-testing-best-practices-mini-project/content/07-theory.md` | 3.24.0 | 3.38.0+ | GitHub Actions workflow |
| `11-api-integration-and-auth-flows/lessons/10-testing-best-practices-mini-project/content/08-theory.md` | 3.24.0 | 3.38.0+ | GitHub Actions workflow |
| `16-deployment-and-devops/lessons/07-cicd-with-github-actions/challenges/01-complete-flutter-ci-workflow/solution.dart` | 3.24.0 | 3.38.0+ | Challenge solution |
| `16-deployment-and-devops/lessons/07-cicd-with-github-actions/challenges/02-build-and-deploy-workflow/solution.dart` | 3.24.0 | 3.38.0+ | Challenge solution |
| `16-deployment-and-devops/lessons/07-cicd-with-github-actions/challenges/02-build-and-deploy-workflow/starter.dart` | 3.24.0 | 3.38.0+ | Challenge starter |
| `18-capstone-social-chat-app/lessons/12-deploy-launch/challenges/01-automated-release-pipeline/starter.dart` | 3.24.0 | 3.38.0+ | Challenge starter - FLUTTER_VERSION env |

### Mixed: Some 3.38.0 (Acceptable but could be 3.41.0)

| File Path | Current Version | Recommended | Context |
|-----------|----------------|-------------|---------|
| `16-deployment-and-devops/lessons/07-cicd-with-github-actions/content/02-example.md` | 3.38.0 | 3.41.0 | FLUTTER_VERSION env var |
| `16-deployment-and-devops/lessons/07-cicd-with-github-actions/content/03-example.md` | 3.38.0 | 3.41.0 | GitHub Actions workflow |
| `16-deployment-and-devops/lessons/07-cicd-with-github-actions/content/07-example.md` | 3.36.0, 3.38.0 | 3.41.0 | Matrix testing |
| `16-deployment-and-devops/lessons/08-web-desktop-builds/content/02-example.md` | 3.38.0 | 3.41.0 | GitHub Actions workflow |
| `16-deployment-and-devops/lessons/08-web-desktop-builds/content/03-example.md` | 3.38.0 | 3.41.0 | GitHub Actions workflow |
| `16-deployment-and-devops/lessons/08-web-desktop-builds/content/04-example.md` | 3.38.0 | 3.41.0 | GitHub Actions workflow |
| `16-deployment-and-devops/lessons/08-web-desktop-builds/content/05-example.md` | 3.38.0 | 3.41.0 | GitHub Actions workflow |
| `18-capstone-social-chat-app/lessons/12-deploy-launch/content/05-example.md` | 3.38.0 | 3.41.0 | GitHub Actions workflow (multiple references) |

---

## Dart SDK Version References

### Outdated: 3.10.0 constraint

| File Path | Current Version | Recommended | Context |
|-----------|----------------|-------------|---------|
| `18-capstone-social-chat-app/lessons/01-project-setup-architecture/content/03-example.md` | >=3.10.0 <4.0.0 | >=3.10.0 <4.0.0 | pubspec.yaml - Acceptable minimum |
| `18-capstone-social-chat-app/lessons/01-project-setup-architecture/content/04-example.md` | >=3.10.0 <4.0.0 | >=3.10.0 <4.0.0 | pubspec.yaml - Acceptable minimum |
| `18-capstone-social-chat-app/lessons/03-backend-auth-endpoints/content/02-example.md` (3 refs) | >=3.10.0 <4.0.0 | >=3.10.0 <4.0.0 | pubspec.yaml - Acceptable minimum |

**Note:** Dart 3.10.0 constraint is acceptable as a minimum version, but documentation suggests 3.10.0 specifically for Flutter 3.38.0 compatibility.

---

## Firebase Package Versions

### Critical Outdated: firebase_core ^2.x (Should be ^3.x+)

| File Path | Current Version | Recommended | Context |
|-----------|----------------|-------------|---------|
| `11-api-integration-and-auth-flows/lessons/06-auth-flow-oauth/content/03-example.md` | ^2.24.2 | ^3.12.0+ | pubspec.yaml dependency |
| `12-real-time-features/lessons/05-push-notifications/content/02-example.md` | ^2.24.0 | ^3.12.0+ | Commented example in code |

### Critical Outdated: firebase_core ^4.x (MAJOR version issue)

| File Path | Current Version | Recommended | Context |
|-----------|----------------|-------------|---------|
| `09-serverpod-production-backend/lessons/11-module-8-lesson-2-firebase-authentication/content/06-theory.md` | ^4.2.0 | ^3.12.0+ | pubspec.yaml - **Wrong major version** |

### Potentially Outdated: firebase_auth

| File Path | Current Version | Recommended | Context |
|-----------|----------------|-------------|---------|
| `09-serverpod-production-backend/lessons/11-module-8-lesson-2-firebase-authentication/content/06-theory.md` | ^6.1.1 | ^5.5.0+ | pubspec.yaml - **Version doesn't exist** |
| `11-api-integration-and-auth-flows/lessons/06-auth-flow-oauth/content/03-example.md` | ^4.16.0 | ^5.5.0+ | pubspec.yaml |

### Potentially Outdated: Other Firebase packages

| File Path | Current Version | Recommended | Context |
|-----------|----------------|-------------|---------|
| `12-real-time-features/lessons/05-push-notifications/content/02-example.md` | ^14.7.0 | ^15.2.0+ | firebase_messaging - Commented example |
| `17-production-operations/lessons/01-crash-reporting/content/03-example.md` | ^4.3.0 | ^4.3.0+ | firebase_crashlytics - Acceptable |
| `17-production-operations/lessons/02-analytics/content/03-example.md` | ^11.4.0 | ^11.4.0+ | firebase_analytics - Acceptable |
| `17-production-operations/lessons/03-performance-monitoring/content/03-example.md` | ^0.10.0 | ^0.10.0+ | firebase_performance - Acceptable |
| `17-production-operations/lessons/04-feature-flags/content/03-example.md` | ^5.3.0 | ^5.4.0+ | firebase_remote_config - Acceptable |
| `17-production-operations/lessons/04-feature-flags/content/03-example.md` | ^11.4.0 | ^11.4.0+ | firebase_analytics - Acceptable |

---

## Other Package Versions

### Package Version References

| File Path | Package | Current Version | Recommended | Context |
|-----------|---------|----------------|-------------|---------|
| `15-advanced-ui/lessons/04-rive-lottie/content/02-theory.md` | lottie | ^3.1.0 | ^3.3.0+ | pubspec.yaml |
| `16-deployment-and-devops/lessons/08-web-desktop-builds/content/03-example.md` | msix | ^3.16.0 | ^3.16.0+ | pubspec.yaml - Acceptable |
| `17-production-operations/lessons/01-crash-reporting/content/04-example.md` | sentry_flutter | ^8.12.0 | ^8.12.0+ | pubspec.yaml - Acceptable |
| `18-capstone-social-chat-app/lessons/01-project-setup-architecture/content/03-example.md` | cached_network_image | ^3.3.0 | ^3.3.0+ | pubspec.yaml - Acceptable |
| `18-capstone-social-chat-app/lessons/01-project-setup-architecture/content/03-example.md` | flutter_animate | ^4.5.0 | ^4.5.0+ | pubspec.yaml - Acceptable |
| `11-api-integration-and-auth-flows/lessons/06-auth-flow-oauth/content/05-example.md` | sign_in_with_apple | ^5.0.0 | ^6.1.0+ | pubspec.yaml |
| `11-api-integration-and-auth-flows/lessons/06-auth-flow-oauth/content/05-example.md` | crypto | ^3.0.3 | ^3.0.3+ | pubspec.yaml - Acceptable |
| `11-api-integration-and-auth-flows/lessons/06-auth-flow-oauth/content/03-example.md` | google_sign_in | ^6.2.1 | ^6.2.1+ | pubspec.yaml - Acceptable |
| `11-api-integration-and-auth-flows/lessons/08-mini-project-auth-system/content/02-theory.md` | google_sign_in | ^6.2.1 | ^6.2.1+ | pubspec.yaml - Acceptable |
| `13-offline-first-and-persistence/lessons/02-local-storage-options/content/04-theory.md` | isar | ^3.1.0 | ^3.1.0+ | pubspec.yaml (commented) - Acceptable |

---

## GitHub Actions Version References

### Acceptable Versions (Current)

| File Path | Action | Current Version | Recommended | Context |
|-----------|--------|----------------|-------------|---------|
| Multiple files | actions/checkout | v4 | v4 | Current |
| Multiple files | actions/setup-java | v3/v4 | v4 | v4 preferred |
| Multiple files | actions/upload-artifact | v4 | v4 | Current |
| Multiple files | subosito/flutter-action | v2 | v2 | Current |
| `18-capstone-social-chat-app/lessons/12-deploy-launch/content/02-example.md` | dart-lang/setup-dart | v1 | v1 | Acceptable |

### Issue Found:
- `11-api-integration-and-auth-flows/lessons/09-cicd-for-flutter-apps/content/05-theory.md` uses `actions/setup-java@v3` - should be `v4`

---

## Priority Rankings

### Critical (Breaking Changes Likely)
1. **firebase_core ^4.2.0** in M09 - This major version doesn't exist, likely typo
2. **firebase_auth ^6.1.1** in M09 - This version doesn't exist
3. **firebase_core ^2.x** in M11/M12 - Should be ^3.x for Flutter 3.38+

### High (Outdated by Multiple Versions)
1. **Flutter 3.24.0** references (13 files) - 2+ versions behind current stable
2. **Flutter 3.22.0** in matrix testing - Very outdated

### Medium (Acceptable but Aging)
1. **Flutter 3.38.0** references (8 files) - Could be updated to 3.41.0
2. **sign_in_with_apple ^5.0.0** - Current is ^6.1.0+
3. **lottie ^3.1.0** - Could be ^3.3.0+

### Low (Current/Adequate)
1. Dart SDK constraints (>=3.10.0) - Acceptable as minimum
2. Most Firebase packages in M17 (already using ^3.9.0+)
3. GitHub Actions versions (mostly v4)

---

## Files Requiring Updates by Module

### Module 11: API Integration and Auth Flows
- `lessons/09-cicd-for-flutter-apps/content/01-theory.md`
- `lessons/09-cicd-for-flutter-apps/content/04-theory.md`
- `lessons/09-cicd-for-flutter-apps/content/05-theory.md`
- `lessons/09-cicd-for-flutter-apps/content/07-example.md`
- `lessons/09-cicd-for-flutter-apps/content/08-theory.md`
- `lessons/09-cicd-for-flutter-apps/content/10-example.md`
- `lessons/10-testing-best-practices-mini-project/content/07-theory.md`
- `lessons/10-testing-best-practices-mini-project/content/08-theory.md`
- `lessons/06-auth-flow-oauth/content/03-example.md`
- `lessons/06-auth-flow-oauth/content/05-example.md`

### Module 12: Real-Time Features
- `lessons/05-push-notifications/content/02-example.md`

### Module 16: Deployment and DevOps
- `lessons/07-cicd-with-github-actions/challenges/01-complete-flutter-ci-workflow/solution.dart`
- `lessons/07-cicd-with-github-actions/challenges/02-build-and-deploy-workflow/solution.dart`
- `lessons/07-cicd-with-github-actions/challenges/02-build-and-deploy-workflow/starter.dart`

### Module 18: Capstone Social Chat App
- `lessons/12-deploy-launch/challenges/01-automated-release-pipeline/starter.dart`

---

## Recommendations

1. **Immediate Action Required:**
   - Fix M09 firebase packages (impossible versions referenced)
   - Update all Flutter 3.24.0 references to 3.38.0 or 3.41.0
   - Update firebase_core from ^2.x to ^3.12.0+

2. **Standardization:**
   - Establish a consistent Flutter version (recommend 3.38.0 or 3.41.0)
   - Update all GitHub Actions workflows to use same version
   - Standardize Firebase package versions across all modules

3. **Documentation Update:**
   - Update any version check commands that reference outdated versions
   - Ensure challenge instructions match solution versions

---

*Report generated: March 28, 2026*
*Current Flutter Stable: 3.41.0*
*Target Course Version: 3.38.x*
