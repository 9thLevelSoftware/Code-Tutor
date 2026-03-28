/// TestFlight Testing Strategy Selector
///
/// This exercise helps you choose the appropriate TestFlight
/// testing approach based on your beta testing requirements.

enum TestFlightApproach {
  internalTesting,
  externalEmailInvites,
  externalPublicLink,
  skipTestFlight,
}

class BetaTestingRequirements {
  final int testerCount;
  final bool hasKnownTesterList;
  final bool needFeedbackBeforeLaunch;
  final List<String> testerEmails;

  BetaTestingRequirements({
    required this.testerCount,
    required this.hasKnownTesterList,
    required this.needFeedbackBeforeLaunch,
    required this.testerEmails,
  });
}

class TestFlightCapabilities {
  final int maxTesters;
  final bool requiresTeamMembership;
  final bool usesPublicLink;

  TestFlightCapabilities({
    required this.maxTesters,
    required this.requiresTeamMembership,
    required this.usesPublicLink,
  });
}

/// TestFlight approach capabilities
final Map<TestFlightApproach, TestFlightCapabilities> testFlightInfo = {
  TestFlightApproach.internalTesting: TestFlightCapabilities(
    maxTesters: 100,
    requiresTeamMembership: true,
    usesPublicLink: false,
  ),
  TestFlightApproach.externalEmailInvites: TestFlightCapabilities(
    maxTesters: 10000,
    requiresTeamMembership: false,
    usesPublicLink: false,
  ),
  TestFlightApproach.externalPublicLink: TestFlightCapabilities(
    maxTesters: 10000,
    requiresTeamMembership: false,
    usesPublicLink: true,
  ),
  TestFlightApproach.skipTestFlight: TestFlightCapabilities(
    maxTesters: 0,
    requiresTeamMembership: false,
    usesPublicLink: false,
  ),
};

/// Selects the appropriate TestFlight approach based on requirements.
///
/// Guidelines:
/// - Internal testing: max 100 people, requires App Store Connect team role
/// - External with email invites: up to 10,000, controlled list via email import
/// - External with public link: up to 10,000, anyone with link can join
/// - Skipping TestFlight means going directly to App Store
TestFlightApproach selectTestFlightApproach(BetaTestingRequirements requirements) {
  // Guide: Check if internal team testing is appropriate (requires team membership)

  // Guide: If you have a known list of testers, consider email invites

  // Guide: If you need control over exactly who tests, avoid public links

  // Guide: If feedback is needed before launch, don't skip TestFlight

  return TestFlightApproach.skipTestFlight;
}

void main() {
  // Scenario: 500 external beta users who signed up on website
  // Need feedback before public launch
  final requirements = BetaTestingRequirements(
    testerCount: 500,
    hasKnownTesterList: true,
    needFeedbackBeforeLaunch: true,
    testerEmails: List.generate(500, (i) => 'tester$i@example.com'),
  );

  final approach = selectTestFlightApproach(requirements);
  print('TestFlight approach: $approach');

  final capabilities = testFlightInfo[approach]!;
  print('Max testers: ${capabilities.maxTesters}');
  print('Requires team membership: ${capabilities.requiresTeamMembership}');
  print('Uses public link: ${capabilities.usesPublicLink}');
}
