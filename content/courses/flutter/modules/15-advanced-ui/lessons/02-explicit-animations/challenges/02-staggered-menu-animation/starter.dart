class StaggeredMenu extends StatefulWidget {
  final List<String> menuItems;

  const StaggeredMenu({super.key, required this.menuItems});

  @override
  State<StaggeredMenu> createState() => _StaggeredMenuState();
}

class _StaggeredMenuState extends State<StaggeredMenu>
    with SingleTickerProviderStateMixin {
  // Create AnimationController here
  // Create lists of slide and fade animations

  @override
  void initState() {
    super.initState();
    // Initialize controller here
    // Create staggered animations using Interval
    // Hint: Each item should have Interval(i * 0.15, (i * 0.15) + 0.4)
    // Start animation here
  }

  @override
  void dispose() {
    // Dispose controller here
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    // Build menu with animated items below
    return Container();
  }
}