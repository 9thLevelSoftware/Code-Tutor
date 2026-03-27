using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Wpf.Views;

public partial class CourseSidebar : UserControl
{
    private readonly Course _course;
    private readonly ICourseService _courseService;
    private readonly INavigationService _navigation;
    private readonly Dictionary<string, bool> _expandedModules = new();
    private readonly IProgressService _progressService;
    private readonly ICodeExecutionService _codeExecutionService;
    private readonly ITutorService _tutorService;
    private readonly IModelDownloadService _downloadService;
    private Lesson? _currentLesson;
    private readonly Dictionary<string, Button> _lessonButtons = new();

    public CourseSidebar(Course course, ICourseService courseService, INavigationService navigation, IProgressService progressService, ICodeExecutionService codeExecutionService, ITutorService tutorService, IModelDownloadService downloadService)
    {
        InitializeComponent();
        _course = course;
        _courseService = courseService;
        _navigation = navigation;
        _progressService = progressService;
        _codeExecutionService = codeExecutionService;
        _tutorService = tutorService;
        _downloadService = downloadService;

        CourseTitle.Text = course.Title;
        ModulesList.ItemsSource = course.Modules;

        foreach (var module in course.Modules)
        {
            _expandedModules[module.Id] = true;
        }
    }

    public void SetCurrentLesson(Lesson lesson)
    {
        _currentLesson = lesson;
        UpdateLessonStyles();
    }

    private async void UpdateLessonStyles()
    {
        var progress = await _progressService.LoadProgressAsync();

        foreach (var (lessonId, button) in _lessonButtons)
        {
            if (_currentLesson?.Id == lessonId)
            {
                button.Style = (Style)FindResource("ActiveSidebarItemButton");  
            }
            else if (progress.CompletedLessons.Contains(lessonId))
            {
                button.Style = (Style)FindResource("CompletedSidebarItemButton");
            }
            else
            {
                button.Style = (Style)FindResource("SidebarItemButton");        
            }
        }

        if (_currentLesson != null && _lessonButtons.TryGetValue(_currentLesson.Id, out var activeButton))
        {
            MoveActiveIndicator(activeButton);
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        _navigation.GoBack();
    }

    private void CourseOverview_Click(object sender, RoutedEventArgs e)
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

    private void ModuleHeader_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Module module)
        {
            _expandedModules[module.Id] = !_expandedModules[module.Id];

            // Find the lessons panel within the parent and toggle visibility
            var parent = button.Parent as StackPanel;
            if (parent != null)
            {
                foreach (var child in parent.Children)
                {
                    if (child is ItemsControl lessonsPanel)
                    {
                        if (_expandedModules[module.Id])
                        {
                            AnimateExpand(lessonsPanel);
                        }
                        else
                        {
                            AnimateCollapse(lessonsPanel);
                        }
                    }
                }
            }

            // Update chevron
            if (button.Content is Grid grid)
            {
                foreach (var child in grid.Children)
                {
                    if (child is TextBlock chevron && chevron.Name == "ExpandIcon")
                    {
                        chevron.Text = _expandedModules[module.Id] ? "\u25BC" : "\u25B6";
                    }
                }
            }
        }
    }

    private void LessonItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Lesson lesson)
        {
            _lessonButtons[lesson.Id] = button;
            _currentLesson = lesson;
            UpdateLessonStyles();

            if (_navigation is NavigationService navService)
            {
                navService.NavigateWithFactory(() =>
                    new LessonPage(_course, lesson, _courseService, _navigation, _progressService, _codeExecutionService, _tutorService, _downloadService));
            }
            else
            {
                var lessonPage = new LessonPage(_course, lesson, _courseService, _navigation, _progressService, _codeExecutionService, _tutorService, _downloadService);
                _navigation.NavigateTo(lessonPage, lesson);
            }
        }
    }

    private void LessonItem_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Lesson lesson)
        {
            _lessonButtons[lesson.Id] = button;

            if (_currentLesson?.Id == lesson.Id)
            {
                button.Style = (Style)FindResource("ActiveSidebarItemButton");
                MoveActiveIndicator(button);
            }
        }
    }

    private void MoveActiveIndicator(Button button)
    {
        if (!button.IsLoaded || ActiveIndicatorLayer == null || ActiveIndicator == null)
            return;

        try
        {
            var transform = button.TransformToVisual(ActiveIndicatorLayer);
            var point = transform.Transform(new Point(0, 0));
            var targetHeight = Math.Max(16, button.ActualHeight - 4);
            var targetTop = point.Y + (button.ActualHeight - targetHeight) / 2;

            ActiveIndicator.Opacity = 1;
            Canvas.SetLeft(ActiveIndicator, 0);

            if (double.IsNaN(Canvas.GetTop(ActiveIndicator)))
            {
                Canvas.SetTop(ActiveIndicator, targetTop);
                ActiveIndicator.Height = targetHeight;
                return;
            }

            var duration = TimeSpan.FromMilliseconds(220);
            var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

            ActiveIndicator.BeginAnimation(Canvas.TopProperty, new DoubleAnimation
            {
                To = targetTop,
                Duration = duration,
                EasingFunction = easing
            });

            ActiveIndicator.BeginAnimation(FrameworkElement.HeightProperty, new DoubleAnimation
            {
                To = targetHeight,
                Duration = duration,
                EasingFunction = easing
            });
        }
        catch
        {
            // Ignore transform errors during initialization
        }
    }

    private void AnimateExpand(ItemsControl lessonsPanel)
    {
        lessonsPanel.Visibility = Visibility.Visible;
        lessonsPanel.Opacity = 0;

        if (lessonsPanel.RenderTransform is not TranslateTransform translate)
        {
            translate = new TranslateTransform();
            lessonsPanel.RenderTransform = translate;
        }

        translate.Y = -8;

        var duration = TimeSpan.FromMilliseconds(200);
        var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

        lessonsPanel.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, duration) { EasingFunction = easing });
        translate.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(-8, 0, duration) { EasingFunction = easing });
    }

    private void AnimateCollapse(ItemsControl lessonsPanel)
    {
        if (lessonsPanel.RenderTransform is not TranslateTransform translate)
        {
            translate = new TranslateTransform();
            lessonsPanel.RenderTransform = translate;
        }

        var duration = TimeSpan.FromMilliseconds(160);
        var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

        var opacityAnim = new DoubleAnimation(1, 0, duration) { EasingFunction = easing };
        opacityAnim.Completed += (_, _) =>
        {
            lessonsPanel.Visibility = Visibility.Collapsed;
            lessonsPanel.Opacity = 1;
            translate.Y = 0;
        };

        lessonsPanel.BeginAnimation(OpacityProperty, opacityAnim);
        translate.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(0, -8, duration) { EasingFunction = easing });
    }
}
