using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CodeTutor.Wpf.Models;

namespace CodeTutor.Wpf.Controls;

public partial class MultiQuizChallenge : UserControl
{
    private readonly Challenge _challenge;
    private readonly List<QuestionState> _questionStates = new();

    public event EventHandler<string>? ChallengeCompleted;
#pragma warning disable CS0067
    public event EventHandler<ChallengeContextEventArgs>? ContextChanged;
#pragma warning restore CS0067
    public bool IsCompleted { get; private set; }
    public string ChallengeId => _challenge.Id;

    public MultiQuizChallenge(Challenge challenge)
    {
        InitializeComponent();
        _challenge = challenge;

        QuizTitle.Text = challenge.Title;
        QuizDescription.Text = challenge.Description;

        var questions = challenge.Questions ?? new List<QuizQuestion>();
        ProgressText.Text = $"0 of {questions.Count} answered";

        for (int qi = 0; qi < questions.Count; qi++)
        {
            var q = questions[qi];
            var state = new QuestionState { QuestionIndex = qi, Question = q };
            _questionStates.Add(state);
            BuildQuestionUI(state, qi + 1, questions.Count);
        }
    }

    private void BuildQuestionUI(QuestionState state, int number, int total)
    {
        var container = new Border
        {
            Background = (Brush)FindResource("BackgroundLightBrush"),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 0, 0, 12),
        };

        var stack = new StackPanel();

        // Question text
        var questionText = new TextBlock
        {
            Text = $"Q{number}. {state.Question.QuestionText}",
            Style = (Style)FindResource("BodyText"),
            TextWrapping = TextWrapping.Wrap,
            FontWeight = FontWeights.Medium,
            Margin = new Thickness(0, 0, 0, 10),
        };
        stack.Children.Add(questionText);

        // Radio buttons for options
        var options = state.Question.Options;
        state.RadioButtons = new RadioButton[options.Count];

        for (int i = 0; i < options.Count; i++)
        {
            var index = i;
            var rb = new RadioButton
            {
                GroupName = $"MQ_{_challenge.Id}_{state.QuestionIndex}",
                Margin = new Thickness(0, 0, 0, 6),
                Cursor = System.Windows.Input.Cursors.Hand,
            };

            var optionBorder = new Border
            {
                Background = Brushes.Transparent,
                BorderBrush = (Brush)FindResource("BorderDefaultBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10, 8, 10, 8),
            };

            var optionText = new TextBlock
            {
                Text = options[i],
                Style = (Style)FindResource("BodyText"),
                TextWrapping = TextWrapping.Wrap,
            };

            optionBorder.Child = optionText;
            rb.Content = optionBorder;

            rb.Checked += (s, e) =>
            {
                state.SelectedIndex = index;
                optionBorder.BorderBrush = (Brush)FindResource("AccentBlueBrush");
                optionBorder.BorderThickness = new Thickness(2);
                UpdateAnsweredCount();
            };
            rb.Unchecked += (s, e) =>
            {
                optionBorder.BorderBrush = (Brush)FindResource("BorderDefaultBrush");
                optionBorder.BorderThickness = new Thickness(1);
            };

            state.RadioButtons[i] = rb;
            stack.Children.Add(rb);
        }

        // Per-question result (hidden until submit)
        state.ResultText = new TextBlock
        {
            Style = (Style)FindResource("BodyText"),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 8, 0, 0),
            Visibility = Visibility.Collapsed,
        };
        stack.Children.Add(state.ResultText);

