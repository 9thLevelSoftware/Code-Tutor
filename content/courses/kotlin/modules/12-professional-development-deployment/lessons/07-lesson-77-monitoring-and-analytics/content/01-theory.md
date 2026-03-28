---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes

Monitoring and analytics provide visibility into your KMP application's performance and user behavior. Implementing comprehensive observability helps you catch issues early and make data-driven decisions.

**Analytics Implementation Strategy**

Create a shared analytics interface with platform-specific implementations:

```kotlin
// commonMain
interface Analytics {
    fun track(event: String, properties: Map<String, Any> = emptyMap())
    fun identify(userId: String, traits: Map<String, Any> = emptyMap())
    fun screen(name: String, properties: Map<String, Any> = emptyMap())
}

expect fun getAnalytics(): Analytics

// Usage in shared code
class TaskViewModel(private val analytics: Analytics) {
    fun completeTask(taskId: String) {
        // Business logic...
        analytics.track("task_completed", mapOf("task_id" to taskId))
    }
}
```

**Platform Implementations**

```kotlin
// androidMain
actual fun getAnalytics(): Analytics = object : Analytics {
    override fun track(event: String, properties: Map<String, Any>) {
        FirebaseAnalytics.getInstance().logEvent(event) {
            properties.forEach { (key, value) ->
                param(key, value.toString())
            }
        }
    }
    override fun identify(userId: String, traits: Map<String, Any>) {
        FirebaseAnalytics.getInstance().setUserId(userId)
    }
}

// iosMain
actual fun getAnalytics(): Analytics = object : Analytics {
    override fun track(event: String, properties: Map<String, Any>) {
        // Mixpanel or Amplitude integration
        Mixpanel.mainInstance().track(event, properties)
    }
}
```

**Performance Monitoring**

```kotlin
// Track API performance
class MonitoredApiClient(private val client: HttpClient) {
    suspend inline fun <reified T> get(url: String): T {
        val start = Clock.System.now()
        return try {
            client.get(url).body<T>().also {
                val duration = Clock.System.now() - start
                analytics.track("api_success", mapOf(
                    "endpoint" to url,
                    "duration_ms" to duration.inWholeMilliseconds
                ))
            }
        } catch (e: Exception) {
            analytics.track("api_error", mapOf(
                "endpoint" to url,
                "error" to e.message
            ))
            throw e
        }
    }
}
```

**Crash Reporting**

```kotlin
// Common error handler
object ErrorHandler {
    private val reporters = mutableListOf<CrashReporter>()
    
    fun register(reporter: CrashReporter) {
        reporters.add(reporter)
    }
    
    fun report(throwable: Throwable, context: Map<String, String> = emptyMap()) {
        reporters.forEach { it.report(throwable, context) }
    }
}

// In ViewModels
try {
    repository.fetchData()
} catch (e: Exception) {
    ErrorHandler.report(e, mapOf("screen" to "dashboard"))
    _uiState.value = UiState.Error
}
```

**Analytics Events to Track**

| Category | Events | Purpose |
|----------|--------|---------|
| User | login, signup, logout | Engagement |
| Feature | task_created, task_completed | Feature adoption |
| Performance | screen_load_time, api_latency | Performance |
| Error | api_error, crash | Reliability |
| Business | purchase, subscription | Revenue |

**Privacy Considerations**

```kotlin
class PrivacyAwareAnalytics(private val analytics: Analytics) : Analytics {
    override fun track(event: String, properties: Map<String, Any>) {
        // Filter PII
        val sanitized = properties.filterKeys { 
            it !in listOf("email", "phone", "name") 
        }
        analytics.track(event, sanitized)
    }
}
```

**Best Practices**

1. Track events consistently across platforms
2. Use structured event names: `category_action`
3. Include timestamps and user context
4. Respect user privacy settings (GDPR, CCPA)
5. Batch events to reduce network usage
6. Set up alerts for critical error rates

Data-driven development requires reliable analytics - instrument your app from day one.
