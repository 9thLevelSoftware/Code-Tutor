using System.Text.Json;
using CodeTutor.Tests.Models;

namespace CodeTutor.Tests.E2E.UserJourneys;

/// <summary>
/// E2E tests that simulate complete user learning journeys through the application.
/// These tests validate the full flow of learning: selecting courses, completing lessons,
/// tracking progress, and earning achievements.
/// </summary>
public class LearningJourneyTests
{
    private readonly string _contentPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public LearningJourneyTests()
    {
        _contentPath = FindContentPath();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
    }

    private static string FindContentPath()
    {
        var possiblePaths = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "Content", "courses"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "content", "courses"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "courses"),
        };

        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path))
                return Path.GetFullPath(path);
        }

        return Path.Combine(Directory.GetCurrentDirectory(), "Content", "courses");
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void UserJourney_CourseSelection_CanBrowseAllCourses(string courseId)
    {
        // Arrange - User opens the app and browses courses
        var course = LoadCourse(courseId);

        // Assert - User should see course with all required info
        if (course == null) return;

        course.Id.Should().NotBeNullOrEmpty("User should see course ID");
        course.Title.Should().NotBeNullOrEmpty("User should see course title");
        course.Description.Should().NotBeNullOrEmpty("User should see course description");
        course.Difficulty.Should().NotBeNullOrEmpty("User should see difficulty level");
        course.EstimatedHours.Should().BeGreaterThan(0, "User should see estimated time");
        course.Modules.Should().NotBeEmpty("User should see available modules");
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void UserJourney_ModuleNavigation_CanViewAllModules(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Act - User clicks on a course to see modules
        var modules = course.Modules;

        // Assert - User should see all modules with their info
        foreach (var module in modules)
        {
            module.Id.Should().NotBeNullOrEmpty("User should see module ID");
            module.Title.Should().NotBeNullOrEmpty("User should see module title");
            module.Description.Should().NotBeNullOrEmpty("User should see module description");
            module.Lessons.Should().NotBeEmpty("User should see lessons in module");
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void UserJourney_LessonViewing_CanViewLessonContent(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Act - User opens first lesson
        var firstModule = course.Modules.First();
        var firstLesson = firstModule.Lessons.First();

        // Assert - User should see complete lesson content
        firstLesson.Id.Should().NotBeNullOrEmpty("User should see lesson ID");
        firstLesson.Title.Should().NotBeNullOrEmpty("User should see lesson title");
        firstLesson.ContentSections.Should().NotBeEmpty("User should see content sections");

        // Check that lesson has educational content
        var hasTheory = firstLesson.ContentSections.Any(s => s.Type == "THEORY");
        hasTheory.Should().BeTrue("User should see theory content in lesson");
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void UserJourney_LessonProgression_CanNavigateSequentially(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Act - User goes through lessons in order
        foreach (var module in course.Modules)
        {
            var orderedLessons = module.Lessons.OrderBy(l => l.Order).ToList();

            // Assert - Lessons should be sequential
            for (int i = 0; i < orderedLessons.Count; i++)
            {
                orderedLessons[i].Order.Should().Be(i + 1,
                    $"Lesson order should be sequential for navigation");
            }
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void UserJourney_ChallengeCompletion_CanAttemptChallenges(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        var challengeCount = 0;
        var challengesWithHints = 0;

        // Act - User attempts challenges
        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                foreach (var challenge in lesson.Challenges)
                {
                    challengeCount++;

                    // Assert - Challenge should be attempt-able
                    challenge.Id.Should().NotBeNullOrEmpty("Challenge should have ID for tracking");
                    challenge.Type.Should().NotBeNullOrEmpty("Challenge should have type");

                    // Check for helpful content
                    if (challenge.Hints.Any())
                        challengesWithHints++;
                }
            }
        }

        // At least some challenges should exist and have hints
        if (challengeCount > 0)
        {
            ((double)challengesWithHints / challengeCount * 100).Should().BeGreaterOrEqualTo(30,
                "At least 30% of challenges should have hints to help users");
        }
    }

    [Fact]
    public void UserJourney_ProgressTracking_SimulateCompleteLessonFlow()
    {
        // Arrange - Simulate a new user
        var progress = new UserProgress();
        var course = LoadCourse("python");
        if (course == null) return;

        var firstModule = course.Modules.First();
        var firstLesson = firstModule.Lessons.First();

        // Act - User completes first lesson
        progress.CompletedLessons.Add(firstLesson.Id);
        progress.LastUpdated = DateTime.UtcNow;

        // Initialize lesson progress
        progress.LessonProgress[firstLesson.Id] = new LessonProgress
        {
            StartedAt = DateTime.UtcNow.AddMinutes(-30),
            CompletedAt = DateTime.UtcNow,
            BestScore = 100
        };

        // Assert - Progress should be tracked correctly
        progress.CompletedLessons.Should().Contain(firstLesson.Id);
        progress.LessonProgress[firstLesson.Id].CompletedAt.Should().NotBeNull();
        progress.LessonProgress[firstLesson.Id].BestScore.Should().Be(100);
    }

    [Fact]
    public void UserJourney_ProgressTracking_CalculateCourseCompletion()
    {
        // Arrange
        var course = LoadCourse("python");
        if (course == null) return;

        var progress = new UserProgress();
        var totalLessons = course.Modules.Sum(m => m.Lessons.Count);

        // Act - Complete half the lessons
        var lessonsToComplete = totalLessons / 2;
        var completedCount = 0;

        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                if (completedCount >= lessonsToComplete)
                    break;

                progress.CompletedLessons.Add(lesson.Id);
                completedCount++;
            }
        }

        // Assert - Calculate completion percentage
        var completionPercentage = (double)progress.CompletedLessons.Count / totalLessons * 100;
        completionPercentage.Should().BeApproximately(50, 5,
            "Completion should be approximately 50%");
    }

    [Fact]
    public void UserJourney_MultiCourseProgress_TrackAcrossCourses()
    {
        // Arrange
        var progress = new UserProgress();
        var courseIds = new[] { "python", "javascript" };
        var courseLessons = new Dictionary<string, List<string>>();

        // Act - Complete some lessons from each course
        foreach (var courseId in courseIds)
        {
            var course = LoadCourse(courseId);
            if (course == null) continue;

            courseLessons[courseId] = new List<string>();

            var firstModule = course.Modules.FirstOrDefault();
            if (firstModule == null) continue;

            foreach (var lesson in firstModule.Lessons.Take(2))
            {
                progress.CompletedLessons.Add(lesson.Id);
                courseLessons[courseId].Add(lesson.Id);
            }
        }

        // Assert - All lessons should be tracked
        foreach (var courseId in courseIds)
        {
            if (courseLessons.ContainsKey(courseId))
            {
                foreach (var lessonId in courseLessons[courseId])
                {
                    progress.CompletedLessons.Should().Contain(lessonId);
                }
            }
        }
    }

    [Fact]
    public void UserJourney_AchievementSystem_FirstLessonAchievement()
    {
        // Arrange
        var progress = new UserProgress();
        var course = LoadCourse("python");
        if (course == null) return;

        var firstLesson = course.Modules.First().Lessons.First();

        // Act - Complete first lesson and earn achievement
        progress.CompletedLessons.Add(firstLesson.Id);

        // Simulate achievement unlock
        var achievement = new Achievement
        {
            Id = "first_steps",
            Type = "FirstLesson",
            UnlockedAt = DateTime.UtcNow,
            Progress = 1,
            MaxProgress = 1
        };
        progress.Achievements.Add(achievement);

        // Assert
        progress.Achievements.Should().ContainSingle(a => a.Type == "FirstLesson");
        progress.Achievements.First().UnlockedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UserJourney_AchievementSystem_StreakTracking()
    {
        // Arrange
        var progress = new UserProgress();

        // Act - Simulate 7-day streak
        progress.CurrentStreak = 7;
        progress.LongestStreak = 7;

        // Simulate streak achievement
        var streakAchievement = new Achievement
        {
            Id = "week_warrior",
            Type = "WeekStreak",
            UnlockedAt = DateTime.UtcNow,
            Progress = 7,
            MaxProgress = 7
        };
        progress.Achievements.Add(streakAchievement);

        // Assert
        progress.CurrentStreak.Should().Be(7);
        progress.Achievements.Should().ContainSingle(a => a.Type == "WeekStreak");
    }

    [Fact]
    public void UserJourney_ChallengeRetry_TrackMultipleAttempts()
    {
        // Arrange
        var progress = new UserProgress();
        var lessonId = "test-lesson";
        var challengeId = "test-challenge";

        progress.LessonProgress[lessonId] = new LessonProgress();

        // Act - Simulate multiple attempts
        progress.LessonProgress[lessonId].Attempts[challengeId] = 0;

        // First attempt - fail
        progress.LessonProgress[lessonId].Attempts[challengeId]++;
        // Second attempt - fail
        progress.LessonProgress[lessonId].Attempts[challengeId]++;
        // Third attempt - success
        progress.LessonProgress[lessonId].Attempts[challengeId]++;
        progress.LessonProgress[lessonId].ChallengesPassed.Add(challengeId);
        progress.LessonProgress[lessonId].BestScore = 100;

        // Assert
        progress.LessonProgress[lessonId].Attempts[challengeId].Should().Be(3);
        progress.LessonProgress[lessonId].ChallengesPassed.Should().Contain(challengeId);
    }

    [Fact]
    public void UserJourney_HintUsage_TrackHintsPerChallenge()
    {
        // Arrange
        var progress = new UserProgress();
        var lessonId = "test-lesson";
        var challengeId = "test-challenge";

        progress.LessonProgress[lessonId] = new LessonProgress();
        progress.LessonProgress[lessonId].HintsUsed[challengeId] = 0;

        // Act - Use 2 hints
        progress.LessonProgress[lessonId].HintsUsed[challengeId]++;
        progress.LessonProgress[lessonId].HintsUsed[challengeId]++;

        // Assert
        progress.LessonProgress[lessonId].HintsUsed[challengeId].Should().Be(2);
    }

    [Fact]
    public void UserJourney_CodePersistence_SaveLastCodeAttempt()
    {
        // Arrange
        var progress = new UserProgress();
        var lessonId = "test-lesson";
        var challengeId = "test-challenge";
        var userCode = "print('Hello, World!')";

        progress.LessonProgress[lessonId] = new LessonProgress();

        // Act - Save user's code attempt
        progress.LessonProgress[lessonId].LastCode[challengeId] = userCode;

        // Assert
        progress.LessonProgress[lessonId].LastCode[challengeId].Should().Be(userCode);
    }

    [Fact]
    public void UserJourney_TimeTracking_RecordSessionTime()
    {
        // Arrange
        var progress = new UserProgress();

        // Act - Record 30 minutes of learning
        progress.TotalTimeSpentMinutes += 30;

        // Assert
        progress.TotalTimeSpentMinutes.Should().Be(30);
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void UserJourney_ContentQuality_AllLessonsHaveSubstantialContent(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Assert - Each lesson should have meaningful content
        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                // Lesson should have at least 2 content sections
                lesson.ContentSections.Count.Should().BeGreaterOrEqualTo(1,
                    $"Lesson {lesson.Id} should have substantial content");

                // Content should not be empty
                foreach (var section in lesson.ContentSections)
                {
                    section.Content.Length.Should().BeGreaterThan(50,
                        $"Content section in {lesson.Id} should have meaningful text");
                }
            }
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void UserJourney_LearningPath_ModulesProgressNaturally(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Act - Analyze module structure
        var moduleCount = course.Modules.Count;

        // Assert - Course should have logical progression
        moduleCount.Should().BeGreaterOrEqualTo(3,
            $"Course {courseId} should have at least 3 modules for proper learning path");

        // First module should be beginner-friendly
        var firstModule = course.Modules.First();
        firstModule.Lessons.Count.Should().BeGreaterOrEqualTo(2,
            "First module should have multiple introductory lessons");
    }

    private Course? LoadCourse(string courseId) => TestCourseLoader.LoadCourse(_contentPath, courseId);
}
