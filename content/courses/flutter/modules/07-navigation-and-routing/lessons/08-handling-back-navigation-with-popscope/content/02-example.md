---
type: "EXAMPLE"
title: "Basic PopScope Usage"
---


Prevent accidental back navigation (e.g., during form editing):



```dart
PopScope(
  canPop: false, // Prevents back gesture/button
  onPopInvokedWithResult: (didPop, result) {
    if (!didPop) {
      // Show confirmation dialog
      showDialog(
        context: context,
        builder: (context) => AlertDialog(
          title: const Text('Discard changes?'),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('Cancel'),
            ),
            TextButton(
              onPressed: () {
                Navigator.pop(context); // Close dialog
                Navigator.pop(context); // Actually go back
              },
              child: const Text('Discard'),
            ),
          ],
        ),
      );
    }
  },
  child: const FormScreen(),
)
```
