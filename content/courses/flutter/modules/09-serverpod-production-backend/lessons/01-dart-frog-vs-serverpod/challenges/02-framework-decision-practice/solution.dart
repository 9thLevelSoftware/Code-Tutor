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

class FrameworkRecommendation {
  final String framework;
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
  return FrameworkRecommendation(
    framework: 'Serverpod',
    reasons: [
      'Built-in authentication supports email and Google OAuth out of the box',
      'Real-time sync for smartwatch requires WebSocket streaming - Serverpod has this built-in',
      'Complex relational data (users, workouts, exercises, sets) needs a proper ORM',
      'File storage for progress photos is included',
      'Team has no backend experience - Serverpod reduces complexity with code generation',
      '3-month timeline is tight - cannot afford to build auth, real-time, file storage from scratch',
      'Social features (following, sharing) require complex queries that ORM handles well',
      'Type-safe client generation prevents API bugs during rapid development',
    ],
    keyFeatures: [
      'Authentication (email + Google OAuth)',
      'Real-time streaming (smartwatch sync)',
      'File storage (progress photos)',
      'ORM with relations (workouts contain exercises contain sets)',
      'Automatic client code generation (team velocity)',
    ],
    concerns: [
      'Team needs to learn Docker and PostgreSQL basics',
      'Initial setup takes longer than Dart Frog (15-30 min vs 2 min)',
      'Hosting costs slightly higher due to PostgreSQL requirement',
      'Push notifications still require additional setup (Firebase Cloud Messaging)',
    ],
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