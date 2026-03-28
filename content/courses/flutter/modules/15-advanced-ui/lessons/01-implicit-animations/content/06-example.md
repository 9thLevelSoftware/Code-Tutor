---
type: "EXAMPLE"
title: "AnimatedOpacity and AnimatedScale Example"
---


A favorite button that animates when toggled:



```dart
class AnimatedFavoriteButton extends StatefulWidget {
  const AnimatedFavoriteButton({super.key});

  @override
  State<AnimatedFavoriteButton> createState() => _AnimatedFavoriteButtonState();
}

class _AnimatedFavoriteButtonState extends State<AnimatedFavoriteButton> {
  bool _isFavorite = false;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => setState(() => _isFavorite = !_isFavorite),
      child: AnimatedScale(
        duration: const Duration(milliseconds: 200),
        scale: _isFavorite ? 1.2 : 1.0,
        curve: Curves.elasticOut,
        child: AnimatedOpacity(
          duration: const Duration(milliseconds: 150),
          opacity: _isFavorite ? 1.0 : 0.6,
          child: Icon(
            _isFavorite ? Icons.favorite : Icons.favorite_border,
            color: _isFavorite ? Colors.red : Colors.grey,
            size: 48,
          ),
        ),
      ),
    );
  }
}
```