        container.Child = stack;
        state.Container = container;
        QuestionsPanel.Children.Add(container);
    }

    private void UpdateAnsweredCount()
    {
        int answered = _questionStates.Count(s => s.SelectedIndex >= 0);
        int total = _questionStates.Count;
        ProgressText.Text = $"{answered} of {total} answered";
        SubmitAllButton.IsEnabled = answered == total;
    }

    private void SubmitAll_Click(object sender, RoutedEventArgs e)
    {
        int correct = 0;

        foreach (var state in _questionStates)
        {
            int correctIndex = QuizChallenge.GetCorrectAnswerIndex(state.Question.CorrectAnswer);
            bool isCorrect = state.SelectedIndex == correctIndex;

            if (isCorrect)
            {
                correct++;
                state.Container!.BorderBrush = (Brush)FindResource("AccentGreenBrush");
                state.Container.BorderThickness = new Thickness(2);

                if (!string.IsNullOrEmpty(state.Question.Explanation))
                {
                    state.ResultText!.Text = state.Question.Explanation;
                    state.ResultText.Foreground = (Brush)FindResource("AccentGreenBrush");
                    state.ResultText.Visibility = Visibility.Visible;
                }
            }
            else
            {
                state.Container!.BorderBrush = (Brush)FindResource("AccentRedBrush");
                state.Container.BorderThickness = new Thickness(2);

                var explanation = state.Question.Explanation ?? "";
                state.ResultText!.Text = $"Incorrect. {explanation}";
                state.ResultText.Foreground = (Brush)FindResource("AccentRedBrush");
                state.ResultText.Visibility = Visibility.Visible;

                // Highlight correct answer
                if (correctIndex >= 0 && correctIndex < state.RadioButtons.Length)
                {
                    if (state.RadioButtons[correctIndex].Content is Border correctBorder)
                    {
                        correctBorder.BorderBrush = (Brush)FindResource("AccentGreenBrush");
                        correctBorder.BorderThickness = new Thickness(2);
                    }
                }
            }

            // Disable radio buttons
            foreach (var rb in state.RadioButtons)
                rb.IsEnabled = false;
        }

        int total = _questionStates.Count;
        OverallResultPanel.Visibility = Visibility.Visible;

        if (correct == total)
        {
            OverallResultText.Text = $"All {total} correct!";
            OverallResultText.Foreground = (Brush)FindResource("AccentGreenBrush");
            OverallResultPanel.Background = (Brush)FindResource("SuccessBackgroundBrush");
            OverallResultPanel.BorderBrush = (Brush)FindResource("AccentGreenBrush");
            OverallResultPanel.BorderThickness = new Thickness(1);

            if (!IsCompleted)
            {
                IsCompleted = true;
                ConfettiOverlay.Play();
                AchievementBadge.Show("Perfect!", $"{total}/{total} correct");
                ChallengeCompleted?.Invoke(this, _challenge.Id);
            }
        }
        else
        {
            OverallResultText.Text = $"{correct} of {total} correct. Try again!";
            OverallResultText.Foreground = (Brush)FindResource("AccentRedBrush");
            OverallResultPanel.Background = (Brush)FindResource("ErrorBackgroundBrush");
            OverallResultPanel.BorderBrush = (Brush)FindResource("AccentRedBrush");
            OverallResultPanel.BorderThickness = new Thickness(1);
            ResetAllButton.Visibility = Visibility.Visible;
        }

        SubmitAllButton.IsEnabled = false;
    }

    private void ResetAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var state in _questionStates)
        {
            state.SelectedIndex = -1;
            state.Container!.BorderBrush = Brushes.Transparent;
            state.Container.BorderThickness = new Thickness(0);
            state.ResultText!.Visibility = Visibility.Collapsed;

            foreach (var rb in state.RadioButtons)
            {
                rb.IsEnabled = true;
                rb.IsChecked = false;
                if (rb.Content is Border border)
                {
                    border.BorderBrush = (Brush)FindResource("BorderDefaultBrush");
                    border.BorderThickness = new Thickness(1);
                }
            }
        }

        OverallResultPanel.Visibility = Visibility.Collapsed;
        ResetAllButton.Visibility = Visibility.Collapsed;
        SubmitAllButton.IsEnabled = false;
        UpdateAnsweredCount();
    }

    private class QuestionState
    {
        public int QuestionIndex { get; set; }
        public QuizQuestion Question { get; set; } = null!;
        public int SelectedIndex { get; set; } = -1;
        public RadioButton[] RadioButtons { get; set; } = Array.Empty<RadioButton>();
        public Border? Container { get; set; }
        public TextBlock? ResultText { get; set; }
    }
}
