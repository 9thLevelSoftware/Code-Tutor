enum TransitionType {
  fade,
  slideUp,
  slideRight,
  scale,
}

class CustomPageRoute<T> extends PageRouteBuilder<T> {
  final Widget page;
  final TransitionType type;

  CustomPageRoute({
    required this.page,
    this.type = TransitionType.fade,
  }) : super(
    // Configure pageBuilder here
    // Configure transitionDuration here
    // Configure transitionsBuilder based on type
    pageBuilder: (context, animation, secondaryAnimation) => page,
    transitionsBuilder: (context, animation, secondaryAnimation, child) {
      // Return appropriate transition widget based on type
      return child;
    },
  );
}

// Usage example screen
class TransitionDemo extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    // Create buttons that navigate with different transition types below
    return Container();
  }
}