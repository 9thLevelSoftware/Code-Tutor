using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Behaviors;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Wpf.Controls;

public class ChallengeContextEventArgs : EventArgs
{
    public string ChallengeTitle { get; init; } = string.Empty;
    public string ChallengeInstructions { get; init; } = string.Empty;
    public string UserCode { get; init; } = string.Empty;
    public string? ExecutionError { get; init; }
    public string? ExpectedOutput { get; init; }
}

public partial class CodingChallenge : UserControl
{
    private readonly Challenge _challenge;
    private readonly ICodeExecutionService _executionService;
    private int _currentHintIndex = 0;
    private int _failedAttempts = 0;
    private readonly string _originalCode;

    // Interactive session state
    private IInteractiveSession? _currentSession;
    private CancellationTokenSource? _executionCts;
    private readonly System.Text.StringBuilder _outputBuffer = new();

    public event EventHandler<string>? ChallengeCompleted;
    public event EventHandler<ChallengeContextEventArgs>? ContextChanged;
    public bool IsCompleted { get; private set; }
    public string ChallengeId => _challenge.Id;

    public CodingChallenge(Challenge challenge, ICodeExecutionService executionService)
    {
        InitializeComponent();
        _challenge = challenge;
        _originalCode = challenge.StarterCode;
        _executionService = executionService;

        if (PerformanceProfile.IsSoftwareRendering && CodeEditorBorder.Effect is DropShadowEffect effect)
        {
            effect.BlurRadius = 0;
            effect.Opacity = 0;
        }

        ChallengeTitle.Text = challenge.Title;
        Description.Text = challenge.Description;
        Instructions.Text = challenge.Instructions;
        CodeEditor.Text = challenge.StarterCode;

        // Set syntax highlighting based on language
        var highlighting = SyntaxHighlightingService.GetHighlightingForLanguage(challenge.Language);
        if (highlighting != null)
        {
            CodeEditor.SyntaxHighlighting = highlighting;
        }

        // Set up status bar
        StatusBar.SetLanguage(challenge.Language);
        StatusBar.SetStatus("Ready", true);

        // Update position on caret change
        CodeEditor.TextArea.Caret.PositionChanged += (s, e) =>
        {
            StatusBar.UpdatePosition(
                CodeEditor.TextArea.Caret.Line,
                CodeEditor.TextArea.Caret.Column);
        };

        UpdateHintButtonState();

        // Check runtime availability asynchronously
        _ = CheckRuntimeAsync();
    }

    private async Task CheckRuntimeAsync()
    {
        var runtimeInfo = await _executionService.GetRuntimeInfoAsync(_challenge.Language);
        StatusBar.SetRuntimeStatus(runtimeInfo.IsAvailable, runtimeInfo.Version);

        if (!runtimeInfo.IsAvailable && !string.IsNullOrEmpty(runtimeInfo.InstallHint))
        {
            OutputPanel.Visibility = Visibility.Visible;
            OutputText.Text = $"Note: {runtimeInfo.InstallHint}";
            OutputText.Foreground = (System.Windows.Media.Brush)FindResource("AccentYellowBrush");
        }
    }

    private void UpdateHintButtonState()
    {
        if (_challenge.Hints == null || _challenge.Hints.Count == 0)
        {
            HintButton.Visibility = Visibility.Collapsed;
            return;
        }

        if (_failedAttempts == 0)
        {
            HintButton.Content = new TextBlock { Text = "Hint (run code first)" };
            HintButton.IsEnabled = false;
        }
        else if (_currentHintIndex >= _challenge.Hints.Count)
        {
            HintButton.Content = new TextBlock { Text = "No More Hints" };
            HintButton.IsEnabled = false;
        }
        else
        {
            int hintsAvailable = Math.Min(_failedAttempts, _challenge.Hints.Count);
            if (_currentHintIndex < hintsAvailable)
            {
                HintButton.Content = new TextBlock { Text = $"Show Hint ({_currentHintIndex + 1}/{hintsAvailable})" };
                HintButton.IsEnabled = true;
            }
            else
            {
                HintButton.Content = new TextBlock { Text = "Try again to unlock more hints" };
                HintButton.IsEnabled = false;
            }
        }
    }

