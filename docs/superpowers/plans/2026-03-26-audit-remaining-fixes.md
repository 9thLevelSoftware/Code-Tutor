# Code Tutor Audit Remaining Fixes - Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix the remaining 2 app BLOCKERs and ~12 WARNINGs identified in the comprehensive ship-readiness audit, plus 7 installer/documentation issues.

**Architecture:** All fixes are surgical edits to existing files. The DI consolidation (Task 1) is foundational -- it centralizes service lifetimes so that ProgressService cache is shared across views. Navigation refresh (Task 2) eliminates stale back-navigation. Remaining tasks are independent and can be parallelized.

**Tech Stack:** .NET 8, WPF, LLamaSharp 0.25.0, ONNX Runtime GenAI, Microsoft.Extensions.DependencyInjection

---

## File Map

| File | Responsibility | Tasks |
|------|---------------|-------|
| `native-app-wpf/MainWindow.xaml.cs` | DI container, service lifecycle, sidebar management | 1, 2, 5 |
| `native-app-wpf/Services/NavigationService.cs` | View navigation, back-navigation refresh | 2 |
| `native-app-wpf/Services/ProgressService.cs` | Progress persistence, streak/time tracking | 4 |
| `native-app-wpf/Services/ModelDownloadService.cs` | Model download with partial-file cleanup | 3 |
| `native-app-wpf/Services/CodeExecutionService.cs` | Code execution, temp file lifecycle | 5 |
| `native-app-wpf/Services/InteractiveProcessSession.cs` | Process session, temp file cleanup | 5 |
| `native-app-wpf/Views/CoursePage.xaml.cs` | Course overview, progress bar | 1, 6 |
| `native-app-wpf/Views/LessonPage.xaml.cs` | Lesson rendering, async safety | 1, 6 |
| `native-app-wpf/Views/CourseSidebar.xaml.cs` | Course nav sidebar | 1 |
| `native-app-wpf/Views/LandingPage.xaml.cs` | Landing page, sidebar reset | 2 |
| `native-app-wpf/Services/Phi4TutorService.cs` | Dead code removal, AppendLine fix | 7 |
| `native-app-wpf/Services/QwenTutorService.cs` | AppendLine fix | 7 |
| `native-app-wpf/Models/UserProgress.cs` | Add daily activity tracking fields | 4 |
| `BUILD.md` | Fix impossible build docs | 8 |

---

### Task 1: Consolidate DI Registration and Service Lifecycle

**Files:**
- Modify: `native-app-wpf/MainWindow.xaml.cs`
- Modify: `native-app-wpf/Views/CoursePage.xaml.cs`
- Modify: `native-app-wpf/Views/LessonPage.xaml.cs`
- Modify: `native-app-wpf/Views/CourseSidebar.xaml.cs`
- Modify: `native-app-wpf/Views/LandingPage.xaml.cs`

**Why:** `IProgressService` and `ICodeExecutionService` are created via `new` in each view, causing stale caches and redundant Piston HTTP pings. The `ServiceProvider` is a local variable that never disposes `IDisposable` services.

- [ ] **Step 1: Register missing services in MainWindow DI container**

In `native-app-wpf/MainWindow.xaml.cs`, add registrations and store the provider:

```csharp
// Change line 14-20 fields to:
private readonly INavigationService _navigation;
private readonly ICourseService _courseService;
private readonly ITutorService _tutorService;
private readonly IModelDownloadService _downloadService;
private readonly IProgressService _progressService;
private readonly ICodeExecutionService _codeExecutionService;
private readonly ServiceProvider _serviceProvider;
private Controls.TutorChat? _tutorChat;
private TutorContext _latestTutorContext = new();
private bool _isTutorOpen = false;
```

In the constructor, add the two new registrations:

```csharp
// After line 31:
services.AddSingleton<IProgressService, ProgressService>();
services.AddSingleton<ICodeExecutionService, CodeExecutionService>();
var provider = services.BuildServiceProvider();
_serviceProvider = provider;
```

Add the new field assignments after line 37:

```csharp
_progressService = provider.GetRequiredService<IProgressService>();
_codeExecutionService = provider.GetRequiredService<ICodeExecutionService>();
```

- [ ] **Step 2: Add Window Closed handler to dispose ServiceProvider**

