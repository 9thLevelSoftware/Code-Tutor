using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using CodeTutor.Wpf.Models;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace CodeTutor.Wpf.Services;

public class Phi4TutorService : ITutorService, IDisposable
{
    private Model? _model;
    private Tokenizer? _tokenizer;
    private readonly string _modelPath;
    private bool _disposed;

    public bool IsModelLoaded => _model != null && _tokenizer != null;
    public int LoadingProgress { get; private set; }
    public event EventHandler<int>? LoadingProgressChanged;

    public Phi4TutorService()
    {
        // Model path relative to application directory
        _modelPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "models", "phi4", "gpu", "gpu-int4-rtn-block-32");
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
                    "The AI Tutor model was not found. Please reopen the AI Tutor panel to download it.");
            }

            UpdateProgress(30);
            _model = new Model(_modelPath);

            UpdateProgress(70);
            _tokenizer = new Tokenizer(_model);

            UpdateProgress(100);
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

        // OPTIMIZATION: Build prompt with reduced context for faster tokenization
        var prompt = BuildPromptOptimized(userMessage, context, history);

        // OPTIMIZATION: Yield immediately to show "Thinking..." indicator
        await Task.Yield();

        // Tokenization happens here - can take 1-3 seconds for long prompts
        using var tokens = _tokenizer!.Encode(prompt);

        // OPTIMIZATION: Use faster generation settings
        // - max_length: 512 instead of 2048 (tutoring doesn't need long responses)
        // - do_sample: false for greedy decoding (faster than sampling)
        // - top_k: 1 equivalent to greedy when do_sample is true
        using var generatorParams = new GeneratorParams(_model!);
        generatorParams.SetSearchOption("max_length", 512);      // Reduced from 2048
        generatorParams.SetSearchOption("do_sample", false);      // Greedy = faster
        generatorParams.SetSearchOption("temperature", 0.7);      // Ignored when do_sample=false
        generatorParams.SetSearchOption("top_p", 0.9);            // Ignored when do_sample=false

        using var generator = new Generator(_model!, generatorParams);
        generator.AppendTokenSequences(tokens);

        // First token generation - often takes 5-15 seconds
        // Yield before starting to keep UI responsive
        await Task.Yield();

        int tokenCount = 0;
        while (!generator.IsDone())
        {
            cancellationToken.ThrowIfCancellationRequested();

            generator.GenerateNextToken();

            var tokenText = GetLastGeneratedToken(generator);

            // Skip special tokens
            if (!tokenText.Contains("<|") && !tokenText.Contains("|>"))
            {
                yield return tokenText;
            }

            // OPTIMIZATION: Yield every token with small delay for smoother streaming
            // This prevents UI freezing while maintaining generation speed
            tokenCount++;
            if (tokenCount % 1 == 0)
            {
                await Task.Delay(1);  // 1ms delay allows UI thread to process
            }
        }
    }

    /// <summary>
    /// Optimized prompt building with reduced context for faster inference.
    /// </summary>
    private string BuildPromptOptimized(string userMessage, TutorContext context, IReadOnlyList<TutorMessage> history)
    {
        var sb = new StringBuilder();

        // System prompt - keep concise
        sb.Append("<|system|>").Append('\n');
        sb.Append("You are a friendly programming tutor. Be concise and helpful.").Append('\n');

        // Add context if available (truncated for speed)
        if (!string.IsNullOrEmpty(context.CurrentLanguage))
            sb.Append($"Language: {context.CurrentLanguage}").Append('\n');
        if (!string.IsNullOrEmpty(context.LessonTitle))
            sb.Append($"Lesson: {context.LessonTitle}").Append('\n');
        if (!string.IsNullOrEmpty(context.UserCode))
        {
            // Truncate code to first 20 lines for faster processing
            var codeLines = context.UserCode.Split('\n').Take(20);
            sb.Append($"\nCode:\n```{context.CurrentLanguage?.ToLower() ?? ""}\n{string.Join("\n", codeLines)}\n```").Append('\n');
        }
        if (!string.IsNullOrEmpty(context.ExecutionError))
            sb.Append($"\nError: {context.ExecutionError}").Append('\n');
        sb.Append("<|end|>").Append('\n');

        // OPTIMIZATION: Only include last 3 messages instead of 6
        // Reduces tokenization time and memory usage
        var recentHistory = history.TakeLast(3);
        foreach (var msg in recentHistory)
        {
            var role = msg.Role == MessageRole.User ? "user" : "assistant";
            // Truncate long messages
            var content = msg.Content.Length > 200
                ? msg.Content.Substring(0, 200) + "..."
                : msg.Content;
            sb.Append($"<|{role}|>").Append('\n');
            sb.Append(content).Append('\n');
            sb.Append("<|end|>").Append('\n');
        }

        // Current user message (truncated)
        var truncatedMessage = userMessage.Length > 300
            ? userMessage.Substring(0, 300) + "..."
            : userMessage;
        sb.Append("<|user|>").Append('\n');
        sb.Append(truncatedMessage).Append('\n');
        sb.Append("<|end|>").Append('\n');
        sb.Append("<|assistant|>").Append('\n');

        return sb.ToString();
    }

    private string GetLastGeneratedToken(Generator generator)
    {
        var outputTokens = generator.GetSequence(0);
        var newToken = outputTokens[^1];
        return _tokenizer!.Decode(new ReadOnlySpan<int>(new[] { newToken }));
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

    /// <summary>
    /// Warms up the model with a dummy inference to reduce first-response latency.
    /// Call this after LoadModelAsync completes.
    /// </summary>
    public async Task WarmUpAsync(CancellationToken cancellationToken = default)
    {
        if (!IsModelLoaded) return;

        // Run dummy inference on background thread to warm up GPU/cache
        await Task.Run(() =>
        {
            try
            {
                var dummyPrompt = "<|system|>Hello<|end|><|user|>Hi<|end|><|assistant|>";
                using var tokens = _tokenizer!.Encode(dummyPrompt);
                using var generatorParams = new GeneratorParams(_model!);
                generatorParams.SetSearchOption("max_length", 10);
                generatorParams.SetSearchOption("do_sample", false);
                using var generator = new Generator(_model!, generatorParams);
                generator.AppendTokenSequences(tokens);

                // Generate just 2 tokens to warm up the model
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

    public void Dispose()
    {
        if (_disposed) return;
        UnloadModel();
        _disposed = true;
    }
}
