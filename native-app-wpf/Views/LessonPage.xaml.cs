using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CodeTutor.Wpf.Controls;
using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Wpf.Views;

public partial class LessonPage : UserControl
{
    private readonly Course _course;
    private readonly Lesson _lesson;
    private readonly ICourseService _courseService;
    private readonly INavigationService _navigation;
    private readonly IProgressService _progressService;
    private readonly ICodeExecutionService _codeExecutionService;
    private readonly ITutorService _tutorService;
    private readonly IModelDownloadService _downloadService;
    private readonly List<ChallengeControlWrapper> _challengeControls = new();

    public LessonPage(Course course, Lesson lesson, ICourseService courseService, INavigationService navigation, IProgressService progressService, ICodeExecutionService codeExecutionService, ITutorService tutorService, IModelDownloadService downloadService)
    {
        try
        {
            InitializeComponent();
            _course = course ?? throw new ArgumentNullException(nameof(course));
            _lesson = lesson ?? throw new ArgumentNullException(nameof(lesson));
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
            _codeExecutionService = codeExecutionService ?? throw new ArgumentNullException(nameof(codeExecutionService));
            _tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
            _downloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));

            // Refresh sidebar and highlight active lesson
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                var sidebar = new CourseSidebar(course, courseService, navigation, progressService, codeExecutionService, tutorService, downloadService);
                sidebar.SetCurrentLesson(lesson);
                mainWindow.SetSidebarContent(sidebar);
            }

            SetupNavigation();
            Loaded += LessonPage_Loaded;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing lesson page: {ex.Message}\n\n{ex.StackTrace}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }

    private async void LessonPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= LessonPage_Loaded; // Only fire once
        try
        {
            await LoadLessonAsync();
            await CheckCompletionStatusAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LessonPage] Load failed: {ex.Message}");
        }
    }

    private async Task CheckCompletionStatusAsync()
    {
        if (await _progressService.IsLessonCompleteAsync(_lesson.Id))
        {
            CompleteButton.Content = new TextBlock { Text = "Completed" };
            CompleteButton.IsEnabled = false;
        }
    }

    private async Task LoadLessonAsync()
    {
        LessonTitle.Text = _lesson.Title;
        LessonTime.Text = $"{_lesson.EstimatedMinutes} min";
        LessonDifficulty.Text = _lesson.Difficulty;

        // Set lesson position
        var allLessons = _course.Modules.SelectMany(m => m.Lessons).ToList();
        var currentIndex = allLessons.FindIndex(l => l.Id == _lesson.Id);
        var currentModule = _course.Modules.FirstOrDefault(m => m.Lessons.Any(l => l.Id == _lesson.Id));
        LessonPosition.Text = $"Module: {currentModule?.Title} • Lesson {currentIndex + 1} of {allLessons.Count}";

        // Set breadcrumb navigation
        BreadcrumbCourseText.Text = _course.Title;
        BreadcrumbModuleText.Text = currentModule?.Title ?? "";

        // Trigger lazy loading of content sections and challenges
        var moduleId = currentModule?.Id ?? _lesson.ModuleId;
        var loadedLesson = await _courseService.GetLessonAsync(_course.Id, moduleId, _lesson.Id);

        // Use the loaded lesson's content (in case it's a different reference)
        var contentSections = loadedLesson?.ContentSections ?? _lesson.ContentSections;
        var challenges = loadedLesson?.Challenges ?? _lesson.Challenges;

        // Add content sections dynamically
        ContentPanel.Children.Clear();

        foreach (var section in contentSections)
        {
            var sectionControl = CreateSectionControl(section);
            if (sectionControl != null)
            {
                ContentPanel.Children.Add(sectionControl);
            }
        }

        // Add challenges with type-appropriate controls
        foreach (var challenge in challenges)
        {
            var challengeType = (challenge.Type ?? "").ToUpperInvariant();

            if (challengeType == "QUIZ" && challenge.Options != null && challenge.Options.Count > 0)
            {
                // Multiple-choice quiz - use QuizChallenge control
                var quizControl = new Controls.QuizChallenge(challenge);
                quizControl.ChallengeCompleted += OnChallengeCompleted;
                quizControl.ContextChanged += OnChallengeContextChanged;
                _challengeControls.Add(new ChallengeControlWrapper(quizControl));
                ContentPanel.Children.Add(quizControl);
            }
            else
            {
                // Code-based challenges (FREE_CODING, CODE, implementation, etc.)
                var challengeControl = new Controls.CodingChallenge(challenge, _codeExecutionService);
                challengeControl.ChallengeCompleted += OnChallengeCompleted;
                challengeControl.ContextChanged += OnChallengeContextChanged;
                _challengeControls.Add(new ChallengeControlWrapper(challengeControl));
                ContentPanel.Children.Add(challengeControl);
            }
        }

        // Set initial tutor context with lesson info
        UpdateTutorWithLessonContext(contentSections);
    }

    private UIElement? CreateSectionControl(ContentSection section)
    {
        return section.Type?.ToUpperInvariant() switch
        {
            "THEORY" => new Controls.TheorySection(section),
            "EXAMPLE" => new Controls.CodeExampleSection(section),
            "KEY_POINT" => new Controls.KeyPointSection(section),
            "ANALOGY" => new Controls.AnalogySection(section),
            "WARNING" => new Controls.WarningSection(section),
            "LEGACY_COMPARISON" => new Controls.LegacyComparisonSection(section),
            _ => CreateDefaultSection(section)
        };
    }

    private UIElement CreateDefaultSection(ContentSection section)
    {
        var container = new StackPanel { Margin = new Thickness(0, 0, 0, 24) };

        if (!string.IsNullOrEmpty(section.Title))
        {
            var title = new TextBlock
            {
                Text = section.Title,
                Style = (Style)FindResource("SubheadingText"),
                Margin = new Thickness(0, 0, 0, 12)
            };
            container.Children.Add(title);
        }

        var content = new TextBlock
        {
            Text = section.Content,
            Style = (Style)FindResource("BodyText"),
            TextWrapping = TextWrapping.Wrap
        };
        container.Children.Add(content);

        // Add code block if present
        if (!string.IsNullOrEmpty(section.Code))
        {
            var codeBorder = new Border
            {
                Background = (System.Windows.Media.Brush)FindResource("BackgroundDarkBrush"),
                BorderBrush = (System.Windows.Media.Brush)FindResource("BorderDefaultBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(16),
                Margin = new Thickness(0, 12, 0, 0)
            };

            var codeText = new TextBlock
            {
                Text = section.Code,
                Style = (Style)FindResource("CodeText"),
                TextWrapping = TextWrapping.Wrap
            };
            codeBorder.Child = codeText;
            container.Children.Add(codeBorder);
        }

        return container;
    }

    private void SetupNavigation()
    {
        var allLessons = _course.Modules.SelectMany(m => m.Lessons).ToList();
        var currentIndex = allLessons.FindIndex(l => l.Id == _lesson.Id);

        if (currentIndex > 0)
        {
            PrevButton.IsEnabled = true;
            PrevButton.Tag = allLessons[currentIndex - 1];
        }

        if (currentIndex < allLessons.Count - 1)
        {
            NextButton.IsEnabled = true;
            NextButton.Tag = allLessons[currentIndex + 1];
        }
    }

    private void PrevButton_Click(object sender, RoutedEventArgs e)
    {
        if (PrevButton.Tag is Lesson prevLesson)
        {
            if (_navigation is NavigationService navService)
            {
                navService.NavigateWithFactory(() =>
                    new LessonPage(_course, prevLesson, _courseService, _navigation, _progressService, _codeExecutionService, _tutorService, _downloadService));
            }
            else
            {
                var lessonPage = new LessonPage(_course, prevLesson, _courseService, _navigation, _progressService, _codeExecutionService, _tutorService, _downloadService);
                _navigation.NavigateTo(lessonPage, prevLesson);
            }
        }
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (NextButton.Tag is Lesson nextLesson)
        {
            if (_navigation is NavigationService navService)
            {
                navService.NavigateWithFactory(() =>
                    new LessonPage(_course, nextLesson, _courseService, _navigation, _progressService, _codeExecutionService, _tutorService, _downloadService));
            }
            else
            {
                var lessonPage = new LessonPage(_course, nextLesson, _courseService, _navigation, _progressService, _codeExecutionService, _tutorService, _downloadService);
                _navigation.NavigateTo(lessonPage, nextLesson);
            }
        }
    }

    private async void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        await _progressService.MarkLessonCompleteAsync(_lesson.Id);
        CompleteButton.Content = new TextBlock { Text = "Completed" };
        CompleteButton.IsEnabled = false;
    }

    private void BreadcrumbCourse_Click(object sender, RoutedEventArgs e)
    {
        if (_navigation is NavigationService navService)
        {
            navService.NavigateWithFactory(() =>
                new CoursePage(_courseService, _navigation, _course, _progressService, _codeExecutionService, _tutorService, _downloadService));
        }
        else
        {
            var coursePage = new CoursePage(_courseService, _navigation, _course, _progressService, _codeExecutionService, _tutorService, _downloadService);
            _navigation.NavigateTo(coursePage, _course);
        }
    }

    private async void OnChallengeCompleted(object? sender, string challengeId)
    {
        // Check if all challenges are now complete
        if (_challengeControls.Count > 0 && _challengeControls.All(c => c.IsCompleted))
        {
            // Auto-mark lesson as complete
            if (!await _progressService.IsLessonCompleteAsync(_lesson.Id))
            {
                await _progressService.MarkLessonCompleteAsync(_lesson.Id);
                CompleteButton.Content = new TextBlock { Text = "Completed" };
                CompleteButton.IsEnabled = false;
            }
        }
    }

    private void UpdateTutorWithLessonContext(List<ContentSection> contentSections)
    {
        // Build a condensed summary from section titles/types
        var summaryParts = contentSections
            .Where(s => !string.IsNullOrEmpty(s.Title))
            .Select(s => s.Title)
            .ToList();
        var lessonContent = summaryParts.Count > 0
            ? string.Join(", ", summaryParts)
            : null;

        var context = new Services.TutorContext
        {
            CurrentLanguage = _course.Language,
            LessonTitle = _lesson.Title,
            LessonContent = lessonContent,
        };

        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.UpdateTutorContext(context);
        }
    }

    private void OnChallengeContextChanged(object? sender, Controls.ChallengeContextEventArgs e)
    {
        var context = new Services.TutorContext
        {
            CurrentLanguage = _course.Language,
            LessonTitle = _lesson.Title,
            LessonContent = $"Challenge: {e.ChallengeTitle}\nInstructions: {e.ChallengeInstructions}",
            UserCode = e.UserCode,
            ExecutionError = e.ExecutionError,
            ExpectedOutput = e.ExpectedOutput,
        };

        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.UpdateTutorContext(context);
        }
    }
}

/// <summary>
/// Wraps either CodingChallenge or QuizChallenge to provide a uniform IsCompleted interface.
/// </summary>
internal class ChallengeControlWrapper
{
    private readonly Controls.CodingChallenge? _codingChallenge;
    private readonly Controls.QuizChallenge? _quizChallenge;

    public ChallengeControlWrapper(Controls.CodingChallenge codingChallenge)
    {
        _codingChallenge = codingChallenge;
    }

    public ChallengeControlWrapper(Controls.QuizChallenge quizChallenge)
    {
        _quizChallenge = quizChallenge;
    }

    public bool IsCompleted => _codingChallenge?.IsCompleted ?? _quizChallenge?.IsCompleted ?? false;
}
