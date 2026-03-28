import 'package:flutter/material.dart';

// Release Track Selection Challenge - Solution
// The Play Console offers multiple release tracks for distributing your app.

enum ReleaseTrack {
  production,
  beta,
  alpha,
  internal,
}

class ReleaseTrackSelector {
  /// Selects the appropriate release track based on app state and testing needs.
  /// 
  /// For first releases, start with internal testing to catch major issues.
  /// After internal validation, move to closed testing (alpha/beta).
  /// Only promote to production after successful beta with sufficient testers.
  ReleaseTrack selectTrack({
    required bool isFirstRelease,
    required bool needsInternalTesting,
    required bool hasCompletedBetaTesting,
    required int betaTesterCount,
  }) {
    // First release or needs internal validation -> internal track
    if (isFirstRelease || needsInternalTesting) {
      return ReleaseTrack.internal;
    }
    
    // Completed beta testing with sufficient testers -> production ready
    if (hasCompletedBetaTesting && betaTesterCount >= 100) {
      return ReleaseTrack.production;
    }
    
    // Has some beta testing but not complete -> beta track
    if (betaTesterCount > 0) {
      return ReleaseTrack.beta;
    }
    
    // Default to alpha for early external testing
    return ReleaseTrack.alpha;
  }
  
  /// Returns a description of what each track is used for.
  String getTrackDescription(ReleaseTrack track) {
    switch (track) {
      case ReleaseTrack.production:
        return 'Production: Available to all users on Google Play. Use only after thorough testing.';
      case ReleaseTrack.beta:
        return 'Beta: Closed testing with up to 2000 testers. For pre-release validation.';
      case ReleaseTrack.alpha:
        return 'Alpha: Early testing with a smaller group. May have known issues.';
      case ReleaseTrack.internal:
        return 'Internal: Only for your organization. Fastest updates for quick iteration.';
    }
  }
}

void main() {
  final selector = ReleaseTrackSelector();
  
  // Test scenario 1: First release, needs internal testing
  final track1 = selector.selectTrack(
    isFirstRelease: true,
    needsInternalTesting: true,
    hasCompletedBetaTesting: false,
    betaTesterCount: 0,
  );
  print('First release track: $track1');
  print('Description: ${selector.getTrackDescription(track1)}');
  
  // Test scenario 2: Ready for public release
  final track2 = selector.selectTrack(
    isFirstRelease: false,
    needsInternalTesting: false,
    hasCompletedBetaTesting: true,
    betaTesterCount: 1000,
  );
  print('\nPublic release track: $track2');
  print('Description: ${selector.getTrackDescription(track2)}');
}
