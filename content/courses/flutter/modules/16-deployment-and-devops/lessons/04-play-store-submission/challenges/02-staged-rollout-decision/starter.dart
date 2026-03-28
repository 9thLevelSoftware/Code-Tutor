import 'package:flutter/material.dart';

// Staged Rollout Decision Challenge
// When releasing to production, you can use staged rollouts to limit risk.
// Implement the decision logic for when to pause, resume, or halt a rollout.

enum RolloutAction {
  continueRollout,
  pauseRollout,
  haltRollout,
}

class RolloutDecisionMaker {
  // TODO: Implement the logic to decide the appropriate rollout action
  // based on crash rates, user feedback, and current rollout percentage
  
  RolloutAction decideAction({
    required double currentRolloutPercentage,
    required double crashRate,
    required double previousCrashRate,
    required double oneStarReviewPercentage,
    required int dayOfRollout,
  }) {
    // Your implementation here
    throw UnimplementedError('Implement rollout decision logic');
  }
  
  String getActionExplanation(RolloutAction action) {
    // TODO: Explain why each action was taken
    switch (action) {
      case RolloutAction.continueRollout:
        return '';
      case RolloutAction.pauseRollout:
        return '';
      case RolloutAction.haltRollout:
        return '';
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
}
