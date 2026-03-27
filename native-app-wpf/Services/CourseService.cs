using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CodeTutor.Wpf.Models;

namespace CodeTutor.Wpf.Services;

public interface ICourseService
{
    Task<List<Course>> GetAllCoursesAsync();
    Task<Course?> GetCourseAsync(string courseId);
    Task<Lesson?> GetLessonAsync(string courseId, string moduleId, string lessonId);
}

public class CourseService : ICourseService
{
    private readonly string _contentPath;
    private readonly ConcurrentDictionary<string, Course> _courseCache = new();
    private readonly ConcurrentDictionary<string, string> _lessonPaths = new();

    public CourseService()
    {
        _contentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "courses");
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        var courses = new List<Course>();

        if (!Directory.Exists(_contentPath))
            return courses;

        foreach (var courseDir in Directory.GetDirectories(_contentPath))
        {
            var courseFile = Path.Combine(courseDir, "course.json");
            if (!File.Exists(courseFile))
                continue;

            try
            {
                var json = await File.ReadAllTextAsync(courseFile);
                var course = JsonSerializer.Deserialize<Course>(json);
                if (course != null)
                {
                    await LoadModulesAsync(course, courseDir);
                    _courseCache.TryAdd(course.Id, course);
                    courses.Add(course);
                }
            }
            catch (Exception ex)
            {
                LogError(courseDir, ex);
            }
        }

