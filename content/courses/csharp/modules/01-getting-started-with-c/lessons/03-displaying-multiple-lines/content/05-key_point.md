---
type: "KEY_POINT"
title: "Multi-Line Output and Escape Sequences"
---

## Key Takeaways

- **Use `\n` for newlines inside a single string** -- `Console.WriteLine("Line1\nLine2")` prints two lines. Multiple `WriteLine` calls also work but are more verbose.

- **Escape sequences are special character combinations** -- `\t` (tab), `\\` (literal backslash), `\"` (literal quote). C# 13 adds `\e` for terminal color codes.

- **Choose readability over cleverness** -- multiple `WriteLine` calls are clearer for beginners; escape sequences are more compact for experienced developers. Pick whichever your team can read faster.

## Escape Sequence Quick Reference

| Sequence | Output | Common Use |
|----------|--------|------------|
| `\n` | New line | Multi-line messages |
| `\t` | Tab | Aligning columns |
| `\\` | Single backslash | File paths, regex |
| `\"` | Double quote | Quoting inside strings |
| `\e` | Escape (C# 13) | Terminal colors |

## Output Method Decision Guide

| Method | Best For | Example |
|--------|----------|---------|
| Multiple WriteLine | Simple, clear output | Menu display |
| `\n` escape | Compact code | Short multi-line |
| `Environment.NewLine` | Cross-platform files | Saving to disk |
| Verbatim `@` | ASCII art, code blocks | Formatted blocks |
| Interpolated `$` | Dynamic content | User messages |

## Write vs WriteLine

- **`Console.Write()`**: Stays on the same line
- **`Console.WriteLine()`**: Adds automatic newline after

Both work with all string types (regular, interpolated, verbatim).

## C# 13 Escape Sequence Highlight

The new `\e` escape sequence (C# 13) simplifies terminal color codes:

```csharp
// Old way (still works)
Console.WriteLine("\x1b[32mGreen Text\x1b[0m");

// C# 13 way (clearer!)
Console.WriteLine("\e[32mGreen Text\e[0m");
```

Note: Terminal colors depend on your console supporting ANSI escape codes.

## Verbatim String Rules

When using `@` for verbatim strings:
- **Line breaks in code = line breaks in output**
- **Escape sequences are NOT processed** (except `""` for double quotes)
- **Perfect for multi-line literals** that are hard to read with `\n`

```csharp
// This verbatim string:
Console.WriteLine(@"Line 1
Line 2");

// Produces the same as:
Console.WriteLine("Line 1\nLine 2");
```

## Best Practices Summary

1. **For beginners**: Use multiple WriteLine calls - they're clearest
2. **For file output**: Use Environment.NewLine for cross-platform compatibility
3. **For compact code**: Use `\n` escape sequences
4. **For complex layouts**: Consider verbatim strings with `@`
5. **Always test** your output looks correct on your target platform

## Common Pitfalls to Remember

- Forward slash `/` vs backslash `\` - escape sequences need backslash
- Verbatim strings don't process escapes - `\n` stays as literal text
- Raw string literals (triple quotes) also ignore escape sequences
- Missing quotes around escape sequences cause compiler errors
