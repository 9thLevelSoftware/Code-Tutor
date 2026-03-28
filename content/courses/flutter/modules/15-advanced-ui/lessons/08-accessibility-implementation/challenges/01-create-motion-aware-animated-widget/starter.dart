import 'package:flutter/material.dart';

class NotificationCard extends StatefulWidget {
  final String title;
  final String message;
  final VoidCallback onDismiss;
  final bool isVisible;

  const NotificationCard({
    super.key,
    required this.title,
    required this.message,
    required this.onDismiss,
    required this.isVisible,
  });

  @override
  State<NotificationCard> createState() => _NotificationCardState();
}

class _NotificationCardState extends State<NotificationCard>
    with SingleTickerProviderStateMixin {
  // Add AnimationController and animations here
  
  @override
  void initState() {
    super.initState();
    // Initialize animations here
  }

  @override
  void didUpdateWidget(NotificationCard oldWidget) {
    super.didUpdateWidget(oldWidget);
    // Handle visibility changes with motion awareness here
  }

  @override
  Widget build(BuildContext context) {
    // Check for reduced motion preference here
    // Build animated card that respects motion preferences below
    return Container();
  }
}