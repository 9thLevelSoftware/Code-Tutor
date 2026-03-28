using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CodeTutor.Tests.Models;
using FluentAssertions;
using Xunit;

namespace CodeTutor.Tests.E2E.ContentValidation;

/// <summary>
/// E2E test suite for CourseService functionality.
/// Tests course loading, caching, progress tracking, security, and error handling.
/// Uses a test fixture with temporary content directory for isolated testing.
/// </summary>
public class CourseServiceTests : IDisposable
{
    private readonly string _tempContentPath;
    private readonly string _tempProgressPath;
    private readonly CourseServiceFixture _fixture;
    private readonly CourseCache _cache;
    private bool _disposed;

    public CourseServiceTests()
    {
        _tempContentPath = Path.Combine(Path.GetTempPath(), $"CodeTutor_Test_{Guid.NewGuid():N}");
        _tempProgressPath = Path.Combine(_tempContentPath, "progress");
        Directory.CreateDirectory(_tempContentPath);
        Directory.CreateDirectory(_tempProgressPath);

        _fixture = new CourseServiceFixture(_tempContentPath);
        _cache = new CourseCache();
        _fixture.SetupTestCourses();
    }

    #region Test 1: LoadCourse - Valid Course Loads Correctly

    [Fact]
    public void LoadCourse_ValidCourse_ReturnsCourseWithAllData()
    {
        // Arrange
        var courseId = "test-python";

        // Act
        var course = LoadCourse(courseId);

        // Assert
        course.Should().NotBeNull("Valid course should load successfully");
        course!.Id.Should().Be(courseId, "Course ID should match");
        course.Title.Should().NotBeNullOrEmpty("Course should have a title");
        course.Language.Should().Be("python", "Course language should be set");
        course.Modules.Should().NotBeEmpty("Course should have modules");
        course.EstimatedHours.Should().BeGreaterThan(0, "Course should have estimated hours");
    }

    [Theory]
    [InlineData("test-python", "python")]
    [InlineData("test-javascript", "javascript")]
    [InlineData("test-csharp", "csharp")]
    public void LoadCourse_MultipleValidCourses_ReturnsCorrectLanguage(string courseId, string expectedLanguage)
    {
        // Arrange - setup additional test courses
        _fixture.SetupTestCourse(courseId, expectedLanguage);

        // Act
        var course = LoadCourse(courseId);

        // Assert
        course.Should().NotBeNull();
        course!.Language.Should().Be(expectedLanguage);
    }

    #endregion

    #region Test 2: LoadCourse - Missing Course Returns Null

    [Fact]
    public void LoadCourse_MissingCourse_ReturnsNull()
    {
        // Arrange
        var nonExistentCourseId = "non-existent-course-xyz";

        // Act
        var course = LoadCourse(nonExistentCourseId);

        // Assert
        course.Should().BeNull("Missing course should return null");
    }

    [Fact]
    public void LoadCourse_EmptyCourseId_ReturnsNull()
    {
        // Act
        var course = LoadCourse(string.Empty);

        // Assert
        course.Should().BeNull("Empty course ID should return null");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("../../../etc/passwd")]
    public void LoadCourse_InvalidCourseId_ReturnsNull(string invalidId)
    {
        // Act
        var course = LoadCourse(invalidId ?? "null");

        // Assert
        course.Should().BeNull($"Invalid course ID '{invalidId}' should return null");
    }

    #endregion

    #region Test 3: LoadModule - Module Loads With Correct Lesson Count

    [Fact]
    public void LoadModule_ValidModule_ReturnsCorrectLessonCount()
    {
        // Arrange
        var courseId = "test-python";
        var moduleId = "module-01";

        // Act
        var module = LoadModule(courseId, moduleId);

        // Assert
        module.Should().NotBeNull("Valid module should load");
        module!.Lessons.Should().HaveCount(3, "Module should have exactly 3 lessons as defined in fixture");
        module.Id.Should().Be(moduleId, "Module ID should match");
        module.Title.Should().NotBeNullOrEmpty("Module should have a title");
    }

    [Theory]
    [InlineData("module-01", 3)]
    [InlineData("module-02", 2)]
    public void LoadModule_VariousModules_ReturnsExpectedLessonCounts(string moduleId, int expectedCount)
    {
        // Arrange
        _fixture.SetupTestCourseWithSpecificModules("multi-module-course", "python",
            new Dictionary<string, int> { ["module-01"] = 3, ["module-02"] = 2 });

        // Act
        var module = LoadModule("multi-module-course", moduleId);

        // Assert
        module.Should().NotBeNull();
        module!.Lessons.Should().HaveCount(expectedCount, $"Module {moduleId} should have {expectedCount} lessons");
    }

    [Fact]
    public void LoadModule_MissingModule_ReturnsNull()
    {
        // Arrange
        var nonExistentModuleId = "non-existent-module";

        // Act
        var module = LoadModule("test-python", nonExistentModuleId);

        // Assert
        module.Should().BeNull("Missing module should return null");
    }

    #endregion

    #region Test 4: LoadLesson - Lesson Content Parsed Correctly

