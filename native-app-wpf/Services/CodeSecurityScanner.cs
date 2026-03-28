using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Result of security scanning before execution.
/// </summary>
public record SecurityScanResult(bool IsSafe, string BlockedPatterns, string Message);

/// <summary>
/// Scans code for potentially dangerous patterns before execution.
/// </summary>
public class CodeSecurityScanner
{
    private readonly Dictionary<string, List<SecurityPattern>> LanguagePatterns;
    private readonly List<SecurityPattern> UniversalPatterns;

    private static readonly Dictionary<string, string[]> AllowedImports = new()
    {
        ["python"] = new[]
        {
            "math", "random", "datetime", "collections", "itertools", "functools",
            "statistics", "fractions", "decimal", "typing", "dataclasses",
            "enum", "json", "re", "string", "textwrap", "hashlib",
            "base64", "copy", "pprint", "numbers", "abc"
        },
        ["java"] = new[]
        {
            "java.lang.", "java.util.", "java.math.", "java.time.", "java.text.",
            "java.util.stream.", "java.util.function.", "java.util.regex.",
            "java.nio.charset.", "java.security.MessageDigest"
        },
        ["kotlin"] = new[] { "kotlin.", "kotlinx." },
        ["dart"] = new[] { "dart:core", "dart:collection", "dart:math", "dart:convert", "dart:typed_data" },
    };