Add at the end of MainWindow constructor (after line 61):

```csharp
Closed += (_, _) =>
{
    _serviceProvider.Dispose();
};
```

- [ ] **Step 3: Pass IProgressService through view constructors**

Update `LandingPage` constructor call in MainWindow (line 60):

```csharp
var landingPage = new LandingPage(_courseService, _navigation, _tutorService, _downloadService, _progressService, _codeExecutionService);
```

Update `LandingPage.xaml.cs` to accept and store the new services:

```csharp
// Add fields:
private readonly IProgressService _progressService;
private readonly ICodeExecutionService _codeExecutionService;

// Update constructor signature:
public LandingPage(ICourseService courseService, INavigationService navigation, ITutorService tutorService, IModelDownloadService downloadService, IProgressService progressService, ICodeExecutionService codeExecutionService)
{
    // ... existing code ...
    _progressService = progressService;
    _codeExecutionService = codeExecutionService;
    // ...
}
```

Pass them through to `CoursePage` in `CourseCard_Click`:

```csharp
var coursePage = new CoursePage(_courseService, _navigation, course, _tutorService, _downloadService, _progressService, _codeExecutionService);
```

- [ ] **Step 4: Update CoursePage to accept injected services**

In `native-app-wpf/Views/CoursePage.xaml.cs`:

Remove the field initializer on line 14:
```csharp
// REMOVE: private readonly IProgressService _progressService = new ProgressService();
// REPLACE WITH:
private readonly IProgressService _progressService;
private readonly ICodeExecutionService _codeExecutionService;
```

Update constructor signature:
```csharp
public CoursePage(ICourseService courseService, INavigationService navigation, Course course, ITutorService tutorService, IModelDownloadService downloadService, IProgressService progressService, ICodeExecutionService codeExecutionService)
{
    // ... existing setup ...
    _progressService = progressService;
    _codeExecutionService = codeExecutionService;
    // ...
}
```

Pass services through to `LessonPage` and `CourseSidebar` everywhere they are constructed in this file.

- [ ] **Step 5: Update LessonPage to accept injected ProgressService**

In `native-app-wpf/Views/LessonPage.xaml.cs`:

Remove the field initializer on line 20:
```csharp
// REMOVE: private readonly IProgressService _progressService = new ProgressService();
// REPLACE WITH:
private readonly IProgressService _progressService;
private readonly ICodeExecutionService _codeExecutionService;
```

Update constructor signature to include `IProgressService progressService, ICodeExecutionService codeExecutionService` and assign them.

Pass `_codeExecutionService` when constructing `CodingChallenge` controls (they currently create their own `new CodeExecutionService()`).

- [ ] **Step 6: Update CourseSidebar to accept injected ProgressService**

In `native-app-wpf/Views/CourseSidebar.xaml.cs`:

Remove the field initializer on line 18:
```csharp
// REMOVE: private readonly IProgressService _progressService = new ProgressService();
// REPLACE WITH:
private readonly IProgressService _progressService;
```

Update constructor signature to include `IProgressService progressService` and assign it.

- [ ] **Step 7: Update CodingChallenge to accept injected ICodeExecutionService**

In `native-app-wpf/Controls/CodingChallenge.xaml.cs`:

Change the field and constructor:
```csharp
// Change line 28:
private readonly ICodeExecutionService _executionService;

// Change constructor to accept service:
public CodingChallenge(Challenge challenge, ICodeExecutionService executionService)
{
    InitializeComponent();
    _challenge = challenge;
    _originalCode = challenge.StarterCode;
    _executionService = executionService;
    // ... rest unchanged ...
}
```

Update all call sites in `LessonPage.xaml.cs` where `new CodingChallenge(challenge)` is called to pass `_codeExecutionService`.

- [ ] **Step 8: Build and verify**

Run: `dotnet build native-app-wpf/CodeTutor.Wpf.csproj`
Expected: 0 errors, 0 warnings

Run: `dotnet test native-app.Tests/native-app.Tests.csproj`
Expected: 261 passed

- [ ] **Step 9: Commit**

```bash
git add native-app-wpf/MainWindow.xaml.cs native-app-wpf/Views/*.cs native-app-wpf/Controls/CodingChallenge.xaml.cs
git commit -m "refactor: consolidate DI registration for ProgressService and CodeExecutionService

Register IProgressService and ICodeExecutionService as singletons in the
DI container. Inject them through view constructors instead of creating
new instances per-view. Store and dispose ServiceProvider on window close."
```

