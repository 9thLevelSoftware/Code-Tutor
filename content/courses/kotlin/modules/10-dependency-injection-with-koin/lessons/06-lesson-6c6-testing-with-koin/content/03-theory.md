---
type: "THEORY"
title: "Testing with Koin: Setup"
---

### Test Dependencies

```toml
# gradle/libs.versions.toml
[libraries]
koin-test = { module = "io.insert-koin:koin-test", version.ref = "koin" }
koin-test-junit5 = { module = "io.insert-koin:koin-test-junit5", version.ref = "koin" }
```

```kotlin
// commonTest
commonTest.dependencies {
    implementation(libs.koin.test)
}
```

### Modern KoinTestRule Pattern (Recommended)

Use `KoinTestRule` for proper test isolation. This is the modern, preferred approach:

```kotlin
import org.koin.test.KoinTest
import org.koin.test.KoinTestRule
import org.koin.test.inject
import org.junit.Rule
import kotlin.test.Test

class NotesViewModelTest : KoinTest {

    @get:Rule
    val koinTestRule = KoinTestRule.create {
        modules(testModule)
    }
    
    private val viewModel: NotesViewModel by inject()
    
    @Test
    fun `test view model`() {
        // Koin is automatically started before and stopped after each test
        val result = viewModel.loadNotes()
        assertNotNull(result)
    }
}
```

### Alternative: Manual Start/Stop

For Kotlin Multiplatform or edge cases where JUnit rules aren't available, use manual start/stop:

```kotlin
import org.koin.core.context.startKoin
import org.koin.core.context.stopKoin
import org.koin.test.KoinTest
import org.koin.test.inject
import kotlin.test.AfterTest
import kotlin.test.BeforeTest
import kotlin.test.Test

abstract class BaseKoinTestMP : KoinTest {
    
    @BeforeTest
    fun setUp() {
        startKoin {
            modules(testModules())
        }
    }
    
    @AfterTest
    fun tearDown() {
        stopKoin()
    }
    
    abstract fun testModules(): List<Module>
}

// Example usage
class RepositoryTest : BaseKoinTestMP() {
    
    private val repository: NotesRepository by inject()
    
    override fun testModules() = listOf(testModule)
    
    @Test
    fun `test repository`() {
        val result = repository.getAll()
        assertNotNull(result)
    }
}
```

### Key Differences

| Approach | Use When | Pros |
|----------|----------|------|
| **KoinTestRule** | JVM/Android tests with JUnit 4/5 | Automatic lifecycle, cleaner code, recommended |
| **Manual Start/Stop** | Kotlin Multiplatform, edge cases | Full control, works everywhere |

### Best Practice

Always use `KoinTestRule` as your primary approach. Manual start/stop is reserved for:
- Pure Kotlin Multiplatform tests without JUnit
- Tests requiring special setup order
- Legacy test migration scenarios
---