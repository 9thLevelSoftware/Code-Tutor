import 'package:flutter/material.dart';

// Phased Release Decision Challenge - Solution
// App Store Connect allows phased releases over 7 days.

enum PhasedReleaseAction {
  continueRelease,
  pauseRelease,
  haltRelease,
}

class PhasedReleaseDecisionMaker {
  /// Decides the phased release action based on metrics.
  /// 
  /// Rules:
  /// - Halt if crash rate > 2x previous version AND > 3%
  /// - Halt if critical bug reports received
  /// - Halt if 1-star rate > 10%
  /// - Pause if crash rate increased > 50% from previous
  /// - Continue if metrics are within acceptable bounds
  PhasedReleaseAction decideAction({
    required int dayOfRelease,
    required double currentRolloutPercentage,
    required double crashRatePercentage,
    required double previousVersionCrashRate,
    required double oneStarReviewRate,
    required bool hasCriticalBugReports,
  }) {
    // Critical: Has critical bug reports - halt immediately
    if (hasCriticalBugReports) {
      return PhasedReleaseAction.haltRelease;
    }
    
    // Critical: Crash rate too high relative to baseline
    if (crashRatePercentage > previousVersionCrashRate * 2 && crashRatePercentage > 3.0) {
      return PhasedReleaseAction.haltRelease;
    }
    
    // Critical: Too many negative reviews
    if (oneStarReviewRate > 10.0) {
      return PhasedReleaseAction.haltRelease;
    }
    
    // Warning: Significant crash increase - pause and investigate
    if (crashRatePercentage > previousVersionCrashRate * 1.5) {
      return PhasedReleaseAction.pauseRelease;
    }
    
    // All metrics acceptable - continue phased release
    return PhasedReleaseAction.continueRelease;
  }
  
  /// Explains the reasoning for each decision.
  String getActionRationale(PhasedReleaseAction action) {
    switch (action) {
      case PhasedReleaseAction.continueRelease:
        return 'Metrics look good. Continuing the 7-day phased release to the App Store.';
      case PhasedReleaseAction.pauseRelease:
        return 'Concerning metrics detected. Pausing release to investigate before affecting more users.';
      case PhasedReleaseAction.haltRelease:
        return 'Critical issues detected. Halt the release immediately, fix the problems, and submit a new version.';
    }
  }
}

void main() {
  final decisionMaker = PhasedReleaseDecisionMaker();
  
  // Scenario: Day 3, 5% rollout, crash rate jumped to 4%
  final action = decisionMaker.decideAction(
    dayOfRelease: 3,
    currentRolloutPercentage: 5.0,
    crashRatePercentage: 4.0,
    previousVersionCrashRate: 0.5,
    oneStarReviewRate: 15.0,
    hasCriticalBugReports: true,
  );
  print('Phased release decision: $action');
  print('Rationale: ${decisionMaker.getActionRationale(action)}');
  // Expected: haltRelease - multiple critical indicators
}
