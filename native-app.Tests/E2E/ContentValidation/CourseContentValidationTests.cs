using System.Text.Json;
using CodeTutor.Tests.Models;

namespace CodeTutor.Tests.E2E.ContentValidation;

/// <summary>
/// Comprehensive E2E tests for validating all course content JSON files.
/// These tests ensure that all course files are correctly structured and contain valid data.
/// </summary>
public class CourseContentValidationTests
{
    private readonly string _contentPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public CourseContentValidationTests()
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
            Path.Combine(Directory.GetCurrentDirectory(), "..", "Content", "courses"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Content", "courses"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Content", "courses"),
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

    [Fact]
    public void ContentPath_ShouldExist()
    {
        // Assert
        Directory.Exists(_contentPath).Should().BeTrue(
            $"Content path should exist at {_contentPath}");
    }

    [Fact]
    public void AllCourses_ShouldHaveCourseJsonFiles()
    {
        // Arrange
        var courseDirectories = Directory.GetDirectories(_contentPath);

        // Assert
        courseDirectories.Should().NotBeEmpty("At least one course should exist");

        foreach (var dir in courseDirectories)
        {
            var courseFile = Path.Combine(dir, "course.json");
            File.Exists(courseFile).Should().BeTrue(
                $"Course file should exist at {courseFile}");
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void Course_ShouldLoadSuccessfully(string courseId)
    {
        // Arrange
        var courseFile = Path.Combine(_contentPath, courseId, "course.json");

        if (!File.Exists(courseFile))
        {
            // Skip if course doesn't exist in test environment
            return;
        }

        // Act
        var json = File.ReadAllText(courseFile);
        var course = JsonSerializer.Deserialize<Course>(json, _jsonOptions);

        // Assert
        course.Should().NotBeNull($"Course {courseId} should deserialize successfully");
        course!.Id.Should().NotBeNullOrEmpty($"Course {courseId} should have an ID");
        course.Title.Should().NotBeNullOrEmpty($"Course {courseId} should have a title");
        course.Language.Should().NotBeNullOrEmpty($"Course {courseId} should have a language");
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void Course_ShouldHaveValidModules(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Assert
        course.Modules.Should().NotBeEmpty($"Course {courseId} should have at least one module");

        foreach (var module in course.Modules)
        {
            module.Id.Should().NotBeNullOrEmpty($"Module in {courseId} should have an ID");
            module.Title.Should().NotBeNullOrEmpty($"Module {module.Id} should have a title");
            module.Lessons.Should().NotBeEmpty($"Module {module.Id} should have at least one lesson");
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void Course_ShouldHaveValidLessons(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Assert
        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                lesson.Id.Should().NotBeNullOrEmpty($"Lesson in {module.Id} should have an ID");
                lesson.Title.Should().NotBeNullOrEmpty($"Lesson {lesson.Id} should have a title");
                lesson.ContentSections.Should().NotBeEmpty(
                    $"Lesson {lesson.Id} should have at least one content section");
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
    public void Course_ShouldHaveValidContentSections(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        var validSectionTypes = new[] { "THEORY", "EXAMPLE", "KEY_POINT", "LEGACY_COMPARISON", "ANALOGY", "WARNING" };

        // Assert
        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                foreach (var section in lesson.ContentSections)
                {
                    section.Type.Should().NotBeNullOrEmpty(
                        $"Content section in {lesson.Id} should have a type");
                    validSectionTypes.Should().Contain(section.Type,
                        $"Content section type '{section.Type}' in {lesson.Id} should be valid");
                    section.Content.Should().NotBeNullOrEmpty(
                        $"Content section in {lesson.Id} should have content");

                    // EXAMPLE sections should have code
                    if (section.Type == "EXAMPLE")
                    {
                        section.Code.Should().NotBeNullOrEmpty(
                            $"EXAMPLE section in {lesson.Id} should have code");
                    }
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
    public void Course_AllModuleIdsInLessons_ShouldMatchParentModule(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Assert
        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                lesson.ModuleId.Should().Be(module.Id,
                    $"Lesson {lesson.Id} moduleId should match parent module {module.Id}");
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
    public void Course_LessonOrderNumbers_ShouldBeSequential(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Assert
        foreach (var module in course.Modules)
        {
            var orderedLessons = module.Lessons.OrderBy(l => l.Order).ToList();
            for (int i = 0; i < orderedLessons.Count; i++)
            {
                orderedLessons[i].Order.Should().Be(i + 1,
                    $"Lesson order in module {module.Id} should be sequential starting from 1");
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
    public void Course_AllChallengeIds_ShouldBeUnique(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        var allChallengeIds = new HashSet<string>();

        // Act & Assert
        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                foreach (var challenge in lesson.Challenges)
                {
                    allChallengeIds.Add(challenge.Id).Should().BeTrue(
                        $"Challenge ID '{challenge.Id}' in {lesson.Id} should be unique");
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
    public void Course_AllLessonIds_ShouldBeUnique(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        var allLessonIds = new HashSet<string>();

        // Act & Assert
        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                allLessonIds.Add(lesson.Id).Should().BeTrue(
                    $"Lesson ID '{lesson.Id}' should be unique in course {courseId}");
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
    public void Course_EstimatedTime_ShouldBeReasonable(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        // Assert
        course.EstimatedHours.Should().BeGreaterThan(0,
            $"Course {courseId} should have positive estimated hours");
        course.EstimatedHours.Should().BeLessOrEqualTo(200,
            $"Course {courseId} estimated hours should be reasonable (< 200)");

        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                lesson.EstimatedMinutes.Should().BeGreaterOrEqualTo(0,
                    $"Lesson {lesson.Id} should have non-negative estimated minutes");
                lesson.EstimatedMinutes.Should().BeLessOrEqualTo(180,
                    $"Lesson {lesson.Id} should have reasonable estimated time (< 3 hours)");
            }
        }
    }

    [Fact]
    public void AllCourses_TotalLessonCount_ShouldBeSubstantial()
    {
        // Arrange
        var courses = LoadAllCourses();
        var totalLessons = courses.Sum(c => c.TotalLessons);

        // Assert
        totalLessons.Should().BeGreaterThan(50,
            "All courses combined should have substantial content (> 50 lessons)");
    }

    [Fact]
    public void AllCourses_TotalChallengeCount_ShouldBeSubstantial()
    {
        // Arrange
        var courses = LoadAllCourses();
        var totalChallenges = courses.Sum(c => c.TotalChallenges);

        // Assert
        totalChallenges.Should().BeGreaterThan(100,
            "All courses combined should have substantial challenges (> 100)");
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void Course_Difficulty_ShouldBeValid(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        var validDifficulties = new[] { "beginner", "intermediate", "advanced", "beginner-to-advanced" };

        // Assert
        validDifficulties.Should().Contain(course.Difficulty.ToLowerInvariant(),
            $"Course {courseId} should have a valid difficulty level");
    }

    private Course? LoadCourse(string courseId) => TestCourseLoader.LoadCourse(_contentPath, courseId);

    private List<Course> LoadAllCourses() => TestCourseLoader.LoadAllCourses(_contentPath);
}
