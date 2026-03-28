---
type: "THEORY"
title: "Pattern 2: Platform Interface + Injection"
---

Define interfaces in common, implement per platform:

```kotlin
// commonMain/kotlin/platform/FileStorage.kt
interface FileStorage {
    suspend fun save(filename: String, data: ByteArray)
    suspend fun load(filename: String): ByteArray?
    suspend fun delete(filename: String): Boolean
    suspend fun listFiles(): List<String>
}
```

```kotlin
// androidMain/kotlin/platform/FileStorage.android.kt
class AndroidFileStorage(private val context: Context) : FileStorage {
    private val filesDir = context.filesDir
    
    override suspend fun save(filename: String, data: ByteArray) {
        withContext(Dispatchers.IO) {
            File(filesDir, filename).writeBytes(data)
        }
    }
    
    override suspend fun load(filename: String): ByteArray? {
        return withContext(Dispatchers.IO) {
            File(filesDir, filename).takeIf { it.exists() }?.readBytes()
        }
    }
    // ...
}
```

```kotlin
// iosMain/kotlin/platform/FileStorage.ios.kt
import platform.Foundation.*  // Required for NSFileManager, NSData, etc.

class IOSFileStorage : FileStorage {
    private val fileManager = NSFileManager.defaultManager
    private val documentsDir = NSSearchPathForDirectoriesInDomains(
        NSDocumentDirectory, 
        NSUserDomainMask, 
        true
    ).first() as String
    
    override suspend fun save(filename: String, data: ByteArray) {
        val nsData = data.toNSData()
        nsData.writeToFile("$documentsDir/$filename", atomically = true)
    }
    // ...
}
```

### Register in Platform Modules

```kotlin
// androidMain
val platformModule = module {
    single<FileStorage> { AndroidFileStorage(get()) }
}

// iosMain
val platformModule = module {
    single<FileStorage> { IOSFileStorage() }
}
```