---

### Task 2: Fix Back-Navigation Stale Views and Sidebar Reset

**Files:**
- Modify: `native-app-wpf/Services/NavigationService.cs`
- Modify: `native-app-wpf/MainWindow.xaml.cs`

**Why:** `GoBack()` reuses cached `UserControl` instances whose `Loaded` event won't re-fire, so users see stale progress. Sidebar doesn't reset when returning to LandingPage.

- [ ] **Step 1: Change NavigationService to store factory functions instead of instances**

Replace `native-app-wpf/Services/NavigationService.cs` entirely:

```csharp
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CodeTutor.Wpf.Services;

public interface INavigationService
{
    event EventHandler<object>? Navigated;
    void NavigateTo(UserControl view, object? parameter = null);
    void GoBack();
    bool CanGoBack { get; }
    bool IsBackNavigation { get; }
}

public class NavigationService : INavigationService
{
    private readonly Stack<Func<UserControl>> _history = new();
    private bool _isBackNavigation;

    public event EventHandler<object>? Navigated;

    public bool CanGoBack => _history.Count > 1;
    public bool IsBackNavigation => _isBackNavigation;

    public void NavigateTo(UserControl view, object? parameter = null)
    {
        // Store the view directly for forward navigation (it's already constructed)
        _isBackNavigation = false;
        _history.Push(() => view);
        Navigated?.Invoke(this, view);
    }

    /// <summary>
    /// Register a factory for back-navigation that recreates the view fresh.
    /// Call this instead of NavigateTo when the view should be recreated on GoBack.
    /// </summary>
    public void NavigateWithFactory(Func<UserControl> factory, object? parameter = null)
    {
        _isBackNavigation = false;
        var view = factory();
        _history.Push(factory);
        Navigated?.Invoke(this, view);
    }

    public void GoBack()
    {
        if (_history.Count > 1)
        {
            _isBackNavigation = true;
            _history.Pop(); // Remove current
            var factory = _history.Peek();
            var freshView = factory(); // Recreate the view
            Navigated?.Invoke(this, freshView);
        }
    }
}
```

- [ ] **Step 2: Update MainWindow to clear sidebar on LandingPage navigation**

In MainWindow's navigation subscription (around line 40), add sidebar clearing:

```csharp
_navigation.Navigated += (_, view) =>
{
    // Reset sidebar when navigating to landing page
    if (view is LandingPage)
    {
        SidebarContent.Content = null;
    }

    if (MainContent is AnimatedContentControl animated)
    {
        if (_navigation.IsBackNavigation)
        {
            animated.NavigateBack(view);
        }
        else
        {
            animated.NavigateForward(view);
        }
    }
    else
    {
        MainContent.Content = view;
    }
};
```

- [ ] **Step 3: Use NavigateWithFactory for pages that need fresh back-navigation**

In `LandingPage.CourseCard_Click`, use factory-based navigation:

```csharp
private void CourseCard_Click(object sender, RoutedEventArgs e)
{
    if (sender is Button button && button.Tag is Course course)
    {
        if (_navigation is NavigationService navService)
        {
            navService.NavigateWithFactory(() =>
                new CoursePage(_courseService, _navigation, course, _tutorService, _downloadService, _progressService, _codeExecutionService));
        }
        else
        {
            var coursePage = new CoursePage(_courseService, _navigation, course, _tutorService, _downloadService, _progressService, _codeExecutionService);
            _navigation.NavigateTo(coursePage, course);
        }
    }
}
```

Apply the same pattern in `CourseSidebar.LessonItem_Click` and `LessonPage.PrevButton_Click`/`NextButton_Click`.

- [ ] **Step 4: Build and verify**

Run: `dotnet build native-app-wpf/CodeTutor.Wpf.csproj`
Expected: 0 errors, 0 warnings

- [ ] **Step 5: Commit**

```bash
git add native-app-wpf/Services/NavigationService.cs native-app-wpf/MainWindow.xaml.cs native-app-wpf/Views/*.cs
git commit -m "fix: recreate views on back-navigation to show fresh data

NavigationService now stores factory functions. GoBack() recreates
views instead of reusing stale cached instances. Sidebar resets
when navigating back to LandingPage."
```