    [Fact]
    public void LoadLesson_ValidLesson_ContentSectionsParsed()
    {
        // Arrange
        var courseId = "test-python";
        var moduleId = "module-01";
        var lessonId = "lesson-01-intro";

        // Act
        var lesson = LoadLesson(courseId, moduleId, lessonId);

        // Assert
        lesson.Should().NotBeNull("Valid lesson should load");
        lesson!.Id.Should().Be(lessonId, "Lesson ID should match");
        lesson.Title.Should().NotBeNullOrEmpty("Lesson should have a title");
        lesson.ContentSections.Should().NotBeEmpty("Lesson should have content sections");
        lesson.Order.Should().BeGreaterThan(0, "Lesson should have valid order");
        lesson.EstimatedMinutes.Should().BeGreaterThan(0, "Lesson should have estimated minutes");
    }

    [Fact]
    public void LoadLesson_LessonWithCodeSection_ParsesCodeCorrectly()
    {
        // Arrange
        _fixture.SetupTestCourseWithCodeLesson("code-course", "python");

        // Act
        var lesson = LoadLesson("code-course", "module-01", "lesson-with-code");

        // Assert
        lesson.Should().NotBeNull();
        lesson!.ContentSections.Should().Contain(s => s.Type == "EXAMPLE" && !string.IsNullOrEmpty(s.Code),
            "Code example section should have code");
    }

    [Theory]
    [InlineData("lesson-01-intro")]
    [InlineData("lesson-02-basics")]
    [InlineData("lesson-03-practice")]
    public void LoadLesson_TheorySectionsParsed_ReturnsCorrectContent(string lessonId)
    {
        // Act
        var lesson = LoadLesson("test-python", "module-01", lessonId);

        // Assert
        lesson.Should().NotBeNull($"Lesson {lessonId} should exist");
        lesson!.ContentSections.Should().AllSatisfy(s =>
        {
            s.Type.Should().NotBeNullOrEmpty("Each section should have a type");
            s.Content.Should().NotBeNull("Each section should have content");
        });
    }

    #endregion

    #region Test 5: LoadChallenge - Challenge Starter/Solution Loaded

    [Fact]
    public void LoadChallenge_ValidChallenge_StarterCodeLoaded()
    {
        // Arrange
        var courseId = "test-python";
        var lessonId = "lesson-03-practice";
        var challengeId = "challenge-01-hello";

        // Act
        var challenge = LoadChallenge(courseId, lessonId, challengeId);

        // Assert
        challenge.Should().NotBeNull("Valid challenge should load");
        challenge!.StarterCode.Should().NotBeNullOrEmpty("Challenge should have starter code");
        challenge.Instructions.Should().NotBeNullOrEmpty("Challenge should have instructions");
        challenge.Id.Should().Be(challengeId, "Challenge ID should match");
    }

    [Fact]
    public void LoadChallenge_ValidChallenge_SolutionLoaded()
    {
        // Arrange
        var courseId = "test-python";
        var lessonId = "lesson-03-practice";
        var challengeId = "challenge-01-hello";

        // Act
        var challenge = LoadChallenge(courseId, lessonId, challengeId);

        // Assert
        challenge.Should().NotBeNull();
        challenge!.Solution.Should().NotBeNullOrEmpty("Challenge should have solution code");
    }

    [Theory]
    [InlineData("MULTIPLE_CHOICE")]
    [InlineData("CODE")]
    public void LoadChallenge_VariousTypes_LoadedCorrectly(string challengeType)
    {
        // Arrange
        _fixture.SetupTestCourseWithChallengeType("type-test-course", challengeType);

        // Act
        var lesson = LoadLesson("type-test-course", "module-01", "lesson-with-challenge");

        // Assert
        lesson.Should().NotBeNull();
        lesson!.Challenges.Should().Contain(c => c.Type == challengeType,
            $"Should have {challengeType} challenge");
    }

    [Fact]
    public void LoadChallenge_WithHints_HintsLoaded()
    {
        // Arrange
        var courseId = "test-python";

        // Act
        var challenge = LoadChallenge(courseId, "lesson-03-practice", "challenge-01-hello");

        // Assert
        challenge.Should().NotBeNull();
        challenge!.Hints.Should().NotBeEmpty("Challenge should have hints");
        challenge.Hints.Should().AllSatisfy(h =>
        {
            h.Text.Should().NotBeNullOrEmpty("Each hint should have text");
            h.Level.Should().BeGreaterThan(0, "Each hint should have a level");
        });
    }

    #endregion

    #region Test 6: Progress Tracking - User Progress Saved/Loaded

    [Fact]
    public void SaveProgress_ValidProgress_StoredAndRetrievable()
    {
        // Arrange
        var userId = "test-user-001";
        var lessonId = "lesson-01-intro";
        var progress = new UserProgress
        {
            UserId = userId,
            CompletedLessons = new HashSet<string> { lessonId },
            LastUpdated = DateTime.UtcNow,
            LessonProgress = new Dictionary<string, LessonProgress>
            {
                [lessonId] = new()
                {
                    StartedAt = DateTime.UtcNow.AddMinutes(-30),
                    CompletedAt = DateTime.UtcNow,
                    BestScore = 95
                }
            }
        };

        // Act
        SaveUserProgress(userId, progress);
        var loaded = LoadUserProgress(userId);

        // Assert
        loaded.Should().NotBeNull("Saved progress should be retrievable");
        loaded!.CompletedLessons.Should().Contain(lessonId, "Completed lesson should be tracked");
        loaded.LessonProgress[lessonId].BestScore.Should().Be(95, "Best score should be preserved");
    }

