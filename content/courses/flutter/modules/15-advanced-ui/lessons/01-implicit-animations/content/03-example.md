---
type: "EXAMPLE"
title: "AnimatedContainer Example"
---


This example shows a box that changes size, color, and border radius when tapped:



```dart
class AnimatedBoxDemo extends StatefulWidget {
  const AnimatedBoxDemo({super.key});

  @override
  State<AnimatedBoxDemo> createState() => _AnimatedBoxDemoState();
}

class _AnimatedBoxDemoState extends State<AnimatedBoxDemo> {
  bool _isExpanded = false;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => setState(() => _isExpanded = !_isExpanded),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 300),
        curve: Curves.easeInOut,
        width: _isExpanded ? 200 : 100,
        height: _isExpanded ? 200 : 100,
        decoration: BoxDecoration(
          color: _isExpanded ? Colors.blue : Colors.red,
          borderRadius: BorderRadius.circular(
            _isExpanded ? 32 : 8,
          ),
          boxShadow: [
            BoxShadow(
              color: Colors.black26,
              blurRadius: _isExpanded ? 20 : 5,
              offset: Offset(0, _isExpanded ? 10 : 2),
            ),
          ],
        ),
        child: Center(
          child: Text(
            _isExpanded ? 'Expanded!' : 'Tap me',
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
      ),
    );
  }
}
```
