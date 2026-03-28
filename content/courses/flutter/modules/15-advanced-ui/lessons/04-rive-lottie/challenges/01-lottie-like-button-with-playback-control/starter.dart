import 'package:flutter/material.dart';
import 'package:lottie/lottie.dart';

class LikeButton extends StatefulWidget {
  final bool initiallyLiked;
  final ValueChanged<bool> onLikedChanged;
  
  const LikeButton({
    super.key,
    this.initiallyLiked = false,
    required this.onLikedChanged,
  });

  @override
  State<LikeButton> createState() => _LikeButtonState();
}

class _LikeButtonState extends State<LikeButton>
    with SingleTickerProviderStateMixin {
  // Create AnimationController here
  // Track liked state here
  
  @override
  void initState() {
    super.initState();
    // Initialize controller here
    // Set initial state based on initiallyLiked
  }

  @override
  void dispose() {
    // Dispose controller here
    super.dispose();
  }

  void _toggleLike() {
    // Toggle state and play animation below
    // - If not liked: play forward
    // - If liked: play reverse
    // - Call onLikedChanged callback
  }

  @override
  Widget build(BuildContext context) {
    // Return GestureDetector with Lottie.asset below
    // Use 'assets/animations/heart.json' as the path
    // Size: 60x60
    return Container();
  }
}