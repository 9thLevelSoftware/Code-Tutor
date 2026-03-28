import 'package:flutter/material.dart';
import 'package:rive/rive.dart';

class RiveToggle extends StatefulWidget {
  final bool initialValue;
  final ValueChanged<bool> onChanged;
  
  const RiveToggle({
    super.key,
    this.initialValue = false,
    required this.onChanged,
  });

  @override
  State<RiveToggle> createState() => _RiveToggleState();
}

class _RiveToggleState extends State<RiveToggle> {
  // Store SMIBool reference for 'isOn' input here
  // Track current value here

  void _onRiveInit(Artboard artboard) {
    // Get StateMachineController from artboard
    // State machine name: 'ToggleState'
    // Find 'isOn' input and store reference
    // Set initial value here
  }

  void _toggle() {
    // Toggle value here
    // Update Rive input here
    // Call onChanged callback here
  }

  @override
  Widget build(BuildContext context) {
    // Return GestureDetector with RiveAnimation.asset below
    // Asset path: 'assets/animations/toggle.riv'
    // Size: 80x40
    return Container();
  }
}