using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Tests.E2E.AITutor;

/// <summary>
/// Base class for AI Tutor service tests providing common mock implementations
/// and test data factories. Uses mock LLM clients to avoid loading actual models.
/// </summary>
public abstract class TutorServiceTestBase : IAsyncLifetime
{
    // Test data for different programming languages
    protected static class TestData
    {
        // C# Code Samples
        public const string CSharpHelloWorld = @"
using System;
class Program {
    static void Main() {
        Console.WriteLine(""Hello, World!"");
    }
}";

        public const string CSharpWithError = @"
using System;
class Program {
    static void Main() {
        Console.WriteLine(message); // Error: message not defined
    }
}";

        public const string CSharpSuboptimalCode = @"
// Using ArrayList instead of List<T>
using System.Collections;
ArrayList list = new ArrayList();
list.Add(1);
list.Add(""two"");
";

        // Python Code Samples
        public const string PythonHelloWorld = @"
def main():
    print('Hello, World!')

if __name__ == '__main__':
    main()
";

        public const string PythonWithError = @"
def greet(name):
    print('Hello, ' + undefined_variable)  # NameError

greet('Alice')
";

        public const string PythonSuboptimalCode = @"
# Using range(len()) instead of enumerate
items = ['a', 'b', 'c']
for i in range(len(items)):
    print(str(i) + ': ' + items[i])
";

        // Java Code Samples
        public const string JavaHelloWorld = @"
public class Main {
    public static void main(String[] args) {
        System.out.println(""Hello, World!"");
    }
}
";

        public const string JavaWithError = @"
public class Main {
    public static void main(String[] args) {
        String s = null;
        System.out.println(s.length()); // NullPointerException
    }
}
";

        public const string JavaSuboptimalCode = @"
// Using String concatenation in a loop
String result = "";
for (int i = 0; i < 1000; i++) {
    result += i; // Inefficient - creates new String each time
}
";

        // Compiler/Runtime Error Messages
        public const string CSharpCompileError = "error CS0103: The name 'message' does not exist in the current context";
        public const string PythonRuntimeError = "NameError: name 'undefined_variable' is not defined";
        public const string JavaRuntimeError = "Exception in thread \"main\" java.lang.NullPointerException";

        // Expected Responses (for mock setup)
        public const string MockHintResponse = "Consider checking if the variable is defined before using it.";
        public const string MockErrorExplanation = "This error occurs when you try to use a variable that hasn't been declared or assigned a value.";
        public const string MockImprovementSuggestion = "Consider using a generic List<T> instead of ArrayList for type safety.";
        public const string MockAnswerResponse = "In C#, you can declare a variable using the syntax: type name = value;";
    }

    // Helper to create a standard TutorContext
    protected static TutorContext CreateContext(
        string language = "C#",
        string? lessonTitle = null,
        string? userCode = null,
        string? executionError = null)
    {
        return new TutorContext
        {
            CurrentLanguage = language,
            LessonTitle = lessonTitle ?? "Test Lesson",
            LessonContent = "This is a test lesson about basic programming concepts.",
            UserCode = userCode,
            ExecutionError = executionError,
            ExpectedOutput = "Expected output here"
        };
    }

    // Helper to create conversation history
    protected static List<TutorMessage> CreateHistory(params (MessageRole Role, string Content)[] messages)
    {
        return messages.Select(m => new TutorMessage(m.Role, m.Content)).ToList();
    }

    // Mock implementation of ITutorService for testing without actual models
    protected class MockTutorService : ITutorService
    {
        private readonly Dictionary<string, string[]> _mockResponses;
        private bool _isLoaded = false;

        public bool IsModelLoaded => _isLoaded;
        public int LoadingProgress { get; private set; }
        public event EventHandler<int>? LoadingProgressChanged;

        public MockTutorService(Dictionary<string, string[]> mockResponses)
        {
            _mockResponses = mockResponses;
        }

        public Task LoadModelAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                UpdateProgress(10);
                Thread.Sleep(50); // Simulate loading time
                UpdateProgress(50);
                Thread.Sleep(50);
                UpdateProgress(100);
                _isLoaded = true;
            }, cancellationToken);
        }

        public Task WarmUpAsync(CancellationToken cancellationToken = default)
        {
            if (!_isLoaded)
                return Task.CompletedTask;

            return Task.Run(() =>
            {
                // Simulate warm-up
                Thread.Sleep(20);
            }, cancellationToken);
        }

        public async IAsyncEnumerable<string> SendMessageAsync(
            string userMessage,
            TutorContext context,
            IReadOnlyList<TutorMessage> history,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!_isLoaded)
                throw new InvalidOperationException("Model not loaded. Call LoadModelAsync first.");

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                yield return "I didn't receive a message. How can I help you today?";
                yield break;
            }

            // Check for timeout simulation
            if (userMessage.Contains("[TIMEOUT]"))
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                throw new OperationCanceledException("Operation timed out");
            }

            // Check for rate limit simulation
            if (userMessage.Contains("[RATE_LIMIT]"))
            {
                yield return "Rate limit exceeded. Please wait a moment before sending another message.";
                yield break;
            }

            // Get appropriate mock response based on message content
            var response = GetMockResponse(userMessage, context);

            foreach (var token in response)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return token;
                await Task.Delay(1); // Simulate streaming
            }
        }

        private string[] GetMockResponse(string userMessage, TutorContext context)
        {
            // Determine response type based on message intent
            var lowerMessage = userMessage.ToLower();

            if (lowerMessage.Contains("hint") || lowerMessage.Contains("help me with"))
                return _mockResponses.GetValueOrDefault("hint", new[] { TestData.MockHintResponse });

            if (lowerMessage.Contains("error") || lowerMessage.Contains("exception") || lowerMessage.Contains("fix"))
                return _mockResponses.GetValueOrDefault("explain_error", new[] { TestData.MockErrorExplanation });

            if (lowerMessage.Contains("improve") || lowerMessage.Contains("better") || lowerMessage.Contains("optimize"))
                return _mockResponses.GetValueOrDefault("improve", new[] { TestData.MockImprovementSuggestion });

            if (lowerMessage.Contains("what is") || lowerMessage.Contains("how do") || lowerMessage.Contains("explain"))
                return _mockResponses.GetValueOrDefault("answer", new[] { TestData.MockAnswerResponse });

            return _mockResponses.GetValueOrDefault("default", new[] { "I'm here to help with your programming questions." });
        }

        public void UnloadModel()
        {
            _isLoaded = false;
            LoadingProgress = 0;
        }

        private void UpdateProgress(int progress)
        {
            LoadingProgress = progress;
            LoadingProgressChanged?.Invoke(this, progress);
        }
    }

    // IAsyncLifetime implementation
    public virtual Task InitializeAsync() => Task.CompletedTask;
    public virtual Task DisposeAsync() => Task.CompletedTask;
}