    public CodeSecurityScanner()
    {
        LanguagePatterns = new Dictionary<string, List<SecurityPattern>>();

        // Python patterns - using regular strings with double escaping
        LanguagePatterns["python"] = new List<SecurityPattern>
        {
            new SecurityPattern("\\bos\\.system\\s*\\(", "Command execution via os.system"),
            new SecurityPattern("\\bos\\.popen\\s*\\(", "Command execution via os.popen"),
            new SecurityPattern("\\bsubprocess\\.(call|run|Popen|check_output)\\s*\\(", "Subprocess execution"),
            new SecurityPattern("\\bexec\\s*\\(", "Dynamic code execution via exec()"),
            new SecurityPattern("\\beval\\s*\\(", "Dynamic code execution via eval()"),
            new SecurityPattern("\\bcompile\\s*\\(", "Code compilation"),
            new SecurityPattern("\\b__import__\\s*\\(", "Dynamic import"),
            new SecurityPattern("\\bimportlib", "Dynamic module loading"),
            new SecurityPattern("\\bsocket\\.", "Network socket operations"),
            new SecurityPattern("\\burllib", "Network URL operations"),
            new SecurityPattern("\\brequests\\.", "HTTP requests"),
            new SecurityPattern("\\bhttp\\.(client|server)", "HTTP operations"),
            new SecurityPattern("\\bftplib", "FTP operations"),
            new SecurityPattern("\\bsmtplib", "SMTP operations"),
            new SecurityPattern("\\bctypes\\.", "Native library access via ctypes"),
            new SecurityPattern("\\bcffi\\.", "Foreign function interface"),
            new SecurityPattern("\\bswig\\b", "SWIG native interface"),
            new SecurityPattern("\\bmultiprocessing\\.", "Process spawning"),
            new SecurityPattern("\\bpathlib\\.(Path|PurePath)\\s*\\([^'\"]*\\.{2,}", "Path traversal attempt"),
            new SecurityPattern("\\.\\.\\/|\\.\\.\\\\", "Directory traversal"),
        };

        // JavaScript patterns
        LanguagePatterns["javascript"] = new List<SecurityPattern>
        {
            new SecurityPattern("\\beval\\s*\\(", "Dynamic code execution via eval()"),
            new SecurityPattern("\\bFunction\\s*\\(\\s*[\"'].*[\"']\\s*\\)", "Dynamic function creation"),
            new SecurityPattern("\\bsetTimeout\\s*\\(\\s*[\"'].*[\"']\\s*\\)", "setTimeout with string"),
            new SecurityPattern("\\bsetInterval\\s*\\(\\s*[\"'].*[\"']\\s*\\)", "setInterval with string"),
            new SecurityPattern("\\brequire\\s*\\(\\s*[\"']child_process[\"']\\s*\\)", "Process spawning"),
            new SecurityPattern("\\bchild_process\\.", "Child process module"),
            new SecurityPattern("\\bprocess\\.", "Process object access"),
            new SecurityPattern("\\bglobal\\.", "Global object manipulation"),
            new SecurityPattern("\\bws\\b|\\bWebSocket\\b", "WebSocket connections"),
            new SecurityPattern("\\bhttp\\.", "HTTP module"),
            new SecurityPattern("\\bhttps\\.", "HTTPS module"),
            new SecurityPattern("\\bnet\\.", "Network module"),
            new SecurityPattern("\\bdgram\\.", "UDP/datagram module"),
            new SecurityPattern("\\bfetch\\s*\\(", "Fetch API"),
            new SecurityPattern("\\bXMLHttpRequest\\b", "XHR network requests"),
        };

        // Java patterns
        LanguagePatterns["java"] = new List<SecurityPattern>
        {
            new SecurityPattern("\\bRuntime\\.getRuntime\\(\\)\\.exec\\s*\\(", "Process execution"),
            new SecurityPattern("\\bProcessBuilder\\b", "Process builder"),
            new SecurityPattern("\\bSystem\\.exit\\s*\\(", "System exit call"),
            new SecurityPattern("\\bRuntime\\.halt\\s*\\(", "Runtime halt"),
            new SecurityPattern("\\bURL\\s*\\(\\s*[\"']", "URL instantiation"),
            new SecurityPattern("\\bURLConnection\\b", "URLConnection"),
            new SecurityPattern("\\bHttpClient\\b", "HTTP client"),
            new SecurityPattern("\\bSocket\\s*\\(", "Socket creation"),
            new SecurityPattern("\\bServerSocket\\s*\\(", "Server socket creation"),
            new SecurityPattern("\\bSystem\\.loadLibrary\\s*\\(", "Native library loading"),
            new SecurityPattern("\\bSystem\\.load\\s*\\(", "Native code loading"),
            new SecurityPattern("\\bJNI\\b", "Java Native Interface"),
            new SecurityPattern("\\bUnsafe\\b", "Unsafe operations"),
            new SecurityPattern("setAccessible\\s*\\(", "Reflection accessibility"),
            new SecurityPattern("java\\.lang\\.reflect", "Reflection package"),
        };

        // Kotlin patterns
        LanguagePatterns["kotlin"] = new List<SecurityPattern>
        {
            new SecurityPattern("\\bRuntime\\.getRuntime\\(\\)\\.exec\\s*\\(", "Process execution"),
            new SecurityPattern("\\bProcessBuilder\\b", "Process builder"),
            new SecurityPattern("\\bSystem\\.exit\\s*\\(", "System exit"),
            new SecurityPattern("\\bjava\\.net\\.(URL|URI|HttpURLConnection)\\b", "Network operations"),
            new SecurityPattern("\\bjava\\.net\\.Socket\\b", "Socket operations"),
            new SecurityPattern("\\bkhttp\\.", "KHTTP networking"),
            new SecurityPattern("\\bktor\\.", "Ktor networking"),
        };

        // Dart patterns
        LanguagePatterns["dart"] = new List<SecurityPattern>
        {
            new SecurityPattern("\\bProcess\\.(run|start)\\s*\\(", "Process execution"),
            new SecurityPattern("\\bdart:io\\s+.*\\bProcess\\b", "Process import"),
            new SecurityPattern("\\bdart:io\\s+.*\\bServerSocket\\b", "Server socket"),
            new SecurityPattern("\\bdart:io\\s+.*\\bSocket\\b", "Socket operations"),
            new SecurityPattern("\\bHttpServer\\.", "HTTP server"),
            new SecurityPattern("\\bHttpClient\\.", "HTTP client"),
            new SecurityPattern("\\bdart:ffi\\b", "FFI native interface"),
            new SecurityPattern("\\bdynamic\\s+library\\b", "Dynamic library loading"),
            new SecurityPattern("\\bimport\\s+[\"']dart:isolate[\"']", "Isolate spawning"),
        };

        // C# patterns
        LanguagePatterns["csharp"] = new List<SecurityPattern>
        {
            new SecurityPattern("\\bProcess\\.(Start|Run)\\s*\\(", "Process execution"),
            new SecurityPattern("\\bProcessStartInfo\\b", "Process start info"),
            new SecurityPattern("\\bEnvironment\\.(Exit|FailFast)\\s*\\(", "Environment exit"),
            new SecurityPattern("\\bWebClient\\b", "Web client"),
            new SecurityPattern("\\bHttpClient\\b", "HTTP client"),
            new SecurityPattern("\\bHttpWebRequest\\b", "HTTP web request"),
            new SecurityPattern("\\bSocket\\.(Connect|Bind|Listen)\\s*\\(", "Socket operations"),
            new SecurityPattern("\\bTcpClient\\b", "TCP client"),
            new SecurityPattern("\\bUdpClient\\b", "UDP client"),
            new SecurityPattern("\\bAssembly\\.(Load|LoadFrom|LoadFile)\\s*\\(", "Assembly loading"),
            new SecurityPattern("\\bAppDomain\\.(ExecuteAssembly|CreateInstanceFrom)\\s*\\(", "AppDomain operations"),
            new SecurityPattern("\\bDllImport\\s*\\(", "P/Invoke native call"),
            new SecurityPattern("\\bLoadLibrary\\b", "Native library loading"),
            new SecurityPattern("\\bActivator\\.CreateInstance\\s*\\(\\s*typeof\\s*\\(\\s*System\\.Reflection", "Reflection activation"),
        };

        // Universal patterns
        UniversalPatterns = new List<SecurityPattern>
        {
            new SecurityPattern("\\brm\\s+-rf\\s+[/\\~]", "Dangerous rm command"),
            new SecurityPattern("\\bformat\\s+[C-Z]:", "Disk format attempt"),
            new SecurityPattern("\\bdel\\s+/[fq]\\s+.*\\*\\.\\*", "Mass delete attempt"),
            new SecurityPattern("\\bwget\\s+.*\\|.*sh", "Pipe wget to shell"),
            new SecurityPattern("\\bcurl\\s+.*\\|.*sh", "Pipe curl to shell"),
        };
    }

