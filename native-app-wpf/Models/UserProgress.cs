using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CodeTutor.Wpf.Models;

public class UserProgress
{
    [JsonPropertyName("completedLessons")]
    public HashSet<string> CompletedLessons { get; set; } = new();

    [JsonPropertyName("lessonProgress")]
    public Dictionary<string, LessonProgress> LessonProgress { get; set; } = new();

    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("dailyActivity")]
    public Dictionary<string, bool> DailyActivity { get; set; } = new();

    [JsonPropertyName("sessionStartTimes")]
    public List<DateTime> SessionStartTimes { get; set; } = new();
}

public class LessonProgress
{
    [JsonPropertyName("challengesPassed")]
    public HashSet<string> ChallengesPassed { get; set; } = new();

    [JsonPropertyName("lastCode")]
    public Dictionary<string, string> LastCode { get; set; } = new();
}
