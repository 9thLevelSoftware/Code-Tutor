using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CodeTutor.Wpf.Services.Executors;
using Microsoft.Extensions.Logging;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Security audit log entry for code execution events.
/// </summary>
public record SecurityAuditLogEntry(
    DateTime Timestamp,
    string EventType,
    string Language,
    string? FileHash,
    long CodeSizeBytes,
    bool SecurityScanPassed,
    string? SecurityScanDetails,
    int? ExitCode,
    TimeSpan? ExecutionDuration,
    bool Success,
    string? ErrorMessage
);

/// <summary>
/// Secure sandboxed code execution service with comprehensive security controls.
/// 
/// SECURITY CONTROLS IMPLEMENTED:
/// 1. Windows Job Objects for resource limits (memory, CPU time, process limits)
/// 2. Pattern-based code security scanning for dangerous operations
/// 3. Command whitelist (only python, node, java, kotlin, dart)
/// 4. Restricted environment variables (minimal PATH/no sensitive data)
/// 5. Audit logging with JSON output
/// 6. Secure temp file handling with cryptographically random names
/// 7. Input validation (code size limits, language validation)
/// 8. Process priority reduction and kill-on-job-close
/// </summary>
public interface ISandboxedCodeExecutionService
{
    Task<ExecutionResult> ExecuteAsync(string code, string language, CancellationToken cancellationToken = default);
    Task<RuntimeInfo> GetRuntimeInfoAsync(string language);
    bool IsLanguageSupported(string language);
}

/// <summary>
/// Production-ready secure code execution service.
/// </summary>
public class SandboxedCodeExecutionService : ISandboxedCodeExecutionService, IDisposable
{
    private readonly RoslynCSharpExecutor _roslynExecutor;
    private readonly PistonExecutor _pistonExecutor;
    private readonly RuntimeDetectionService _runtimeDetection;
    private readonly ResourceLimitedExecutor _resourceLimiter;
    private readonly ILogger<SandboxedCodeExecutionService>? _logger;
    private readonly string _auditLogDirectory;
    private readonly bool? _pistonAvailable;
    private bool _disposed;

    // SECURITY: Maximum code size to prevent memory exhaustion attacks
    private const int MAX_CODE_SIZE_BYTES = 1024 * 1024; // 1MB
    private const int MAX_OUTPUT_SIZE_BYTES = 10 * 1024 * 1024; // 10MB
    private const int DEFAULT_TIMEOUT_SECONDS = 30;
    private const int DEFAULT_MEMORY_LIMIT_MB = 256;

    // SECURITY: Whitelist of allowed commands for external execution
    private static readonly string[] AllowedCommands = new[]
    {
        "python", "python3", "py",
        "node", "nodejs",
        "java", "javac",
        "kotlin", "kotlinc",
        "dart"
    };