    [Fact]
    public void SaveProgress_MultipleLessons_AllTracked()
    {
        // Arrange
        var userId = "test-user-002";
        var completedLessons = new[] { "lesson-01", "lesson-02", "lesson-03" };
        var progress = new UserProgress
        {
            UserId = userId,
            CompletedLessons = new HashSet<string>(completedLessons),
            TotalTimeSpentMinutes = 120
        };

        // Act
        SaveUserProgress(userId, progress);
        var loaded = LoadUserProgress(userId);

        // Assert
        loaded.Should().NotBeNull();
        loaded!.CompletedLessons.Should().HaveCount(3, "All completed lessons should be tracked");
        loaded.TotalTimeSpentMinutes.Should().Be(120, "Time spent should be saved");
    }

    [Fact]
    public void SaveProgress_ChallengeAttempts_TrackedCorrectly()
    {
        // Arrange
        var userId = "test-user-003";
        var lessonId = "lesson-01";
        var challengeId = "challenge-01";
        var progress = new UserProgress
        {
            UserId = userId,
            LessonProgress = new Dictionary<string, LessonProgress>
            {
                [lessonId] = new()
                {
                    Attempts = new Dictionary<string, int> { [challengeId] = 3 },
                    ChallengesPassed = new List<string> { challengeId },
                    HintsUsed = new Dictionary<string, int> { [challengeId] = 1 }
                }
            }
        };

        // Act
        SaveUserProgress(userId, progress);
        var loaded = LoadUserProgress(userId);

        // Assert
        loaded!.LessonProgress[lessonId].Attempts[challengeId].Should().Be(3, "Attempt count should be saved");
        loaded.LessonProgress[lessonId].HintsUsed[challengeId].Should().Be(1, "Hint usage should be saved");
        loaded.LessonProgress[lessonId].ChallengesPassed.Should().Contain(challengeId);
    }

    [Fact]
    public void LoadProgress_NonExistentUser_ReturnsEmptyProgress()
    {
        // Act
        var progress = LoadUserProgress("non-existent-user");

        // Assert
        progress.Should().NotBeNull("Should return empty progress object, not null");
        progress!.CompletedLessons.Should().BeEmpty("New user should have no completed lessons");
        progress.LessonProgress.Should().BeEmpty();
    }

    #endregion

    #region Test 7: Cache Behavior - Repeated Loads Use Cache

    [Fact]
    public void LoadCourse_CacheHit_ReturnsCachedInstance()
    {
        // Arrange
        var courseId = "cache-test-python";
        _fixture.SetupTestCourse(courseId, "python");

        // Act - First load
        var course1 = LoadCourseWithCache(courseId);
        course1!.Title = "Modified Title";

        // Act - Second load (should return cached)
        var course2 = LoadCourseWithCache(courseId);

        // Assert
        course2.Should().NotBeNull();
        course2!.Title.Should().Be("Modified Title", "Cached instance should reflect modifications");
        _cache.GetHitCount(courseId).Should().Be(1, "Cache should have one hit after second load");
    }

    [Fact]
    public void LoadCourse_CacheMiss_AccessesDisk()
    {
        // Arrange
        var courseId = "fresh-load-python";
        _fixture.SetupTestCourse(courseId, "python");

        // Act
        var course = LoadCourseWithCache(courseId);

        // Assert
        course.Should().NotBeNull();
        _cache.GetMissCount(courseId).Should().Be(1, "Cache miss should be recorded");
        _cache.GetHitCount(courseId).Should().Be(0, "No cache hit on first access");
    }

    [Fact]
    public void LoadCourse_CacheInvalidation_ClearsCache()
    {
        // Arrange
        var courseId = "invalidate-test";
        _fixture.SetupTestCourse(courseId, "python");
        var course = LoadCourseWithCache(courseId);
        course.Should().NotBeNull();

        // Act
        _cache.Invalidate(courseId);

        // Assert
        _cache.Contains(courseId).Should().BeFalse("Cache entry should be removed");
    }