---

### Task 3: Fix Partial Model Download Corruption

**Files:**
- Modify: `native-app-wpf/Services/ModelDownloadService.cs`

**Why:** If download is interrupted, a partial file remains on disk. `IsModelInstalledAsync()` returns true, and `LlamaTutorService.LoadModelAsync` tries to load the corrupt file. The 4GB size check only catches very small partials.

- [ ] **Step 1: Download to a temp file, rename on success**

In `ModelDownloadService.DownloadModelAsync`, change the download target to use a `.downloading` suffix, then rename on completion. Replace lines 107-159:

```csharp
var targetDirectory = Path.GetDirectoryName(CurrentModel.LocalPath)!;
Directory.CreateDirectory(targetDirectory);

var targetPath = CurrentModel.LocalPath;
var tempPath = targetPath + ".downloading";
var fileName = CurrentModel.ModelFile;

StatusChanged?.Invoke(this, $"Downloading {CurrentModel.Name}...");

// Clean up any previous partial download
if (File.Exists(tempPath))
{
    File.Delete(tempPath);
}

var url = $"{HuggingFaceBaseUrl}/{CurrentModel.HuggingFaceRepo}/resolve/main/{fileName}";

System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] Downloading from: {url}");
System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] Saving to: {tempPath}");

using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

if (!response.IsSuccessStatusCode)
{
    var errorMsg = $"Download failed: HTTP {(int)response.StatusCode} - {response.ReasonPhrase}";
    System.Diagnostics.Debug.WriteLine($"[ModelDownloadService] {errorMsg}");
    StatusChanged?.Invoke(this, errorMsg);
    return false;
}

var totalBytes = response.Content.Headers.ContentLength ?? 0;

await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
await using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

var buffer = new byte[81920];
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

// Close the file stream before renaming
await fileStream.DisposeAsync();

// Atomic rename: only move to final path on complete download
File.Move(tempPath, targetPath, overwrite: true);

StatusChanged?.Invoke(this, $"{CurrentModel.Name} download complete!");
return true;
```

- [ ] **Step 2: Clean up temp file in catch blocks**

In each catch block, add cleanup:

```csharp
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
    StatusChanged?.Invoke(this, errorMsg);
    return false;
}
catch (Exception ex)
{
    CleanupPartialDownload();
    var errorMsg = $"Download failed: {ex.Message}";
    StatusChanged?.Invoke(this, errorMsg);
    return false;
}
```

Add the helper method:

```csharp
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
```

- [ ] **Step 3: Build and verify**

Run: `dotnet build native-app-wpf/CodeTutor.Wpf.csproj`
Expected: 0 errors, 0 warnings

- [ ] **Step 4: Commit**

```bash
git add native-app-wpf/Services/ModelDownloadService.cs
git commit -m "fix: download model to temp file, rename on success

Downloads to .downloading suffix file. Only renames to final path
after complete transfer. Cleans up partial file on cancellation,
network error, or any other failure. Prevents corrupt model files
from being loaded by LlamaTutorService."
```

---

### Task 4: Implement ProgressService Streak and Session Tracking

**Files:**
- Modify: `native-app-wpf/Models/UserProgress.cs`
- Modify: `native-app-wpf/Services/ProgressService.cs`

**Why:** `GetCurrentStreak()` always returns 0 and `TimeThisWeek` is always `TimeSpan.Zero`. Both are placeholder implementations. The streak feature UI is permanently hidden because of the `if (stats.CurrentStreak > 0)` guard.

- [ ] **Step 1: Add tracking fields to UserProgress model**

In `native-app-wpf/Models/UserProgress.cs`, add:

```csharp
[JsonPropertyName("dailyActivity")]
public Dictionary<string, bool> DailyActivity { get; set; } = new();

[JsonPropertyName("sessionStartTimes")]
public List<DateTime> SessionStartTimes { get; set; } = new();
```

The `DailyActivity` key is a date string like `"2026-03-26"`, value is always `true`. This is intentionally simple -- no need for session duration tracking, just "was the user active today".

- [ ] **Step 2: Implement GetCurrentStreak**

Replace the placeholder in `native-app-wpf/Services/ProgressService.cs`:

