using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using CodeTutor.Wpf.Models;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Tutor service implementation for Qwen2.5-Coder-7B model.
/// Optimized for coding tasks with faster inference than Phi-4.
/// </summary>
public class QwenTutorService : ITutorService, IDisposable
{
    private Model? _model;
    private Tokenizer? _tokenizer;
    private readonly string _modelPath;
    private bool _disposed;

    public bool IsModelLoaded => _model != null && _tokenizer != null;
    public int LoadingProgress { get; private set; }
    public event EventHandler<int>? LoadingProgressChanged;

    public QwenTutorService()
    {
        _modelPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "models", "qwen2.5-coder-7b");
    }

    public async Task LoadModelAsync(CancellationToken cancellationToken = default)
    {
        if (IsModelLoaded) return;

        await Task.Run(() =>
        {
            UpdateProgress(10);

            if (!Directory.Exists(_modelPath))
            {
                throw new DirectoryNotFoundException(
                    "The Qwen2.5-Coder-7B model was not found. Please download it from the AI Tutor panel.");
            }

            UpdateProgress(30);
            _model = new Model(_modelPath);

            UpdateProgress(70);
            _tokenizer = new Tokenizer(_model);

            UpdateProgress(100);
        }, cancellationToken);
    }

    public async Task WarmUpAsync(CancellationToken cancellationToken = default)
    {
        if (!IsModelLoaded) return;

        await Task.Run(() =>
        {
            try
            {
                // Qwen chat template
                var dummyPrompt = "<|im_start|>system\nYou are a helpful assistant.<|im_end|>\n<|im_start|>user\nHello<|im_end|>\n<|im_start|>assistant\n";
                using var tokens = _tokenizer!.Encode(dummyPrompt);
                
                using var generatorParams = new GeneratorParams(_model!);
                generatorParams.SetSearchOption("max_length", 10);
                generatorParams.SetSearchOption("do_sample", false);
                
                using var generator = new Generator(_model!, generatorParams);
                generator.AppendTokenSequences(tokens);
                
                for (int i = 0; i < 2 && !generator.IsDone(); i++)
                {
                    generator.GenerateNextToken();
                }
            }
            catch
            {
                // Warm-up failures are non-critical
            }
        }, cancellationToken);
    }

    public async IAsyncEnumerable<string> SendMessageAsync(
        string userMessage,
        TutorContext context,
        IReadOnlyList<TutorMessage> history,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!IsModelLoaded)
            throw new InvalidOperationException("Model not loaded. Call LoadModelAsync first.");

        var prompt = BuildPromptOptimized(userMessage, context, history);

        // Yield immediately to show "Thinking..." indicator
        await Task.Yield();

        using var tokens = _tokenizer!.Encode(prompt);

        // Optimized for speed with Qwen2.5-Coder-7B
        using var generatorParams = new GeneratorParams(_model!);
        generatorParams.SetSearchOption("max_length", 512);
        generatorParams.SetSearchOption("do_sample", false); // Greedy = faster

        using var generator = new Generator(_model!, generatorParams);
        generator.AppendTokenSequences(tokens);

        await Task.Yield();

        int tokenCount = 0;
        while (!generator.IsDone())
        {
            cancellationToken.ThrowIfCancellationRequested();

            generator.GenerateNextToken();

            var tokenText = GetLastGeneratedToken(generator);

            // Skip special tokens for Qwen
            if (!tokenText.Contains("<|") && !tokenText.Contains("|>"))
            {
                yield return tokenText;
            }

            tokenCount++;
            if (tokenCount % 1 == 0)
            {
                await Task.Delay(1);
            }
        }
    }

    private string GetLastGeneratedToken(Generator generator)
    {
        var outputTokens = generator.GetSequence(0);
        var newToken = outputTokens[^1];
        return _tokenizer!.Decode(new ReadOnlySpan<int>(new[] { newToken }));
    }

    private string BuildPromptOptimized(string userMessage, TutorContext context, IReadOnlyList<TutorMessage> history)
    {
        var sb = new StringBuilder();

        // Qwen chat template uses <|im_start|> and <|im_end|>
        sb.Append("<|im_start|>system").Append('\n');
        sb.Append("You are an expert coding tutor specializing in helping beginners learn programming.").Append('\n');
        sb.Append("Guidelines:").Append('\n');
        sb.Append("- Explain concepts clearly with code examples").Append('\n');
        sb.Append("- Help debug errors step by step").Append('\n');
        sb.Append("- Encourage good coding practices").Append('\n');
        sb.Append("- Be concise but thorough").Append('\n');
        sb.Append("- Focus on practical, working solutions").Append('\n');

        if (!string.IsNullOrEmpty(context.CurrentLanguage))
            sb.Append($"\nProgramming language: {context.CurrentLanguage}").Append('\n');
        if (!string.IsNullOrEmpty(context.LessonTitle))
            sb.Append($"Current lesson: {context.LessonTitle}").Append('\n');
        if (!string.IsNullOrEmpty(context.UserCode))
        {
            var codeLines = context.UserCode.Split('\n').Take(20);
            sb.Append($"\nStudent's code:\n```{context.CurrentLanguage?.ToLower() ?? ""}\n{string.Join("\n", codeLines)}\n```").Append('\n');
        }
        if (!string.IsNullOrEmpty(context.ExecutionError))
            sb.Append($"\nError message: {context.ExecutionError}").Append('\n');

        sb.Append("<|im_end|>").Append('\n');

        // Add conversation history (last 3 messages)
        var recentHistory = history.TakeLast(3);
        foreach (var msg in recentHistory)
        {
            var role = msg.Role == MessageRole.User ? "user" : "assistant";
            var content = msg.Content.Length > 200
                ? msg.Content.Substring(0, 200) + "..."
                : msg.Content;
            sb.Append($"<|im_start|>{role}").Append('\n');
            sb.Append(content).Append('\n');
            sb.Append("<|im_end|>").Append('\n');
        }

        // Current user message
        var truncatedMessage = userMessage.Length > 300
            ? userMessage.Substring(0, 300) + "..."
            : userMessage;
        sb.Append("<|im_start|>user").Append('\n');
        sb.Append(truncatedMessage).Append('\n');
        sb.Append("<|im_end|>").Append('\n');
        sb.Append("<|im_start|>assistant").Append('\n');

        return sb.ToString();
    }

    private void UpdateProgress(int progress)
    {
        LoadingProgress = progress;
        LoadingProgressChanged?.Invoke(this, progress);
    }

    public void UnloadModel()
    {
        _tokenizer?.Dispose();
        _tokenizer = null;
        _model?.Dispose();
        _model = null;
        LoadingProgress = 0;
    }

    public void Dispose()
    {
        if (_disposed) return;
        UnloadModel();
        _disposed = true;
    }
}
