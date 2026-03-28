---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 50 minutes

Unit testing shared Kotlin code is straightforward with `kotlin.test`. Since common code is pure Kotlin without platform dependencies, tests run identically across all targets, giving you confidence in cross-platform behavior.

**Setting Up Common Tests**

Add test dependencies to `commonTest`:

```kotlin
// build.gradle.kts
kotlin {
    sourceSets {
        commonTest.dependencies {
            implementation(kotlin("test"))
            implementation(kotlin("test-annotations-common"))
            implementation("org.jetbrains.kotlinx:kotlinx-coroutines-test:1.8.0")
        }
    }
}
```

**Writing Common Tests**

Use standard JUnit-style annotations that work everywhere:

```kotlin
import kotlin.test.Test
import kotlin.test.assertEquals
import kotlin.test.assertTrue
import kotlin.test.assertFailsWith

class CalculatorTest {
    @Test
    fun additionWorks() {
        val calculator = Calculator()
        assertEquals(4, calculator.add(2, 2))
    }
    
    @Test
    fun divisionByZeroThrows() {
        val calculator = Calculator()
        assertFailsWith<IllegalArgumentException> {
            calculator.divide(5, 0)
        }
    }
}
```

**Testing with Coroutines**

Use `runTest` for coroutine testing in common code:

```kotlin
import kotlinx.coroutines.test.runTest

class RepositoryTest {
    @Test
    fun fetchDataReturnsResult() = runTest {
        val repository = FakeRepository()
        val result = repository.fetchData()
        
        assertTrue(result.isSuccess)
        assertEquals("data", result.getOrNull())
    }
}
```

**Parameterized Tests**

Test multiple scenarios efficiently:

```kotlin
@Test
fun validEmailsAreRecognized() {
    val validator = EmailValidator()
    val validEmails = listOf(
        "user@example.com",
        "test+tag@domain.org",
        "name.surname@company.co.uk"
    )
    
    validEmails.forEach { email ->
        assertTrue(validator.isValid(email), "Failed for $email")
    }
}
```

**Best Practices**

1. Keep tests in `commonTest` unless platform-specific behavior is tested
2. Use fakes over mocks when possible - they're more maintainable
3. Test edge cases: empty inputs, nulls, boundary values
4. Run `./gradlew allTests` to verify across all targets
5. Name tests descriptively: `userRepositoryReturnsErrorWhenNetworkFails`

Common tests are your safety net - they ensure business logic works consistently whether your app runs on Android, iOS, or any other platform.
