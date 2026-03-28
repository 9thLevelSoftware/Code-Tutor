---
type: "EXAMPLE"
title: "Consuming MVI in Compose"
---

How to use MVI ViewModel in Compose:

```kotlin
@Composable
fun NotesScreen(
    viewModel: NotesViewModel,
    onNavigateToNote: (String) -> Unit
) {
    val state by viewModel.state.collectAsState()
    val snackbarHostState = remember { SnackbarHostState() }
    
    // Handle side effects
    LaunchedEffect(Unit) {
        viewModel.sideEffects.collect { effect ->
            when (effect) {
                is NotesSideEffect.ShowSnackbar -> {
                    snackbarHostState.showSnackbar(effect.message)
                }
                is NotesSideEffect.NavigateToNote -> {
                    onNavigateToNote(effect.noteId)
                }
            }
        }
    }
    
    Scaffold(
        snackbarHost = { SnackbarHost(snackbarHostState) }
    ) { padding ->
        Column(modifier = Modifier.padding(padding)) {
            // Search bar - sends Intent
            SearchBar(
                query = state.searchQuery,
                onQueryChange = { query ->
                    viewModel.processIntent(NotesIntent.SearchNotes(query))
                }
            )
            
            // Filter chips - sends Intent
            FilterChips(
                selected = state.selectedFilter,
                onFilterSelected = { filter ->
                    viewModel.processIntent(NotesIntent.SelectFilter(filter))
                }
            )
            
            // Pull to refresh
            Box(
                modifier = Modifier.fillMaxSize()
            ) {
                val pullRefreshState = rememberPullToRefreshState()
                
                if (pullRefreshState.isRefreshing) {
                    LaunchedEffect(true) {
                        viewModel.processIntent(NotesIntent.Refresh)
                        pullRefreshState.endRefresh()
                    }
                }
                
                PullToRefreshContainer(
                    state = pullRefreshState,
                    onRefresh = { viewModel.processIntent(NotesIntent.Refresh) },
                    modifier = Modifier.align(Alignment.TopCenter)
                )
                
                when {
                    state.isLoading -> LoadingIndicator()
                    state.error != null -> {
                        ErrorMessage(
                            message = state.error!!,
                            onDismiss = {
                                viewModel.processIntent(NotesIntent.DismissError)
                            }
                        )
                    }
                    else -> {
                        NotesList(
                            notes = state.notes,
                            onDelete = { id ->
                                viewModel.processIntent(NotesIntent.DeleteNote(id))
                            },
                            onTogglePin = { id ->
                                viewModel.processIntent(NotesIntent.TogglePin(id))
                            }
                        )
                    }
                }
            }
        }
    }
}
```