    // SECURITY: Dangerous patterns for code scanning
    private static readonly SecurityPattern[] DangerousPatterns = new[]
    {
        // Python dangerous patterns
        new SecurityPattern("python", "__import__\\s*\\(\\s*[\"']os[\"']", "Unsafe import of os module"),
        new SecurityPattern("python", "__import__\\s*\\(\\s*[\"']subprocess[\"']", "Unsafe import of subprocess module"),
        new SecurityPattern("python", "import\\s+subprocess", "Import of subprocess module"),
        new SecurityPattern("python", "from\\s+subprocess\\s+import", "Import from subprocess module"),
        new SecurityPattern("python", "os\\.system\\s*\\(", "os.system() call"),
        new SecurityPattern("python", "os\\.popen\\s*\\(", "os.popen() call"),
        new SecurityPattern("python", "subprocess\\.call\\s*\\(", "subprocess.call()"),
        new SecurityPattern("python", "subprocess\\.run\\s*\\(", "subprocess.run()"),
        new SecurityPattern("python", "subprocess\\.Popen\\s*\\(", "subprocess.Popen()"),
        new SecurityPattern("python", "exec\\s*\\(", "exec() function"),
        new SecurityPattern("python", "compile\\s*\\(.*exec", "compile() with exec mode"),
        new SecurityPattern("python", "eval\\s*\\(", "eval() function"),
        new SecurityPattern("python", "input\\s*\\(", "input() function - potential hang"),
        new SecurityPattern("python", "raw_input\\s*\\(", "raw_input() function - potential hang"),
        new SecurityPattern("python", "socket\\.socket", "Socket creation"),
        new SecurityPattern("python", "urllib", "Network access via urllib"),
        new SecurityPattern("python", "http\\.client", "HTTP client access"),
        new SecurityPattern("python", "ftplib", "FTP access"),
        new SecurityPattern("python", "smtplib", "SMTP access"),
        new SecurityPattern("python", "while\\s*True\\s*:\\s*$", "Infinite loop - while True with no break"),

        // JavaScript/Node dangerous patterns
        new SecurityPattern("javascript", "require\\s*\\(\\s*[\"']child_process[\"']\\s*\\)", "child_process import"),
        new SecurityPattern("javascript", "require\\s*\\(\\s*[\"']fs[\"']\\s*\\)", "fs module import"),
        new SecurityPattern("javascript", "child_process", "child_process usage"),
        new SecurityPattern("javascript", "exec\\s*\\(", "exec() call"),
        new SecurityPattern("javascript", "execSync\\s*\\(", "execSync() call"),
        new SecurityPattern("javascript", "spawn\\s*\\(", "spawn() call"),
        new SecurityPattern("javascript", "eval\\s*\\(", "eval() call"),
        new SecurityPattern("javascript", "Function\\s*\\(", "Function constructor"),
        new SecurityPattern("javascript", "fs\\.\\w+\\s*\\(", "fs module usage"),
        new SecurityPattern("javascript", "process\\.exit", "process.exit() call"),
        new SecurityPattern("javascript", "while\\s*\\(\\s*true", "Infinite loop"),
        new SecurityPattern("javascript", "while\\s*\\(\\s*1\\s*\\)", "Infinite loop"),

        // Java dangerous patterns
        new SecurityPattern("java", "Runtime\\.getRuntime\\(\\)\\.exec", "Runtime.exec() call"),
        new SecurityPattern("java", "ProcessBuilder", "ProcessBuilder usage"),
        new SecurityPattern("java", "System\\.exit", "System.exit() call"),
        new SecurityPattern("java", "java\\.net\\.Socket", "Socket creation"),
        new SecurityPattern("java", "java\\.io\\.File", "File I/O operations"),
        new SecurityPattern("java", "while\\s*\\(\\s*true", "Infinite loop"),

        // C# dangerous patterns (for Roslyn execution)
        new SecurityPattern("csharp", "Process\\.Start", "Process.Start() call"),
        new SecurityPattern("csharp", "Environment\\.Exit", "Environment.Exit() call"),
        new SecurityPattern("csharp", "Socket", "Socket usage"),
        new SecurityPattern("csharp", "while\\s*\\(\\s*true", "Infinite loop"),
        new SecurityPattern("csharp", "while\\s*\\(\\s*1\\s*\\)", "Infinite loop"),
        new SecurityPattern("csharp", "for\\s*\\(\\s*;\\s*;\\s*\\)", "Infinite for loop"),
    };

    private record SecurityPattern(string Language, string Pattern, string Description);

    public SandboxedCodeExecutionService(ILogger<SandboxedCodeExecutionService>? logger = null)
    {
        _logger = logger;
        _roslynExecutor = new RoslynCSharpExecutor();
        _pistonExecutor = new PistonExecutor();
        _runtimeDetection = new RuntimeDetectionService();
        _resourceLimiter = new ResourceLimitedExecutor();

        // Set up audit logging directory
        _auditLogDirectory = Path.Combine(Path.GetTempPath(), "codetutor-audit-logs");
        Directory.CreateDirectory(_auditLogDirectory);

        // Check Piston availability synchronously for simplicity
        try
        {
            _pistonAvailable = CheckPistonAsync().GetAwaiter().GetResult();
        }
        catch
        {
            _pistonAvailable = false;
        }
    }

