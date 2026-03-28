---
type: "THEORY"
title: "Step 3: Create the Main Scaffold"
---

The `MainScaffold` manages the `NavigationBar` and coordinates with GoRouter to highlight the correct destination.

```dart
class MainScaffold extends StatelessWidget {
  final Widget child;
  const MainScaffold({required this.child, super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: child,
      bottomNavigationBar: NavigationBar(
        selectedIndex: _calculateSelectedIndex(context),
        onDestinationSelected: (index) => _onDestinationSelected(context, index),
        destinations: const [
          NavigationDestination(icon: Icon(Icons.home), label: 'Home'),
          NavigationDestination(icon: Icon(Icons.search), label: 'Search'),
          // ...
        ],
      ),
    );
  }
}
```