```csharp
public int GetCurrentStreak()
{
    if (_cachedProgress == null || _cachedProgress.DailyActivity.Count == 0)
        return 0;

    var today = DateTime.UtcNow.Date;
    int streak = 0;

    // Count consecutive days backwards from today (or yesterday if not yet active today)
    var checkDate = _cachedProgress.DailyActivity.ContainsKey(today.ToString("yyyy-MM-dd"))
        ? today
        : today.AddDays(-1);

    while (_cachedProgress.DailyActivity.ContainsKey(checkDate.ToString("yyyy-MM-dd")))
    {
        streak++;
        checkDate = checkDate.AddDays(-1);
    }

    return streak;
}
```

- [ ] **Step 3: Record daily activity on lesson completion**

In `MarkLessonCompleteAsync`, add activity tracking:

```csharp
public async Task MarkLessonCompleteAsync(string lessonId)
{
    var progress = await LoadProgressAsync();
    progress.CompletedLessons.Add(lessonId);

    // Track daily activity for streak calculation
    var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
    progress.DailyActivity.TryAdd(today, true);

    await SaveProgressAsync(progress);
}
```

- [ ] **Step 4: Replace TimeThisWeek placeholder with session count**

In `GetCourseProgressAsync`, replace `TimeSpan.Zero`:

```csharp
// Replace the TimeThisWeek calculation:
var weekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
int daysActiveThisWeek = 0;
for (var d = weekStart; d <= DateTime.UtcNow.Date; d = d.AddDays(1))
{
    if (progress.DailyActivity.ContainsKey(d.ToString("yyyy-MM-dd")))
        daysActiveThisWeek++;
}

return new CourseProgressStats(
    CompletedLessons: completed,
    TotalLessons: total,
    PercentComplete: Math.Round(percent, 1),
    CurrentStreak: GetCurrentStreak(),
    TimeThisWeek: TimeSpan.FromDays(daysActiveThisWeek) // Days active this week
);
```

- [ ] **Step 5: Add logging for corrupt progress file instead of silent reset**

Replace the bare catch in `LoadProgressAsync`:

```csharp
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"[ProgressService] Failed to load progress, starting fresh: {ex.Message}");
    // Back up the corrupt file for debugging
    try
    {
        var backupPath = _progressFilePath + ".corrupt";
        if (File.Exists(_progressFilePath))
            File.Copy(_progressFilePath, backupPath, overwrite: true);
    }
    catch { /* Best-effort backup */ }

    _cachedProgress = new UserProgress();
}
```

- [ ] **Step 6: Build and verify**

Run: `dotnet build native-app-wpf/CodeTutor.Wpf.csproj`
Expected: 0 errors, 0 warnings

Run: `dotnet test native-app.Tests/native-app.Tests.csproj`
Expected: 261 passed

- [ ] **Step 7: Commit**

```bash
git add native-app-wpf/Models/UserProgress.cs native-app-wpf/Services/ProgressService.cs
git commit -m "feat: implement streak tracking and daily activity logging

GetCurrentStreak() now counts consecutive active days.
MarkLessonCompleteAsync records daily activity. TimeThisWeek
reports days active this week. Corrupt progress files are backed
up and logged instead of silently reset."
```

---

### Task 5: Fix Temp File Cleanup in CodeExecutionService

**Files:**
- Modify: `native-app-wpf/Services/CodeExecutionService.cs`
- Modify: `native-app-wpf/Services/InteractiveProcessSession.cs`

**Why:** Every code execution leaks temp files. Over time this accumulates hundreds of files in TEMP.

- [ ] **Step 1: Track temp files in InteractiveProcessSession and clean up on dispose**

Add a temp file path field to `InteractiveProcessSession`:

```csharp
public class InteractiveProcessSession : IInteractiveSession
{
    private readonly Process _process;
    private readonly CancellationTokenSource _cts;
    private readonly string? _tempFilePath;
    private readonly string? _tempDirPath;
    private bool _isDisposed;

    // ... events ...

    public InteractiveProcessSession(Process process, string? tempFilePath = null, string? tempDirPath = null)
    {
        _process = process;
        _cts = new CancellationTokenSource();
        _tempFilePath = tempFilePath;
        _tempDirPath = tempDirPath;
        // ... existing event wiring ...
    }
```

