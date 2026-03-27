using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CodeTutor.Wpf.Models;

namespace CodeTutor.Wpf.Controls;

public partial class TrueFalseChallenge : UserControl
{
    private readonly Challenge _challenge;
    private readonly bool _correctAnswer;

    public event EventHandler<string>? ChallengeCompleted;
#pragma warning disable CS0067 // Event is required by LessonPage wiring but true/false challenges don't emit context changes
    public event EventHandler<ChallengeContextEventArgs>? ContextChanged;
#pragma warning restore CS0067
    public bool IsCompleted { get; private set; }
    public string ChallengeId => _challenge.Id;

    public TrueFalseChallenge(Challenge challenge)
    {
        InitializeComponent();
        _challenge = challenge;
        _correctAnswer = challenge.IsTrue ?? true;

        TfTitle.Text = challenge.Title;
        TfDescription.Text = challenge.Description;
        StatementText.Text = challenge.Statement ?? challenge.Question ?? challenge.Instructions;
    }

    private void TrueButton_Click(object sender, RoutedEventArgs e) => CheckAnswer(true);
    private void FalseButton_Click(object sender, RoutedEventArgs e) => CheckAnswer(false);

    private void CheckAnswer(bool userAnswer)
    {
        bool isCorrect = userAnswer == _correctAnswer;

        ResultPanel.Visibility = Visibility.Visible;
        TrueButton.IsEnabled = false;
        FalseButton.IsEnabled = false;

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
                AchievementBadge.Show("Correct!", "True/False passed");
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
        }

        if (!string.IsNullOrEmpty(_challenge.Explanation))
        {
            ExplanationText.Text = _challenge.Explanation;
            ExplanationText.Visibility = Visibility.Visible;
        }
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        ResultPanel.Visibility = Visibility.Collapsed;
        ResetButton.Visibility = Visibility.Collapsed;
        TrueButton.IsEnabled = true;
        FalseButton.IsEnabled = true;
    }
}
