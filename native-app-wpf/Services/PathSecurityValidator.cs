using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CodeTutor.Wpf.Services;

/// <summary>
/// Validates file paths to prevent path traversal attacks.
/// </summary>
public static class PathSecurityValidator
{
    // Characters that are not allowed in file paths
    private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
    
    // Maximum path length to prevent buffer overflow scenarios
    private const int MaxPathLength = 260;
    
    // Suspicious patterns that might indicate path traversal attempts
    private static readonly string[] SuspiciousPatterns = new[]
    {
        "..",           // Parent directory traversal
        "~",            // Home directory reference  
        "%",            // Environment variable expansion
        "$",            // Shell variable (Unix)
        "\\",           // Windows backslash (for mixed paths)
        "//",           // Double slash
        ":",            // Drive letter or stream
        "*",            // Wildcard
        "?",            // Single char wildcard
        "|",            // Pipe
        "<",            // Redirection
        ">",            // Redirection
        "\x00",         // Null byte injection
    };

    /// <summary>
    /// Validates that an identifier (courseId, moduleId, lessonId) is safe to use in path construction.
    /// </summary>
    public static bool IsValidIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        if (identifier.Length > MaxPathLength)
            return false;

        // Check for null bytes
        if (identifier.Contains('\x00'))
            return false;

        // Check for invalid filename characters
        if (identifier.Any(c => InvalidFileNameChars.Contains(c)))
            return false;

        // Check for suspicious patterns
        foreach (var pattern in SuspiciousPatterns)
        {
            if (identifier.Contains(pattern))
                return false;
        }

        // Must be alphanumeric with only safe separators
        // Allow: letters, numbers, hyphens, underscores, and single dots (for extensions)
        var safePattern = new Regex(@"^[a-zA-Z0-9._-]+$", RegexOptions.Compiled);
        if (!safePattern.IsMatch(identifier))
            return false;

        // Check that dots are only used for valid extensions, not traversal
        if (identifier.Contains(".."))
            return false;

        // Cannot start with a dot (hidden files)
        if (identifier.StartsWith("."))
            return false;

        // Cannot end with a dot (Windows compatibility)
        if (identifier.EndsWith("."))
            return false;

        return true;
    }

    /// <summary>
    /// Validates that a constructed path stays within the expected base directory.
    /// </summary>
    public static bool IsPathWithinBaseDirectory(string fullPath, string baseDirectory)
    {
        try
        {
            // Normalize both paths
            var normalizedFullPath = Path.GetFullPath(fullPath);
            var normalizedBaseDir = Path.GetFullPath(baseDirectory);

            // Ensure base directory ends with separator for proper prefix checking
            if (!normalizedBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                normalizedBaseDir += Path.DirectorySeparatorChar;

            // Check if the full path starts with the base directory
            return normalizedFullPath.StartsWith(normalizedBaseDir, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Sanitizes a filename to make it safe for filesystem operations.
    /// </summary>
    public static string SanitizeFilename(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return "unnamed";

        // Remove invalid characters
        var sanitized = new string(filename
            .Where(c => !InvalidFileNameChars.Contains(c))
            .ToArray());

        // Remove path traversal sequences
        sanitized = sanitized.Replace("..", "");
        
        // Trim whitespace and dots
        sanitized = sanitized.Trim(' ', '.', '-', '_');

        // Limit length
        if (sanitized.Length > 200)
            sanitized = sanitized.Substring(0, 200);

        // Ensure not empty
        if (string.IsNullOrWhiteSpace(sanitized))
            return "unnamed";

        return sanitized.ToLowerInvariant();
    }

    /// <summary>
    /// Throws an exception if the identifier is not valid.
    /// </summary>
    public static void ValidateIdentifierOrThrow(string identifier, string paramName)
    {
        if (!IsValidIdentifier(identifier))
        {
            throw new ArgumentException(
                $"Invalid identifier '{identifier}'. Identifiers must be alphanumeric with hyphens and underscores only.",
                paramName);
        }
    }
}
