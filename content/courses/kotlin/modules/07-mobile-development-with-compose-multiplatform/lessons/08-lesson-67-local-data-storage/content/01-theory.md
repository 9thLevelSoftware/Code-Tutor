---
type: "THEORY"
title: "Local Data Storage in Compose Multiplatform"
---

**Estimated Time**: 75 minutes

**Learning Objectives**:
- Use DataStore for key-value persistence
- Implement MultiplatformSettings for simple preferences
- Apply expect/actual pattern for platform-specific storage
- Build offline-first architecture

---

## Data Persistence in CMP

Every serious mobile app needs local data storage. Whether it's user preferences, cached API responses, or offline-available content, Compose Multiplatform offers multiple solutions through the `expect/actual` mechanism.

### DataStore for Preferences

**DataStore**—available in CMP via JetBrains' port—provides type-safe key-value storage:

```kotlin
// commonMain
expect class DataStoreProvider {
    fun createDataStore(): DataStore<Preferences>
}

// Usage
class SettingsRepository(private val dataStore: DataStore<Preferences>) {
    private val THEME_KEY = stringPreferencesKey("theme")
    
    val theme: Flow<String> = dataStore.data
        .map { it[THEME_KEY] ?: "system" }
    
    suspend fun setTheme(theme: String) {
        dataStore.edit { it[THEME_KEY] = theme }
    }
}
```

### MultiplatformSettings

For simpler use cases, **MultiplatformSettings** (by Russell Wolf) offers a clean API:

```kotlin
// Dependency
implementation("com.russhwolf:multiplatform-settings:1.2.0")

// Usage
class Settings(private val settings: ObservableSettings) {
    var username: String?
        get() = settings.getStringOrNull("username")
        set(value) { settings["username"] = value }
    
    val usernameFlow: Flow<String?> = settings
        .getStringOrNullFlow("username")
}
```

### expect/actual for Complex Storage

When you need platform-specific implementations (like SQLDelight for databases):

```kotlin
// commonMain
expect class DatabaseDriverFactory {
    fun createDriver(): SqlDriver
}

// androidMain
actual class DatabaseDriverFactory(private val context: Context) {
    actual fun createDriver(): SqlDriver =
        AndroidSqliteDriver(Database.Schema, context, "database.db")
}

// iosMain
actual class DatabaseDriverFactory : NativeSession() {
    actual fun createDriver(): SqlDriver =
        NativeSqliteDriver(Database.Schema, "database.db")
}
```

### Building Offline-First Apps

The real power comes from combining local storage with networking:

```kotlin
class TaskRepository(
    private val api: ApiClient,
    private val db: Database
) {
    fun getTasks(): Flow<List<Task>> = db.taskQueries
        .selectAll()
        .asFlow()
        .mapToList()
        .onStart { refreshFromNetwork() }
}
```

Apps like **Pocket Casts** and **Todoist** use this pattern—immediate local data with background synchronization. Users see content instantly, even offline, while the app syncs when connectivity returns.
