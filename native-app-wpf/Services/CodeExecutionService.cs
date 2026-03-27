using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CodeTutor.Wpf.Services.Executors;

namespace CodeTutor.Wpf.Services;

public interface ICodeExecutionService
{
    Task<ExecutionResult> ExecuteAsync(string code, string language);
    Task<IInteractiveSession> StartInteractiveSessionAsync(string code, string language);
    Task<RuntimeInfo> GetRuntimeInfoAsync(string language);
    bool IsPistonAvailable { get; }
}

public record ExecutionResult(bool Success, string Output, string Error);

public class CodeExecutionService : ICodeExecutionService
{
    private readonly RoslynCSharpExecutor _roslynExecutor;
    private readonly PistonExecutor _pistonExecutor;
    private readonly RuntimeDetectionService _runtimeDetection;
    private bool? _pistonAvailable;
    private Task<bool>? _pistonCheckTask;

    public bool IsPistonAvailable => _pistonAvailable ?? false;

    public CodeExecutionService()
    {
        _roslynExecutor = new RoslynCSharpExecutor();
        _pistonExecutor = new PistonExecutor();
        _runtimeDetection = new RuntimeDetectionService();

        // Start check but store the task for lazy initialization
        _pistonCheckTask = CheckPistonAsync();
    }

    private async Task<bool> CheckPistonAsync()
    {
        var available = await _pistonExecutor.IsAvailableAsync();
        _pistonAvailable = available;
        return available;
    }

    public async Task<RuntimeInfo> GetRuntimeInfoAsync(string language)
    {
        return await _runtimeDetection.CheckRuntimeAsync(language);
    }

    public async Task<ExecutionResult> ExecuteAsync(string code, string language)
    {
        // For C# and Piston (if available), keep the old batch behavior for now
        // or wrap the interactive session in a batch wait.
        
        // Input validation
        if (string.IsNullOrWhiteSpace(language))
            return new ExecutionResult(false, "", "Language cannot be empty");

        var langLower = language.ToLowerInvariant();

        // C# always uses Roslyn (no external dependency)
        if (langLower is "csharp" or "c#")
        {
            return await _roslynExecutor.ExecuteAsync(code);
        }
        
        // Existing Piston logic (omitted for brevity in this interactive focused update, 
        // assuming local execution is prioritized for interactivity)
        if (_pistonCheckTask != null && !_pistonCheckTask.IsCompleted) await _pistonCheckTask;
        if (_pistonAvailable == true) {
             var pistonResult = await _pistonExecutor.ExecuteAsync(code, language);
             if (pistonResult.Success || !pistonResult.Error.Contains("connection failed")) return pistonResult;
        }

        // Use the new interactive session mechanism but wait for it to finish for backward compatibility
        var outputBuilder = new System.Text.StringBuilder();
        var errorBuilder = new System.Text.StringBuilder();
        var tcs = new TaskCompletionSource<int>();

        try
        {
            using var session = await StartInteractiveSessionAsync(code, language);
            session.OutputReceived += (s, data) => outputBuilder.Append(data);
            session.ErrorReceived += (s, data) => errorBuilder.Append(data);
            session.Exited += (s, code) => tcs.TrySetResult(code);
            session.Start();

            // Wait for exit or timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var reg = cts.Token.Register(() => {
                session.StopAsync();
                tcs.TrySetResult(-1);
            });

            var exitCode = await tcs.Task;
            
            return new ExecutionResult(
                exitCode == 0,
                outputBuilder.ToString().Trim(),
                errorBuilder.ToString().Trim()
            );
        }
        catch (Exception ex)
        {
            return new ExecutionResult(false, "", ex.Message);
        }
    }

    public async Task<IInteractiveSession> StartInteractiveSessionAsync(string code, string language)
    {
        var langLower = language.ToLowerInvariant();
        
        return langLower switch
        {
            "python" => await StartLocalSessionAsync("python", code, ".py", "-u"), // -u for unbuffered output
            "javascript" or "js" => await StartLocalSessionAsync("node", code, ".js"), // run script file directly
            "java" => await StartJavaSessionAsync(code),
            "kotlin" => await StartLocalSessionAsync("kotlin", code, ".kts"), // kotlin runs .kts scripts directly
            // "rust" => await StartRustSessionAsync(code), // Needs compilation step first
            "dart" or "flutter" => await StartLocalSessionAsync("dart", code, ".dart"), // dart runs .dart files directly without 'run' subcommand
            _ => throw new NotSupportedException($"Interactive mode not supported for '{language}'")
        };
    }

    private async Task<IInteractiveSession> StartLocalSessionAsync(string command, string code, string extension, string? extraArgs = null)
    {
        var runtimeInfo = await _runtimeDetection.CheckRuntimeAsync(command);
        if (!runtimeInfo.IsAvailable)
        {
             throw new Exception($"{command} is not installed.\n\n{runtimeInfo.InstallHint}");
        }

        var tempFile = Path.Combine(Path.GetTempPath(), $"codetutor_{Guid.NewGuid()}{extension}");
        await File.WriteAllTextAsync(tempFile, code);
        // Note: Temp file cleanup is tricky with async sessions. 
        // ideally we track it and delete on session dispose.
        // For now, we leave it (OS cleans temp eventually) or we need a wrapper.

        var args = extraArgs != null ? $"{extraArgs} \"{tempFile}\"" : $"\"{tempFile}\"";
        
        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true, // KEY CHANGE
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = Process.Start(psi);
        if (process == null) throw new Exception("Failed to start process");
        
        process.EnableRaisingEvents = true;

        return new InteractiveProcessSession(process, tempFilePath: tempFile);
    }

    private async Task<IInteractiveSession> StartJavaSessionAsync(string code)
    {
        var runtimeInfo = await _runtimeDetection.CheckRuntimeAsync("java");
        if (!runtimeInfo.IsAvailable) throw new Exception("Java not installed");

        var className = "Main";
        var classMatch = Regex.Match(code, @"public\s+class\s+(\w+)");
        if (classMatch.Success) className = classMatch.Groups[1].Value;

        var tempDir = Path.Combine(Path.GetTempPath(), $"codetutor_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        var javaFile = Path.Combine(tempDir, $"{className}.java");
        await File.WriteAllTextAsync(javaFile, code);

        // Compile first
        var compilePsi = new ProcessStartInfo
        {
            FileName = "javac",
            Arguments = $"\"{javaFile}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using var compileProc = Process.Start(compilePsi);
        await compileProc!.WaitForExitAsync();
        
        if (compileProc.ExitCode != 0)
        {
            var error = await compileProc.StandardError.ReadToEndAsync();
            throw new Exception($"Compilation failed: {error}");
        }

        // Run
        var psi = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-cp \"{tempDir}\" {className}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = Process.Start(psi);
        if (process == null) throw new Exception("Failed to start Java process");
        process.EnableRaisingEvents = true;

        return new InteractiveProcessSession(process, tempDirPath: tempDir);
    }
}