---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 55 minutes

Testing asynchronous code requires special consideration. Kotlin's `kotlinx-coroutines-test` and the Turbine library provide powerful tools to test coroutines and Flows predictably across all platforms.

**Testing Coroutines with runTest**

The `runTest` function provides a test dispatcher that automatically advances time:

```kotlin
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.delay

class CoroutineTests {
    @Test
    fun timeoutRespectsDelay() = runTest {
        val repository = Repository()
        
        // This delay is skipped in test time
        val result = repository.fetchWithTimeout()
        
        assertTrue(result.isSuccess)
    }
}
```

**Virtual Time Control**

Control timing explicitly in tests:

```kotlin
@Test
fun debounceWorksCorrectly() = runTest {
    val viewModel = SearchViewModel()
    
    viewModel.onQueryChanged("kotlin")
    viewModel.onQueryChanged("kotlin multiplatform")
    
    // Advance virtual time by 300ms (debounce delay)
    advanceTimeBy(300)
    
    // Now assert search was triggered
    assertEquals("kotlin multiplatform", viewModel.searchResults.value.query)
}
```

**Testing Flows with Turbine**

Turbine provides a turbine-like API for testing Flows:

```kotlin
import app.cash.turbine.test

@Test
fun stateFlowEmitsUpdates() = runTest {
    val viewModel = CounterViewModel()
    
    viewModel.count.test {
        assertEquals(0, awaitItem()) // Initial value
        
        viewModel.increment()
        assertEquals(1, awaitItem())
        
        viewModel.increment()
        assertEquals(2, awaitItem())
        
        cancelAndIgnoreRemainingEvents()
    }
}
```

**Testing SharedFlow**

```kotlin
@Test
fun eventsAreEmittedCorrectly() = runTest {
    val handler = EventHandler()
    
    handler.events.test {
        handler.emitEvent("user_login")
        assertEquals("user_login", awaitItem())
        
        handler.emitEvent("purchase_complete")
        assertEquals("purchase_complete", awaitItem())
    }
}
```

**Best Practices**

1. Always use `runTest` for coroutine tests - never use real dispatchers
2. Turbine's `test { }` block handles collection cancellation automatically
3. Use `advanceUntilIdle()` to process all pending coroutines
4. Test timeout behavior with `advanceTimeBy()`
5. For hot flows, consider `testIn(backgroundScope)` for concurrent testing

These tools make testing asynchronous code as reliable and readable as testing synchronous code.
