/// Play Store Release Track Selector
///
/// This exercise helps you understand how to programmatically
/// select the appropriate Play Store release track based on testing needs.

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

/// Track information mapping
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
    maxTesters: -1,
    visibleInSearch: true,
    requiresInvitation: false,
  ),
  ReleaseTrack.production: TrackCapabilities(
    maxTesters: -1,
    visibleInSearch: true,
    requiresInvitation: false,
  ),
};

/// Selects the appropriate release track based on testing requirements.
///
/// Guidelines:
/// - For internal team testing (up to 100 people), use internal testing
/// - For external beta testing that shouldn't appear in search, use closed testing
/// - For public beta that should be discoverable, use open testing
/// - Production is for full release, not testing
ReleaseTrack selectReleaseTrack(TestingRequirements requirements) {
  // Guide: Check if this is internal team testing first

  // Guide: If not internal and they don't want search visibility,
  // consider closed testing if tester count is within limits

  // Guide: If they want search visibility, use open testing

  // Default to production (not recommended for testing)
  return ReleaseTrack.production;
}

void main() {
  // Scenario: 500 external beta testers, no search visibility needed
  final requirements = TestingRequirements(
    testerCount: 500,
    requiresSearchVisibility: false,
    isInternalTeam: false,
  );

  final selectedTrack = selectReleaseTrack(requirements);
  print('Selected track: $selectedTrack');

  // Verify the selection meets requirements
  final capabilities = trackInfo[selectedTrack]!;
  print('Max testers supported: ${capabilities.maxTesters}');
  print('Visible in search: ${capabilities.visibleInSearch}');
}
