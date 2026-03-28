using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Audit logger for code execution events.
/// 
/// SECURITY: Provides accountability and forensic analysis capability:
/// - Timestamps all execution attempts
/// - Records code hashes (not full code) for privacy
/// - Tracks success/failure and blocked patterns
/// - Logs resource usage
/// - Rotates log files to prevent disk exhaustion
/// </summary>
public class ExecutionAuditLogger : IDisposable
{
    private readonly string _logDirectory;
    private readonly SemaphoreSlim _logLock = new(1, 1);
    private bool _disposed;

    public ExecutionAuditLogger(string? logDirectory = null)
    {
        _logDirectory = logDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodeTutor",
            "AuditLogs");

        // Ensure log directory exists
        Directory.CreateDirectory(_logDirectory);
    }

    /// <summary>
    /// Log an execution event.
    /// </summary>
    public async Task LogAsync(ExecutionAuditEntry entry)
    {
        if (_disposed) return;

        await _logLock.WaitAsync();
        try
        {
            var logFile = GetCurrentLogFile();
            var logLine = FormatLogEntry(entry);

            await File.AppendAllTextAsync(logFile, logLine + Environment.NewLine);

            // Cleanup old logs periodically (keep last 30 days)
            CleanupOldLogs();
        }
        catch (Exception ex)
        {
            // Audit logging should not break execution
            Debug.WriteLine($"[ExecutionAuditLogger] Failed to write log: {ex.Message}");
        }
        finally
        {
            _logLock.Release();
        }
    }

    /// <summary>
    /// Get recent audit entries for analysis.
    /// </summary>
    public async Task<ExecutionAuditEntry[]> GetRecentEntriesAsync(int count = 100)
    {
        await _logLock.WaitAsync();
        try
        {
            var logFile = GetCurrentLogFile();
            if (!File.Exists(logFile))
            {
                return [];
            }

            var lines = await File.ReadAllLinesAsync(logFile);
            var entries = new System.Collections.Generic.List<ExecutionAuditEntry>();

            foreach (var line in lines.Reverse().Take(count))
            {
                if (TryParseLogEntry(line, out var entry) && entry != null)
                {
                    entries.Add(entry);
                }
            }

            return entries.ToArray();
        }
        finally
        {
            _logLock.Release();
        }
    }

    /// <summary>
    /// Get execution statistics for security analysis.
    /// </summary>
    public ExecutionStatistics GetStatistics(TimeSpan period)
    {
        var cutoff = DateTime.UtcNow - period;
        var logFiles = Directory.GetFiles(_logDirectory, "execution_audit_*.log");

        int totalExecutions = 0;
        int blockedExecutions = 0;
        int failedExecutions = 0;
        long totalExecutionTimeMs = 0;
        var languageCounts = new System.Collections.Generic.Dictionary<string, int>();
        var blockedPatterns = new System.Collections.Generic.Dictionary<string, int>();

        foreach (var logFile in logFiles)
        {
            try
            {
                var lines = File.ReadAllLines(logFile);
                foreach (var line in lines)
                {
                    if (!TryParseLogEntry(line, out var entry) || entry == null) continue;
                    if (entry.Timestamp < cutoff) continue;

                    totalExecutions++;
                    totalExecutionTimeMs += entry.ExecutionTimeMs;

                    if (!entry.Success && entry.BlockedPatterns != null)
                    {
                        blockedExecutions++;
                        blockedPatterns[entry.BlockedPatterns] = blockedPatterns.GetValueOrDefault(entry.BlockedPatterns) + 1;
                    }
                    else if (!entry.Success)
                    {
                        failedExecutions++;
                    }

                    languageCounts[entry.Language] = languageCounts.GetValueOrDefault(entry.Language) + 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ExecutionAuditLogger] Error reading {logFile}: {ex.Message}");
            }
        }

        return new ExecutionStatistics(
            totalExecutions,
            blockedExecutions,
            failedExecutions,
            totalExecutions > 0 ? totalExecutionTimeMs / totalExecutions : 0,
            languageCounts,
            blockedPatterns
        );
    }

    private string GetCurrentLogFile()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        return Path.Combine(_logDirectory, $"execution_audit_{date}.log");
    }

    private string FormatLogEntry(ExecutionAuditEntry entry)
    {
        // JSON format for structured logging
        return JsonSerializer.Serialize(new
        {
            entry.Timestamp,
            entry.Language,
            entry.Success,
            entry.ExitCode,
            entry.ExecutionTimeMs,
            entry.BlockedPatterns,
            entry.ErrorMessage,
            entry.CodeHash
        });
    }

    private bool TryParseLogEntry(string line, out ExecutionAuditEntry? entry)
    {
        entry = null;
        try
        {
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;

            entry = new ExecutionAuditEntry(
                root.GetProperty("Timestamp").GetDateTime(),
                root.GetProperty("Language").GetString() ?? "unknown",
                root.GetProperty("Success").GetBoolean(),
                root.GetProperty("ExitCode").GetInt32(),
                root.GetProperty("ExecutionTimeMs").GetInt64(),
                root.GetProperty("BlockedPatterns").ValueKind == JsonValueKind.Null ? null : root.GetProperty("BlockedPatterns").GetString(),
                root.GetProperty("ErrorMessage").ValueKind == JsonValueKind.Null ? null : root.GetProperty("ErrorMessage").GetString(),
                root.GetProperty("CodeHash").GetString() ?? ""
            );

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void CleanupOldLogs()
    {
        try
        {
            var cutoff = DateTime.UtcNow.AddDays(-30);
            var logFiles = Directory.GetFiles(_logDirectory, "execution_audit_*.log");

            foreach (var logFile in logFiles)
            {
                var fileInfo = new FileInfo(logFile);
                if (fileInfo.LastWriteTimeUtc < cutoff)
                {
                    File.Delete(logFile);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ExecutionAuditLogger] Cleanup failed: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _logLock.Dispose();
    }
}

/// <summary>
/// Audit log entry for code execution events.
/// </summary>
public record ExecutionAuditEntry(
    DateTime Timestamp,
    string Language,
    bool Success,
    int ExitCode,
    long ExecutionTimeMs,
    string? BlockedPatterns,
    string? ErrorMessage,
    string CodeHash
);

/// <summary>
/// Statistics for security analysis.
/// </summary>
public record ExecutionStatistics(
    int TotalExecutions,
    int BlockedExecutions,
    int FailedExecutions,
    long AverageExecutionTimeMs,
    IReadOnlyDictionary<string, int> ExecutionsByLanguage,
    IReadOnlyDictionary<string, int> BlockedPatterns
);
