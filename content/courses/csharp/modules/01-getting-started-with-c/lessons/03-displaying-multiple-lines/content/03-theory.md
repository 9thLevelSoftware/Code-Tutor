---
type: "THEORY"
title: "Syntax Breakdown"
---

## Breaking Down the Syntax

**`\n`**: The backslash-n creates a newline (line break). It's called an 'escape sequence' – special characters that do something instead of displaying.

**`Multiple WriteLine vs. \n`**: Both methods work! Multiple WriteLine is clearer for beginners. Using \n is more compact but harder to read at first.

## Common Escape Sequences

| Sequence | Meaning |
|----------|--------|
| `\n` | Newline (line break) |
| `\t` | Tab (horizontal indent) |
| `\r` | Carriage return |
| `\\` | Literal backslash |
| `\"` | Literal double quote |
| `\'` | Literal single quote |
| `\0` | Null character |
| `\e` | Escape character (C# 13+) |

**C# 13 Feature**: The `\e` escape sequence is new in C# 13! It represents the escape character (Unicode 0x1B), commonly used for ANSI terminal colors:

```csharp
// C# 13: \e escape sequence for terminal colors
Console.WriteLine("\e[32mGreen text\e[0m");
Console.WriteLine("\e[1;31mBold red\e[0m");
```

Note: ANSI colors may not work in all terminals.

## Write vs. WriteLine

**`Console.Write()`**: Outputs text WITHOUT moving to a new line.
**`Console.WriteLine()`**: Outputs text AND moves to a new line.

```csharp
Console.Write("Hello ");
Console.Write("World");  // Output: Hello World (same line)

Console.WriteLine("Hello");
Console.WriteLine("World"); // Output on separate lines
```

## Understanding String Concatenation with Newlines

When building multi-line strings, you can concatenate (join) strings together using the `+` operator:

```csharp
string message = "Line 1" + "\n" + "Line 2" + "\n" + "Line 3";
Console.WriteLine(message);
```

This creates a single string with embedded newlines before passing it to WriteLine.

## Environment.NewLine for Cross-Platform Compatibility

Different operating systems use different characters for line breaks:
- **Windows**: Uses `\r\n` (carriage return + newline)
- **Linux/macOS**: Uses `\n` (newline only)

**`Environment.NewLine`** automatically uses the correct line ending for the current operating system:

```csharp
string multiLine = "Line A" + Environment.NewLine + "Line B";
Console.WriteLine(multiLine);
```

This is the most robust way to create multi-line strings that work on any platform.

## Verbatim Strings (@) for Multi-Line Content

The `@` symbol before a string creates a "verbatim string literal" that preserves:
- Line breaks exactly as typed
- Backslashes without escaping

```csharp
Console.WriteLine(@"First line
Second line
Third line");
```

**Important**: Escape sequences like `\n` and `\t` are NOT interpreted in verbatim strings. The actual line breaks in your code become the line breaks in the output.

## Interpolated Strings with Newlines

C# string interpolation (the `$` prefix) works beautifully with escape sequences:

```csharp
string name = "Alice";
int age = 25;
Console.WriteLine($"Name: {name}\nAge: {age}\nCity: New York");
```

The variables are evaluated and the `\n` characters create line breaks, all in one clean expression.