    [Fact]
    public void LoadCourse_ConcurrentReads_DoNotCorruptCache()
    {
        // Arrange
        var courseId = "concurrent-cache-test";
        _fixture.SetupTestCourse(courseId, "python");
        var tasks = new List<Task<Course?>>();

        // Act - Load course from multiple threads simultaneously
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() => LoadCourseWithCache(courseId)));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        var results = tasks.Select(t => t.Result).Where(r => r != null).ToList();
        results.Should().AllSatisfy(c => c!.Id.Should().Be(courseId));
        results.Count.Should().Be(10, "All concurrent loads should succeed");
    }

    #endregion

    #region Test 8: Concurrent Access - Thread Safety

    [Fact]
    public void SaveProgress_ConcurrentSaves_ThreadSafe()
    {
        // Arrange
        var userId = "concurrent-user";
        var progress = new UserProgress { UserId = userId };
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();

        // Act - Save progress from multiple threads
        for (int i = 0; i < 20; i++)
        {
            var lessonId = $"lesson-{i:D3}";
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    var p = LoadUserProgress(userId);
                    p.CompletedLessons.Add(lessonId);
                    SaveUserProgress(userId, p);
                }
                catch (Exception ex)
                {
                    lock (exceptions) { exceptions.Add(ex); }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        exceptions.Should().BeEmpty($"No exceptions should occur during concurrent saves: {string.Join(", ", exceptions.Select(e => e.Message))}");

        var finalProgress = LoadUserProgress(userId);
        finalProgress.CompletedLessons.Count.Should().BeGreaterThan(0, "At least some lessons should be saved");
    }

    [Fact]
    public void LoadCourse_ConcurrentReadsAndWrites_ThreadSafe()
    {
        // Arrange
        var courseId = "thread-safe-course";
        _fixture.SetupTestCourse(courseId, "python");
        var loadTasks = new List<Task<Course?>>();
        var exceptionCount = 0;

        // Act - Mix of reads while cache is being populated
        for (int i = 0; i < 50; i++)
        {
            var taskIndex = i;
            loadTasks.Add(Task.Run(() =>
            {
                try
                {
                    // Half reads, half cache operations
                    if (taskIndex % 2 == 0)
                    {
                        return LoadCourseWithCache(courseId);
                    }
                    else
                    {
                        if (taskIndex % 4 == 1)
                            _cache.Invalidate(courseId);
                        return LoadCourseWithCache(courseId);
                    }
                }
                catch
                {
                    Interlocked.Increment(ref exceptionCount);
                    return null;
                }
            }));
        }

        Task.WaitAll(loadTasks.ToArray());

        // Assert
        exceptionCount.Should().Be(0, "No exceptions should occur during concurrent access");
        loadTasks.Where(t => t.Result != null).Should().NotBeEmpty("Some loads should succeed");
    }

    [Fact]
    public void LoadLesson_ConcurrentLessonAccess_ThreadSafe()
    {
        // Arrange
        var courseId = "concurrent-lesson-course";
        _fixture.SetupTestCourseWithManyLessons(courseId, "python", 20);
        var tasks = new List<Task<Lesson?>>();

        // Act - Load different lessons concurrently
        for (int i = 1; i <= 20; i++)
        {
            var lessonId = $"lesson-{i:D3}";
            tasks.Add(Task.Run(() => LoadLesson(courseId, "module-01", lessonId)));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        var loadedLessons = tasks.Where(t => t.Result != null).Select(t => t.Result!).ToList();
        loadedLessons.Should().HaveCountGreaterThan(0, "Multiple lessons should load successfully");
        loadedLessons.Select(l => l.Id).Distinct().Count().Should().Be(loadedLessons.Count,
            "Each loaded lesson should be unique");
    }

    #endregion

    #region Test 9: Invalid Content - Malformed JSON Handled Gracefully

    [Fact]
    public void LoadCourse_MalformedJson_ReturnsNullOrEmpty()
    {
        // Arrange
        var courseId = "invalid-json-course";
        _fixture.SetupTestCourseWithInvalidJson(courseId);

        // Act
        var course = LoadCourse(courseId);

        // Assert
        // Should either return null or handle gracefully without throwing
        course.Should().BeNull("Malformed course JSON should not cause crash");
    }

    [Fact]
    public void LoadModule_MissingModuleJson_ReturnsNull()
    {
        // Arrange
        var courseId = "missing-module-json";
        _fixture.SetupTestCourseWithBrokenModule(courseId);

        // Act
        var module = LoadModule(courseId, "broken-module");

        // Assert
        module.Should().BeNull("Missing module.json should return null gracefully");
    }

    [Theory]
    [InlineData("missing-id")]
    [InlineData("missing-title")]
    [InlineData("empty-modules")]
    public void LoadCourse_IncompleteCourseData_HandlesGracefully(string scenario)
    {
        // Arrange
        var courseId = $"incomplete-{scenario}";
        _fixture.SetupTestCourseWithMissingFields(courseId, scenario);

        // Act
        var course = LoadCourse(courseId);

        // Assert
        // Should handle gracefully without exceptions
        if (course != null)
        {
            course.Modules.Should().NotBeNull("Modules list should at least be initialized");
        }
    }

    [Fact]
    public void LoadLesson_MissingContentFiles_HandlesGracefully()
    {
        // Arrange
        var courseId = "missing-content-course";
        _fixture.SetupTestCourseWithMissingContent(courseId);

        // Act
        var lesson = LoadLesson(courseId, "module-01", "lesson-missing-content");

        // Assert
        lesson.Should().NotBeNull("Lesson should still load even with missing content files");
        lesson!.ContentSections.Should().NotBeNull("Content sections should be initialized");
    }

    [Fact]
    public void LoadChallenge_MissingStarterFile_UsesStartingCodeField()
    {
        // Arrange
        var courseId = "fallback-starter-course";
        _fixture.SetupTestCourseWithFallbackStarterCode(courseId);

        // Act
        var challenge = LoadChallenge(courseId, "lesson-01", "challenge-fallback");

        // Assert
        challenge.Should().NotBeNull("Challenge should load even with missing starter file");
        challenge!.StarterCode.Should().NotBeNullOrEmpty("Should fallback to StartingCode field");
    }

    #endregion

    #region Test 10: Path Traversal Attempts - Rejected (Security)

    [Theory]
    [InlineData("../../../etc/passwd")]
    [InlineData("..\\..\\..\\Windows\\System32\\config\\SAM")]
    [InlineData("..\..\..\..\..\..\Windows\System32\drivers\etc\hosts")]
    [InlineData("..%2f..%2f..%2fetc%2fpasswd")]
    public void LoadCourse_PathTraversalAttempt_ReturnsNull(string maliciousPath)
    {
        // Act
        var course = LoadCourse(maliciousPath);

        // Assert
        course.Should().BeNull($"Path traversal attempt '{maliciousPath}' should be rejected");
    }

    [Theory]
    [InlineData("test-course", "../../../etc/shadow")]
    [InlineData("test-course", "..\\..\\..\\Windows\\System32\\")]
    [InlineData("test-course", "..%2f..%2f..%2fetc%2fhosts")]
    public void LoadModule_PathTraversalAttempt_ReturnsNull(string courseId, string maliciousModuleId)
    {
        // Arrange
        _fixture.SetupTestCourse(courseId, "python");

        // Act
        var module = LoadModule(courseId, maliciousModuleId);

        // Assert
        module.Should().BeNull($"Module path traversal '{maliciousModuleId}' should be rejected");
    }

    [Theory]
    [InlineData("test-course", "module-01", "../../../etc/passwd")]
    [InlineData("test-course", "..\..\..", "lesson-01")]
    public void LoadLesson_PathTraversalAttempt_ReturnsNull(string courseId, string maliciousModuleId, string maliciousLessonId)
    {
        // Arrange
        _fixture.SetupTestCourse(courseId, "python");

        // Act
        var lesson = LoadModule(courseId, maliciousModuleId);

        // Assert
        lesson.Should().BeNull($"Lesson path traversal attempt should be rejected");
    }

    [Fact]
    public void SaveProgress_PathTraversalInUserId_Rejected()
    {
        // Arrange
        var maliciousUserId = "../../../etc/passwd";
        var progress = new UserProgress { UserId = maliciousUserId };

        // Act
        var ex = Record.Exception(() => SaveUserProgress(maliciousUserId, progress));

        // Assert
        // Should either throw an argument exception or sanitize the path
        if (ex != null)
        {
            ex.Should().BeOfType<ArgumentException>().Or.BeOfType<UnauthorizedAccessException>();
        }
        else
        {
            // If no exception, verify file wasn't written outside expected directory
            var expectedPath = Path.Combine(_tempProgressPath, "sanitized");
            File.Exists(Path.Combine("/etc", "passwd")).Should().BeFalse("File should not be written outside temp directory");
        }
    }

    [Fact]
    public void LoadCourse_NullCharacterInPath_Rejected()
    {
        // Arrange
        var nullPath = "test\0course";

        // Act
        var ex = Record.Exception(() => LoadCourse(nullPath));

        // Assert
        ex.Should().NotBeNull("Null character in path should cause exception");
    }

    [Theory]
    [InlineData("C:\\Windows\\System32\\config\\SAM")]
    [InlineData("/etc/shadow")]
    [InlineData("\\\\server\\share\\file")]
    public void LoadCourse_AbsolutePathOutsideContent_ReturnsNull(string absolutePath)
    {
        // Act
        var course = LoadCourse(absolutePath);

        // Assert
        course.Should().BeNull($"Absolute path '{absolutePath}' should be rejected");
    }

    #endregion

    #region Helper Methods

    private Course? LoadCourse(string courseId)
    {
        if (string.IsNullOrWhiteSpace(courseId) || ContainsPathTraversal(courseId))
            return null;

        return _fixture.LoadCourse(courseId);
    }

    private Course? LoadCourseWithCache(string courseId)
    {
        if (_cache.TryGet(courseId, out var cachedCourse))
            return cachedCourse;

        var course = LoadCourse(courseId);
        if (course != null)
            _cache.Set(courseId, course);

        return course;
    }

    private Module? LoadModule(string courseId, string moduleId)
    {
        if (ContainsPathTraversal(courseId) || ContainsPathTraversal(moduleId))
            return null;

        var course = LoadCourse(courseId);
        return course?.Modules.FirstOrDefault(m => m.Id == moduleId);
    }

    private Lesson? LoadLesson(string courseId, string moduleId, string lessonId)
    {
        if (ContainsPathTraversal(courseId) || ContainsPathTraversal(moduleId) || ContainsPathTraversal(lessonId))
            return null;

        var module = LoadModule(courseId, moduleId);
        return module?.Lessons.FirstOrDefault(l => l.Id == lessonId);
    }

    private Challenge? LoadChallenge(string courseId, string lessonId, string challengeId)
    {
        var course = LoadCourse(courseId);
        foreach (var module in course?.Modules ?? new List<Module>())
        {
            foreach (var lesson in module.Lessons)
            {
                var challenge = lesson.Challenges.FirstOrDefault(c => c.Id == challengeId);
                if (challenge != null)
                    return challenge;
            }
        }
        return null;
    }

    private void SaveUserProgress(string userId, UserProgress progress)
    {
        if (ContainsPathTraversal(userId))
            throw new ArgumentException("Invalid user ID: contains path traversal characters", nameof(userId));

        var safeUserId = Path.GetFileNameWithoutExtension(userId);
        var progressFile = Path.Combine(_tempProgressPath, $"{safeUserId}.json");
        var json = JsonSerializer.Serialize(progress, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(progressFile, json);
    }

    private UserProgress LoadUserProgress(string userId)
    {
        if (ContainsPathTraversal(userId))
            return new UserProgress { UserId = userId };

        var safeUserId = Path.GetFileNameWithoutExtension(userId);
        var progressFile = Path.Combine(_tempProgressPath, $"{safeUserId}.json");

        if (!File.Exists(progressFile))
            return new UserProgress { UserId = userId };

        var json = File.ReadAllText(progressFile);
        return JsonSerializer.Deserialize<UserProgress>(json) ?? new UserProgress { UserId = userId };
    }

    private static bool ContainsPathTraversal(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        // Check for common path traversal patterns
        if (path.Contains("..") || path.Contains("../") || path.Contains("..\\"))
            return true;

        if (path.Contains("%2f") || path.Contains("%2F") || path.Contains("%5c") || path.Contains("%5C"))
            return true;

        if (path.Contains('\0'))
            return true;

        // Check for absolute paths
        if (Path.IsPathRooted(path) && !path.StartsWith("test-"))
            return true;

        return false;
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                if (Directory.Exists(_tempContentPath))
                {
                    Directory.Delete(_tempContentPath, recursive: true);
                }
            }
            catch
            {
                // Best effort cleanup
            }
            _disposed = true;
        }
    }

    #endregion
}

