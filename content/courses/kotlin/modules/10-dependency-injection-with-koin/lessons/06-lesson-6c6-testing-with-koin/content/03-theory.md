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

### Base Test Class (Recommended Pattern)

Use `KoinTestRule` for proper test isolation:

```kotlin
import org.koin.test.KoinTest
import org.koin.test.KoinTestRule
import org.koin.test.inject
import kotlin.test.Test

abstract class BaseKoinTest : KoinTest {
    
    // This rule automatically starts/stops Koin for each test
    @get:Rule
    val koinTestRule = KoinTestRule.create { 
        modules(testModules())
    }
    
    abstract fun testModules(): List<Module>
}

// Example usage
class NotesViewModelTest : BaseKoinTest() {
    
    private val viewModel: NotesViewModel by inject()
    
    override fun testModules() = listOf(testModule)
    
    @Test
    fun `test view model`() {
        // Koin is automatically started before and stopped after
        val result = viewModel.loadNotes()
        assertNotNull(result)
    }
}
```

### Alternative: Manual Start/Stop

For Kotlin Multiplatform (where JUnit rules may not be available):

```kotlin
import org.koin.core.context.startKoin
import org.koin.core.context.stopKoin
import org.koin.test.KoinTest
import org.koin.test.inject
import kotlin.test.AfterTest
import kotlin.test.BeforeTest

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
```