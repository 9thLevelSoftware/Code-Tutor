class ExpandableProductCard extends StatefulWidget {
  final String title;
  final String description;
  final double price;

  const ExpandableProductCard({
    super.key,
    required this.title,
    required this.description,
    required this.price,
  });

  @override
  State<ExpandableProductCard> createState() => _ExpandableProductCardState();
}

class _ExpandableProductCardState extends State<ExpandableProductCard> {
  bool _isExpanded = false;

  @override
  Widget build(BuildContext context) {
    // TODO: Implement the expandable card
    // 1. Use AnimatedContainer for height (100 collapsed, 200 expanded)
    // 2. Use AnimatedOpacity to show/hide description
    // 3. Use AnimatedRotation to rotate the chevron icon
    return Container();
  }
}