import 'package:flutter/material.dart';

class AccessibleProductCard extends StatelessWidget {
  final String imageUrl;
  final String name;
  final String price;
  final double rating;
  final VoidCallback onTap;

  const AccessibleProductCard({
    super.key,
    required this.imageUrl,
    required this.name,
    required this.price,
    required this.rating,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    // Implement accessible product card below
    // 1. Use MergeSemantics to combine announcements
    // 2. Add button semantics for the tap action
    // 3. Exclude decorative stars but include rating label
    // 4. Ensure proper touch target size
    return Container();
  }
}