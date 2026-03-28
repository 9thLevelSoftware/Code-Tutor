---
type: "EXAMPLE"
title: "Real-World: Analytics Provider"
---

Different analytics SDKs per platform:

```kotlin
// ========== commonMain ==========

interface AnalyticsProvider {
    fun logEvent(name: String, params: Map<String, Any> = emptyMap())
    fun setUserProperty(key: String, value: String)
    fun setUserId(userId: String?)
}

// ========== androidMain ==========

class FirebaseAnalyticsProvider(
    private val context: Context
) : AnalyticsProvider {
    private val analytics = Firebase.analytics
    
    override fun logEvent(name: String, params: Map<String, Any>) {
        val bundle = Bundle().apply {
            params.forEach { (key, value) ->
                when (value) {
                    is String -> putString(key, value)
                    is Int -> putInt(key, value)
                    is Long -> putLong(key, value)
                    is Double -> putDouble(key, value)
                    is Boolean -> putBoolean(key, value)
                }
            }
        }
        analytics.logEvent(name, bundle)
    }
    
    override fun setUserProperty(key: String, value: String) {
        analytics.setUserProperty(key, value)
    }
    
    override fun setUserId(userId: String?) {
        analytics.setUserId(userId)
    }
}

// ========== iosMain ==========

import platform.Foundation.*  // Required for NSNumber, NSString

class FirebaseAnalyticsIOSProvider : AnalyticsProvider {
    override fun logEvent(name: String, params: Map<String, Any>) {
        val nsParams = params.mapValues { (_, value) ->
            when (value) {
                is String -> value as NSString
                is Number -> NSNumber(value.toDouble())
                else -> value.toString() as NSString
            }
        }.toMap() as Map<Any?, *>
        
        FIRAnalytics.logEventWithName(name, parameters = nsParams)
    }
    
    override fun setUserProperty(key: String, value: String) {
        FIRAnalytics.setUserPropertyString(value, forName = key)
    }
    
    override fun setUserId(userId: String?) {
        FIRAnalytics.setUserID(userId)
    }
}

// ========== Platform Modules ==========

// androidMain
val platformModule = module {
    single<AnalyticsProvider> { FirebaseAnalyticsProvider(get()) }
}

// iosMain
val platformModule = module {
    single<AnalyticsProvider> { FirebaseAnalyticsIOSProvider() }
}

// ========== Usage in Common Code ==========

class NotesViewModel(
    private val repository: NotesRepository,
    private val analytics: AnalyticsProvider  // Injected!
) {
    fun onNoteCreated(note: Note) {
        analytics.logEvent("note_created", mapOf(
            "title_length" to note.title.length,
            "has_content" to (note.content.isNotBlank())
        ))
    }
}
```
