---
type: "THEORY"
title: "Step 2: Configure GoRouter with ShellRoute"
---

Use `ShellRoute` to wrap your main tabs with a persistent bottom navigation bar. This ensures the bar stays on screen as you switch between primary destinations.

```dart
final router = GoRouter(
  initialLocation: AppRoutes.home,
  routes: [
    ShellRoute(
      builder: (context, state, child) => MainScaffold(child: child),
      routes: [
        GoRoute(path: AppRoutes.home, builder: (context, state) => const FeedScreen()),
        GoRoute(path: AppRoutes.search, builder: (context, state) => const SearchScreen()),
        // ... other tab routes
      ],
    ),
    // Routes outside the shell (no bottom bar)
    GoRoute(
      path: AppRoutes.postDetail,
      builder: (context, state) => PostDetailScreen(id: state.pathParameters['id']!),
    ),
  ],
);
```