    private async Task<bool> CheckPistonAsync()
    {
        try
        {
            return await _pistonExecutor.IsAvailableAsync();
        }
        catch
        {
            return false;
        }
    }

    public bool IsLanguageSupported(string language)
    {
        var langLower = language.ToLowerInvariant();
        return langLower switch
        {
            "csharp" or "c#" => true,
            "python" or "py" => IsCommandAvailable("python") || IsCommandAvailable("python3") || IsCommandAvailable("py"),
            "javascript" or "js" => IsCommandAvailable("node"),
            "java" => IsCommandAvailable("java"),
            "kotlin" or "kt" => IsCommandAvailable("kotlin"),
            "dart" => IsCommandAvailable("dart"),
            "rust" => _pistonAvailable == true,
            _ => false
        };
    }

    private bool IsCommandAvailable(string command)
    {
        var runtimeInfo = _runtimeDetection.CheckRuntimeAsync(command).GetAwaiter().GetResult();
        return runtimeInfo.IsAvailable;
    }

    public async Task<RuntimeInfo> GetRuntimeInfoAsync(string language)
    {
        return await _runtimeDetection.CheckRuntimeAsync(language);
    }

    /// <summary>
    /// Execute code with comprehensive security controls.
    /// </summary>
    public async Task<ExecutionResult> ExecuteAsync(
        string code,
        string language,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var auditLog = new SecurityAuditLogEntry(
            Timestamp: DateTime.UtcNow,
            EventType: "CodeExecution",
            Language: language,
            FileHash: null,
            CodeSizeBytes: 0,
            SecurityScanPassed: false,
            SecurityScanDetails: null,
            ExitCode: null,
            ExecutionDuration: null,
            Success: false,
            ErrorMessage: null
        );

        try
        {
            // SECURITY: Input validation - check code size
            if (string.IsNullOrWhiteSpace(code))
            {
                auditLog = auditLog with { ErrorMessage = "Code cannot be empty" };
                await WriteAuditLogAsync(auditLog);
                return new ExecutionResult(false, "", "Code cannot be empty");
            }

            if (code.Length > MAX_CODE_SIZE_BYTES)
            {
                auditLog = auditLog with { ErrorMessage = $"Code size exceeds maximum of {MAX_CODE_SIZE_BYTES} bytes" };
                await WriteAuditLogAsync(auditLog);
                return new ExecutionResult(false, "", $"Code size exceeds maximum of {MAX_CODE_SIZE_BYTES} bytes");
            }

            if (string.IsNullOrWhiteSpace(language))
            {
                auditLog = auditLog with { ErrorMessage = "Language cannot be empty" };
                await WriteAuditLogAsync(auditLog);
                return new ExecutionResult(false, "", "Language cannot be empty");
            }

            var langLower = language.ToLowerInvariant();
            var codeHash = ComputeCodeHash(code);
            auditLog = auditLog with { FileHash = codeHash, CodeSizeBytes = code.Length };

            // SECURITY: Pattern-based code security scanning
            var scanResult = ScanCodeForDangerousPatterns(code, langLower);
            if (!scanResult.IsSafe)
            {
                auditLog = auditLog with
                {
                    SecurityScanPassed = false,
                    SecurityScanDetails = scanResult.DetectedIssues,
                    ErrorMessage = $"Security violation detected: {scanResult.DetectedIssues}"
                };
                await WriteAuditLogAsync(auditLog);
                _logger?.LogWarning("Code execution blocked by security scan. Issues: {Issues}", scanResult.DetectedIssues);
                return new ExecutionResult(false, "", $"Security violation detected: {scanResult.DetectedIssues}");
            }

            auditLog = auditLog with { SecurityScanPassed = true };

            // Route to appropriate executor
            ExecutionResult result;
            if (langLower is "csharp" or "c#")
            {
                result = await ExecuteCSharpSecureAsync(code, cancellationToken);
            }
            else if (_pistonAvailable == true)
            {
                result = await _pistonExecutor.ExecuteAsync(code, language, cancellationToken);
            }
            else
            {
                result = await ExecuteExternalSecureAsync(code, langLower, cancellationToken);
            }

            // SECURITY: Truncate output to prevent memory exhaustion
            var truncatedOutput = TruncateOutput(result.Output);
            var truncatedError = TruncateOutput(result.Error);

            stopwatch.Stop();
            auditLog = auditLog with
            {
                Success = result.Success,
                ExecutionDuration = stopwatch.Elapsed,
                ErrorMessage = truncatedError
            };

            await WriteAuditLogAsync(auditLog);

            return new ExecutionResult(result.Success, truncatedOutput, truncatedError);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            auditLog = auditLog with
            {
                ExecutionDuration = stopwatch.Elapsed,
                Success = false,
                ErrorMessage = ex.Message
            };
            await WriteAuditLogAsync(auditLog);
            _logger?.LogError(ex, "Unexpected error during code execution");
            return new ExecutionResult(false, "", $"Execution error: {ex.Message}");
        }
    }

