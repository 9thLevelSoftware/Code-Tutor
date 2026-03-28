/// Phased Release Decision Monitor (App Store)
///
/// This exercise helps you understand when to pause a phased release
/// on the App Store based on metrics and user reviews.

class PhasedReleaseMetrics {
  final int dayOfRelease;
  final double currentPercentage;
  final double crashRate;
  final double previousCrashRate;
  final int oneStarReviewCount;
  final List<String> criticalReviewComments;

  PhasedReleaseMetrics({
    required this.dayOfRelease,
    required this.currentPercentage,
    required this.crashRate,
    required this.previousCrashRate,
    required this.oneStarReviewCount,
    required this.criticalReviewComments,
  });
}

enum PhasedReleaseAction {
  continueRelease,
  pauseRelease,
  releaseToAll,
  ignoreIssues,
}

class PhasedReleaseDecision {
  final PhasedReleaseAction action;
  final String reason;

  PhasedReleaseDecision({
    required this.action,
    required this.reason,
  });
}

/// Analyzes phased release metrics and determines the appropriate action.
///
/// Guidelines:
/// - App Store phased release goes: 1% → 2% → 5% → 10% → 20% → 50% → 100%
/// - If crash rate jumps significantly (e.g., 0.5% → 4%), pause immediately
/// - If multiple 1-star reviews mention the same crash, pause immediately
/// - Never release to all users when issues are detected
/// - Pausing allows you to fix and submit a new version
PhasedReleaseDecision analyzePhasedRelease(PhasedReleaseMetrics metrics) {
  // Guide: Calculate the crash rate increase

  // Guide: Check if crash rate indicates a serious regression
  // (e.g., from 0.5% to 4% is a significant jump)

  // Guide: Check if critical reviews indicate a pattern

  // Guide: Return appropriate action with clear reasoning

  return PhasedReleaseDecision(
    action: PhasedReleaseAction.continueRelease,
    reason: 'No critical issues detected',
  );
}

void main() {
  // Scenario: Day 3 of phased release at 5%
  // Crash rate jumped from 0.5% to 4%
  // Multiple 1-star reviews mention crash when taking a photo
  final metrics = PhasedReleaseMetrics(
    dayOfRelease: 3,
    currentPercentage: 5.0,
    crashRate: 4.0,
    previousCrashRate: 0.5,
    oneStarReviewCount: 8,
    criticalReviewComments: [
      'App crashes when taking a photo',
      'Camera crashes every time',
      'Crashes when I try to take pictures',
      'Photo feature is completely broken',
      'App freezes and crashes on camera',
      'Cannot take photos, app crashes',
      'Photo crash is frustrating',
      'Crashes immediately when opening camera',
    ],
  );

  final decision = analyzePhasedRelease(metrics);
  print('Phased Release Analysis');
  print('Day: ${metrics.dayOfRelease}');
  print('Rollout: ${metrics.currentPercentage}%');
  print('Crash rate: ${metrics.crashRate}%');
  print('Previous crash rate: ${metrics.previousCrashRate}%');
  print('1-star reviews: ${metrics.oneStarReviewCount}');
  print('\nDecision: ${decision.action}');
  print('Reason: ${decision.reason}');
}
