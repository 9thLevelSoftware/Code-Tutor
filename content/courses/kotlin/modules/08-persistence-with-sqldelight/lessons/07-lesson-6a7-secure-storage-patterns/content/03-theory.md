---
type: "THEORY"
title: "Platform Secure Storage"
---

### Android: EncryptedSharedPreferences

```kotlin
// For small sensitive data (tokens, keys)
import androidx.security.crypto.EncryptedSharedPreferences
import androidx.security.crypto.MasterKey

class AndroidSecureStorage(context: Context) {
    private val masterKey = MasterKey.Builder(context)
        .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
        .build()
    
    private val prefs = EncryptedSharedPreferences.create(
        context,
        "secure_prefs",
        masterKey,
        EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
        EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
    )
    
    fun saveToken(token: String) {
        prefs.edit().putString("auth_token", token).apply()
    }
    
    fun getToken(): String? = prefs.getString("auth_token", null)
    
    fun clearToken() {
        prefs.edit().remove("auth_token").apply()
    }
}
```

### iOS: Keychain

```kotlin
// iosMain/kotlin/SecureStorage.ios.kt
import platform.Foundation.*  // Required for toNSData() extension
import platform.Security.*
import kotlinx.cinterop.*

class IosSecureStorage {
    fun saveToken(token: String) {
        val query = mapOf(
            kSecClass to kSecClassGenericPassword,
            kSecAttrAccount to "auth_token",
            kSecValueData to token.encodeToByteArray().toNSData()
        )
        SecItemDelete(query.toCFDictionary())
        SecItemAdd(query.toCFDictionary(), null)
    }
    
    fun getToken(): String? {
        // Keychain query implementation
        // Returns decrypted token or null
    }
}
```