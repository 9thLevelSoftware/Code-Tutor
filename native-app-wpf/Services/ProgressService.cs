using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CodeTutor.Wpf.Models;

namespace CodeTutor.Wpf.Services;

public interface IProgressService
{
    Task<UserProgress> LoadProgressAsync();
    Task SaveProgressAsync(UserProgress progress);
    Task MarkLessonCompleteAsync(string lessonId);
    Task<bool> IsLessonCompleteAsync(string lessonId);
    Task<CourseProgressStats> GetCourseProgressAsync(Course course);
    int GetCurrentStreak();
}

public record CourseProgressStats(
    int CompletedLessons,
    int TotalLessons,
    double PercentComplete,
    int CurrentStreak,
    TimeSpan TimeThisWeek
);

public class ProgressService : IProgressService
{
    private readonly string _progressFilePath;
    private UserProgress? _cachedProgress;

    public ProgressService()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodeTutor");
        Directory.CreateDirectory(appDataPath);
        _progressFilePath = Path.Combine(appDataPath, "progress.json");
    }

    public async Task<UserProgress> LoadProgressAsync()
    {
        if (_cachedProgress != null) return _cachedProgress;

        if (!File.Exists(_progressFilePath))
        {
            _cachedProgress = new UserProgress();
            return _cachedProgress;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_progressFilePath);
            _cachedProgress = JsonSerializer.Deserialize<UserProgress>(json) ?? new UserProgress();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProgressService] Failed to load progress, starting fresh: {ex.Message}");
            try
            {
                var backupPath = _progressFilePath + ".corrupt";
                if (File.Exists(_progressFilePath))
                    File.Copy(_progressFilePath, backupPath, overwrite: true);
            }
            catch { /* Best-effort backup */ }

            _cachedProgress = new UserProgress();
        }

        return _cachedProgress;
    }

    public async Task SaveProgressAsync(UserProgress progress)
    {
        progress.LastUpdated = DateTime.UtcNow;
        _cachedProgress = progress;

        var json = JsonSerializer.Serialize(progress, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_progressFilePath, json);
    }

    public async Task MarkLessonCompleteAsync(string lessonId)
    {
        var progress = await LoadProgressAsync();
        progress.CompletedLessons.Add(lessonId);

        var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        progress.DailyActivity.TryAdd(today, true);

        await SaveProgressAsync(progress);
    }

    public async Task<bool> IsLessonCompleteAsync(string lessonId)
    {
        var progress = await LoadProgressAsync();
        return progress.CompletedLessons.Contains(lessonId);
    }

    public async Task<CourseProgressStats> GetCourseProgressAsync(Course course)
    {
        var progress = await LoadProgressAsync();

        var allLessons = course.Modules.SelectMany(m => m.Lessons).ToList();
        int completed = allLessons.Count(l => progress.CompletedLessons.Contains(l.Id));
        int total = allLessons.Count;
        double percent = total > 0 ? (double)completed / total * 100 : 0;

        var weekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        int daysActiveThisWeek = 0;
        for (var d = weekStart; d <= DateTime.UtcNow.Date; d = d.AddDays(1))
        {
            if (progress.DailyActivity.ContainsKey(d.ToString("yyyy-MM-dd")))
                daysActiveThisWeek++;
        }

        return new CourseProgressStats(
            CompletedLessons: completed,
            TotalLessons: total,
            PercentComplete: Math.Round(percent, 1),
            CurrentStreak: GetCurrentStreak(),
            TimeThisWeek: TimeSpan.FromDays(daysActiveThisWeek)
        );
    }

    public int GetCurrentStreak()
    {
        if (_cachedProgress == null || _cachedProgress.DailyActivity.Count == 0)
            return 0;

        var today = DateTime.UtcNow.Date;
        int streak = 0;

        var checkDate = _cachedProgress.DailyActivity.ContainsKey(today.ToString("yyyy-MM-dd"))
            ? today
            : today.AddDays(-1);

        while (_cachedProgress.DailyActivity.ContainsKey(checkDate.ToString("yyyy-MM-dd")))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }

        return streak;
    }
}
