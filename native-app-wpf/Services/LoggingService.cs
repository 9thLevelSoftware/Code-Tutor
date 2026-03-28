using System;
using System.IO;
using System.Text.Json;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Simple logging service implementation for security events and general logging.
/// </summary>
public class LoggingService : ILoggingService
{
    private readonly string _logDirectory;
    private readonly object _logLock = new();

    public LoggingService()
    {
        _logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodeTutor",
            "Logs");
        Directory.CreateDirectory(_logDirectory);
    }

    public void LogInfo(string message)
    {
        WriteLog("INFO", message);
    }

    public void LogWarning(string message)
    {
        WriteLog("WARN", message);
    }

    public void LogError(string message, Exception? ex = null)
    {
        var fullMessage = ex != null ? $"{message} - Exception: {ex.GetType().Name}: {ex.Message}" : message;
        WriteLog("ERROR", fullMessage);
    }

    public void LogSecurityEvent(string message)
    {
        WriteLog("SECURITY", message);
        // Also write to separate security audit log
        WriteSecurityAudit(message);
    }

    private void WriteLog(string level, string message)
    {
        lock (_logLock)
        {
            try
            {
                var logFile = Path.Combine(_logDirectory, $"app_{DateTime.UtcNow:yyyyMMdd}.log");
                var entry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                File.AppendAllText(logFile, entry + Environment.NewLine);
            }
            catch
            {
                // Ignore logging errors to prevent cascading failures
            }
        }
    }

    private void WriteSecurityAudit(string message)
    {
        lock (_logLock)
        {
            try
            {
                var auditFile = Path.Combine(_logDirectory, $"security_audit_{DateTime.UtcNow:yyyyMM}.log");
                var entry = JsonSerializer.Serialize(new
                {
                    Timestamp = DateTime.UtcNow.ToString("O"),
                    Event = "JSON_VALIDATION_SECURITY",
                    Message = message
                });
                File.AppendAllText(auditFile, entry + Environment.NewLine);
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
