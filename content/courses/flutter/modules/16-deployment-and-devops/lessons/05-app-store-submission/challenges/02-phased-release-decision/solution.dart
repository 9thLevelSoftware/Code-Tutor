/// Phased Release Decision Monitor (App Store) - Solution
///
/// This solution demonstrates how to programmatically analyze
/// App Store phased release metrics and decide when to pause
/// the release to address issues.

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

/// Thresholds for phased release decisions
const double _crashRateMultiplierThreshold = 3.0; // 3x increase is serious
const int _criticalReviewPatternThreshold = 3; // 3+ similar reviews

/// Analyzes phased release metrics and determines the appropriate action.
///
/// Decision logic:
/// 1. App Store phased release schedule: 1% → 2% → 5% → 10% → 20% → 50% → 100%
/// 2. If crash rate increases by 3x or more, pause immediately
/// 3. If multiple 1-star reviews mention the same issue, pause immediately
/// 4. Pausing stops the automatic progression and allows you to submit a fix
/// 5. Never continue or release to all when serious issues detected
PhasedReleaseDecision analyzePhasedRelease(PhasedReleaseMetrics metrics) {
  // Calculate crash rate multiplier (how many times worse it is)
  final crashRateMultiplier = metrics.crashRate / metrics.previousCrashRate;

  // Critical: Crash rate increased significantly (e.g., 0.5% → 4% = 8x increase)
  if (crashRateMultiplier >= _crashRateMultiplierThreshold) {
    return PhasedReleaseDecision(
      action: PhasedReleaseAction.pauseRelease,
      reason: 'Crash rate increased ${crashRateMultiplier.toStringAsFixed(1)}x '
          '(from ${metrics.previousCrashRate}% to ${metrics.crashRate}%). '
          'This is a serious regression. Pausing phased release to prevent '
          'more users from being affected while we fix the issue.',
    );
  }

  // Check for patterns in critical reviews
  final patternDetected = _detectCriticalPattern(metrics.criticalReviewComments);
  if (patternDetected != null) {
    return PhasedReleaseDecision(
      action: PhasedReleaseAction.pauseRelease,
      reason: 'Multiple 1-star reviews mention the same critical issue: '
          '"$patternDetected". This confirms the crash reports. Pausing '
          'phased release to fix the ${patternDetected.toLowerCase()} crash '
          'before continuing.',
    );
  }

  // High number of 1-star reviews without clear pattern but concerning
  if (metrics.oneStarReviewCount >= 5 && metrics.dayOfRelease <= 7) {
    return PhasedReleaseDecision(
      action: PhasedReleaseAction.pauseRelease,
      reason: '${metrics.oneStarReviewCount} 1-star reviews received in just '
          '${metrics.dayOfRelease} days. Early negative feedback suggests '
          'serious issues. Pausing to investigate.',
    );
  }

  // No critical issues - safe to continue automatic phased release
  return PhasedReleaseDecision(
    action: PhasedReleaseAction.continueRelease,
    reason: 'No critical issues detected. Crash rate stable, no concerning review patterns. '
        'Phased release will continue automatically through its schedule.',
  );
}

/// Detects patterns in critical review comments
String? _detectCriticalPattern(List<String> comments) {
  if (comments.length < _criticalReviewPatternThreshold) return null;

  final Map<String, int> keywordCounts = {};

  for (final comment in comments) {
    final lowerComment = comment.toLowerCase();

    // Camera/photo related issues
    if (lowerComment.contains('photo') ||
        lowerComment.contains('camera') ||
        lowerComment.contains('picture')) {
      keywordCounts['photo/camera'] = (keywordCounts['photo/camera'] ?? 0) + 1;
    }

    // Crash related
    if (lowerComment.contains('crash')) {
      keywordCounts['crash'] = (keywordCounts['crash'] ?? 0) + 1;
    }

    // Freeze/hang related
    if (lowerComment.contains('freeze') ||
        lowerComment.contains('hang') ||
        lowerComment.contains('stuck')) {
      keywordCounts['freeze'] = (keywordCounts['freeze'] ?? 0) + 1;
    }

    // Data loss related
    if (lowerComment.contains('lost') || lowerComment.contains('deleted')) {
      keywordCounts['data loss'] = (keywordCounts['data loss'] ?? 0) + 1;
    }
  }

  // Return the first pattern that meets the threshold
  for (final entry in keywordCounts.entries) {
    if (entry.value >= _criticalReviewPatternThreshold) {
      return entry.key;
    }
  }

  return null;
}

/// Gets the next percentage in App Store's phased release schedule
int getNextPhasedReleasePercentage(int currentPercentage) {
  final schedule = [1, 2, 5, 10, 20, 50, 100];
  final currentIndex = schedule.indexOf(currentPercentage);

  if (currentIndex >= 0 && currentIndex < schedule.length - 1) {
    return schedule[currentIndex + 1];
  }

  return 100; // Final stage
}

void main() {
  // Scenario from challenge: Day 3 at 5%, crash rate 0.5% → 4%
  // Multiple 1-star reviews mention crash when taking a photo
  final metrics = PhasedReleaseMetrics(
    dayOfRelease: 3,
    currentPercentage: 5,
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
  print('=== App Store Phased Release Analysis ===');
  print('Day ${metrics.dayOfRelease} of release');
  print('Current rollout: ${metrics.currentPercentage}%');
  print(
    'Next scheduled: ${getNextPhasedReleasePercentage(metrics.currentPercentage)}%',
  );
  print('Crash rate: ${metrics.crashRate}%');
  print('Previous crash rate: ${metrics.previousCrashRate}%');
  print(
    'Crash multiplier: ${(metrics.crashRate / metrics.previousCrashRate).toStringAsFixed(1)}x',
  );
  print('1-star reviews: ${metrics.oneStarReviewCount}');
  print('\n>>> DECISION: ${decision.action.toString().split('.').last} <<<');
  print('Reason: ${decision.reason}');

  // Additional test scenarios
  print('\n=== Additional Test Scenarios ===');

  // Scenario 2: Healthy release
  final healthyMetrics = PhasedReleaseMetrics(
    dayOfRelease: 5,
    currentPercentage: 10,
    crashRate: 0.6,
    previousCrashRate: 0.5,
    oneStarReviewCount: 1,
    criticalReviewComments: ['minor complaint'],
  );
  final healthyDecision = analyzePhasedRelease(healthyMetrics);
  print('\nHealthy release (Day 5, 10%): ${healthyDecision.action.toString().split('.').last}');

  // Scenario 3: Moderate concern (no clear pattern but many reviews)
  final moderateMetrics = PhasedReleaseMetrics(
    dayOfRelease: 2,
    currentPercentage: 2,
    crashRate: 1.0,
    previousCrashRate: 0.5,
    oneStarReviewCount: 6,
    criticalReviewComments: [
      'slow performance',
      'ui is confusing',
      'battery drain',
      'slow to load',
      'confusing navigation',
      'uses too much battery',
    ],
  );
  final moderateDecision = analyzePhasedRelease(moderateMetrics);
  print('Moderate concern (Day 2, 2%): ${moderateDecision.action.toString().split('.').last}');
  print('Reason: ${moderateDecision.reason}');
}
