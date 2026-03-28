---
type: "EXAMPLE"
title: "Android ViewModel Pattern"
---


Integrating with Android architecture:



```kotlin
import arrow.core.*
import kotlinx.coroutines.flow.*
import androidx.lifecycle.*

sealed interface UserState {
    data object Loading : UserState
    data class Success(val user: User) : UserState
    data class Error(val message: String) : UserState
    data object Offline : UserState
}

class UserViewModel(
    private val repository: UserRepository
) : ViewModel() {
    
    private val _state = MutableStateFlow<UserState>(UserState.Loading)
    val state: StateFlow<UserState> = _state.asStateFlow()
    
    fun loadUser(id: Long) {
        viewModelScope.launch {
            _state.value = UserState.Loading
            
            repository.getUser(id).fold(
                ifLeft = { error ->
                    _state.value = when (error) {
                        is ApiError.NetworkError -> UserState.Offline
                        is ApiError.Timeout -> UserState.Error("Request timed out")
                        is ApiError.ServerError -> UserState.Error(error.message)
                        is ApiError.DeserializationError -> UserState.Error("Invalid response")
                    }
                },
                ifRight = { user ->
                    _state.value = UserState.Success(user)
                }
            )
        }
    }
    
    fun updateUser(user: User) {
        viewModelScope.launch {
            repository.updateUser(user)
                .onRight { updated ->
                    _state.value = UserState.Success(updated)
                }
                .onLeft { error ->
                    // Keep current user, show error message
                    showErrorToast(errorToMessage(error))
                }
        }
    }
}
```
