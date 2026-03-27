using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CodeTutor.Tests.Models;

namespace CodeTutor.Tests.E2E;

public static class TestCourseLoader
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
    };

    public static List<Course> LoadAllCourses(string contentPath)
    {
        var courses = new List<Course>();

        if (!Directory.Exists(contentPath))
            return courses;

        foreach (var dir in Directory.GetDirectories(contentPath))
        {
            var courseId = Path.GetFileName(dir);
            var course = LoadCourse(contentPath, courseId);
            if (course != null)
                courses.Add(course);
        }

        return courses;
    }

    public static Course? LoadCourse(string contentPath, string courseId)
    {
        var courseDir = Path.Combine(contentPath, courseId);
        var courseFile = Path.Combine(courseDir, "course.json");

        if (!File.Exists(courseFile))
            return null;

        var json = File.ReadAllText(courseFile);
        var course = JsonSerializer.Deserialize<Course>(json, _jsonOptions);
        if (course == null) return null;

        course.Modules ??= new List<Module>();

        var modulesDir = Path.Combine(courseDir, "modules");
        if (Directory.Exists(modulesDir))
        {
            var moduleDirs = Directory.GetDirectories(modulesDir).OrderBy(d => Path.GetFileName(d));
            foreach (var moduleDir in moduleDirs)
            {
                var moduleFile = Path.Combine(moduleDir, "module.json");
                if (File.Exists(moduleFile))
                {
                    var mJson = File.ReadAllText(moduleFile);
                    var module = JsonSerializer.Deserialize<Module>(mJson, _jsonOptions);
                    if (module != null)
                    {
                        module.Lessons ??= new List<Lesson>();
                        LoadLessons(module, moduleDir);
                        course.Modules.Add(module);
                    }
                }
            }
        }

        return course;
    }

    private static void LoadLessons(Module module, string moduleDir)
    {
        var lessonsDir = Path.Combine(moduleDir, "lessons");
        if (!Directory.Exists(lessonsDir)) return;

        var lessonDirs = Directory.GetDirectories(lessonsDir).OrderBy(d => Path.GetFileName(d));
        foreach (var lessonDir in lessonDirs)
        {
            var lessonFile = Path.Combine(lessonDir, "lesson.json");
            if (File.Exists(lessonFile))
            {
                var lJson = File.ReadAllText(lessonFile);
                var lesson = JsonSerializer.Deserialize<Lesson>(lJson, _jsonOptions);
                if (lesson != null)
                {
                    lesson.ContentSections ??= new List<ContentSection>();
                    lesson.Challenges ??= new List<Challenge>();
                    LoadContentSections(lesson, lessonDir);
                    LoadChallenges(lesson, lessonDir);
                    module.Lessons.Add(lesson);
                }
            }
        }
    }

    private static void LoadContentSections(Lesson lesson, string lessonDir)
    {
        var contentDir = Path.Combine(lessonDir, "content");
        if (Directory.Exists(contentDir))
        {
            var mdFiles = Directory.GetFiles(contentDir, "*.md").OrderBy(f => Path.GetFileName(f));
            foreach (var mdFile in mdFiles)
            {
                var content = File.ReadAllText(mdFile);
                var (frontmatter, body) = ParseMarkdown(content);
                if (frontmatter.Count > 0)
                {
                    lesson.ContentSections.Add(new ContentSection
                    {
                        Type = frontmatter.GetValueOrDefault("type", "theory"),
                        Title = frontmatter.GetValueOrDefault("title", ""),
                        Content = body.Trim(),
                        Code = body.Trim()
                    });
                }
            }
        }
    }

    private static (Dictionary<string, string> frontmatter, string body) ParseMarkdown(string content)
    {
        var frontmatter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var body = content;

        if (!content.StartsWith("---")) return (frontmatter, body);

        var endIndex = content.IndexOf("---", 3);
        if (endIndex < 0) return (frontmatter, body);

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

    private static void LoadChallenges(Lesson lesson, string lessonDir)
    {
        var challengesDir = Path.Combine(lessonDir, "challenges");
        if (Directory.Exists(challengesDir))
        {
            var challengeDirs = Directory.GetDirectories(challengesDir).OrderBy(d => Path.GetFileName(d));
            foreach (var challengeDir in challengeDirs)
            {
                var challengeFile = Path.Combine(challengeDir, "challenge.json");
                if (File.Exists(challengeFile))
                {
                    var cJson = File.ReadAllText(challengeFile);
                    var challenge = JsonSerializer.Deserialize<Challenge>(cJson, _jsonOptions);
                    if (challenge != null)
                    {
                        challenge.Hints ??= new List<Hint>();
                        challenge.TestCases ??= new List<TestCase>();
                        challenge.CommonMistakes ??= new List<CommonMistake>();
                        
                        var starterFile = Directory.GetFiles(challengeDir, "starter.*").FirstOrDefault();
                        if (starterFile != null)
                        {
                            challenge.StarterCode = File.ReadAllText(starterFile);
                            if (string.IsNullOrEmpty(challenge.Language))
                            {
                                challenge.Language = Path.GetExtension(starterFile).TrimStart('.');
                            }
                        }
                        else if (!string.IsNullOrEmpty(challenge.StartingCode))
                        {
                            challenge.StarterCode = challenge.StartingCode;
                        }
                        
                        var solutionFile = Directory.GetFiles(challengeDir, "solution.*").FirstOrDefault();
                        if (solutionFile != null)
                        {
                            challenge.Solution = File.ReadAllText(solutionFile);
                        }
                        
                        lesson.Challenges.Add(challenge);
                    }
                }
            }
        }
    }
}
