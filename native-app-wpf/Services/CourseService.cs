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
    private readonly IJsonValidationService _validationService;

    public CourseService()
    {
        _contentPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "courses"));
        _validationService = new JsonValidationService(new LoggingService());
    }

    public CourseService(IJsonValidationService validationService)
    {
        _contentPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "courses"));
        _validationService = validationService;
    }

    /// <summary>
    /// Validates and sanitizes a file path to ensure it stays within the content directory.
    /// </summary>
    private string? ValidateAndGetFullPath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return null;

        try
        {
            var fullPath = Path.GetFullPath(Path.Combine(_contentPath, relativePath));
            
            // Ensure the resolved path is within the content directory
            if (!PathSecurityValidator.IsPathWithinBaseDirectory(fullPath, _contentPath))
                return null;

            return fullPath;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Validates that all identifiers are safe before processing.
    /// </summary>
    private static void ValidateIdentifiers(string courseId, string? moduleId = null, string? lessonId = null)
    {
        PathSecurityValidator.ValidateIdentifierOrThrow(courseId, nameof(courseId));
        
        if (moduleId != null)
            PathSecurityValidator.ValidateIdentifierOrThrow(moduleId, nameof(moduleId));
        
        if (lessonId != null)
            PathSecurityValidator.ValidateIdentifierOrThrow(lessonId, nameof(lessonId));
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        var courses = new List<Course>();

        if (!Directory.Exists(_contentPath))
            return courses;

        foreach (var courseDir in Directory.GetDirectories(_contentPath))
        {
            // Validate that discovered directory is within content path
            if (!PathSecurityValidator.IsPathWithinBaseDirectory(courseDir, _contentPath))
            {
                Log($"SECURITY: Skipping directory outside content path: {courseDir}");
                continue;
            }

            var courseFile = Path.Combine(courseDir, "course.json");
            if (!File.Exists(courseFile))
                continue;

            try
            {
                var json = await File.ReadAllTextAsync(courseFile);
                var (course, errors) = _validationService.Deserialize<Course>(json, "course");
                if (errors.Count > 0)
                {
                    Log($"SECURITY: Course validation failed for {courseFile}: {string.Join(", ", errors)}");
                    continue;
                }
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
        // Validate course directory is within content path
        if (!PathSecurityValidator.IsPathWithinBaseDirectory(courseDir, _contentPath))
        {
            Log($"SECURITY: Attempted to load modules from outside content path: {courseDir}");
            return;
        }

        var modulesDir = Path.Combine(courseDir, "modules");
        if (!Directory.Exists(modulesDir))
            return;

        var moduleDirs = Directory.GetDirectories(modulesDir).OrderBy(d => Path.GetFileName(d));

        foreach (var moduleDir in moduleDirs)
        {
            // Validate module directory is within the course's modules directory
            if (!PathSecurityValidator.IsPathWithinBaseDirectory(moduleDir, modulesDir))
            {
                Log($"SECURITY: Skipping module directory outside modules path: {moduleDir}");
                continue;
            }

            var moduleFile = Path.Combine(moduleDir, "module.json");
            if (!File.Exists(moduleFile))
                continue;

            try
            {
                var json = await File.ReadAllTextAsync(moduleFile);
                var (module, errors) = _validationService.Deserialize<Module>(json, "module");
                if (errors.Count > 0)
                {
                    Log($"SECURITY: Module validation failed for {moduleFile}: {string.Join(", ", errors)}");
                    continue;
                }
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
        // Validate module directory is within content path
        if (!PathSecurityValidator.IsPathWithinBaseDirectory(moduleDir, _contentPath))
        {
            Log($"SECURITY: Attempted to load lessons from outside content path: {moduleDir}");
            return;
        }

        var lessonsDir = Path.Combine(moduleDir, "lessons");
        if (!Directory.Exists(lessonsDir))
            return;

        var lessonDirs = Directory.GetDirectories(lessonsDir).OrderBy(d => Path.GetFileName(d));

        foreach (var lessonDir in lessonDirs)
        {
            // Validate lesson directory is within the module's lessons directory
            if (!PathSecurityValidator.IsPathWithinBaseDirectory(lessonDir, lessonsDir))
            {
                Log($"SECURITY: Skipping lesson directory outside lessons path: {lessonDir}");
                continue;
            }

            var lessonFile = Path.Combine(lessonDir, "lesson.json");
            if (!File.Exists(lessonFile))
                continue;

            try
            {
                var json = await File.ReadAllTextAsync(lessonFile);
                var (lesson, errors) = _validationService.Deserialize<Lesson>(json, "lesson");
                if (errors.Count > 0)
                {
                    Log($"SECURITY: Lesson validation failed for {lessonFile}: {string.Join(", ", errors)}");
                    continue;
                }
                if (lesson != null)
                {
                    // Validate lesson ID before using as dictionary key
                    if (PathSecurityValidator.IsValidIdentifier(lesson.Id))
                    {
                        _lessonPaths[lesson.Id] = lessonDir;
                        module.Lessons.Add(lesson);
                    }
                    else
                    {
                        Log($"SECURITY: Skipping lesson with invalid ID: {lesson.Id}");
                    }
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
        // Validate courseId to prevent path traversal
        if (!PathSecurityValidator.IsValidIdentifier(courseId))
        {
            Log($"SECURITY: Invalid course ID rejected: {courseId}");
            return null;
        }

        if (_courseCache.TryGetValue(courseId, out var cached))
            return cached;

        await GetAllCoursesAsync();
        return _courseCache.GetValueOrDefault(courseId);
    }

    public async Task<Lesson?> GetLessonAsync(string courseId, string moduleId, string lessonId)
    {
        // Validate all identifiers to prevent path traversal
        try
        {
            ValidateIdentifiers(courseId, moduleId, lessonId);
        }
        catch (ArgumentException ex)
        {
            Log($"SECURITY: Invalid identifier rejected: {ex.Message}");
            return null;
        }

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
                                // Validate the cached path is still within content directory
                                if (!PathSecurityValidator.IsPathWithinBaseDirectory(lessonDir, _contentPath))
                                {
                                    Log($"SECURITY: Cached lesson path is outside content directory: {lessonDir}");
                                    return null;
                                }
                                
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
        // Validate lesson directory is within content path
        if (!PathSecurityValidator.IsPathWithinBaseDirectory(lessonDir, _contentPath))
        {
            Log($"SECURITY: Attempted to load content from outside content path: {lessonDir}");
            return;
        }

        Log($"Loading content for lesson: {lesson.Id} from {lessonDir}");

        // Load content sections from markdown files
        var contentDir = Path.Combine(lessonDir, "content");
        Log($"Content dir: {contentDir}, exists: {Directory.Exists(contentDir)}");

        if (Directory.Exists(contentDir))
        {
            // Validate content directory is within lesson directory
            if (!PathSecurityValidator.IsPathWithinBaseDirectory(contentDir, lessonDir))
            {
                Log($"SECURITY: Content directory is outside lesson directory: {contentDir}");
                return;
            }

            var mdFiles = Directory.GetFiles(contentDir, "*.md").OrderBy(f => Path.GetFileName(f)).ToList();
            Log($"Found {mdFiles.Count} markdown files");

            foreach (var mdFile in mdFiles)
            {
                // Validate each file is within the content directory
                if (!PathSecurityValidator.IsPathWithinBaseDirectory(mdFile, contentDir))
                {
                    Log($"SECURITY: Skipping file outside content directory: {mdFile}");
                    continue;
                }

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
            // Validate challenges directory is within lesson directory
            if (!PathSecurityValidator.IsPathWithinBaseDirectory(challengesDir, lessonDir))
            {
                Log($"SECURITY: Challenges directory is outside lesson directory: {challengesDir}");
                return;
            }

            var challengeDirs = Directory.GetDirectories(challengesDir).OrderBy(d => Path.GetFileName(d));
            foreach (var challengeDir in challengeDirs)
            {
                // Validate each challenge directory is within the challenges directory
                if (!PathSecurityValidator.IsPathWithinBaseDirectory(challengeDir, challengesDir))
                {
                    Log($"SECURITY: Skipping challenge directory outside challenges path: {challengeDir}");
                    continue;
                }

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
        // Validate challenge directory is within content path
        if (!PathSecurityValidator.IsPathWithinBaseDirectory(challengeDir, _contentPath))
        {
            Log($"SECURITY: Attempted to load challenge from outside content path: {challengeDir}");
            return null;
        }

        var challengeFile = Path.Combine(challengeDir, "challenge.json");
        if (!File.Exists(challengeFile))
            return null;

        // Validate challenge.json is within the challenge directory
        if (!PathSecurityValidator.IsPathWithinBaseDirectory(challengeFile, challengeDir))
        {
            Log($"SECURITY: Challenge file is outside challenge directory: {challengeFile}");
            return null;
        }

        var json = await File.ReadAllTextAsync(challengeFile);
        var (challenge, errors) = _validationService.Deserialize<Challenge>(json, "challenge");
        if (errors.Count > 0)
        {
            Log($"SECURITY: Challenge validation failed for {challengeFile}: {string.Join(", ", errors)}");
            return null;
        }
        if (challenge == null)
            return null;

        // Load starter code
        var starterFile = Directory.GetFiles(challengeDir, "starter.*").OrderBy(f => f).FirstOrDefault();
        if (starterFile != null)
        {
            // Validate starter file is within challenge directory
            if (PathSecurityValidator.IsPathWithinBaseDirectory(starterFile, challengeDir))
            {
                challenge.StarterCode = await File.ReadAllTextAsync(starterFile);
                if (string.IsNullOrEmpty(challenge.Language))
                {
                    challenge.Language = GetLanguageFromExtension(Path.GetExtension(starterFile));
                }
            }
            else
            {
                Log($"SECURITY: Starter file is outside challenge directory: {starterFile}");
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
            // Validate solution file is within challenge directory
            if (PathSecurityValidator.IsPathWithinBaseDirectory(solutionFile, challengeDir))
            {
                challenge.Solution = await File.ReadAllTextAsync(solutionFile);
            }
            else
            {
                Log($"SECURITY: Solution file is outside challenge directory: {solutionFile}");
            }
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
            var logDir = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CodeTutor"));
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "course-service.log");
            File.AppendAllText(logPath, $"{DateTime.Now:HH:mm:ss.fff}: {message}\n");
        }
        catch { /* Ignore logging errors */ }
    }
}