    private async void RunCode_Click(object sender, RoutedEventArgs e)
    {
        await StopCurrentSessionAsync();

        _executionCts?.Dispose();
        _executionCts = new CancellationTokenSource();
        _outputBuffer.Clear();

        RunningIndicator.Visibility = Visibility.Visible;
        StopButton.Visibility = Visibility.Visible;
        InputPanel.Visibility = Visibility.Visible; // Show input box
        InputBox.Clear();
        InputBox.Focus();

        StatusBar.SetStatus("Running...", false);
        
        OutputPanel.Visibility = Visibility.Visible;
        TestResultsPanel.Visibility = Visibility.Collapsed;
        OutputText.Text = ""; // Start clean
        OutputText.Foreground = (System.Windows.Media.Brush)FindResource("TextPrimaryBrush");
        OutputPanel.BorderBrush = (System.Windows.Media.Brush)FindResource("BorderDefaultBrush");
        OutputPanel.Background = (System.Windows.Media.Brush)FindResource("BackgroundDarkBrush");

        try
        {
            _currentSession = await _executionService.StartInteractiveSessionAsync(CodeEditor.Text, _challenge.Language);

            // Attach handlers BEFORE Start() so no events are missed
            _currentSession.OutputReceived += (s, data) => Dispatcher.BeginInvoke(() => AppendOutput(data, false));
            _currentSession.ErrorReceived += (s, data) => Dispatcher.BeginInvoke(() => AppendOutput(data, true));
            _currentSession.Exited += (s, exitCode) => Dispatcher.BeginInvoke(() => OnSessionExited(exitCode));
            _currentSession.Start();
        }
        catch (Exception ex)
        {
            OutputText.Text = $"Execution failed: {ex.Message}";
            OutputText.Foreground = (System.Windows.Media.Brush)FindResource("AccentRedBrush");
            RunningIndicator.Visibility = Visibility.Collapsed;
            StopButton.Visibility = Visibility.Collapsed;
            InputPanel.Visibility = Visibility.Collapsed;
            StatusBar.SetStatus("Error", true);
        }
    }

    private void AppendOutput(string text, bool isError)
    {
        _outputBuffer.Append(text);
        OutputText.Text += text;
        
        // Scroll to bottom
        if (OutputText.Parent is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToBottom();
        }
    }

    private void OnSessionExited(int exitCode)
    {
        RunningIndicator.Visibility = Visibility.Collapsed;
        StopButton.Visibility = Visibility.Collapsed;
        InputPanel.Visibility = Visibility.Collapsed;
        StatusBar.SetStatus("Ready", true);

        bool success = exitCode == 0;

        if (success)
        {
            OutputPanel.BorderBrush = (System.Windows.Media.Brush)FindResource("AccentGreenBrush");
            OutputPanel.Background = (System.Windows.Media.Brush)FindResource("SuccessBackgroundBrush");

            if (_challenge.TestCases != null && _challenge.TestCases.Count > 0)
            {
                ValidateTestCases(_outputBuffer.ToString());
            }
            else
            {
                RaiseContextChanged(executionError: null);
            }
        }
        else
        {
            OutputPanel.BorderBrush = (System.Windows.Media.Brush)FindResource("AccentRedBrush");
            OutputPanel.Background = (System.Windows.Media.Brush)FindResource("ErrorBackgroundBrush");
            _failedAttempts++;
            UpdateHintButtonState();
            RaiseContextChanged(executionError: _outputBuffer.ToString());
        }

        _currentSession?.Dispose();
        _currentSession = null;
    }

    private async Task StopCurrentSessionAsync()
    {
        if (_currentSession != null)
        {
            await _currentSession.StopAsync();
            _currentSession.Dispose();
            _currentSession = null;
        }
    }

    private async void StopExecution_Click(object sender, RoutedEventArgs e)
    {
        await StopCurrentSessionAsync();
        OutputText.Text += "\nExecution cancelled.";
        RunningIndicator.Visibility = Visibility.Collapsed;
        StopButton.Visibility = Visibility.Collapsed;
        InputPanel.Visibility = Visibility.Collapsed;
        StatusBar.SetStatus("Cancelled", true);
    }

    private async void SendInput_Click(object sender, RoutedEventArgs e)
    {
        await SendInputAsync();
    }

