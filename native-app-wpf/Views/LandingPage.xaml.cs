using System;
using System.Windows;
using System.Windows.Controls;
using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Wpf.Views;

public partial class LandingPage : UserControl
{
    private readonly ICourseService _courseService;
    private readonly INavigationService _navigation;
    private readonly IProgressService _progressService;
    private readonly ICodeExecutionService _codeExecutionService;
    private readonly ITutorService _tutorService;
    private readonly IModelDownloadService _downloadService;

    public LandingPage(ICourseService courseService, INavigationService navigation, IProgressService progressService, ICodeExecutionService codeExecutionService, ITutorService tutorService, IModelDownloadService downloadService)
    {
        InitializeComponent();
        _courseService = courseService;
        _navigation = navigation;
        _progressService = progressService;
        _codeExecutionService = codeExecutionService;
        _tutorService = tutorService;
        _downloadService = downloadService;
        Loaded += LandingPage_Loaded;
    }

    private async void LandingPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Show skeleton loading
            SkeletonContainer.Visibility = Visibility.Visible;
            for (int i = 0; i < 4; i++)
            {
                SkeletonContainer.Children.Add(new Controls.SkeletonCard { Margin = new Thickness(0, 0, 16, 16) });
            }

            var courses = await _courseService.GetAllCoursesAsync();

            // Hide skeletons and show courses
            SkeletonContainer.Visibility = Visibility.Collapsed;
            SkeletonContainer.Children.Clear();
            CourseList.ItemsSource = courses;
        }
        catch (Exception ex)
        {
            SkeletonContainer.Visibility = Visibility.Collapsed;
            SkeletonContainer.Children.Clear();

            // Show error in a more visible way
            var errorText = new TextBlock
            {
                Text = $"Failed to load courses: {ex.Message}",
                TextWrapping = TextWrapping.Wrap
            };

            if (FindResource("BodyText") is Style bodyStyle)
            {
                errorText.Style = bodyStyle;
            }

            if (FindResource("AccentRedBrush") is System.Windows.Media.Brush redBrush)
            {
                errorText.Foreground = redBrush;
            }

            CourseList.ItemsSource = null;

            if (CourseList.Parent is Panel parentPanel)
            {
                parentPanel.Children.Add(errorText);
            }
        }
    }

    private void CourseCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Course course)
        {
            if (_navigation is NavigationService navService)
            {
                navService.NavigateWithFactory(() =>
                    new CoursePage(_courseService, _navigation, course, _progressService, _codeExecutionService, _tutorService, _downloadService));
            }
            else
            {
                var coursePage = new CoursePage(_courseService, _navigation, course, _progressService, _codeExecutionService, _tutorService, _downloadService);
                _navigation.NavigateTo(coursePage, course);
            }
        }
    }
}