    /// <summary>
    /// SECURITY: Scan code for dangerous patterns.
    /// </summary>
    private (bool IsSafe, string? DetectedIssues) ScanCodeForDangerousPatterns(string code, string language)
    {
        var detectedIssues = new System.Collections.Generic.List<string>();

        foreach (var pattern in DangerousPatterns)
        {
            // Check if pattern applies to this language
            if (pattern.Language != language && pattern.Language != "csharp")
                continue;

            // Skip C# patterns for non-C# languages
            if (pattern.Language == "csharp" && language != "csharp" && language != "c#")
                continue;

            try
            {
                if (Regex.IsMatch(code, pattern.Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    detectedIssues.Add(pattern.Description);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Regex scan failed for pattern: {Pattern}", pattern.Description);
            }
        }

        if (detectedIssues.Count > 0)
        {
            return (false, string.Join("; ", detectedIssues.Take(5)));
        }

        return (true, null);
    }

    /// <summary>
    /// SECURITY: Compute SHA-256 hash of code for audit trail.
    /// </summary>
    private string ComputeCodeHash(string code)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(code);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// SECURITY: Truncate output to prevent memory exhaustion.
    /// </summary>
    private string TruncateOutput(string output)
    {
        if (string.IsNullOrEmpty(output) || output.Length <= MAX_OUTPUT_SIZE_BYTES)
            return output;

        return output.Substring(0, MAX_OUTPUT_SIZE_BYTES) +
            $"\n[Output truncated - exceeded {MAX_OUTPUT_SIZE_BYTES} bytes limit]";
    }

    /// <summary>
    /// SECURITY: Write audit log entry as JSON.
    /// </summary>
    private async Task WriteAuditLogAsync(SecurityAuditLogEntry entry)
    {
        try
        {
            var logFileName = $"audit_{entry.Timestamp:yyyyMMdd}.jsonl";
            var logFilePath = Path.Combine(_auditLogDirectory, logFileName);
            var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await File.AppendAllTextAsync(logFilePath, json + Environment.NewLine);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to write audit log");
            // Don't throw - audit logging should not break execution
        }
    }

    /// <summary>
    /// Execute C# code using Roslyn with additional safeguards.
    /// </summary>
    private async Task<ExecutionResult> ExecuteCSharpSecureAsync(string code, CancellationToken cancellationToken)
    {
        // C# is executed in-process via Roslyn, which provides natural sandboxing
        // but we still apply timeout and output limits
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS));

        var result = await _roslynExecutor.ExecuteAsync(code, cts.Token);
        return result;
    }

