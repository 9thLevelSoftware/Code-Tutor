// Project Scenario: FitTrack Pro
//
// A fitness startup wants to build a mobile app with these features:
// - User accounts with email and Google sign-in
// - Workout logging with exercises, sets, reps, weights
// - Progress tracking with charts over time
// - Social features: follow friends, share workouts
// - Real-time workout sync between phone and smartwatch
// - Photo uploads for progress pictures
// - Push notifications for workout reminders
//
// Team: 2 Flutter developers, no backend experience
// Timeline: 3 months to launch
// Infrastructure: Can use any cloud provider
//
// Your task: Recommend a framework and explain why.

class FrameworkRecommendation {
  final String framework; // 'Dart Frog' or 'Serverpod'
  final List<String> reasons;
  final List<String> keyFeatures;
  final List<String> concerns;
  
  FrameworkRecommendation({
    required this.framework,
    required this.reasons,
    required this.keyFeatures,
    required this.concerns,
  });
}

FrameworkRecommendation analyzeProject() {
  // TODO: Return your recommendation
  // Consider:
  // - What features does the project need?
  // - Which framework provides those features built-in?
  // - What is the team's experience level?
  // - What is the timeline pressure?
  
  return FrameworkRecommendation(
    framework: '', // Your choice
    reasons: [], // Why this framework?
    keyFeatures: [], // Which features seal the deal?
    concerns: [], // Any concerns with your choice?
  );
}

void main() {
  final recommendation = analyzeProject();
  
  print('Recommended Framework: ${recommendation.framework}');
  print('');
  print('Reasons:');
  for (final reason in recommendation.reasons) {
    print('  - $reason');
  }
  print('');
  print('Key Features Needed:');
  for (final feature in recommendation.keyFeatures) {
    print('  - $feature');
  }
  print('');
  print('Concerns:');
  for (final concern in recommendation.concerns) {
    print('  - $concern');
  }
}