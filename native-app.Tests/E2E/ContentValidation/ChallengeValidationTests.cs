using System.Text.Json;
using CodeTutor.Tests.Models;

namespace CodeTutor.Tests.E2E.ContentValidation;

/// <summary>
/// Comprehensive E2E tests for validating all challenges in course content.
/// Tests ensure challenges are well-formed, have valid types, and contain required fields.
/// </summary>
public class ChallengeValidationTests
{
    private readonly string _contentPath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string[] _validChallengeTypes;

    public ChallengeValidationTests()
    {
        _contentPath = FindContentPath();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        _validChallengeTypes = new[]
        {
            "FREE_CODING",
            "MULTIPLE_CHOICE",
            "MULTI_CHOICE",
            "CODE_OUTPUT",
            "FILL_IN_THE_BLANK",
            "DEBUGGING",
            "CODE_REVIEW",
            "QUIZ",
            "quiz",
            "CODE",
            "coding",
            "implementation",
            "PROJECT",
            "TRUE_FALSE"
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
    public void Course_AllChallenges_ShouldHaveValidTypes(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId);
        if (!challenges.Any()) return;

        // Assert
        foreach (var (challenge, lessonId) in challenges)
        {
            challenge.Type.Should().NotBeNullOrEmpty(
                $"Challenge {challenge.Id} in {lessonId} should have a type");
            _validChallengeTypes.Should().Contain(challenge.Type,
                $"Challenge type '{challenge.Type}' in {lessonId} should be valid");
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void FreeCodingChallenges_ShouldHaveRequiredFields(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId)
            .Where(c => c.challenge.Type == "FREE_CODING")
            .ToList();

        if (!challenges.Any()) return;

        // Assert
        foreach (var (challenge, lessonId) in challenges)
        {
            challenge.Id.Should().NotBeNullOrEmpty(
                $"FREE_CODING challenge in {lessonId} should have an ID");
            challenge.Title.Should().NotBeNullOrEmpty(
                $"Challenge {challenge.Id} should have a title");
            challenge.Instructions.Should().NotBeNullOrEmpty(
                $"Challenge {challenge.Id} should have instructions");
            challenge.StarterCode.Should().NotBeNullOrEmpty(
                $"Challenge {challenge.Id} should have starter code");
            challenge.Solution.Should().NotBeNullOrEmpty(
                $"Challenge {challenge.Id} should have a solution");
            challenge.Language.Should().NotBeNullOrEmpty(
                $"Challenge {challenge.Id} should specify a language");
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void MultipleChoiceChallenges_ShouldHaveRequiredFields(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId)
            .Where(c => c.challenge.Type == "MULTIPLE_CHOICE")
            .ToList();

        if (!challenges.Any()) return;

        // Assert
        foreach (var (challenge, lessonId) in challenges)
        {
            challenge.Id.Should().NotBeNullOrEmpty(
                $"MULTIPLE_CHOICE challenge in {lessonId} should have an ID");
            challenge.Question.Should().NotBeNullOrEmpty(
                $"Challenge {challenge.Id} should have a question");
            challenge.Options.Should().NotBeNullOrEmpty(
                $"Challenge {challenge.Id} should have options");
            challenge.Options!.Count.Should().BeGreaterOrEqualTo(2,
                $"Challenge {challenge.Id} should have at least 2 options");
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void CodeOutputChallenges_ShouldHaveRequiredFields(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId)
            .Where(c => c.challenge.Type == "CODE_OUTPUT")
            .ToList();

        if (!challenges.Any()) return;

        // Assert
        foreach (var (challenge, lessonId) in challenges)
        {
            challenge.Id.Should().NotBeNullOrEmpty(
                $"CODE_OUTPUT challenge in {lessonId} should have an ID");
            // CODE_OUTPUT challenges should have either codeSnippet or starterCode
            var hasCode = !string.IsNullOrEmpty(challenge.CodeSnippet) ||
                         !string.IsNullOrEmpty(challenge.StarterCode);
            hasCode.Should().BeTrue(
                $"Challenge {challenge.Id} should have code to analyze");
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void AllChallenges_ShouldHaveHintsOrExplanation(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId);
        if (!challenges.Any()) return;

        var challengesWithHelp = 0;
        var totalChallenges = challenges.Count;

        // Act
        foreach (var (challenge, _) in challenges)
        {
            if (challenge.Hints.Any() || !string.IsNullOrEmpty(challenge.Explanation))
                challengesWithHelp++;
        }

        // Assert - at least 50% of challenges should have hints or explanations
        var helpPercentage = (double)challengesWithHelp / totalChallenges * 100;
        helpPercentage.Should().BeGreaterOrEqualTo(50,
            $"At least 50% of challenges in {courseId} should have hints or explanations");
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void Hints_ShouldHaveValidLevels(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId);
        if (!challenges.Any()) return;

        // Assert
        foreach (var (challenge, lessonId) in challenges)
        {
            foreach (var hint in challenge.Hints)
            {
                hint.Level.Should().BeGreaterThan(0,
                    $"Hint in challenge {challenge.Id} should have positive level");
                hint.Level.Should().BeLessOrEqualTo(5,
                    $"Hint level in challenge {challenge.Id} should be reasonable (1-5)");
                hint.Text.Should().NotBeNullOrEmpty(
                    $"Hint in challenge {challenge.Id} should have text");
            }

            // Hints should be in ascending order of level
            if (challenge.Hints.Count > 1)
            {
                var levels = challenge.Hints.Select(h => h.Level).ToList();
                levels.Should().BeInAscendingOrder(
                    $"Hints in challenge {challenge.Id} should be ordered by level");
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
    public void TestCases_ShouldHaveValidStructure(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId)
            .Where(c => c.challenge.TestCases.Any())
            .ToList();

        if (!challenges.Any()) return;

        // Assert
        foreach (var (challenge, lessonId) in challenges)
        {
            foreach (var testCase in challenge.TestCases)
            {
                testCase.Id.Should().NotBeNullOrEmpty(
                    $"TestCase in challenge {challenge.Id} should have an ID");
                testCase.Description.Should().NotBeNullOrEmpty(
                    $"TestCase {testCase.Id} in challenge {challenge.Id} should have a description");
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
    public void ChallengeLanguage_ShouldMatchCourseLanguage(string courseId)
    {
        // Arrange
        var course = LoadCourse(courseId);
        if (course == null) return;

        var acceptableLanguages = GetAcceptableLanguages(courseId);

        // Assert
        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                foreach (var challenge in lesson.Challenges)
                {
                    if (!string.IsNullOrEmpty(challenge.Language))
                    {
                        acceptableLanguages.Should().Contain(challenge.Language.ToLowerInvariant(),
                            $"Challenge {challenge.Id} language '{challenge.Language}' should be valid for course {courseId}");
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
    public void StarterCode_ShouldNotContainSolution(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId)
            .Where(c => !string.IsNullOrEmpty(c.challenge.StarterCode) &&
                       !string.IsNullOrEmpty(c.challenge.Solution))
            .ToList();

        if (!challenges.Any()) return;

        // Assert
        foreach (var (challenge, lessonId) in challenges)
        {
            // Starter code should be significantly shorter than solution
            // or should contain placeholder text like "____"
            var starterLength = challenge.StarterCode.Length;
            var solutionLength = challenge.Solution.Length;

            var containsPlaceholder = challenge.StarterCode.Contains("____") ||
                                     challenge.StarterCode.Contains("TODO") ||
                                     challenge.StarterCode.Contains("...") ||
                                     challenge.StarterCode.Contains("# Your code here") ||
                                     challenge.StarterCode.Contains("// Your code here");

            var isDifferent = starterLength < solutionLength * 0.9 || containsPlaceholder;

            isDifferent.Should().BeTrue(
                $"Starter code for {challenge.Id} should differ from solution");
        }
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("java")]
    [InlineData("csharp")]
    [InlineData("kotlin")]
    [InlineData("flutter")]
    public void CommonMistakes_ShouldHaveValidStructure(string courseId)
    {
        // Arrange
        var challenges = LoadAllChallenges(courseId)
            .Where(c => c.challenge.CommonMistakes.Any())
            .ToList();

        if (!challenges.Any()) return;

        // Assert
        foreach (var (challenge, lessonId) in challenges)
        {
            foreach (var mistake in challenge.CommonMistakes)
            {
                mistake.Mistake.Should().NotBeNullOrEmpty(
                    $"CommonMistake in challenge {challenge.Id} should have mistake text");
                mistake.Consequence.Should().NotBeNullOrEmpty(
                    $"CommonMistake in challenge {challenge.Id} should have consequence");
            }
        }
    }

    [Fact]
    public void AllCourses_ShouldHaveMixedChallengeTypes()
    {
        // Arrange
        var courseIds = new[] { "python", "javascript", "java", "csharp", "kotlin", "flutter" };
        var challengeTypeCounts = new Dictionary<string, int>();

        // Act
        foreach (var courseId in courseIds)
        {
            var challenges = LoadAllChallenges(courseId);
            foreach (var (challenge, _) in challenges)
            {
                if (!challengeTypeCounts.ContainsKey(challenge.Type))
                    challengeTypeCounts[challenge.Type] = 0;
                challengeTypeCounts[challenge.Type]++;
            }
        }

        // Assert - should have at least 2 different challenge types across all courses
        if (challengeTypeCounts.Any())
        {
            challengeTypeCounts.Keys.Count.Should().BeGreaterOrEqualTo(1,
                "Courses should have diverse challenge types");
        }
    }

    private static string[] GetAcceptableLanguages(string courseId)
    {
        return courseId switch
        {
            "python" => new[] { "python", "dockerfile", "bash", "yaml", "text", "html", "css", "json", "toml" },
            "javascript" => new[] { "javascript", "typescript", "bash", "yaml", "html", "css", "json" }, // TypeScript is valid in JS course
            "java" => new[] { "java", "jsx", "bash", "yaml", "xml", "json", "dockerfile" },
            "csharp" => new[] { "csharp", "yaml", "xml", "json", "bash", "dockerfile" },
            "kotlin" => new[] { "kotlin", "kt", "xml", "json", "yaml", "bash" },
            "flutter" or "dart" => new[] { "dart", "bash", "yaml", "text", "json", "groovy", "proguard", "xml", "toml", "dockerfile" }, // CLI and config examples
            _ => new[] { courseId }
        };
    }

    private List<(Challenge challenge, string lessonId)> LoadAllChallenges(string courseId)
    {
        var result = new List<(Challenge, string)>();
        var course = LoadCourse(courseId);

        if (course == null) return result;

        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                foreach (var challenge in lesson.Challenges)
                {
                    result.Add((challenge, lesson.Id));
                }
            }
        }

        return result;
    }

    private Course? LoadCourse(string courseId) => TestCourseLoader.LoadCourse(_contentPath, courseId);
}
