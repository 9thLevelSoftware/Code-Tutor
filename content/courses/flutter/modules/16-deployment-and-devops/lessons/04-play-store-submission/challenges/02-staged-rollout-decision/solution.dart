import 'package:flutter/material.dart';

// Staged Rollout Decision Challenge - Solution
// When releasing to production, staged rollouts help limit risk.

enum RolloutAction {
  continueRollout,
  pauseRollout,
  haltRollout,
}

class RolloutDecisionMaker {
  /// Decides the appropriate rollout action based on metrics.
  /// 
  /// Rules:
  /// - Halt if crash rate > 3% (significant regression)
  /// - Halt if 1-star reviews > 10% (serious quality issues)
  /// - Pause if crash rate increased > 2x (investigate before continuing)
  /// - Continue if metrics are stable and acceptable
  RolloutAction decideAction({
    required double currentRolloutPercentage,
    required double crashRate,
    required double previousCrashRate,
    required double oneStarReviewPercentage,
    required int dayOfRollout,
  }) {
    // Critical: Crash rate above 3% is unacceptable
    if (crashRate > 3.0) {
      return RolloutAction.haltRollout;
    }
    
    // Critical: Too many negative reviews indicates serious problems
    if (oneStarReviewPercentage > 10.0) {
      return RolloutAction.haltRollout;
    }
    
    // Warning: Crash rate doubled - pause and investigate
    if (crashRate > previousCrashRate * 2.0) {
      return RolloutAction.pauseRollout;
    }
    
    // Warning: Significant crash increase - pause
    if (crashRate - previousCrashRate > 1.5) {
      return RolloutAction.pauseRollout;
    }
    
    // All metrics acceptable - continue rollout
    return RolloutAction.continueRollout;
  }
  
  /// Explains the reasoning behind each action.
  String getActionExplanation(RolloutAction action) {
    switch (action) {
      case RolloutAction.continueRollout:
        return 'Metrics are within acceptable bounds. Continuing staged rollout to production.';
      case RolloutAction.pauseRollout:
        return 'Concerning metrics detected. Pausing rollout to investigate issues before proceeding.';
      case RolloutAction.haltRollout:
        return 'Critical issues detected. Halt rollout immediately, fix problems, and prepare new release.';
    }
  }
}

void main() {
  final decisionMaker = RolloutDecisionMaker();
  
  // Test scenario: Day 3, 5% rollout, crash rate jumped from 0.5% to 4%
  final action = decisionMaker.decideAction(
    currentRolloutPercentage: 5.0,
    crashRate: 4.0,
    previousCrashRate: 0.5,
    oneStarReviewPercentage: 15.0,
    dayOfRollout: 3,
  );
  print('Decision: $action');
  print('Explanation: ${decisionMaker.getActionExplanation(action)}');
  // Expected: haltRollout - crash rate > 3% and high 1-star reviews
}
