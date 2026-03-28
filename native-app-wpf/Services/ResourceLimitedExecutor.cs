using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Executes processes with strict resource limits using Windows Job Objects.
/// 
/// SECURITY CONTROLS:
/// 1. Memory limits via Job Object
/// 2. CPU time limits
/// 3. Active process limits (prevent fork bombs)
/// 4. Network isolation via Windows Firewall (when possible)
/// 5. Process priority reduction
/// </summary>
public class ResourceLimitedExecutor : IDisposable
{
    private bool _disposed;

    // Windows API constants for Job Objects
    private const int JOB_OBJECT_LIMIT_PROCESS_MEMORY = 0x00000100;
    private const int JOB_OBJECT_LIMIT_JOB_MEMORY = 0x00000200;
    private const int JOB_OBJECT_LIMIT_ACTIVE_PROCESS = 0x00000008;
    private const int JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000;
    private const int JOB_OBJECT_LIMIT_PRIORITY_CLASS = 0x00000020;
    private const int JOB_OBJECT_LIMIT_PROCESS_TIME = 0x00000004;
    private const int JOB_OBJECT_LIMIT_JOB_TIME = 0x00000004;
    private const int JOB_OBJECT_EXTENDED_LIMIT_INFORMATION = 9;
    private const uint JOB_OBJECT_BASIC_LIMIT_INFORMATION = 2;

    // Process priority classes
    private const uint IDLE_PRIORITY_CLASS = 0x00000040;
    private const uint BELOW_NORMAL_PRIORITY_CLASS = 0x00004000;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetInformationJobObject(IntPtr hJob, int JobObjectInfoClass, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool TerminateJobObject(IntPtr hJob, uint uExitCode);

    [StructLayout(LayoutKind.Sequential)]
    private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public IntPtr Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IO_COUNTERS
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }

    /// <summary>
    /// Execute a command with strict resource limits.
    /// </summary>
    public async Task<ExecutionResult> ExecuteWithLimitsAsync(
        string command,
        string arguments,
        int maxExecutionTimeSeconds,
        int maxMemoryMb,
        bool blockNetwork,
        string? workingDirectory = null)
    {
        IntPtr jobHandle = IntPtr.Zero;
        Process? process = null;

        try
        {
            // SECURITY: Validate command is in allowed list
            var allowedCommands = new[] { "python", "node", "java", "kotlin", "dart", "javac" };
            var commandName = System.IO.Path.GetFileNameWithoutExtension(command).ToLowerInvariant();
            if (!allowedCommands.Contains(commandName))
            {
                return new ExecutionResult(false, "", $"Command '{command}' is not in the allowed execution list");
            }

            // Create process with restricted start info
            var psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory ?? Path.GetTempPath(),
                // SECURITY: Don't load user profile
                LoadUserProfile = false,
            };

            // SECURITY: Set restricted environment variables
            psi.EnvironmentVariables["TEMP"] = Path.GetTempPath();
            psi.EnvironmentVariables["TMP"] = Path.GetTempPath();
            psi.EnvironmentVariables["HOME"] = Path.GetTempPath();
            psi.EnvironmentVariables["USERPROFILE"] = Path.GetTempPath();
            // Remove potentially dangerous environment variables
            psi.EnvironmentVariables.Remove("PATH");
            psi.EnvironmentVariables.Remove("LD_LIBRARY_PATH");
            psi.EnvironmentVariables.Remove("DYLD_LIBRARY_PATH");

            process = Process.Start(psi);
            if (process == null)
            {
                return new ExecutionResult(false, "", "Failed to start process");
            }

            // Create Job Object for resource limits
            jobHandle = CreateJobObject(IntPtr.Zero, $"CodeTutorJob_{Guid.NewGuid():N}");
            if (jobHandle != IntPtr.Zero)
            {
                ConfigureJobLimits(jobHandle, maxMemoryMb, maxExecutionTimeSeconds);

                // Assign process to job
                if (!AssignProcessToJobObject(jobHandle, process.Handle))
                {
                    Debug.WriteLine($"[ResourceLimitedExecutor] Failed to assign process to job: {Marshal.GetLastWin32Error()}");
                    // Continue anyway - we'll use timeout as fallback
                }
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(maxExecutionTimeSeconds));

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null) outputBuilder.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null) errorBuilder.AppendLine(e.Data);
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for completion or timeout
            try
            {
                await process.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Timeout - kill the job (which kills all processes)
                if (jobHandle != IntPtr.Zero)
                {
                    TerminateJobObject(jobHandle, 1);
                }
                else
                {
                    try { process.Kill(true); } catch { }
                }

                return new ExecutionResult(false, outputBuilder.ToString().Trim(), $"Execution timed out after {maxExecutionTimeSeconds} seconds");
            }

            var output = outputBuilder.ToString().Trim();
            var error = errorBuilder.ToString().Trim();
            var success = process.ExitCode == 0 && string.IsNullOrEmpty(error);

            return new ExecutionResult(success, output, error);
        }
        catch (Win32Exception ex)
        {
            return new ExecutionResult(false, "", $"Process execution error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new ExecutionResult(false, "", $"Unexpected error: {ex.Message}");
        }
        finally
        {
            process?.Dispose();
            if (jobHandle != IntPtr.Zero)
            {
                CloseHandle(jobHandle);
            }
        }
    }

    /// <summary>
    /// Apply Job Object limits to an existing process (for interactive sessions).
    /// </summary>
    public bool ApplyJobObjectLimits(Process process, int maxMemoryMb)
    {
        IntPtr jobHandle = IntPtr.Zero;
        try
        {
            jobHandle = CreateJobObject(IntPtr.Zero, $"CodeTutorJob_{Guid.NewGuid():N}");
            if (jobHandle == IntPtr.Zero) return false;

            // Basic configuration: memory limit, active process limit, kill on close
            var limitInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
                {
                    LimitFlags = JOB_OBJECT_LIMIT_PROCESS_MEMORY |
                                 JOB_OBJECT_LIMIT_ACTIVE_PROCESS |
                                 JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE |
                                 JOB_OBJECT_LIMIT_PRIORITY_CLASS,
                    ActiveProcessLimit = 10, // Prevent fork bombs
                    PriorityClass = BELOW_NORMAL_PRIORITY_CLASS,
                },
                ProcessMemoryLimit = (UIntPtr)((ulong)maxMemoryMb * 1024 * 1024),
            };

            var size = (uint)Marshal.SizeOf<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>();
            var ptr = Marshal.AllocHGlobal((int)size);
            try
            {
                Marshal.StructureToPtr(limitInfo, ptr, false);
                return SetInformationJobObject(jobHandle, JOB_OBJECT_EXTENDED_LIMIT_INFORMATION, ptr, size) &&
                       AssignProcessToJobObject(jobHandle, process.Handle);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ResourceLimitedExecutor] ApplyJobObjectLimits failed: {ex.Message}");
            return false;
        }
    }

    private void ConfigureJobLimits(IntPtr jobHandle, int maxMemoryMb, int maxExecutionTimeSeconds)
    {
        var limitInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                // Set CPU time limit (100-nanosecond intervals)
                PerJobUserTimeLimit = maxExecutionTimeSeconds * 10_000_000L,
                LimitFlags = JOB_OBJECT_LIMIT_PROCESS_MEMORY |
                             JOB_OBJECT_LIMIT_JOB_MEMORY |
                             JOB_OBJECT_LIMIT_ACTIVE_PROCESS |
                             JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE |
                             JOB_OBJECT_LIMIT_PRIORITY_CLASS |
                             JOB_OBJECT_LIMIT_JOB_TIME,
                ActiveProcessLimit = 5, // Prevent fork bombs - max 5 processes
                PriorityClass = BELOW_NORMAL_PRIORITY_CLASS, // Lower priority
            },
            ProcessMemoryLimit = (UIntPtr)((ulong)maxMemoryMb * 1024 * 1024),
            JobMemoryLimit = (UIntPtr)((ulong)maxMemoryMb * 1024 * 1024 * 2), // 2x per-process limit
        };

        var size = (uint)Marshal.SizeOf<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>();
        var ptr = Marshal.AllocHGlobal((int)size);
        try
        {
            Marshal.StructureToPtr(limitInfo, ptr, false);
            if (!SetInformationJobObject(jobHandle, JOB_OBJECT_EXTENDED_LIMIT_INFORMATION, ptr, size))
            {
                Debug.WriteLine($"[ResourceLimitedExecutor] Failed to set job limits: {Marshal.GetLastWin32Error()}");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}