    public SecurityScanResult ScanCode(string code, string language)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new SecurityScanResult(true, "", "Empty code");
        }

        var blockedPatterns = new List<string>();

        // Check language-specific patterns
        if (LanguagePatterns.TryGetValue(language, out var patterns))
        {
            foreach (var pattern in patterns)
            {
                if (Regex.IsMatch(code, pattern.Pattern, RegexOptions.IgnoreCase))
                {
                    blockedPatterns.Add($"{pattern.Description}");
                }
            }
        }

        // Check universal patterns
        foreach (var pattern in UniversalPatterns)
        {
            if (Regex.IsMatch(code, pattern.Pattern, RegexOptions.IgnoreCase))
            {
                blockedPatterns.Add($"{pattern.Description}");
            }
        }

        // Validate imports/packages
        var importValidation = ValidateImports(code, language);
        if (!importValidation.IsValid)
        {
            blockedPatterns.Add($"Blocked import: {importValidation.BlockedImport}");
        }

        if (blockedPatterns.Count > 0)
        {
            return new SecurityScanResult(
                false,
                string.Join("; ", blockedPatterns),
                $"Blocked {blockedPatterns.Count} security violation(s)"
            );
        }

        return new SecurityScanResult(true, "", "Code passed security scan");
    }

    private (bool IsValid, string? BlockedImport) ValidateImports(string code, string language)
    {
        if (!AllowedImports.TryGetValue(language, out var allowed))
        {
            return (true, null);
        }

        // Simple import validation - extract imports and check against allowlist
        var importPatterns = language switch
        {
            "python" => new[] { "^\\s*import\\s+(\\S+)", "^\\s*from\\s+(\\S+)\\s+import" },
            "javascript" => new[] { "^\\s*import\\s+.*?\\s+from\\s*[\"']([^\"']+)[\"']", "^\\s*const\\s+\\w+\\s*=\\s*require\\s*\\(\\s*[\"']([^\"']+)[\"']\\s*\\)" },
            "java" => new[] { "^\\s*import\\s+([\\w\\.]+);" },
            "kotlin" => new[] { "^\\s*import\\s+([\\w\\.]+)" },
            "dart" => new[] { "^\\s*import\\s*[\"']([^\"']+)[\"']" },
            _ => Array.Empty<string>()
        };

        var lines = code.Split('\n');
        foreach (var line in lines)
        {
            foreach (var pattern in importPatterns)
            {
                var match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    var importName = match.Groups[1].Value;

                    // Check if import is allowed
                    var isAllowed = allowed.Any(a => importName.StartsWith(a, StringComparison.OrdinalIgnoreCase)) ||
                                   allowed.Contains(importName, StringComparer.OrdinalIgnoreCase);

                    if (!isAllowed)
                    {
                        return (false, importName);
                    }
                }
            }
        }

        return (true, null);
    }

    private record SecurityPattern(string Pattern, string Description);
}
