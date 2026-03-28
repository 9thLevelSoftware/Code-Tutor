/// Staged Rollout Decision Monitor - Solution
///
/// This solution demonstrates how to programmatically analyze
/// rollout metrics and make informed decisions about halting
/// or continuing a staged rollout.

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

/// Thresholds for rollout decisions
const double _crashRateDropThreshold = 3.0; // 3% drop is concerning
const int _patternThreshold = 3; // 3+ similar reviews indicate pattern

/// Analyzes rollout metrics and determines the appropriate action.
///
/// Decision logic:
/// 1. Calculate crash rate drop from baseline
/// 2. If drop > 3%, halt immediately to prevent more affected users
/// 3. Detect patterns in negative reviews
/// 4. If multiple reviews mention same issue, halt immediately
/// 5. Never increase rollout when issues detected
/// 6. Note: Google Play doesn't support automatic rollback
RolloutDecision analyzeRollout(RolloutMetrics metrics) {
  // Calculate crash rate change
  final crashRateDrop = metrics.previousCrashFreeRate - metrics.crashFreeUsersRate;

  // Critical: Crash rate dropped more than threshold
  if (crashRateDrop > _crashRateDropThreshold) {
    return RolloutDecision(
      action: RolloutAction.haltRollout,
      reason: 'Crash-free rate dropped by ${crashRateDrop.toStringAsFixed(1)}% '
          '(from ${metrics.previousCrashFreeRate}% to ${metrics.crashFreeUsersRate}%). '
          'Threshold is $_crashRateDropThreshold%. Halting to prevent more users '
          'from being affected.',
    );
  }

  // Check for patterns in negative reviews (same issue mentioned multiple times)
  final patternDetected = _detectReviewPattern(metrics.negativeReviewTopics);
  if (patternDetected != null) {
    return RolloutDecision(
      action: RolloutAction.haltRollout,
      reason: 'Multiple reviews mention the same issue: "$patternDetected". '
          'Halting rollout to investigate and fix before continuing.',
    );
  }

  // No issues detected - safe to continue
  return RolloutDecision(
    action: RolloutAction.continueRollout,
    reason: 'No significant issues detected. Crash rate stable, no concerning review patterns.',
  );
}

/// Detects if multiple reviews mention similar topics (simple pattern matching)
String? _detectReviewPattern(List<String> reviewTopics) {
  if (reviewTopics.length < _patternThreshold) return null;

  // Count occurrences of key keywords
  final Map<String, int> keywordCounts = {};

  for (final topic in reviewTopics) {
    final lowerTopic = topic.toLowerCase();

    // Extract key terms related to crashes
    if (lowerTopic.contains('crash')) {
      keywordCounts['crash'] = (keywordCounts['crash'] ?? 0) + 1;
    }
    if (lowerTopic.contains('save')) {
      keywordCounts['save'] = (keywordCounts['save'] ?? 0) + 1;
    }
    if (lowerTopic.contains('freeze')) {
      keywordCounts['freeze'] = (keywordCounts['freeze'] ?? 0) + 1;
    }
    if (lowerTopic.contains('error')) {
      keywordCounts['error'] = (keywordCounts['error'] ?? 0) + 1;
    }
  }

  // If any keyword appears in 3+ reviews, it's a pattern
  for (final entry in keywordCounts.entries) {
    if (entry.value >= _patternThreshold) {
      return entry.key;
    }
  }

  return null;
}

void main() {
  // Scenario from challenge: 10% rollout, crash rate dropped from 99.5% to 96.2%
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
  print('=== Staged Rollout Analysis ===');
  print('Current rollout: ${metrics.currentPercentage}%');
  print('Crash-free rate: ${metrics.crashFreeUsersRate}%');
  print('Previous rate: ${metrics.previousCrashFreeRate}%');
  print('Crash rate drop: ${(metrics.previousCrashFreeRate - metrics.crashFreeUsersRate).toStringAsFixed(1)}%');
  print('Negative reviews: ${metrics.negativeReviewCount}');
  print('\n>>> DECISION: ${decision.action.toString().split('.').last} <<<');
  print('Reason: ${decision.reason}');

  // Additional test scenarios
  print('\n=== Additional Test Scenarios ===');

  // Scenario 2: Stable rollout
  final stableMetrics = RolloutMetrics(
    currentPercentage: 25.0,
    crashFreeUsersRate: 99.4,
    previousCrashFreeRate: 99.5,
    negativeReviewCount: 1,
    negativeReviewTopics: ['minor ui issue'],
  );
  final stableDecision = analyzeRollout(stableMetrics);
  print('\nStable rollout (25%): ${stableDecision.action.toString().split('.').last}');
  print('Reason: ${stableDecision.reason}');

  // Scenario 3: Slight dip but no pattern
  final slightDipMetrics = RolloutMetrics(
    currentPercentage: 15.0,
    crashFreeUsersRate: 98.0,
    previousCrashFreeRate: 99.5,
    negativeReviewCount: 2,
    negativeReviewTopics: ['slow loading', 'confusing ui'],
  );
  final slightDipDecision = analyzeRollout(slightDipMetrics);
  print('\nSlight dip (15%, 98% crash-free): ${slightDipDecision.action.toString().split('.').last}');
  print('Reason: ${slightDipDecision.reason}');
}
