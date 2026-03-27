using System.IO;
using System.Net.Http;

namespace CodeTutor.Wpf.Services;

public class ModelInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string SizeFormatted => FormatBytes(SizeBytes);
    public string HuggingFaceRepo { get; set; } = string.Empty;
    public string ModelFile { get; set; } = string.Empty;  // GGUF filename
    
    // Use AppDomain.CurrentDomain.BaseDirectory as primary -- Assembly.Location returns ""
    // in single-file published apps (PublishSingleFile=true in .NET 6+)
    public string LocalPath
    {
        get
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, "models", Id, "model.gguf");
        }
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}

public class ModelDownloadProgress
{
    public string CurrentFile { get; set; } = string.Empty;
    public long BytesDownloaded { get; set; }
    public long TotalBytes { get; set; }
    public double OverallProgress { get; set; }
}

public interface IModelDownloadService
{
    event EventHandler<ModelDownloadProgress>? ProgressChanged;
    event EventHandler<string>? StatusChanged;

    ModelInfo GetCurrentModelInfo();
    Task<bool> DownloadModelAsync(CancellationToken cancellationToken = default);
    Task<bool> UninstallModelAsync();
    Task<bool> IsModelInstalledAsync();
    Task<long> GetModelSizeAsync();
}

public class ModelDownloadService : IModelDownloadService
{
    private readonly HttpClient _httpClient;
    private const string HuggingFaceBaseUrl = "https://huggingface.co";

    // Qwen2.5-Coder-7B GGUF model from bartowski (reliable GGUF converter)
    private static readonly ModelInfo CurrentModel = new()
    {
        Id = "qwen2.5-coder-7b",
        Name = "Qwen2.5-Coder-7B",
        Description = "AI tutor model optimized for coding. 2x faster than Phi-4 with excellent code understanding.",
        SizeBytes = 4_500_000_000, // ~4.5GB for Q4_K_M quantized
        HuggingFaceRepo = "bartowski/Qwen2.5-Coder-7B-Instruct-GGUF",
        ModelFile = "Qwen2.5-Coder-7B-Instruct-Q4_K_M.gguf"  // Good balance of quality/size
    };

    public event EventHandler<ModelDownloadProgress>? ProgressChanged;
    public event EventHandler<string>? StatusChanged;

    public ModelDownloadService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "CodeTutor/1.0");
    }

    public ModelInfo GetCurrentModelInfo() => CurrentModel;

    public Task<bool> IsModelInstalledAsync()
    {
        return Task.FromResult(File.Exists(CurrentModel.LocalPath));
    }

    public Task<long> GetModelSizeAsync()
    {
        if (File.Exists(CurrentModel.LocalPath))
        {
            var fileInfo = new FileInfo(CurrentModel.LocalPath);
            return Task.FromResult(fileInfo.Length);
        }
        return Task.FromResult(0L);
    }

    public async Task<bool> DownloadModelAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var targetDirectory = Path.GetDirectoryName(CurrentModel.LocalPath)!;
            Directory.CreateDirectory(targetDirectory);

            var targetPath = CurrentModel.LocalPath;
            var tempPath = targetPath + ".downloading";
            var fileName = CurrentModel.ModelFile;

            // Clean up any previous partial download
            if (File.Exists(tempPath)) File.Delete(tempPath);

            StatusChanged?.Invoke(this, $"Downloading {CurrentModel.Name}...");
            StatusChanged?.Invoke(this, $"Target: {targetPath}");

            // Download from HuggingFace
            var url = $"{HuggingFaceBaseUrl}/{CurrentModel.HuggingFaceRepo}/resolve/main/{fileName}";

            System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] Downloading from: {url}");
            System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] Saving to: {tempPath} (will rename on success)");

            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = $"Download failed: HTTP {(int)response.StatusCode} - {response.ReasonPhrase}";
                System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] {errorMsg}");
                StatusChanged?.Invoke(this, errorMsg);
                return false;
            }

            var totalBytes = response.Content.Headers.ContentLength ?? 0;
            System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] Content length: {totalBytes} bytes");

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                var buffer = new byte[81920]; // 80KB buffer
                long totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                    totalBytesRead += bytesRead;

                    var progress = new ModelDownloadProgress
                    {
                        CurrentFile = fileName,
                        BytesDownloaded = totalBytesRead,
                        TotalBytes = totalBytes,
                        OverallProgress = totalBytes > 0 ? (double)totalBytesRead / totalBytes * 100 : 0
                    };
                    ProgressChanged?.Invoke(this, progress);
                }

                System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] Download complete: {totalBytesRead} bytes");
            }

            // Rename temp file to final path only after successful, complete download
            File.Move(tempPath, targetPath, overwrite: true);

            StatusChanged?.Invoke(this, $"{CurrentModel.Name} download complete!");
            return true;
        }
        catch (OperationCanceledException)
        {
            CleanupPartialDownload();
            StatusChanged?.Invoke(this, "Download cancelled.");
            return false;
        }
        catch (HttpRequestException ex)
        {
            CleanupPartialDownload();
            var errorMsg = $"Network error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] {errorMsg}");
            StatusChanged?.Invoke(this, errorMsg);
            return false;
        }
        catch (Exception ex)
        {
            CleanupPartialDownload();
            var errorMsg = $"Download failed: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] {errorMsg}");
            StatusChanged?.Invoke(this, errorMsg);
            return false;
        }
    }

    private void CleanupPartialDownload()
    {
        var tempPath = CurrentModel.LocalPath + ".downloading";
        try
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] Failed to clean up partial download: {ex.Message}");
        }
    }

    public Task<bool> UninstallModelAsync()
    {
        try
        {
            if (!File.Exists(CurrentModel.LocalPath))
            {
                StatusChanged?.Invoke(this, "Model not found.");
                return Task.FromResult(true);
            }

            StatusChanged?.Invoke(this, $"Uninstalling {CurrentModel.Name}...");

            File.Delete(CurrentModel.LocalPath);

            // Try to remove empty directory
            var dir = Path.GetDirectoryName(CurrentModel.LocalPath);
            if (dir != null && Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
            {
                Directory.Delete(dir);
            }

            StatusChanged?.Invoke(this, "Model uninstalled. Disk space freed.");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            StatusChanged?.Invoke(this, $"Uninstall failed: {ex.Message}");
            return Task.FromResult(false);
        }
    }
}