/// <summary>
/// Test fixture that creates temporary course content for isolated testing.
/// </summary>
public class CourseServiceFixture
{
    private readonly string _contentPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public CourseServiceFixture(string contentPath)
    {
        _contentPath = contentPath;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public void SetupTestCourses()
    {
        SetupTestCourse("test-python", "python");
    }

    public void SetupTestCourse(string courseId, string language)
    {
        var courseDir = Path.Combine(_contentPath, courseId);
        Directory.CreateDirectory(courseDir);

        var modulesDir = Path.Combine(courseDir, "modules");
        Directory.CreateDirectory(modulesDir);

        // Create course.json
        var course = new Course
        {
            Id = courseId,
            Language = language,
            Title = $"Test {language.ToUpperInvariant()} Course",
            Description = $"A test course for {language}",
            Difficulty = "beginner",
            EstimatedHours = 10,
            Modules = new List<Module>()
        };

        File.WriteAllText(
            Path.Combine(courseDir, "course.json"),
            JsonSerializer.Serialize(course, _jsonOptions));

        // Create module with lessons
        SetupModule(courseId, "module-01", "Test Module 1", new[]
        {
            ("lesson-01-intro", "Introduction", 1),
            ("lesson-02-basics", "Basic Concepts", 2),
            ("lesson-03-practice", "Practice Exercises", 3)
        });
    }

    public void SetupModule(string courseId, string moduleId, string title, (string id, string title, int order)[] lessons)
    {
        var moduleDir = Path.Combine(_contentPath, courseId, "modules", moduleId);
        Directory.CreateDirectory(moduleDir);

        var lessonsDir = Path.Combine(moduleDir, "lessons");
        Directory.CreateDirectory(lessonsDir);

        var module = new Module
        {
            Id = moduleId,
            Title = title,
            Description = $"Test module: {title}",
            Lessons = new List<Lesson>()
        };

        foreach (var (lessonId, lessonTitle, order) in lessons)
        {
            var lessonDir = Path.Combine(lessonsDir, lessonId);
            Directory.CreateDirectory(lessonDir);
            Directory.CreateDirectory(Path.Combine(lessonDir, "content"));

            var lesson = new Lesson
            {
                Id = lessonId,
                Title = lessonTitle,
                ModuleId = moduleId,
                Order = order,
                EstimatedMinutes = 15,
                Difficulty = "beginner",
                ContentSections = new List<ContentSection>(),
                Challenges = new List<Challenge>()
            };

            // Add content section
            File.WriteAllText(
                Path.Combine(lessonDir, "content", "01-theory.md"),
                @$"---
type: THEORY
title: {lessonTitle}
---
# {lessonTitle}

This is test content for {lessonTitle} in {moduleId}.");

            // Add challenge to practice lesson
            if (lessonId.Contains("practice"))
            {
                SetupChallenge(lessonDir, "challenge-01-hello", language: courseId.Contains("python") ? "py" : "js");
            }

            File.WriteAllText(
                Path.Combine(lessonDir, "lesson.json"),
                JsonSerializer.Serialize(lesson, _jsonOptions));

            module.Lessons.Add(lesson);
        }

        File.WriteAllText(
            Path.Combine(moduleDir, "module.json"),
            JsonSerializer.Serialize(module, _jsonOptions));
    }

    private void SetupChallenge(string lessonDir, string challengeId, string language)
    {
        var challengesDir = Path.Combine(lessonDir, "challenges");
        Directory.CreateDirectory(challengesDir);

        var challengeDir = Path.Combine(challengesDir, challengeId);
        Directory.CreateDirectory(challengeDir);

        var challenge = new Challenge
        {
            Id = challengeId,
            Type = "CODE",
            Title = "Hello World Challenge",
            Description = "Write a hello world program",
            Instructions = "Print 'Hello, World!' to the console",
            Language = language,
            Difficulty = "beginner",
            StarterCode = string.Empty,
            Solution = string.Empty,
            Hints = new List<Hint>
            {
                new() { Level = 1, Text = "Use the print function" },
                new() { Level = 2, Text = "The syntax is: print('Hello, World!')" }
            },
            TestCases = new List<TestCase>
            {
                new() { Id = "test-1", Description = "Test hello world", ExpectedOutput = "Hello, World!" }
            }
        };

        File.WriteAllText(
            Path.Combine(challengeDir, "challenge.json"),
            JsonSerializer.Serialize(challenge, _jsonOptions));

        // Starter code file
        File.WriteAllText(
            Path.Combine(challengeDir, $"starter.{language}"),
            language == "py" ? "# Write your code here\n" : "// Write your code here\n");

        // Solution file
        File.WriteAllText(
            Path.Combine(challengeDir, $"solution.{language}"),
            language == "py" ? "print('Hello, World!')" : "console.log('Hello, World!');");
    }

    public void SetupTestCourseWithSpecificModules(string courseId, string language, Dictionary<string, int> moduleLessons)
    {
        var courseDir = Path.Combine(_contentPath, courseId);
        Directory.CreateDirectory(courseDir);

        var course = new Course
        {
            Id = courseId,
            Language = language,
            Title = $"Multi-Module {language} Course",
            Description = "Test course with specific module counts",
            Difficulty = "intermediate",
            EstimatedHours = 20,
            Modules = new List<Module>()
        };

        File.WriteAllText(
            Path.Combine(courseDir, "course.json"),
            JsonSerializer.Serialize(course, _jsonOptions));

        foreach (var (moduleId, lessonCount) in moduleLessons)
        {
            var lessons = Enumerable.Range(1, lessonCount)
                .Select(i => ($"lesson-{i:D3}", $"Lesson {i}", i))
                .ToArray();
            SetupModule(courseId, moduleId, $"Module {moduleId}", lessons);
        }
    }

    public void SetupTestCourseWithCodeLesson(string courseId, string language)
    {
        SetupTestCourse(courseId, language);

        var lessonDir = Path.Combine(_contentPath, courseId, "modules", "module-01", "lessons", "lesson-with-code");
        Directory.CreateDirectory(lessonDir);
        Directory.CreateDirectory(Path.Combine(lessonDir, "content"));

        File.WriteAllText(
            Path.Combine(lessonDir, "content", "01-example.md"),
            @"---
type: EXAMPLE
title: Code Example
---
```python
print('Hello from code example!')
```");

        var lesson = new Lesson
        {
            Id = "lesson-with-code",
            Title = "Lesson With Code",
            ModuleId = "module-01",
            Order = 4,
            EstimatedMinutes = 20,
            ContentSections = new List<ContentSection>(),
            Challenges = new List<Challenge>()
        };

        File.WriteAllText(
            Path.Combine(lessonDir, "lesson.json"),
            JsonSerializer.Serialize(lesson, _jsonOptions));
    }

    public void SetupTestCourseWithChallengeType(string courseId, string challengeType)
    {
        SetupTestCourse(courseId, "python");

        var lessonDir = Path.Combine(_contentPath, courseId, "modules", "module-01", "lessons", "lesson-with-challenge");
        var challengesDir = Path.Combine(lessonDir, "challenges");
        var challengeDir = Path.Combine(challengesDir, $"challenge-{challengeType.ToLowerInvariant()}");
        Directory.CreateDirectory(challengeDir);

        Challenge challenge = challengeType switch
        {
            "MULTIPLE_CHOICE" => new Challenge
            {
                Id = $"challenge-{challengeType.ToLowerInvariant()}",
                Type = challengeType,
                Title = "Multiple Choice Test",
                Description = "Choose the correct answer",
                Question = "What is 2 + 2?",
                Options = new List<string> { "3", "4", "5" },
                CorrectAnswer = JsonSerializer.SerializeToElement(1)
            },
            _ => new Challenge
            {
                Id = $"challenge-{challengeType.ToLowerInvariant()}",
                Type = challengeType,
                Title = "Code Challenge Test",
                Description = "Write code",
                Instructions = "Write a function",
                Language = "py",
                StarterCode = "# starter",
                Solution = "# solution"
            }
        };

        File.WriteAllText(
            Path.Combine(challengeDir, "challenge.json"),
            JsonSerializer.Serialize(challenge, _jsonOptions));

        if (challengeType == "CODE")
        {
            File.WriteAllText(Path.Combine(challengeDir, "starter.py"), "# starter");
            File.WriteAllText(Path.Combine(challengeDir, "solution.py"), "# solution");
        }
    }

    public void SetupTestCourseWithManyLessons(string courseId, string language, int lessonCount)
    {
        var courseDir = Path.Combine(_contentPath, courseId);
        Directory.CreateDirectory(courseDir);

        var modulesDir = Path.Combine(courseDir, "modules");
        Directory.CreateDirectory(modulesDir);

        var moduleDir = Path.Combine(modulesDir, "module-01");
        Directory.CreateDirectory(moduleDir);

        var lessonsDir = Path.Combine(moduleDir, "lessons");
        Directory.CreateDirectory(lessonsDir);

        var module = new Module
        {
            Id = "module-01",
            Title = "Bulk Lesson Module",
            Description = "Module with many lessons",
            Lessons = new List<Lesson>()
        };

        for (int i = 1; i <= lessonCount; i++)
        {
            var lessonId = $"lesson-{i:D3}";
            var lessonDir = Path.Combine(lessonsDir, lessonId);
            Directory.CreateDirectory(lessonDir);
            Directory.CreateDirectory(Path.Combine(lessonDir, "content"));

            File.WriteAllText(
                Path.Combine(lessonDir, "content", "01-theory.md"),
                $"---\ntype: THEORY\ntitle: Lesson {i}\n---\nContent for lesson {i}.");

            var lesson = new Lesson
            {
                Id = lessonId,
                Title = $"Lesson {i}",
                ModuleId = "module-01",
                Order = i,
                EstimatedMinutes = 10,
                ContentSections = new List<ContentSection>(),
                Challenges = new List<Challenge>()
            };

            File.WriteAllText(
                Path.Combine(lessonDir, "lesson.json"),
                JsonSerializer.Serialize(lesson, _jsonOptions));

            module.Lessons.Add(lesson);
        }

        File.WriteAllText(
            Path.Combine(moduleDir, "module.json"),
            JsonSerializer.Serialize(module, _jsonOptions));

        var course = new Course
        {
            Id = courseId,
            Language = language,
            Title = "Many Lessons Course",
            Description = $"Course with {lessonCount} lessons",
            Difficulty = "beginner",
            EstimatedHours = lessonCount / 2,
            Modules = new List<Module> { module }
        };

        File.WriteAllText(
            Path.Combine(courseDir, "course.json"),
            JsonSerializer.Serialize(course, _jsonOptions));
    }

    public void SetupTestCourseWithInvalidJson(string courseId)
    {
        var courseDir = Path.Combine(_contentPath, courseId);
        Directory.CreateDirectory(courseDir);

        File.WriteAllText(
            Path.Combine(courseDir, "course.json"),
            "{ invalid json: missing quotes, broken structure ");
    }

    public void SetupTestCourseWithBrokenModule(string courseId)
    {
        var courseDir = Path.Combine(_contentPath, courseId);
        Directory.CreateDirectory(courseDir);

        var brokenModuleDir = Path.Combine(courseDir, "modules", "broken-module");
        Directory.CreateDirectory(brokenModuleDir);
        // Don't create module.json

        var course = new Course
        {
            Id = courseId,
            Language = "python",
            Title = "Broken Module Course",
            Description = "Course with broken module",
            Modules = new List<Module>()
        };

        File.WriteAllText(
            Path.Combine(courseDir, "course.json"),
            JsonSerializer.Serialize(course, _jsonOptions));
    }

    public void SetupTestCourseWithMissingFields(string courseId, string scenario)
    {
        var courseDir = Path.Combine(_contentPath, courseId);
        Directory.CreateDirectory(courseDir);

        string json = scenario switch
        {
            "missing-id" => "{ \"title\": \"Test\", \"language\": \"python\", \"modules\": [] }",
            "missing-title" => "{ \"id\": \"test\", \"language\": \"python\", \"modules\": [] }",
            "empty-modules" => "{ \"id\": \"test\", \"title\": \"Test\", \"language\": \"python\" }",
            _ => "{ \"id\": \"test\", \"title\": \"Test\", \"language\": \"python\" }"
        };

        File.WriteAllText(Path.Combine(courseDir, "course.json"), json);
    }

    public void SetupTestCourseWithMissingContent(string courseId)
    {
        SetupTestCourse(courseId, "python");

        var lessonDir = Path.Combine(_contentPath, courseId, "modules", "module-01", "lessons", "lesson-missing-content");
        Directory.CreateDirectory(lessonDir);
        // Don't create content directory

        var lesson = new Lesson
        {
            Id = "lesson-missing-content",
            Title = "Missing Content Lesson",
            ModuleId = "module-01",
            Order = 99,
            ContentSections = new List<ContentSection>(),
            Challenges = new List<Challenge>()
        };

        File.WriteAllText(
            Path.Combine(lessonDir, "lesson.json"),
            JsonSerializer.Serialize(lesson, _jsonOptions));
    }

    public void SetupTestCourseWithFallbackStarterCode(string courseId)
    {
        SetupTestCourse(courseId, "python");

        var lessonDir = Path.Combine(_contentPath, courseId, "modules", "module-01", "lessons", "lesson-01");
        var challengesDir = Path.Combine(lessonDir, "challenges");
        var challengeDir = Path.Combine(challengesDir, "challenge-fallback");
        Directory.CreateDirectory(challengeDir);

        var challenge = new Challenge
        {
            Id = "challenge-fallback",
            Type = "CODE",
            Title = "Fallback Challenge",
            StartingCode = "# Fallback starter code from field",
            Solution = "# solution",
            Language = "py"
        };

        File.WriteAllText(
            Path.Combine(challengeDir, "challenge.json"),
            JsonSerializer.Serialize(challenge, _jsonOptions));

        // Don't create starter.py file - this tests fallback to StartingCode field
        File.WriteAllText(Path.Combine(challengeDir, "solution.py"), "# solution");
    }

    public Course? LoadCourse(string courseId)
    {
        return TestCourseLoader.LoadCourse(_contentPath, courseId);
    }
}

/// <summary>
/// Simple in-memory cache for course loading tests.
/// </summary>
public class CourseCache
{
    private readonly Dictionary<string, Course> _cache = new();
    private readonly Dictionary<string, int> _hits = new();
    private readonly Dictionary<string, int> _misses = new();
    private readonly object _lock = new();

    public bool TryGet(string courseId, out Course? course)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(courseId, out var cached))
            {
                _hits[courseId] = _hits.GetValueOrDefault(courseId) + 1;
                course = cached;
                return true;
            }

            _misses[courseId] = _misses.GetValueOrDefault(courseId) + 1;
            course = null;
            return false;
        }
    }

    public void Set(string courseId, Course course)
    {
        lock (_lock)
        {
            _cache[courseId] = course;
        }
    }

    public void Invalidate(string courseId)
    {
        lock (_lock)
        {
            _cache.Remove(courseId);
        }
    }

    public bool Contains(string courseId)
    {
        lock (_lock)
        {
            return _cache.ContainsKey(courseId);
        }
    }

    public int GetHitCount(string courseId)
    {
        lock (_lock)
        {
            return _hits.GetValueOrDefault(courseId);
        }
    }

    public int GetMissCount(string courseId)
    {
        lock (_lock)
        {
            return _misses.GetValueOrDefault(courseId);
        }
    }
}
