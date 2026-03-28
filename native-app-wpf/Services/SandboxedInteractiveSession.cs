using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Sandboxed interactive process session with automatic cleanup and timeout protection.
/// 
/// SECURITY FEATURES:
/// 1. Automatic timeout termination (prevents resource exhaustion)
/// 2. Process priority reduction (IDLE_PRIORITY_CLASS)
/// 3. Automatic temp file cleanup
/// 4. Kill on dispose (ensures process doesn't outlive session)
/// </summary>
public class SandboxedInteractiveSession : IInteractiveSession
{
    private readonly Process _process;
    private readonly string? _tempFilePath;
    private readonly string? _tempDirPath;
    private readonly bool _cleanupTempFiles;
    private readonly System.Timers.Timer _timeoutTimer;
    private readonly CancellationTokenSource _cts;
    private readonly int _maxExecutionTimeSeconds;
    private bool _isDisposed;
    private bool _hasExceededTimeout;

    public event EventHandler<string>? OutputReceived;
    public event EventHandler<string>? ErrorReceived;
    public event EventHandler<int>? Exited;

    public SandboxedInteractiveSession(
        Process process,
        string? tempFilePath = null,
        bool cleanupTempFiles = true,
        int maxExecutionTimeSeconds = 30,
        string? tempDirPath = null)
    {
        _process = process;
        _tempFilePath = tempFilePath;
        _tempDirPath = tempDirPath;
        _cleanupTempFiles = cleanupTempFiles;
        _maxExecutionTimeSeconds = maxExecutionTimeSeconds;
        _cts = new CancellationTokenSource();

        // SECURITY: Set up timeout timer to prevent resource exhaustion
        _timeoutTimer = new System.Timers.Timer(maxExecutionTimeSeconds * 1000);
        _timeoutTimer.Elapsed += OnTimeoutElapsed;
        _timeoutTimer.AutoReset = false;

        // Set up event handlers
        _process.OutputDataReceived += OnOutputDataReceived;
        _process.ErrorDataReceived += OnErrorDataReceived;
        _process.Exited += OnProcessExited;
        _process.EnableRaisingEvents = true;

        // SECURITY: Lower process priority to prevent CPU starvation
        try
        {
            _process.PriorityClass = ProcessPriorityClass.BelowNormal;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SandboxedInteractiveSession] Could not lower priority: {ex.Message}");
        }
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null) OutputReceived?.Invoke(this, e.Data + Environment.NewLine);
    }

    private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null) ErrorReceived?.Invoke(this, e.Data + Environment.NewLine);
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        _timeoutTimer.Stop();
        Exited?.Invoke(this, _process.ExitCode);
    }

    private void OnTimeoutElapsed(object? sender, ElapsedEventArgs e)
    {
        _hasExceededTimeout = true;
        Debug.WriteLine($"[SandboxedInteractiveSession] Timeout after {_maxExecutionTimeSeconds}s");
        StopAsync().Wait();
    }

    public void Start()
    {
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        _timeoutTimer.Start();

        // Handle case where process exited before we started reading
        if (_process.HasExited)
        {
            _timeoutTimer.Stop();
            Exited?.Invoke(this, _process.ExitCode);
        }
    }

    public async Task InputAsync(string text)
    {
        if (_isDisposed || _process.HasExited) return;

        try
        {
            await _process.StandardInput.WriteLineAsync(text);
            await _process.StandardInput.FlushAsync();
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException)
        {
            // Process may have exited
            Debug.WriteLine($"[SandboxedInteractiveSession] Input failed: {ex.Message}");
        }
    }

    public Task StopAsync()
    {
        _timeoutTimer.Stop();

        if (!_process.HasExited)
        {
            try
            {
                // Try graceful termination first
                _process.StandardInput.Close();

                // Give process 2 seconds to exit gracefully
                var exited = _process.WaitForExit(2000);

                if (!exited)
                {
                    // Force kill
                    _process.Kill(true);
                }
            }
            catch (InvalidOperationException)
            {
                // Process already exited
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SandboxedInteractiveSession] Kill failed: {ex.Message}");
            }
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _timeoutTimer.Stop();
        _timeoutTimer.Dispose();
        _cts.Cancel();
        _cts.Dispose();

        StopAsync().Wait();
        _process.Dispose();

        // Clean up temp files
        if (_cleanupTempFiles)
        {
            CleanupTempFiles();
        }
    }

    private void CleanupTempFiles()
    {
        try
        {
            if (_tempFilePath != null && File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }

            if (_tempDirPath != null && Directory.Exists(_tempDirPath))
            {
                Directory.Delete(_tempDirPath, recursive: true);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SandboxedInteractiveSession] Temp cleanup failed: {ex.Message}");
        }
    }
}