In `Dispose`, clean up temp files:

```csharp
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
```

- [ ] **Step 2: Pass temp paths when creating sessions in CodeExecutionService**

In `StartLocalSessionAsync`, pass the temp file path:

```csharp
return new InteractiveProcessSession(process, tempFilePath: tempFile);
```

In `StartJavaSessionAsync`, pass the temp directory:

```csharp
return new InteractiveProcessSession(process, tempDirPath: tempDir);
```

- [ ] **Step 3: Fix the bare catch in InteractiveProcessSession.StopAsync**

Replace line 73:

```csharp
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
```

- [ ] **Step 4: Build and verify**

Run: `dotnet build native-app-wpf/CodeTutor.Wpf.csproj`
Expected: 0 errors, 0 warnings

- [ ] **Step 5: Commit**

```bash
git add native-app-wpf/Services/CodeExecutionService.cs native-app-wpf/Services/InteractiveProcessSession.cs
git commit -m "fix: clean up temp files on interactive session dispose

InteractiveProcessSession now accepts temp file/dir paths and
deletes them on Dispose. StopAsync logs errors instead of
swallowing them silently."
```

---

### Task 6: Fix Async Safety in View Constructors

**Files:**
- Modify: `native-app-wpf/Views/LessonPage.xaml.cs`
- Modify: `native-app-wpf/Views/CoursePage.xaml.cs`

**Why:** `LoadLesson()` and `CheckCompletionStatus()` are `async void` called from constructors. Unhandled exceptions crash the app. `CoursePage.LoadProgressAsync()` calculates progress bar width before layout (ActualWidth is 0).

- [ ] **Step 1: Move async calls from LessonPage constructor to Loaded event**

In `LessonPage` constructor, replace lines 45-47:

```csharp
// REMOVE from constructor:
// LoadLesson();
// SetupNavigation();   <-- keep this, it's synchronous
// CheckCompletionStatus();

// REPLACE with:
SetupNavigation();
Loaded += LessonPage_Loaded;
```

Add the Loaded handler:

```csharp
private async void LessonPage_Loaded(object sender, RoutedEventArgs e)
{
    Loaded -= LessonPage_Loaded; // Only fire once
    try
    {
        await LoadLessonAsync();
        await CheckCompletionStatusAsync();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[LessonPage] Load failed: {ex.Message}");
    }
}
```

Rename `LoadLesson` to `LoadLessonAsync` and make it return `Task` (change `async void` to `async Task`). Same for `CheckCompletionStatus` -> `CheckCompletionStatusAsync`.

- [ ] **Step 2: Move CoursePage.LoadProgressAsync to Loaded event**

In `CoursePage` constructor, replace line 40:

```csharp
// REMOVE: LoadProgressAsync();
// ADD:
Loaded += CoursePage_Loaded;
```

Add the handler:

```csharp
private async void CoursePage_Loaded(object sender, RoutedEventArgs e)
{
    Loaded -= CoursePage_Loaded;
    try
    {
        await LoadProgressAsync();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[CoursePage] Load progress failed: {ex.Message}");
    }
}
```

Change `LoadProgressAsync` from `async void` to `async Task`.

This also fixes the progress bar width issue -- by the time `Loaded` fires, `ActualWidth` is non-zero.

- [ ] **Step 3: Build and verify**

Run: `dotnet build native-app-wpf/CodeTutor.Wpf.csproj`
Expected: 0 errors, 0 warnings

- [ ] **Step 4: Commit**

```bash
git add native-app-wpf/Views/LessonPage.xaml.cs native-app-wpf/Views/CoursePage.xaml.cs
git commit -m "fix: move async operations from constructors to Loaded events

Prevents unhandled async void exceptions from crashing the app.
CoursePage progress bar now calculates width after layout completes
(ActualWidth is non-zero in Loaded)."
```

---

### Task 7: Clean Up Dead Code and Fix Latent ONNX Tokenizer Bug

**Files:**
- Modify: `native-app-wpf/Services/Phi4TutorService.cs`
- Modify: `native-app-wpf/Services/QwenTutorService.cs`

**Why:** Both ONNX services use `AppendLine()` which injects `\r\n` on Windows, breaking tokenizer special tokens. `Phi4TutorService.BuildPrompt()` is dead code never called. These services aren't wired in DI currently but are latent bugs if anyone switches backends.

