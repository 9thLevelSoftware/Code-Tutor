---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 50 minutes

Security in KMP applications requires platform-specific implementations with shared security principles. Learn how to handle authentication, data storage, and network security across Android, iOS, and Desktop.

**Secure Data Storage**

Use platform-specific secure storage for sensitive data:

```kotlin
// Common interface
interface SecureStorage {
    suspend fun save(key: String, value: String)
    suspend fun get(key: String): String?
    suspend fun delete(key: String)
}

// Android - EncryptedSharedPreferences
actual class PlatformSecureStorage(context: Context) : SecureStorage {
    private val prefs = EncryptedSharedPreferences.create(
        context,
        "secure_prefs",
        MasterKey.Builder(context).setKeyScheme(MasterKey.KeyScheme.AES256_GCM).build(),
        EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
        EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
    )
    
    override suspend fun save(key: String, value: String) {
        prefs.edit().putString(key, value).apply()
    }
    
    override suspend fun get(key: String): String? = prefs.getString(key, null)
    override suspend fun delete(key: String) {
        prefs.edit().remove(key).apply()
    }
}

// iOS - Keychain
actual class PlatformSecureStorage : SecureStorage {
    override suspend fun save(key: String, value: String) {
        val query = mutableMapOf<Any?, Any?>(
            kSecClass to kSecClassGenericPassword,
            kSecAttrAccount to key,
            kSecValueData to value.encodeToByteArray()
        )
        SecItemAdd(query as CFDictionary, null)
    }
    
    override suspend fun get(key: String): String? {
        // Keychain query implementation
    }
}
```

**Network Security**

```kotlin
// Certificate pinning with Ktor
val client = HttpClient {
    engine {
        // Android
        config {
            sslSocketFactory = getPinnedSSLSocketFactory()
        }
    }
    
    install(HttpTimeout) {
        requestTimeoutMillis = 30000
        connectTimeoutMillis = 10000
    }
}

// Validate certificates
fun getPinnedSSLSocketFactory(): SSLSocketFactory {
    val certificatePinner = CertificatePinner.Builder()
        .add("api.example.com", "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")
        .build()
    // Implementation...
}
```

**Authentication Patterns**

```kotlin
class SecureAuthRepository(
    private val storage: SecureStorage,
    private val api: AuthApi
) {
    companion object {
        private const val TOKEN_KEY = "auth_token"
        private const val REFRESH_KEY = "refresh_token"
    }
    
    suspend fun authenticate(email: String, password: String): Result<AuthTokens> {
        return try {
            val response = api.login(email, password)
            // Store tokens securely
            storage.save(TOKEN_KEY, response.accessToken)
            storage.save(REFRESH_KEY, response.refreshToken)
            Result.success(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getToken(): String? = storage.get(TOKEN_KEY)
    
    suspend fun logout() {
        storage.delete(TOKEN_KEY)
        storage.delete(REFRESH_KEY)
    }
}
```

**Obfuscation and Code Protection**

```kotlin
// gradle.pro (Android)
-keep class com.yourcompany.data.model.** { *; }
-dontwarn okhttp3.**
-dontwarn kotlinx.serialization.**
```

**Security Checklist**

- [ ] Use HTTPS for all network requests
- [ ] Pin certificates for critical APIs
- [ ] Store tokens in platform secure storage
- [ ] Implement proper token refresh
- [ ] Obfuscate release builds
- [ ] Validate all user inputs
- [ ] Use parameterized queries (SQL injection prevention)
- [ ] Set appropriate file permissions

Security is not a feature - it's a mindset that should influence every architectural decision.
