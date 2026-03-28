import 'package:flutter/material.dart';

// TestFlight Testing Strategy Challenge - Solution
// Decide which TestFlight group configuration is appropriate
// for different stages of app development.

enum TestFlightGroup {
  internalTesters,      // Company employees (up to 100)
  externalBetaTesters,  // External volunteers (up to 10,000 via email invite)
  publicBetaTesters,    // Public beta via TestFlight link (up to 10,000)
}

class TestFlightStrategy {
  /// Selects the appropriate TestFlight group based on app stage.
  /// 
  /// Rules:
  /// - Internal testers: Pre-alpha, employee validation needed, not feature complete
  /// - External beta: Feature complete, needs specific device testing, < 10k testers
  /// - Public beta: Feature complete, ready for wide distribution, needs many testers
  TestFlightGroup selectGroup({
    required bool isPreAlpha,
    required bool needsEmployeeValidation,
    required bool needsWideDeviceTesting,
    required bool isFeatureComplete,
    required int currentExternalTesterCount,
  }) {
    // Pre-alpha or needs employee validation first -> internal only
    if (isPreAlpha || needsEmployeeValidation) {
      return TestFlightGroup.internalTesters;
    }
    
    // Not feature complete -> stay internal until ready
    if (!isFeatureComplete) {
      return TestFlightGroup.internalTesters;
    }
    
    // Feature complete but needs specific device testing -> external beta
    if (!needsWideDeviceTesting && currentExternalTesterCount < 10000) {
      return TestFlightGroup.externalBetaTesters;
    }
    
    // Feature complete, needs wide testing -> public beta
    return TestFlightGroup.publicBetaTesters;
  }
  
  /// Returns a description of each TestFlight group.
  String getGroupDescription(TestFlightGroup group) {
    switch (group) {
      case TestFlightGroup.internalTesters:
        return 'Internal Testers: Up to 100 employees. Best for early development, confidential features, and quick iteration.';
      case TestFlightGroup.externalBetaTesters:
        return 'External Beta: Up to 10,000 testers via email invite. Best for targeted device testing and feedback from specific users.';
      case TestFlightGroup.publicBetaTesters:
        return 'Public Beta: Up to 10,000 testers via public link. Best for wide distribution before App Store release.';
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
  print('Description: ${strategy.getGroupDescription(group1)}');
  
  // Scenario 2: Feature complete, needs wide testing
  final group2 = strategy.selectGroup(
    isPreAlpha: false,
    needsEmployeeValidation: false,
    needsWideDeviceTesting: true,
    isFeatureComplete: true,
    currentExternalTesterCount: 500,
  );
  print('\nWide testing group: $group2');
  print('Description: ${strategy.getGroupDescription(group2)}');
}
