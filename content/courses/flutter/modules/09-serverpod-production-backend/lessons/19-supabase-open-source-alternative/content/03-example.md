---
type: "EXAMPLE"
title: "Authentication with Supabase"
---


### Sign Up

```dart
Future<void> signUp(String email, String password) async {
  final response = await supabase.auth.signUp(
    email: email,
    password: password,
  );
  
  if (response.user != null) {
    print('User created: ${response.user!.email}');
  }
}
```

### Sign In

```dart
Future<void> signIn(String email, String password) async {
  final response = await supabase.auth.signInWithPassword(
    email: email,
    password: password,
  );
  
  if (response.session != null) {
    print('Logged in: ${response.user!.email}');
  }
}
```

### Sign Out

```dart
Future<void> signOut() async {
  await supabase.auth.signOut();
}
```

### Listen to Auth Changes

```dart
supabase.auth.onAuthStateChange.listen((data) {
  final session = data.session;
  if (session != null) {
    // User logged in
    navigateToHome();
  } else {
    // User logged out
    navigateToLogin();
  }
});
```



```dart
// Complete auth service
class SupabaseAuthService {
  final _supabase = Supabase.instance.client;
  
  User? get currentUser => _supabase.auth.currentUser;
  
  Stream<AuthState> get authStateChanges => 
      _supabase.auth.onAuthStateChange;
  
  Future<AuthResponse> signUp(String email, String password) =>
      _supabase.auth.signUp(email: email, password: password);
  
  Future<AuthResponse> signIn(String email, String password) =>
      _supabase.auth.signInWithPassword(email: email, password: password);
  
  Future<void> signOut() => _supabase.auth.signOut();
}
```
