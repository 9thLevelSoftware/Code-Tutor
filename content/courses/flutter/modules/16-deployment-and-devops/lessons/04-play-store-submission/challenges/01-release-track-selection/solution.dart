/// Play Store Release Track Selector - Solution
///
/// This solution demonstrates programmatic selection of the appropriate
/// Play Store release track based on testing requirements.

enum ReleaseTrack {
  internalTesting,
  closedTesting,
  openTesting,
  production,
}

class TestingRequirements {
  final int testerCount;
  final bool requiresSearchVisibility;
  final bool isInternalTeam;

  TestingRequirements({
    required this.testerCount,
    required this.requiresSearchVisibility,
    required this.isInternalTeam,
  });
}

class TrackCapabilities {
  final int maxTesters;
  final bool visibleInSearch;
  final bool requiresInvitation;

  TrackCapabilities({
    required this.maxTesters,
    required this.visibleInSearch,
    required this.requiresInvitation,
  });
}

/// Track information mapping with Play Store limits
final Map<ReleaseTrack, TrackCapabilities> trackInfo = {
  ReleaseTrack.internalTesting: TrackCapabilities(
    maxTesters: 100,
    visibleInSearch: false,
    requiresInvitation: true,
  ),
  ReleaseTrack.closedTesting: TrackCapabilities(
    maxTesters: 10000,
    visibleInSearch: false,
    requiresInvitation: true,
  ),
  ReleaseTrack.openTesting: TrackCapabilities(
    maxTesters: -1, // Unlimited
    visibleInSearch: true,
    requiresInvitation: false,
  ),
  ReleaseTrack.production: TrackCapabilities(
    maxTesters: -1, // Unlimited
    visibleInSearch: true,
    requiresInvitation: false,
  ),
};

/// Selects the appropriate release track based on testing requirements.
///
/// Logic:
/// 1. Internal team testing (<= 100 testers, internal flag) → Internal testing
/// 2. External testing without search visibility → Closed testing
/// 3. Public beta with search visibility → Open testing
/// 4. Full release → Production (not for testing scenarios)
ReleaseTrack selectReleaseTrack(TestingRequirements requirements) {
  // Internal team testing: up to 100 testers, internal team only
  if (requirements.isInternalTeam && requirements.testerCount <= 100) {
    return ReleaseTrack.internalTesting;
  }

  // Closed testing: up to 10,000 testers, no search visibility, invitation-only
  if (!requirements.requiresSearchVisibility &&
      requirements.testerCount <= 10000) {
    return ReleaseTrack.closedTesting;
  }

  // Open testing: public beta with search visibility (Early Access badge)
  if (requirements.requiresSearchVisibility) {
    return ReleaseTrack.openTesting;
  }

  // Default to production for full release
  return ReleaseTrack.production;
}

/// Validates that the selected track meets all requirements
bool validateTrackSelection(
  ReleaseTrack track,
  TestingRequirements requirements,
) {
  final capabilities = trackInfo[track]!;

  // Check tester count limit
  if (capabilities.maxTesters > 0 &&
      requirements.testerCount > capabilities.maxTesters) {
    return false;
  }

  // Check search visibility requirement
  if (!requirements.requiresSearchVisibility && capabilities.visibleInSearch) {
    return false;
  }

  return true;
}

void main() {
  // Scenario from challenge: 500 external beta testers, no search visibility
  final requirements = TestingRequirements(
    testerCount: 500,
    requiresSearchVisibility: false,
    isInternalTeam: false,
  );

  final selectedTrack = selectReleaseTrack(requirements);
  print('Scenario: 500 external beta testers, no search visibility');
  print('Selected track: $selectedTrack');

  // Verify the selection
  final isValid = validateTrackSelection(selectedTrack, requirements);
  print('Selection valid: $isValid');

  final capabilities = trackInfo[selectedTrack]!;
  print('\nTrack capabilities:');
  print('  - Max testers: ${capabilities.maxTesters}');
  print('  - Visible in search: ${capabilities.visibleInSearch}');
  print('  - Requires invitation: ${capabilities.requiresInvitation}');

  // Demonstrate other scenarios
  print('\n--- Additional Scenarios ---');

  // Scenario 2: Internal team testing (50 people)
  final internalReq = TestingRequirements(
    testerCount: 50,
    requiresSearchVisibility: false,
    isInternalTeam: true,
  );
  print(
    '\nInternal team (50): ${selectReleaseTrack(internalReq)}',
  );

  // Scenario 3: Public beta
  final publicReq = TestingRequirements(
    testerCount: 1000,
    requiresSearchVisibility: true,
    isInternalTeam: false,
  );
  print('Public beta (1000): ${selectReleaseTrack(publicReq)}');
}
