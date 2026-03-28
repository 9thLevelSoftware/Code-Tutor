/// TestFlight Testing Strategy Selector - Solution
///
/// This solution demonstrates how to select the appropriate TestFlight
/// testing approach based on beta testing requirements.

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

/// TestFlight approach capabilities based on Apple documentation
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
/// Decision logic:
/// 1. Internal testing: max 100, requires App Store Connect team role
///    - Use for: Company employees, immediate team
/// 2. External with email invites: up to 10,000, controlled list
///    - Use for: Known beta users, email list signups, targeted testing
/// 3. External with public link: up to 10,000, anyone can join
///    - Use for: Open beta, social media promotion, wide distribution
/// 4. Skip TestFlight: go directly to App Store
///    - Only if you don't need pre-launch feedback
TestFlightApproach selectTestFlightApproach(BetaTestingRequirements requirements) {
  // Must have feedback before launch - can't skip TestFlight
  if (!requirements.needFeedbackBeforeLaunch) {
    return TestFlightApproach.skipTestFlight;
  }

  // Internal testing only works for team members (max 100)
  // and requires giving them App Store Connect roles
  if (requirements.testerCount <= 100) {
    // Small enough for internal, but only if they're team members
    // In practice, external testers are more common for beta programs
  }

  // Best approach: External with email invites
  // - Supports up to 10,000 testers (well above our 500)
  // - Uses known email list from website signups
  // - Controlled - only invited people can test
  // - Ideal for targeted beta feedback
  if (requirements.hasKnownTesterList &&
      requirements.testerEmails.isNotEmpty &&
      requirements.testerCount <= 10000) {
    return TestFlightApproach.externalEmailInvites;
  }

  // Alternative: Public link if we don't have specific emails
  // but still want wide testing (less control)
  if (requirements.testerCount <= 10000) {
    return TestFlightApproach.externalPublicLink;
  }

  // Fallback: If somehow more than 10,000 testers, can't use TestFlight
  return TestFlightApproach.skipTestFlight;
}

/// Validates that the selected approach can accommodate the requirements
bool validateApproach(
  TestFlightApproach approach,
  BetaTestingRequirements requirements,
) {
  final capabilities = testFlightInfo[approach]!;

  // Check tester count
  if (capabilities.maxTesters > 0 &&
      requirements.testerCount > capabilities.maxTesters) {
    return false;
  }

  // Check that we have emails if using email invites approach
  if (approach == TestFlightApproach.externalEmailInvites &&
      requirements.testerEmails.isEmpty) {
    return false;
  }

  return true;
}

void main() {
  // Scenario from challenge: 500 external beta users who signed up on website
  // Need feedback before public launch
  final requirements = BetaTestingRequirements(
    testerCount: 500,
    hasKnownTesterList: true,
    needFeedbackBeforeLaunch: true,
    testerEmails: List.generate(500, (i) => 'tester$i@example.com'),
  );

  final approach = selectTestFlightApproach(requirements);
  print('=== TestFlight Strategy Selection ===');
  print('Scenario: 500 external beta users from website signups');
  print('Need feedback before launch: ${requirements.needFeedbackBeforeLaunch}');
  print('\n>>> SELECTED APPROACH: $approach <<<');

  final capabilities = testFlightInfo[approach]!;
  print('\nApproach capabilities:');
  print('  - Max testers: ${capabilities.maxTesters}');
  print('  - Requires team membership: ${capabilities.requiresTeamMembership}');
  print('  - Uses public link: ${capabilities.usesPublicLink}');

  final isValid = validateApproach(approach, requirements);
  print('\nValidation: ${isValid ? "✓ Valid" : "✗ Invalid"}');

  // Additional scenarios
  print('\n=== Additional Scenarios ===');

  // Scenario 2: Small internal team
  final internalReq = BetaTestingRequirements(
    testerCount: 25,
    hasKnownTesterList: true,
    needFeedbackBeforeLaunch: true,
    testerEmails: List.generate(25, (i) => 'employee$i@company.com'),
  );
  final internalApproach = selectTestFlightApproach(internalReq);
  print('\nInternal team (25): $internalApproach');

  // Scenario 3: Open public beta
  final publicReq = BetaTestingRequirements(
    testerCount: 5000,
    hasKnownTesterList: false,
    needFeedbackBeforeLaunch: true,
    testerEmails: [],
  );
  final publicApproach = selectTestFlightApproach(publicReq);
  print('Open public beta (5000): $publicApproach');

  // Scenario 4: Skip beta (not recommended)
  final skipReq = BetaTestingRequirements(
    testerCount: 0,
    hasKnownTesterList: false,
    needFeedbackBeforeLaunch: false,
    testerEmails: [],
  );
  final skipApproach = selectTestFlightApproach(skipReq);
  print('Skip beta testing: $skipApproach');
}
