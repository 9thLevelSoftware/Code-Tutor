---
type: "THEORY"
title: "iOS Storage APIs"
---


### Cross-Platform Storage with Multiplatform Settings

For Compose Multiplatform apps, use the `multiplatform-settings` library instead of DataStore:

```kotlin
// In commonMain - works on Android AND iOS!
expect class SettingsFactory {
    fun create(): Settings
}

class UserPreferences(private val settings: Settings) {
    var isDarkMode: Boolean
        get() = settings.getBoolean("dark_mode", false)
        set(value) = settings.putBoolean("dark_mode", value)
    
    var username: String?
        get() = settings.getStringOrNull("username")
        set(value) = if (value != null) {
            settings.putString("username", value)
        } else {
            settings.remove("username")
        }
}
```

### Platform Implementations

**Android** uses SharedPreferences:
```kotlin
// androidMain
actual class SettingsFactory(private val context: Context) {
    actual fun create(): Settings {
        return SharedPreferencesSettings(
            context.getSharedPreferences("app_prefs", Context.MODE_PRIVATE)
        )
    }
}
```

**iOS** uses UserDefaults:
```kotlin
// iosMain
import platform.Foundation.*  // Required for NSUserDefaults

actual class SettingsFactory {
    actual fun create(): Settings {
        return NSUserDefaultsSettings(NSUserDefaults.standardUserDefaults)
    }
}
```

### Cross-Platform Database with SQLDelight

For structured data across platforms, use SQLDelight:

| Feature | Room (Android) | SQLDelight (Multiplatform) |
|---------|----------------|---------------------------|
| Platform | Android only | Android, iOS, Desktop, Web |
| Query Language | SQL annotations | .sq files |
| Code Generation | Kotlin | Kotlin |
| Flow Support | Yes | Yes |

### Running on iOS

1. Build and run on iOS Simulator
2. Save some data using your storage APIs
3. Force-close the app
4. Reopen - verify data persisted!
5. UserDefaults and SQLite work transparently on iOS

---

