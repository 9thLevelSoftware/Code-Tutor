using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CodeTutor.Wpf.Models;

namespace CodeTutor.Wpf.Controls;

public partial class QuizChallenge : UserControl
{
    private readonly Challenge _challenge;
    private int _selectedIndex = -1;
    private readonly RadioButton[] _radioButtons;

    public event EventHandler<string>? ChallengeCompleted;
#pragma warning disable CS0067 // Event is required by LessonPage wiring but quizzes don't emit context changes
    public event EventHandler<ChallengeContextEventArgs>? ContextChanged;
#pragma warning restore CS0067
    public bool IsCompleted { get; private set; }
    public string ChallengeId => _challenge.Id;

    public QuizChallenge(Challenge challenge)
    {
        InitializeComponent();
        _challenge = challenge;

        QuizTitle.Text = challenge.Title;
        QuizDescription.Text = challenge.Description;
        QuestionText.Text = challenge.Question ?? challenge.Instructions;

        // Build radio button options
        var options = challenge.Options ?? new System.Collections.Generic.List<string>();
        _radioButtons = new RadioButton[options.Count];

        for (int i = 0; i < options.Count; i++)
        {
            var index = i;
            var radioButton = new RadioButton
            {
                GroupName = $"Quiz_{challenge.Id}",
                Margin = new Thickness(0, 0, 0, 8),
                Cursor = System.Windows.Input.Cursors.Hand,
            };

            var border = new Border
            {
                Background = (Brush)FindResource("BackgroundLightBrush"),
                BorderBrush = (Brush)FindResource("BorderDefaultBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12, 10, 12, 10),
            };

            var textBlock = new TextBlock
            {
                Text = options[i],
                Style = (Style)FindResource("BodyText"),
                TextWrapping = TextWrapping.Wrap,
            };

            border.Child = textBlock;
            radioButton.Content = border;

            radioButton.Checked += (s, e) =>
            {
                _selectedIndex = index;
                SubmitButton.IsEnabled = true;

                // Highlight selected option
                border.BorderBrush = (Brush)FindResource("AccentBlueBrush");
                border.BorderThickness = new Thickness(2);
            };

            radioButton.Unchecked += (s, e) =>
            {
                border.BorderBrush = (Brush)FindResource("BorderDefaultBrush");
                border.BorderThickness = new Thickness(1);
            };

            _radioButtons[i] = radioButton;
            OptionsPanel.Children.Add(radioButton);
        }
    }

    private void Submit_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedIndex < 0) return;

        // Determine correct answer from JSON
        int correctIndex = GetCorrectAnswerIndex();
        bool isCorrect = _selectedIndex == correctIndex;

        // Show result
        ResultPanel.Visibility = Visibility.Visible;

        if (isCorrect)
        {
            ResultText.Text = "Correct!";
            ResultText.Foreground = (Brush)FindResource("AccentGreenBrush");
            ResultPanel.Background = (Brush)FindResource("SuccessBackgroundBrush");
            ResultPanel.BorderBrush = (Brush)FindResource("AccentGreenBrush");
            ResultPanel.BorderThickness = new Thickness(1);

            if (!IsCompleted)
            {
                IsCompleted = true;
                ConfettiOverlay.Play();
                AchievementBadge.Show("Correct!", "Quiz passed");
                ChallengeCompleted?.Invoke(this, _challenge.Id);
            }
        }
        else
        {
            ResultText.Text = "Not quite right. Try again!";
            ResultText.Foreground = (Brush)FindResource("AccentRedBrush");
            ResultPanel.Background = (Brush)FindResource("ErrorBackgroundBrush");
            ResultPanel.BorderBrush = (Brush)FindResource("AccentRedBrush");
            ResultPanel.BorderThickness = new Thickness(1);
            ResetButton.Visibility = Visibility.Visible;

            // Highlight the correct answer
            if (correctIndex >= 0 && correctIndex < _radioButtons.Length)
            {
                if (_radioButtons[correctIndex].Content is Border correctBorder)
                {
                    correctBorder.BorderBrush = (Brush)FindResource("AccentGreenBrush");
                    correctBorder.BorderThickness = new Thickness(2);
                }
            }

            // Highlight wrong answer
            if (_radioButtons[_selectedIndex].Content is Border wrongBorder)
            {
                wrongBorder.BorderBrush = (Brush)FindResource("AccentRedBrush");
                wrongBorder.BorderThickness = new Thickness(2);
            }
        }

        // Show explanation
        if (!string.IsNullOrEmpty(_challenge.Explanation))
        {
            ExplanationText.Text = _challenge.Explanation;
            ExplanationText.Visibility = Visibility.Visible;
        }

        // Disable further selection
        foreach (var rb in _radioButtons)
        {
            rb.IsEnabled = false;
        }
        SubmitButton.IsEnabled = false;
    }

    internal static int GetCorrectAnswerIndex(JsonElement? correctAnswer)
    {
        if (correctAnswer == null) return -1;

        var element = correctAnswer.Value;

        return element.ValueKind switch
        {
            JsonValueKind.Number => element.GetInt32(),
            JsonValueKind.String => ParseAnswerString(element.GetString()),
            _ => -1,
        };
    }

    private int GetCorrectAnswerIndex() => GetCorrectAnswerIndex(_challenge.CorrectAnswer);

    private static int ParseAnswerString(string? value)
    {
        if (value == null) return -1;
        if (int.TryParse(value, out var idx)) return idx;
        // Handle letter answers: a=0, b=1, c=2, d=3
        if (value.Length == 1 && char.IsLetter(value[0]))
            return char.ToLower(value[0]) - 'a';
        return -1;
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _selectedIndex = -1;
        ResultPanel.Visibility = Visibility.Collapsed;
        ResetButton.Visibility = Visibility.Collapsed;
        SubmitButton.IsEnabled = false;

        foreach (var rb in _radioButtons)
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
}
