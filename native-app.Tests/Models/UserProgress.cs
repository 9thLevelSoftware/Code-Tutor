using System.Text.Json.Serialization;

namespace CodeTutor.Tests.Models;

/// <summary>
/// User progress model for testing - mirrors the WPF app model
/// </summary>
public class UserProgress
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("completedLessons")]
    public HashSet<string> CompletedLessons { get; set; } = new();

    [JsonPropertyName("lessonProgress")]
    public Dictionary<string, LessonProgress> LessonProgress { get; set; } = new();

    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("currentStreak")]
    public int CurrentStreak { get; set; }

    [JsonPropertyName("longestStreak")]
    public int LongestStreak { get; set; }

    [JsonPropertyName("totalTimeSpentMinutes")]
    public int TotalTimeSpentMinutes { get; set; }

    [JsonPropertyName("achievements")]
    public List<Achievement> Achievements { get; set; } = new();
}

public class LessonProgress
{
    [JsonPropertyName("challengesPassed")]
    public HashSet<string> ChallengesPassed { get; set; } = new();

    [JsonPropertyName("lastCode")]
    public Dictionary<string, string> LastCode { get; set; } = new();

    [JsonPropertyName("hintsUsed")]
    public Dictionary<string, int> HintsUsed { get; set; } = new();

    [JsonPropertyName("attempts")]
    public Dictionary<string, int> Attempts { get; set; } = new();

    [JsonPropertyName("startedAt")]
    public DateTime? StartedAt { get; set; }

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("bestScore")]
    public int BestScore { get; set; }
}

public class Achievement
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("unlockedAt")]
    public DateTime UnlockedAt { get; set; }

    [JsonPropertyName("progress")]
    public int Progress { get; set; }

    [JsonPropertyName("maxProgress")]
    public int MaxProgress { get; set; }
}

/// <summary>
/// Execution result for code execution testing
/// </summary>
public record ExecutionResult(bool Success, string Output, string Error);

/// <summary>
/// Runtime information for language runtime detection
/// </summary>
public record RuntimeInfo(string Language, bool IsAvailable, string Version, string InstallHint);
