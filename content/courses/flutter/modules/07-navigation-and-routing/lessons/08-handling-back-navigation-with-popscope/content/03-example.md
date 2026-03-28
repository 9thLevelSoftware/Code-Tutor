---
type: "EXAMPLE"
title: "Conditional Back Navigation"
---


Allow back only when form is saved:



```dart
class EditScreen extends StatefulWidget {
  @override
  State<EditScreen> createState() => _EditScreenState();
}

class _EditScreenState extends State<EditScreen> {
  bool _hasUnsavedChanges = false;

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_hasUnsavedChanges, // Allow pop only when saved
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          _showDiscardDialog();
        }
      },
      child: Scaffold(
        appBar: AppBar(title: const Text('Edit')),
        body: TextField(
          onChanged: (value) {
            setState(() => _hasUnsavedChanges = true);
          },
        ),
      ),
    );
  }
}
```
