/// Staged Rollout Decision Monitor
///
/// This exercise helps you understand when to halt a staged rollout
/// based on metrics and user feedback.

class RolloutMetrics {
  final double currentPercentage;
  final double crashFreeUsersRate;
  final double previousCrashFreeRate;
  final int negativeReviewCount;
  final List<String> negativeReviewTopics;

  RolloutMetrics({
    required this.currentPercentage,
    required this.crashFreeUsersRate,
    required this.previousCrashFreeRate,
    required this.negativeReviewCount,
    required this.negativeReviewTopics,
  });
}

enum RolloutAction {
  continueRollout,
  haltRollout,
  increaseRollout,
  rollback,
}

class RolloutDecision {
  final RolloutAction action;
  final String reason;

  RolloutDecision({
    required this.action,
    required this.reason,
  });
}

/// Analyzes rollout metrics and determines the appropriate action.
///
/// Guidelines:
/// - If crash-free rate drops more than 3% from baseline, halt immediately
/// - If multiple reviews mention the same crash pattern, halt immediately
/// - Never increase rollout when issues are detected
/// - Google Play doesn't support automatic rollback
RolloutDecision analyzeRollout(RolloutMetrics metrics) {
  // Guide: Calculate the crash rate change

  // Guide: Check if crash rate drop exceeds 3% threshold

  // Guide: Check if negative reviews indicate a pattern

  // Guide: Return appropriate action with clear reasoning

  return RolloutDecision(
    action: RolloutAction.continueRollout,
    reason: 'No issues detected',
  );
}

void main() {
  // Scenario: 10% rollout, crash rate dropped from 99.5% to 96.2%
  // Multiple 1-star reviews mention crash when saving data
  final metrics = RolloutMetrics(
    currentPercentage: 10.0,
    crashFreeUsersRate: 96.2,
    previousCrashFreeRate: 99.5,
    negativeReviewCount: 5,
    negativeReviewTopics: [
      'crash when saving data',
      'app crashes on save',
      'crashes when I try to save',
      'save button causes crash',
      'crash during data save',
    ],
  );

  final decision = analyzeRollout(metrics);
  print('Current rollout: ${metrics.currentPercentage}%');
  print('Crash-free rate: ${metrics.crashFreeUsersRate}%');
  print('Previous rate: ${metrics.previousCrashFreeRate}%');
  print('\nDecision: ${decision.action}');
  print('Reason: ${decision.reason}');
}
