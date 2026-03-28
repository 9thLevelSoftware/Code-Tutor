---
type: "THEORY"
title: "Modern Navigation with GoRouter"
---

**Estimated Time**: 60 minutes
**Difficulty**: Intermediate

## Introduction

Flutter's built-in Navigator 2.0 is powerful but notoriously complex. GoRouter, an official Flutter package maintained by the Flutter team, provides a declarative, URL-based routing system that dramatically simplifies deep linking, nested navigation, and routing logic.

In this lesson, you'll learn:

1. **Declarative Routing**: Define your entire navigation graph in one place
2. **Path Parameters**: Extract dynamic values from URLs (`/products/:id`)
3. **Query Parameters**: Handle optional navigation data (`?tab=reviews`)
4. **Deep Linking**: Enable users to navigate to any screen via URL
5. **Redirection**: Implement authentication guards and redirects

## Why GoRouter?

Before GoRouter, implementing deep linking in Flutter required complex boilerplate to synchronize URL bar state with navigation stack. GoRouter handles this automatically:

```dart
final GoRouter _router = GoRouter(
  routes: [
    GoRoute(
      path: '/',
      builder: (context, state) => const HomeScreen(),
    ),
    GoRoute(
      path: '/products/:id',
      builder: (context, state) {
        final id = state.pathParameters['id']!;
        return ProductDetailScreen(productId: id);
      },
    ),
  ],
);
```

## Real-World Context

URL-based navigation is essential for web Flutter apps, but increasingly important for mobile too. When users share content, receive push notifications, or scan QR codes, deep links provide seamless entry points into specific app content. Companies like eBay, BMW, and Square use Flutter with sophisticated routing for their multi-platform applications.

GoRouter's declarative approach mirrors React Router and Vue Router, making it familiar for developers coming from web backgrounds. The routing table becomes a clear map of your entire application's navigation structure.

## Key Features

- **ShellRoute**: Shared UI (bottom nav, drawer) across nested routes
- **StatefulShellRoute**: Preserve state when switching between navigation branches
- **Redirection**: Authentication guards, onboarding flows
- **Error Handling**: Custom 404 pages and error builders
- **Type Safety**: Code generation for type-safe route parameters

Modern Flutter apps deserve modern routing—GoRouter delivers.