---
type: "WARNING"
title: "Common Pitfalls"
---

## Watch Out For These Issues!

**File path injection**: Never build file paths from user input without validation! `File.ReadAllText(userInput)` can read ANY file on disk including sensitive system files. This is a critical security vulnerability. Always sanitize and restrict to known directories using `Path.Combine()` with a base directory, then verify the resolved path stays within bounds using `Path.GetFullPath()` and string comparisons.

**Not disposing streams**: `StreamReader` and `StreamWriter` implement `IDisposable`. Forgetting to dispose them leaks file handles and can exhaust system resources! Always use `using var reader = new StreamReader(path);` which automatically disposes at the end of scope. The simpler `File.ReadAllText()` / `File.WriteAllText()` methods handle disposal internally and are preferred for simple scenarios.

**Encoding issues**: `File.ReadAllText()` defaults to UTF-8, but legacy files may use Windows-1252, Latin1, or other encodings. Garbled text or corrupted data usually indicates encoding mismatches. Specify encoding explicitly when reading legacy files: `File.ReadAllText(path, Encoding.Latin1)`. When writing, always use UTF-8 unless you have a specific interoperability requirement.

**Text files for structured data**: CSV and text files seem simple but break when data contains commas, newlines, or special characters. Parsing CSV with string.Split(',') fails on quoted fields containing commas. For anything beyond simple configuration, use proper serialization (JSON, XML) or a database. The apparent simplicity of text files creates hidden complexity in parsing edge cases.

**Ignoring file locking**: When one process opens a file, other processes may be blocked from accessing it. This causes race conditions and IOExceptions in multi-threaded or multi-process scenarios. Use appropriate FileShare flags when opening streams, implement retry logic with exponential backoff, or use databases which handle concurrency properly.

**Blocking the thread with large files**: Reading multi-gigabyte files with `File.ReadAllText()` loads everything into memory and blocks the thread. Use streaming APIs (`FileStream`, `StreamReader.ReadLine()`) for large files to process data in chunks without memory pressure. Async file I/O (`ReadAllTextAsync`) prevents blocking threads but still loads everything into memory.

**Hardcoded paths**: `File.ReadAllText("C:\\data\\file.txt")` works on your machine but fails in production, Docker containers, or other developers' environments. Always use configuration for file paths, environment variables, or well-known locations like `Path.GetTempPath()`. The `SpecialFolder` enum provides safe access to user directories across platforms.