    private async void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await SendInputAsync();
        }
    }

    private async Task SendInputAsync()
    {
        if (_currentSession == null) return;

        var text = InputBox.Text;
        InputBox.Clear();
        
        // Echo input to output for better UX
        AppendOutput(text + Environment.NewLine, false);
        
        await _currentSession.InputAsync(text);
    }

    private void ValidateTestCases(string actualOutput)
    {
        var results = new List<object>();
        int passed = 0;
        int total = 0;

        foreach (var testCase in _challenge.TestCases)
        {
            if (!testCase.IsVisible) continue;
            total++;

            bool testPassed = string.IsNullOrEmpty(testCase.ExpectedOutput) ||
                              actualOutput.Contains(testCase.ExpectedOutput.Trim());

            if (testPassed) passed++;

            results.Add(new
            {
                Description = testCase.Description,
                Status = testPassed ? "\u2713" : "\u2717",
                Color = testPassed
                    ? (System.Windows.Media.Brush)FindResource("AccentGreenBrush")
                    : (System.Windows.Media.Brush)FindResource("AccentRedBrush")
            });
        }

        TestResultsPanel.Visibility = Visibility.Visible;
        TestResultsList.ItemsSource = results;

        bool allPassed = passed == total;
        TestSummary.Text = $"{passed}/{total} tests passed";
        TestSummary.Foreground = allPassed
            ? (System.Windows.Media.Brush)FindResource("AccentGreenBrush")
            : (System.Windows.Media.Brush)FindResource("AccentRedBrush");

        TestResultsPanel.BorderBrush = allPassed
            ? (System.Windows.Media.Brush)FindResource("AccentGreenBrush")
            : (System.Windows.Media.Brush)FindResource("AccentRedBrush");
        TestResultsPanel.Background = allPassed
            ? (System.Windows.Media.Brush)FindResource("SuccessBackgroundBrush")
            : (System.Windows.Media.Brush)FindResource("ErrorBackgroundBrush");

        if (!allPassed)
        {
            _failedAttempts++;
            UpdateHintButtonState();
        }

        if (allPassed && !IsCompleted)
        {
            TriggerSuccessCelebration();
            IsCompleted = true;
            ChallengeCompleted?.Invoke(this, _challenge.Id);
        }

        // Notify tutor context with test results
        var failedTests = _challenge.TestCases
            .Where(tc => tc.IsVisible && !string.IsNullOrEmpty(tc.ExpectedOutput) && !actualOutput.Contains(tc.ExpectedOutput.Trim()))
            .ToList();
        string? error = failedTests.Count > 0
            ? $"Failed tests: {string.Join("; ", failedTests.Select(t => t.Description))}"
            : null;
        string? expected = failedTests.Count > 0
            ? string.Join("\n", failedTests.Select(t => t.ExpectedOutput))
            : null;
        RaiseContextChanged(executionError: error, expectedOutput: expected);
    }

    private void TriggerSuccessCelebration()
    {
        AnimationBehaviors.SetTriggerSuccessFlash(CodeEditorBorder, true);
        ConfettiOverlay.Play();
        AchievementBadge.Show("Perfect!", "All tests passed");
    }

    private void RaiseContextChanged(string? executionError, string? expectedOutput = null)
    {
        ContextChanged?.Invoke(this, new ChallengeContextEventArgs
        {
            ChallengeTitle = _challenge.Title,
            ChallengeInstructions = _challenge.Instructions,
            UserCode = CodeEditor.Text,
            ExecutionError = executionError,
            ExpectedOutput = expectedOutput,
        });
    }

    private void ShowHint_Click(object sender, RoutedEventArgs e)
    {
        if (_challenge.Hints != null && _currentHintIndex < _challenge.Hints.Count && _currentHintIndex < _failedAttempts)
        {
            var hint = _challenge.Hints[_currentHintIndex];
            HintText.Text = hint.Text;
            HintPanel.Visibility = Visibility.Visible;
            _currentHintIndex++;
            UpdateHintButtonState();
        }
    }

    private void ShowSolution_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to see the solution? Try solving it yourself first!",
            "Show Solution",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            CodeEditor.Text = _challenge.Solution;
        }
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        CodeEditor.Text = _originalCode;
        OutputPanel.Visibility = Visibility.Collapsed;
        TestResultsPanel.Visibility = Visibility.Collapsed;
        HintPanel.Visibility = Visibility.Collapsed;
        InputPanel.Visibility = Visibility.Collapsed;
        _currentHintIndex = 0;
        _failedAttempts = 0;

        UpdateHintButtonState();
    }
}