        return courses;
    }

    private async Task LoadModulesAsync(Course course, string courseDir)
    {
        var modulesDir = Path.Combine(courseDir, "modules");
        if (!Directory.Exists(modulesDir))
            return;

        var moduleDirs = Directory.GetDirectories(modulesDir).OrderBy(d => Path.GetFileName(d));

        foreach (var moduleDir in moduleDirs)
        {
            var moduleFile = Path.Combine(moduleDir, "module.json");
            if (!File.Exists(moduleFile))
                continue;

            try
            {
                var json = await File.ReadAllTextAsync(moduleFile);
                var module = JsonSerializer.Deserialize<Module>(json);
                if (module != null)
                {
                    await LoadLessonStubsAsync(module, moduleDir);
                    course.Modules.Add(module);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR: Failed to load module from {moduleDir}: {ex.Message}");
            }
        }
    }

    private async Task LoadLessonStubsAsync(Module module, string moduleDir)
    {
        var lessonsDir = Path.Combine(moduleDir, "lessons");
        if (!Directory.Exists(lessonsDir))
            return;

        var lessonDirs = Directory.GetDirectories(lessonsDir).OrderBy(d => Path.GetFileName(d));

        foreach (var lessonDir in lessonDirs)
        {
            var lessonFile = Path.Combine(lessonDir, "lesson.json");
            if (!File.Exists(lessonFile))
                continue;

            try
            {
                var json = await File.ReadAllTextAsync(lessonFile);
                var lesson = JsonSerializer.Deserialize<Lesson>(json);
                if (lesson != null)
                {
                    _lessonPaths[lesson.Id] = lessonDir;
                    module.Lessons.Add(lesson);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR: Failed to load lesson from {lessonDir}: {ex.Message}");
            }
        }
    }

    public async Task<Course?> GetCourseAsync(string courseId)
    {
        if (_courseCache.TryGetValue(courseId, out var cached))
            return cached;

        await GetAllCoursesAsync();
        return _courseCache.GetValueOrDefault(courseId);
    }

    public async Task<Lesson?> GetLessonAsync(string courseId, string moduleId, string lessonId)
    {
        Log($"GetLessonAsync: course={courseId}, module={moduleId}, lesson={lessonId}");

        var course = await GetCourseAsync(courseId);
        if (course == null)
        {
            Log($"Course not found: {courseId}");
            return null;
        }

        foreach (var module in course.Modules)
        {
            if (module.Id == moduleId)
            {
                foreach (var lesson in module.Lessons)
                {
                    if (lesson.Id == lessonId)
                    {
                        Log($"Found lesson: {lesson.Title}");
                        Log($"ContentSections.Count={lesson.ContentSections.Count}, Challenges.Count={lesson.Challenges.Count}");

                        // Lazy load content if not already loaded
                        if (lesson.ContentSections.Count == 0 && lesson.Challenges.Count == 0)
                        {
                            Log($"Lazy loading content...");
                            if (_lessonPaths.TryGetValue(lessonId, out var lessonDir))
                            {
                                Log($"Lesson path: {lessonDir}");
                                await LoadLessonContentAsync(lesson, lessonDir);
                                Log($"After load: ContentSections.Count={lesson.ContentSections.Count}, Challenges.Count={lesson.Challenges.Count}");
                            }
                            else
                            {
                                Log($"WARNING: No path found for lesson {lessonId}");
                            }
                        }
                        return lesson;
                    }
                }
            }
        }

        Log($"Lesson not found in course structure");
        return null;
    }

    private async Task LoadLessonContentAsync(Lesson lesson, string lessonDir)
    {
        Log($"Loading content for lesson: {lesson.Id} from {lessonDir}");

        // Load content sections from markdown files
        var contentDir = Path.Combine(lessonDir, "content");
        Log($"Content dir: {contentDir}, exists: {Directory.Exists(contentDir)}");

        if (Directory.Exists(contentDir))
        {
            var mdFiles = Directory.GetFiles(contentDir, "*.md").OrderBy(f => Path.GetFileName(f)).ToList();
            Log($"Found {mdFiles.Count} markdown files");

            foreach (var mdFile in mdFiles)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(mdFile);
                    Log($"Read file {Path.GetFileName(mdFile)}: {content.Length} chars");

                    var section = ParseMarkdownToContentSection(content);
                    if (section != null)
                    {
                        Log($"Parsed section: Type={section.Type}, Title={section.Title}, Content={section.Content?.Length ?? 0} chars");
                        lesson.ContentSections.Add(section);
                    }
                    else
                    {
                        Log($"Failed to parse section from {mdFile}");
                    }
                }
                catch (Exception ex)
                {
                    Log($"ERROR: Failed to load content from {mdFile}: {ex.Message}");
                }
            }
        }

        // Load challenges from subdirectories
        var challengesDir = Path.Combine(lessonDir, "challenges");
        if (Directory.Exists(challengesDir))
        {
            var challengeDirs = Directory.GetDirectories(challengesDir).OrderBy(d => Path.GetFileName(d));
            foreach (var challengeDir in challengeDirs)
            {
                try
                {
                    var challenge = await LoadChallengeAsync(challengeDir);
                    if (challenge != null)
                    {
                        lesson.Challenges.Add(challenge);
                    }
                }
                catch (Exception ex)
                {
                    Log($"ERROR: Failed to load challenge from {challengeDir}: {ex.Message}");
                }
            }
        }
    }

    private ContentSection? ParseMarkdownToContentSection(string content)
    {
        var (frontmatter, body) = ParseMarkdown(content);
        if (frontmatter.Count == 0)
            return null;

        return new ContentSection
        {
            Type = frontmatter.GetValueOrDefault("type", "theory") ?? "theory",
            Title = frontmatter.GetValueOrDefault("title", "") ?? "",
            Content = body.Trim()
        };
    }

    private (Dictionary<string, string> frontmatter, string body) ParseMarkdown(string content)
    {
        var frontmatter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var body = content;

        if (!content.StartsWith("---"))
            return (frontmatter, body);

        var endIndex = content.IndexOf("---", 3);
        if (endIndex < 0)
            return (frontmatter, body);

        var frontmatterText = content.Substring(3, endIndex - 3).Trim();
        body = content.Substring(endIndex + 3).Trim();

        foreach (var line in frontmatterText.Split('\n'))
        {
            var colonIndex = line.IndexOf(':');
            if (colonIndex > 0)
            {
                var key = line.Substring(0, colonIndex).Trim();
                var value = line.Substring(colonIndex + 1).Trim().Trim('"');
                frontmatter[key] = value;
            }
        }

        return (frontmatter, body);
    }

    private async Task<Challenge?> LoadChallengeAsync(string challengeDir)
    {
        var challengeFile = Path.Combine(challengeDir, "challenge.json");
        if (!File.Exists(challengeFile))
            return null;

        var json = await File.ReadAllTextAsync(challengeFile);
        var challenge = JsonSerializer.Deserialize<Challenge>(json);
        if (challenge == null)
            return null;

        // Load starter code
        var starterFile = Directory.GetFiles(challengeDir, "starter.*").OrderBy(f => f).FirstOrDefault();
        if (starterFile != null)
        {
            challenge.StarterCode = await File.ReadAllTextAsync(starterFile);
            if (string.IsNullOrEmpty(challenge.Language))
            {
                challenge.Language = GetLanguageFromExtension(Path.GetExtension(starterFile));
            }
        }
        else if (!string.IsNullOrEmpty(challenge.StartingCode))
        {
            challenge.StarterCode = challenge.StartingCode;
        }

        // Load solution code
        var solutionFile = Directory.GetFiles(challengeDir, "solution.*").OrderBy(f => f).FirstOrDefault();
        if (solutionFile != null)
        {
            challenge.Solution = await File.ReadAllTextAsync(solutionFile);
        }

        return challenge;
    }

    private static string GetLanguageFromExtension(string ext) => ext.ToLowerInvariant() switch
    {
        ".java" => "java",
        ".py" => "python",
        ".js" => "javascript",
        ".ts" => "typescript",
        ".cs" => "csharp",
        ".kt" => "kotlin",
        ".dart" => "dart",
        ".swift" => "swift",
        ".go" => "go",
        ".rs" => "rust",
        ".rb" => "ruby",
        ".php" => "php",
        ".cpp" or ".cc" or ".cxx" => "cpp",
        ".c" => "c",
        _ => ext.TrimStart('.').ToLowerInvariant()
    };

    private void LogError(string dir, Exception ex)
    {
        Debug.WriteLine($"Failed to load course from {dir}: {ex.Message}");
        Log($"ERROR: Failed to load {dir}: {ex.Message}\n  Stack: {ex.StackTrace}");
    }

    private static void Log(string message)
    {
        try
        {
            var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CodeTutor");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "course-service.log");
            File.AppendAllText(logPath, $"{DateTime.Now:HH:mm:ss.fff}: {message}\n");
        }
        catch { /* Ignore logging errors */ }
    }
}
