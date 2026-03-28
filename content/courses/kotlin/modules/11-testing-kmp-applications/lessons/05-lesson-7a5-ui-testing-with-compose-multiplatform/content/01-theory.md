---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 55 minutes

Testing UI in Compose Multiplatform combines the declarative nature of Compose with cross-platform testing strategies. While platform-specific runners execute tests, your test code can be shared where possible.

**Compose Testing Basics**

Android uses `compose-ui-test`, while other platforms have varying support:

```kotlin
// Android UI test
@RunWith(AndroidJUnit4::class)
class LoginScreenTest {
    @get:Rule
    val composeTestRule = createComposeRule()
    
    @Test
    fun loginButtonEnabledWhenFieldsFilled() {
        composeTestRule.setContent {
            LoginScreen()
        }
        
        // Find and interact with components
        composeTestRule
            .onNodeWithTag("email_field")
            .performTextInput("user@example.com")
        
        composeTestRule
            .onNodeWithTag("password_field")
            .performTextInput("password123")
        
        // Assert state
        composeTestRule
            .onNodeWithTag("login_button")
            .assertIsEnabled()
    }
}
```

**Shared UI Test Patterns**

Extract test logic to common code:

```kotlin
// commonTest - shared test logic
fun runLoginTest(testHarness: TestHarness) {
    // Given
    testHarness.setContent { LoginScreen() }
    
    // When
    testHarness.enterText("email_field", "user@example.com")
    testHarness.enterText("password_field", "password123")
    
    // Then
    testHarness.assertEnabled("login_button")
}

// androidTest - Android-specific runner
@Test
fun androidLoginFlow() = runLoginTest(AndroidTestHarness(composeTestRule))

// iosTest - iOS-specific runner
@Test
fun iosLoginFlow() = runLoginTest(IOSTestHarness(uiTestRunner))
```

**Testing State Management**

Test ViewModel-to-UI integration:

```kotlin
@Test
fun errorStateShowsSnackbar() = runComposeTest {
    val viewModel = LoginViewModel(fakeRepository)
    
    setContent {
        LoginScreen(viewModel = viewModel)
    }
    
    // Trigger error
    viewModel.simulateLoginError()
    
    // Assert UI shows error
    onNodeWithText("Invalid credentials").assertIsDisplayed()
}
```

**Screenshot Testing**

Use Paparazzi for Android or similar tools:

```kotlin
@Composable
fun ProfilePreview() {
    ProfileCard(user = previewUser)
}

// Generates reference screenshots
```

**Best Practices**

1. Add test tags to important composables: `Modifier.testTag("submit_button")`
2. Prefer semantic matchers over test tags when possible: `onNodeWithText("Submit")`
3. Keep UI tests focused - one assertion per test
4. Use fakes for dependencies - never hit real APIs in UI tests
5. Run UI tests on CI using emulators/simulators

UI testing ensures your Compose Multiplatform app looks and behaves correctly on every platform.