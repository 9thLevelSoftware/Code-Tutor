import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
// Assume AppLocalizations is available with:
// - productPrice(String formattedPrice)
// - reviewCount(int count) - handles pluralization
// - lastUpdated(String formattedDate)

class ProductCard extends StatelessWidget {
  final String productName;
  final double price;
  final String currencyCode;
  final int reviewCount;
  final DateTime lastUpdated;

  const ProductCard({
    super.key,
    required this.productName,
    required this.price,
    required this.currencyCode,
    required this.reviewCount,
    required this.lastUpdated,
  });

  @override
  Widget build(BuildContext context) {
    // Get locale and format values here
    // Build RTL-aware card layout below
    // Display localized price, review count, and date below
    return Container();
  }
}