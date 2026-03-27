using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CodeTutor.Wpf.Models;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Tutor service implementation using LLamaSharp with GGUF models.
/// Supports Qwen2.5-Coder and other GGUF format models without ONNX conversion.
/// </summary>
public class LlamaTutorService : ITutorService, IDisposable
{
    private LLamaWeights? _model;
    private LLamaContext? _context;
    private string _modelPath;
    private readonly string _systemPrompt;
    private bool _disposed;

    public bool IsModelLoaded => _model != null && _context != null;
    public int LoadingProgress { get; private set; }
    public event EventHandler<int>? LoadingProgressChanged;

    public LlamaTutorService()
    {
        // Get the directory where the assembly is located (works for both dotnet run and EXE)
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation) ?? AppDomain.CurrentDomain.BaseDirectory;
        
        // Model path for GGUF format
        _modelPath = Path.Combine(assemblyDirectory, "models", "qwen2.5-coder-7b", "model.gguf");
        
        _systemPrompt = "You are an expert coding tutor. The student is working through a programming course. " +
            "Context about their current lesson and code may be provided. Use it to give specific, relevant help. " +
            "Explain concepts clearly. Help debug errors step by step. Be concise.";
    }

    public async Task LoadModelAsync(CancellationToken cancellationToken = default)
    {
        if (IsModelLoaded) return;

        await Task.Run(() =>
        {
            UpdateProgress(10);

            // Debug: Log the path being used
            System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Looking for model at: {_modelPath}");
            System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] File exists: {File.Exists(_modelPath)}");

            if (!File.Exists(_modelPath))
            {
                // Try to find the model file in alternative locations
                var alternativePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "qwen2.5-coder-7b", "model.gguf"),
                    Path.Combine(Environment.CurrentDirectory, "models", "qwen2.5-coder-7b", "model.gguf"),
                    Path.Combine(Directory.GetCurrentDirectory(), "models", "qwen2.5-coder-7b", "model.gguf"),
                };

                string? foundPath = null;
                foreach (var altPath in alternativePaths)
                {
                    System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Trying alternative: {altPath}");
                    if (File.Exists(altPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Found at alternative path: {altPath}");
                        foundPath = altPath;
                        break;
                    }
                }

                if (foundPath == null)
                {
                    throw new FileNotFoundException(
                        $"The AI Tutor model was not found at '{_modelPath}'. " +
                        "Please download it from the AI Tutor panel.");
                }

                _modelPath = foundPath;
            }

            // Check file size (should be ~4.5GB for Q4_K_M)
            var fileInfo = new FileInfo(_modelPath);
            System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Model file size: {fileInfo.Length / 1024 / 1024}MB");
            
            if (fileInfo.Length < 4_000_000_000) // Less than 4GB is likely incomplete
            {
                throw new InvalidOperationException(
                    $"Model file appears incomplete ({fileInfo.Length / 1024 / 1024}MB, expected ~4500MB). Please re-download.");
            }

            UpdateProgress(30);

            // Load model parameters - simplified for compatibility
            var parameters = new ModelParams(_modelPath)
            {
                ContextSize = 4096,   // Standard context window
                GpuLayerCount = 0,    // CPU only for compatibility
                Threads = Environment.ProcessorCount > 1 ? Environment.ProcessorCount - 1 : 1,
            };

            UpdateProgress(50);
            _model = LLamaWeights.LoadFromFile(parameters);

            UpdateProgress(70);
            _context = _model.CreateContext(parameters);

            UpdateProgress(100);
        }, cancellationToken);
    }

    public async Task WarmUpAsync(CancellationToken cancellationToken = default)
    {
        if (!IsModelLoaded || _context == null) return;

        // Simple warm-up inference
        await Task.Run(() =>
        {
            try
            {
                // Use StatelessExecutor for simpler warm-up
                var executor = new StatelessExecutor(_model!, _context.Params);
                var prompt = $"System: {_systemPrompt}\nUser: Hello\nAssistant:";
                
                var result = executor.InferAsync(prompt, new InferenceParams { MaxTokens = 1 });
                var enumerator = result.GetAsyncEnumerator();
                enumerator.MoveNextAsync().AsTask().Wait();
                enumerator.DisposeAsync().AsTask().Wait();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Warm-up warning: {ex.Message}");
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
        if (!IsModelLoaded || _context == null || _model == null)
            throw new InvalidOperationException("Model not loaded. Call LoadModelAsync first.");

        // Build the full prompt with Qwen chat format
        var prompt = BuildQwenPrompt(userMessage, context, history);

        System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Prompt length: {prompt.Length} chars");
        System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Prompt:\n{prompt}");

        // Yield immediately to show "Thinking..."
        await Task.Yield();

        // Clear KV cache so the context starts fresh for each conversation turn
        _context.NativeHandle.MemoryClear();

        // Use InteractiveExecutor with the existing context (not StatelessExecutor which
        // creates a temporary context and may not parse special tokens correctly)
        var executor = new InteractiveExecutor(_context);

        var inferenceParams = new InferenceParams
        {
            MaxTokens = 512,
            AntiPrompts = new[] { "<|im_end|>" },
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.6f,
                TopP = 0.9f,
            },
        };

        System.Diagnostics.Debug.WriteLine("[LlamaTutorService] Starting inference with InteractiveExecutor...");
        var result = executor.InferAsync(prompt, inferenceParams);

        await using var enumerator = result.GetAsyncEnumerator(cancellationToken);
        bool hasTokens = false;
        int tokenCount = 0;

        while (await enumerator.MoveNextAsync())
        {
            hasTokens = true;
            tokenCount++;
            if (tokenCount <= 3 || tokenCount % 50 == 0)
                System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Token #{tokenCount}: \"{enumerator.Current}\"");
            yield return enumerator.Current;
        }

        System.Diagnostics.Debug.WriteLine($"[LlamaTutorService] Inference complete. Total tokens: {tokenCount}");

        if (!hasTokens)
        {
            System.Diagnostics.Debug.WriteLine("[LlamaTutorService] WARNING: No tokens generated!");
            yield return "[No response generated. The model may need more time to load.]";
        }
    }

    /// <summary>
    /// Builds a prompt in Qwen chat format.
    /// Qwen2.5 uses: <|im_start|>system\n...\n<|im_end|>\n<|im_start|>user\n...\n<|im_end|>\n<|im_start|>assistant\n
    /// </summary>
    private string BuildQwenPrompt(string userMessage, TutorContext context, IReadOnlyList<TutorMessage> history)
    {
        var sb = new StringBuilder();

        // System message
        // Use Append + '\n' instead of AppendLine to avoid Windows \r\n breaking tokenizer special tokens
        sb.Append("<|im_start|>system\n");
        sb.Append(_systemPrompt).Append('\n');
        sb.Append("<|im_end|>\n");

        // Add conversation history
        if (history != null)
        {
            foreach (var msg in history.TakeLast(4)) // Keep last 4 messages for context
            {
                if (msg.Role == MessageRole.User)
                {
                    sb.Append("<|im_start|>user\n");
                    sb.Append(msg.Content).Append('\n');
                    sb.Append("<|im_end|>\n");
                }
                else if (msg.Role == MessageRole.Assistant)
                {
                    sb.Append("<|im_start|>assistant\n");
                    sb.Append(msg.Content).Append('\n');
                    sb.Append("<|im_end|>\n");
                }
            }
        }

        // Current user message with context
        sb.Append("<|im_start|>user\n");

        // Add context metadata
        if (!string.IsNullOrEmpty(context.CurrentLanguage))
            sb.Append($"[Language: {context.CurrentLanguage}]\n");
        if (!string.IsNullOrEmpty(context.LessonTitle))
            sb.Append($"[Lesson: {context.LessonTitle}]\n");
        if (!string.IsNullOrEmpty(context.LessonContent))
            sb.Append($"[Lesson Content:]\n{context.LessonContent}\n");
        if (!string.IsNullOrEmpty(context.UserCode))
        {
            var codeLines = context.UserCode.Split('\n').Take(30);
            sb.Append($"[Code:]\n```{context.CurrentLanguage?.ToLower() ?? ""}\n{string.Join("\n", codeLines)}\n```\n");
        }
        if (!string.IsNullOrEmpty(context.ExecutionError))
            sb.Append($"[Error: {context.ExecutionError}]\n");
        if (!string.IsNullOrEmpty(context.ExpectedOutput))
            sb.Append($"[Expected Output: {context.ExpectedOutput}]\n");

        sb.Append(userMessage).Append('\n');
        sb.Append("<|im_end|>\n");

        // Assistant prefix
        sb.Append("<|im_start|>assistant\n");

        return sb.ToString();
    }

    private void UpdateProgress(int progress)
    {
        LoadingProgress = progress;
        LoadingProgressChanged?.Invoke(this, progress);
    }

    public void UnloadModel()
    {
        _context?.Dispose();
        _context = null;
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
