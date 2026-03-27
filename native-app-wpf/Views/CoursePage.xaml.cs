using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Wpf.Views;

public partial class CoursePage : UserControl
{
    private readonly Course _course;
    private readonly ICourseService _courseService;
    private readonly INavigationService _navigation;
    private readonly IProgressService _progressService;
    private readonly ICodeExecutionService _codeExecutionService;
    private readonly ITutorService _tutorService;
    private readonly IModelDownloadService _downloadService;

    public CoursePage(ICourseService courseService, INavigationService navigation, Course course, IProgressService progressService, ICodeExecutionService codeExecutionService, ITutorService tutorService, IModelDownloadService downloadService)
    {
        InitializeComponent();
        _course = course;
        _courseService = courseService;
        _navigation = navigation;
        _progressService = progressService;
        _codeExecutionService = codeExecutionService;
        _tutorService = tutorService;
        _downloadService = downloadService;

        // Set up sidebar
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var sidebar = new CourseSidebar(course, courseService, navigation, progressService, codeExecutionService, tutorService, downloadService);
        mainWindow?.SetSidebarContent(sidebar);

        // Populate course info
        LanguageBadge.Text = course.Language;
        CourseTitle.Text = course.Title;
        CourseDescription.Text = course.Description;
        ModuleCount.Text = course.Modules.Count.ToString();
        LessonCount.Text = course.Modules.Sum(m => m.Lessons.Count).ToString();
        EstimatedTime.Text = course.EstimatedHours.ToString();

        LoadProgressAsync();
    }

    private async void LoadProgressAsync()
    {
        var stats = await _progressService.GetCourseProgressAsync(_course);

        if (stats.CompletedLessons > 0)
        {
            ProgressSection.Visibility = Visibility.Visible;

            // Calculate progress bar width (parent width * percent / 100)
            var progressWidth = ProgressFill.Parent is Grid grid
                ? grid.ActualWidth * stats.PercentComplete / 100
                : 0;
            ProgressFill.Width = progressWidth > 0 ? progressWidth : stats.PercentComplete * 2; // Fallback

            ProgressPercent.Text = $"{stats.PercentComplete}% Complete";
            CompletedLessonsText.Text = $"{stats.CompletedLessons} of {stats.TotalLessons} lessons completed";

            if (stats.CurrentStreak > 0)
            {
                StreakText.Visibility = Visibility.Visible;
                StreakText.Text = $"{stats.CurrentStreak}-day streak";
            }

            // Update button text for returning users
            StartButton.Content = new TextBlock { Text = "Continue Learning ->" };
        }
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var firstLesson = _course.Modules.FirstOrDefault()?.Lessons.FirstOrDefault();
        if (firstLesson != null)
        {
            var lessonPage = new LessonPage(_course, firstLesson, _courseService, _navigation, _progressService, _codeExecutionService, _tutorService, _downloadService);
            _navigation.NavigateTo(lessonPage, firstLesson);
        }
    }
}