- [ ] **Step 1: Remove dead BuildPrompt method from Phi4TutorService**

Delete lines 172-223 of `native-app-wpf/Services/Phi4TutorService.cs` (the entire `BuildPrompt` method that is never called).

- [ ] **Step 2: Fix AppendLine in Phi4TutorService.BuildPromptOptimized**

In `native-app-wpf/Services/Phi4TutorService.cs`, replace all `sb.AppendLine(...)` calls in `BuildPromptOptimized` with `sb.Append(...).Append('\n')`:

```csharp
// Example: change every occurrence of:
sb.AppendLine("<|system|>");
// to:
sb.Append("<|system|>").Append('\n');
```

Apply this to every `AppendLine` call in the method (approximately 15 occurrences).

- [ ] **Step 3: Fix AppendLine in QwenTutorService.BuildPromptOptimized**

Same pattern in `native-app-wpf/Services/QwenTutorService.cs`. Replace all `sb.AppendLine(...)` calls with `sb.Append(...).Append('\n')`.

- [ ] **Step 4: Build and verify**

Run: `dotnet build native-app-wpf/CodeTutor.Wpf.csproj`
Expected: 0 errors, 0 warnings

- [ ] **Step 5: Commit**

```bash
git add native-app-wpf/Services/Phi4TutorService.cs native-app-wpf/Services/QwenTutorService.cs
git commit -m "fix: replace AppendLine with Append+newline in ONNX tutor services

AppendLine() on Windows injects \\r into special tokens like
<|im_start|> and <|im_end|>, breaking ONNX tokenization. Apply
the same fix already in LlamaTutorService. Remove dead BuildPrompt
method from Phi4TutorService."
```

---

### Task 8: Fix Installer Documentation

**Files:**
- Modify: `BUILD.md`
- Delete: `build-installer.ps1.backup`

**Why:** BUILD.md documents impossible Linux/macOS builds for a WPF-only app, references nonexistent `CodeTutor.Native.exe`, documents a nonexistent `-SelfContained` parameter, and references nonexistent files. The backup build script references an obsolete project structure.

- [ ] **Step 1: Remove impossible Linux/macOS build sections from BUILD.md**

Remove the Linux and macOS build sections entirely. Add a note:

```markdown
> **Note:** Code Tutor is a WPF application and only supports Windows.
> Cross-platform support would require porting to a framework like Avalonia or MAUI.
```

- [ ] **Step 2: Fix executable name references**

Replace all occurrences of `CodeTutor.Native.exe` with `CodeTutor.exe` and `CodeTutor.Native.csproj` with `CodeTutor.Wpf.csproj`.

- [ ] **Step 3: Fix parameter documentation**

Remove the `-SelfContained $false` troubleshooting tip. Document the actual parameters: `-Configuration`, `-Version`, `-SkipBuild`.

- [ ] **Step 4: Remove references to nonexistent docs**

Remove references to `TROUBLESHOOTING.md` and `CONTRIBUTING.md`.

- [ ] **Step 5: Delete stale backup file**

```bash
rm build-installer.ps1.backup
```

- [ ] **Step 6: Commit**

```bash
git add BUILD.md
git rm build-installer.ps1.backup
git commit -m "docs: fix BUILD.md for WPF-only project, remove stale backup

Remove impossible Linux/macOS build sections. Fix executable name
references (CodeTutor.exe not CodeTutor.Native.exe). Document
actual build script parameters. Delete obsolete backup script."
```

---

## Self-Review Checklist

1. **Spec coverage:** All 2 remaining app BLOCKERs (stale navigation, partial download) are covered in Tasks 2 and 3. All ~12 WARNINGs are addressed across Tasks 1-7. Installer issues in Task 8. Content BLOCKERs are intentionally excluded (they require content authoring, not code changes).

2. **Placeholder scan:** No TBD/TODO/placeholder text in any task. All code blocks are complete.

3. **Type consistency:** `IProgressService` and `ICodeExecutionService` type names are consistent across Tasks 1, 5, 6. `NavigateWithFactory` method name is consistent between definition (Task 2 Step 1) and usage (Task 2 Step 3). `InteractiveProcessSession` constructor signature change in Task 5 matches the call sites in Steps 1 and 2.