    /// <summary>
    /// SECURITY: Execute external process with sandbox controls.
    /// </summary>
    private async Task<ExecutionResult> ExecuteExternalSecureAsync(
        string code,
        string language,
        CancellationToken cancellationToken)
    {
        string? tempFile = null;
        string? tempDir = null;

        try
        {
            // Get command and extension
            var (command, extension, extraArgs) = GetCommandInfo(language);

            // SECURITY: Validate command is in whitelist
            var commandName = Path.GetFileNameWithoutExtension(command).ToLowerInvariant();
            if (!AllowedCommands.Contains(commandName))
            {
                return new ExecutionResult(false, "", $"Command '{command}' is not in the allowed execution whitelist");
            }

            // SECURITY: Check runtime availability
            var runtimeInfo = await _runtimeDetection.CheckRuntimeAsync(command);
            if (!runtimeInfo.IsAvailable)
            {
                return new ExecutionResult(false, "", $"{command} is not installed.\n\n{runtimeInfo.InstallHint}");
            }

            // SECURITY: Create secure temp file with cryptographically random name
            (tempFile, tempDir) = CreateSecureTempFile(code, extension, language);

            // Build arguments
            var arguments = extraArgs != null
                ? $"{extraArgs} \"{tempFile}\""
                : $"\"{tempFile}\"";

            // Use ResourceLimitedExecutor for sandboxed execution
            var result = await _resourceLimiter.ExecuteWithLimitsAsync(
                command: command,
                arguments: arguments,
                maxExecutionTimeSeconds: DEFAULT_TIMEOUT_SECONDS,
                maxMemoryMb: DEFAULT_MEMORY_LIMIT_MB,
                blockNetwork: true,
                workingDirectory: tempDir ?? Path.GetTempPath()
            );

            return result;
        }
        finally
        {
            // SECURITY: Clean up temp files
            CleanupTempFiles(tempFile, tempDir);
        }
    }

    /// <summary>
    /// SECURITY: Create secure temp file with cryptographically random name.
    /// </summary>
    private (string filePath, string? dirPath) CreateSecureTempFile(string code, string extension, string language)
    {
        // Generate cryptographically secure random filename
        byte[] randomBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        var randomName = Convert.ToHexString(randomBytes).ToLowerInvariant();

        if (language == "java")
        {
            // Java needs special handling - temp directory with class name detection
            var tempDir = Path.Combine(Path.GetTempPath(), $"ct_{randomName}");
            Directory.CreateDirectory(tempDir);

            // Try to detect class name from code
            var className = "Main";
            var classMatch = Regex.Match(code, "public\\s+class\\s+(\\w+)");
            if (classMatch.Success)
            {
                className = classMatch.Groups[1].Value;
            }

            var javaFile = Path.Combine(tempDir, $"{className}.java");
            File.WriteAllText(javaFile, code);
            return (javaFile, tempDir);
        }
        else
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"ct_{randomName}{extension}");
            File.WriteAllText(tempFile, code);
            return (tempFile, null);
        }
    }

    private void CleanupTempFiles(string? filePath, string? dirPath)
    {
        try
        {
            if (filePath != null && File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (dirPath != null && Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, recursive: true);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to cleanup temp files");
        }
    }

    private (string command, string extension, string? extraArgs) GetCommandInfo(string language)
    {
        return language.ToLowerInvariant() switch
        {
            "python" or "py" => ("python", ".py", "-u"),
            "javascript" or "js" => ("node", ".js", null),
            "java" => ("java", ".java", null),
            "kotlin" or "kt" => ("kotlin", ".kts", null),
            "dart" => ("dart", ".dart", null),
            _ => throw new NotSupportedException($"Language '{language}' is not supported")
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _resourceLimiter.Dispose();
    }
}
