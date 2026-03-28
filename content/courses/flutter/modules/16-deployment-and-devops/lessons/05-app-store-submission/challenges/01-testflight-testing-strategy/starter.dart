import 'package:flutter/material.dart';

// TestFlight Testing Strategy Challenge
// Decide which TestFlight group configuration is appropriate
// for different stages of app development.

enum TestFlightGroup {
  internalTesters,      // Company employees
  externalBetaTesters,  // External volunteers
  publicBetaTesters,    // Public beta via TestFlight link
}

class TestFlightStrategy {
  // TODO: Implement logic to select the appropriate TestFlight group
  // based on the app's current development stage and testing goals
  
  TestFlightGroup selectGroup({
    required bool isPreAlpha,
    required bool needsEmployeeValidation,
    required bool needsWideDeviceTesting,
    required bool isFeatureComplete,
    required int currentExternalTesterCount,
  }) {
    // Your implementation here
    throw UnimplementedError('Implement group selection logic');
  }
  
  String getGroupDescription(TestFlightGroup group) {
    // TODO: Return description for each group
    switch (group) {
      case TestFlightGroup.internalTesters:
        return '';
      case TestFlightGroup.externalBetaTesters:
        return '';
      case TestFlightGroup.publicBetaTesters:
        return '';
    }
  }
}

void main() {
  final strategy = TestFlightStrategy();
  
  // Scenario 1: Early development, need quick feedback from team
  final group1 = strategy.selectGroup(
    isPreAlpha: true,
    needsEmployeeValidation: true,
    needsWideDeviceTesting: false,
    isFeatureComplete: false,
    currentExternalTesterCount: 0,
  );
  print('Early dev group: $group1');
}
