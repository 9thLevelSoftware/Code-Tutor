---
type: "EXAMPLE"
title: "Testing User Interactions"
---

Testing clicks, text input, and state changes:

### Required Dependencies

```toml
# gradle/libs.versions.toml
[versions]
compose-multiplatform = "1.10.3"

[libraries]
compose-ui-test = { module = "org.jetbrains.compose.ui:ui-test-junit4", version.ref = "compose-multiplatform" }
compose-ui-test-manifest = { module = "org.jetbrains.compose.ui:ui-test-manifest", version.ref = "compose-multiplatform" }
```

```kotlin
// shared/build.gradle.kts
kotlin {
    sourceSets {
        commonTest.dependencies {
            implementation(compose.uiTest)
            implementation(compose.uiTestManifest)
        }
    }
}

// For Android-specific tests
androidTest.dependencies {
    implementation(libs.compose.ui.test)
    debugImplementation(libs.compose.ui.test.manifest)
}
```

### Test Implementation

```kotlin
@Composable
fun AddNoteScreen(
    onSave: (String, String) -> Unit,
    onCancel: () -> Unit
) {
    var title by remember { mutableStateOf("") }
    var content by remember { mutableStateOf("") }
    
    Column(modifier = Modifier.padding(16.dp)) {
        OutlinedTextField(
            value = title,
            onValueChange = { title = it },
            label = { Text("Title") },
            modifier = Modifier
                .fillMaxWidth()
                .testTag("title_input")
        )
        
        OutlinedTextField(
            value = content,
            onValueChange = { content = it },
            label = { Text("Content") },
            modifier = Modifier
                .fillMaxWidth()
                .testTag("content_input")
        )
        
        Row {
            TextButton(onClick = onCancel) {
                Text("Cancel")
            }
            Button(
                onClick = { onSave(title, content) },
                enabled = title.isNotBlank()
            ) {
                Text("Save")
            }
        }
    }
}

// Tests
class AddNoteScreenTest {
    @get:Rule
    val composeTestRule = createComposeRule()
    
    @Test
    fun `save button is disabled when title is empty`() {
        composeTestRule.setContent {
            AddNoteScreen(onSave = { _, _ -> }, onCancel = {})
        }
        
        composeTestRule
            .onNodeWithText("Save")
            .assertIsNotEnabled()
    }
    
    @Test
    fun `save button is enabled when title has text`() {
        composeTestRule.setContent {
            AddNoteScreen(onSave = { _, _ -> }, onCancel = {})
        }
        
        composeTestRule
            .onNodeWithTag("title_input")
            .performTextInput("My Note")
        
        composeTestRule
            .onNodeWithText("Save")
            .assertIsEnabled()
    }
    
    @Test
    fun `clicking save calls onSave with input values`() {
        var savedTitle = ""
        var savedContent = ""
        
        composeTestRule.setContent {
            AddNoteScreen(
                onSave = { title, content ->
                    savedTitle = title
                    savedContent = content
                },
                onCancel = {}
            )
        }
        
        composeTestRule
            .onNodeWithTag("title_input")
            .performTextInput("Test Title")
        
        composeTestRule
            .onNodeWithTag("content_input")
            .performTextInput("Test Content")
        
        composeTestRule
            .onNodeWithText("Save")
            .performClick()
        
        assertEquals("Test Title", savedTitle)
        assertEquals("Test Content", savedContent)
    }
    
    @Test
    fun `clicking cancel calls onCancel`() {
        var cancelCalled = false
        
        composeTestRule.setContent {
            AddNoteScreen(
                onSave = { _, _ -> },
                onCancel = { cancelCalled = true }
            )
        }
        
        composeTestRule
            .onNodeWithText("Cancel")
            .performClick()
        
        assertTrue(cancelCalled)
    }
}
```
