using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CodeTutor.Wpf.Services;

public interface IInteractiveSession : IDisposable
{
    event EventHandler<string> OutputReceived;
    event EventHandler<string> ErrorReceived;
    event EventHandler<int> Exited;

    /// <summary>
    /// Begin reading stdout/stderr. Call AFTER attaching event handlers.
    /// </summary>
    void Start();
    Task InputAsync(string text);
    Task StopAsync();
}

public class InteractiveProcessSession : IInteractiveSession
{
    private readonly Process _process;
    private readonly CancellationTokenSource _cts;
    private readonly string? _tempFilePath;
    private readonly string? _tempDirPath;
    private bool _isDisposed;

    public event EventHandler<string>? OutputReceived;
    public event EventHandler<string>? ErrorReceived;
    public event EventHandler<int>? Exited;

    public InteractiveProcessSession(Process process, string? tempFilePath = null, string? tempDirPath = null)
    {
        _process = process;
        _cts = new CancellationTokenSource();
        _tempFilePath = tempFilePath;
        _tempDirPath = tempDirPath;

        _process.OutputDataReceived += (s, e) =>
        {
            if (e.Data != null) OutputReceived?.Invoke(this, e.Data + Environment.NewLine);
        };
        _process.ErrorDataReceived += (s, e) =>
        {
            if (e.Data != null) ErrorReceived?.Invoke(this, e.Data + Environment.NewLine);
        };
        _process.Exited += (s, e) => Exited?.Invoke(this, _process.ExitCode);
    }

    public void Start()
    {
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        // If the process already exited before we started reading,
        // fire the exit event so the UI doesn't get stuck.
        if (_process.HasExited)
        {
            Exited?.Invoke(this, _process.ExitCode);
        }
    }

    public async Task InputAsync(string text)
    {
        if (_isDisposed || _process.HasExited) return;
        
        await _process.StandardInput.WriteLineAsync(text);
        await _process.StandardInput.FlushAsync();
    }

    public Task StopAsync()
    {
        if (!_process.HasExited)
        {
            try { _process.Kill(true); }
            catch (InvalidOperationException) { /* Process already exited */ }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InteractiveProcessSession] Kill failed: {ex.Message}");
            }
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _cts.Cancel();
        StopAsync();
        _process.Dispose();
        _cts.Dispose();

        // Clean up temp files
        try
        {
            if (_tempFilePath != null && File.Exists(_tempFilePath))
                File.Delete(_tempFilePath);
            if (_tempDirPath != null && Directory.Exists(_tempDirPath))
                Directory.Delete(_tempDirPath, recursive: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InteractiveProcessSession] Temp cleanup failed: {ex.Message}");
        }
    }
}
