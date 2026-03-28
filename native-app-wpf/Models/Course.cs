using System.Collections.Generic;
using System.Text.Json.Serialization;
using CodeTutor.Wpf.Models.Validation;

namespace CodeTutor.Wpf.Models;

public class Course
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [JsonPropertyName("estimatedHours")]
    public int EstimatedHours { get; set; }

    [JsonPropertyName("modules")]
    public List<Module> Modules { get; set; } = new();

    // Computed properties
    public int ModuleCount => Modules.Count;
    public bool IsRuntimeAvailable { get; set; } = true;
}

public class Module
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("lessons")]
    public List<Lesson> Lessons { get; set; } = new();
}

public class Lesson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("moduleId")]
    public string ModuleId { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("estimatedMinutes")]
    public int EstimatedMinutes { get; set; }

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [JsonPropertyName("contentSections")]
    public List<ContentSection> ContentSections { get; set; } = new();

    [JsonPropertyName("challenges")]
    public List<Challenge> Challenges { get; set; } = new();
}

public class ContentSection
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("legacy")]
    public string? Legacy { get; set; }
}

public class Challenge
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("instructions")]
    public string Instructions { get; set; } = string.Empty;

    [JsonPropertyName("starterCode")]
    public string StarterCode { get; set; } = string.Empty;

    [JsonPropertyName("startingCode")]
    public string StartingCode { get; set; } = string.Empty;

    [JsonPropertyName("solution")]
    public string Solution { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [JsonPropertyName("hints")]
    public List<Hint> Hints { get; set; } = new();

    [JsonPropertyName("testCases")]
    public List<TestCase> TestCases { get; set; } = new();

    [JsonPropertyName("commonMistakes")]
    public List<CommonMistake> CommonMistakes { get; set; } = new();

    // Multiple choice challenge fields
    [JsonPropertyName("question")]
    public string? Question { get; set; }

    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }

    [JsonPropertyName("correctAnswer")]
    [JsonConverter(typeof(ValidatedCorrectAnswerConverter))]
    public ValidatedCorrectAnswer? CorrectAnswer { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    // TRUE_FALSE challenge fields
    [JsonPropertyName("statement")]
    public string? Statement { get; set; }

    [JsonPropertyName("isTrue")]
    public bool? IsTrue { get; set; }

    // Multi-question quiz fields
    [JsonPropertyName("questions")]
    public List<QuizQuestion>? Questions { get; set; }

    // Code output challenge fields
    [JsonPropertyName("codeSnippet")]
    public string? CodeSnippet { get; set; }

    [JsonPropertyName("expectedOutput")]
    public string? ExpectedOutput { get; set; }
}

public class Hint
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class TestCase
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("input")]
    public string? Input { get; set; }

    [JsonPropertyName("expectedOutput")]
    public string ExpectedOutput { get; set; } = string.Empty;

    [JsonPropertyName("isVisible")]
    public bool IsVisible { get; set; } = true;
}

public class QuizQuestion
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("question")]
    public string? Question { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>Resolved question text: prefers "question", falls back to "text".</summary>
    [JsonIgnore]
    public string QuestionText => Question ?? Text ?? string.Empty;

    [JsonPropertyName("options")]
    public List<string> Options { get; set; } = new();

    [JsonPropertyName("correctAnswer")]
    [JsonConverter(typeof(ValidatedCorrectAnswerConverter))]
    public ValidatedCorrectAnswer? CorrectAnswer { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
}

public class CommonMistake
{
    [JsonPropertyName("mistake")]
    public string Mistake { get; set; } = string.Empty;

    [JsonPropertyName("consequence")]
    public string Consequence { get; set; } = string.Empty;

    [JsonPropertyName("correction")]
    public string Correction { get; set; } = string.Empty;
}
