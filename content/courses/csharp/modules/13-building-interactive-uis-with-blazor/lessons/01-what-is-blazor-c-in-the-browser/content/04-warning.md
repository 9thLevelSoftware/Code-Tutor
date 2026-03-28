---
type: "WARNING"
title: "Common Pitfalls"
---

## Watch Out For These Issues

**Forgetting the @ symbol**: In markup, `currentCount` without @ is literal text! Always use `@currentCount` to access C# variables. This is the #1 beginner mistake.

**Component naming**: Component files MUST start with uppercase! `Counter.razor` works, `counter.razor` may cause issues. Follow C# PascalCase convention.

**JavaScript habits**: Don't look for .js files or write onclick="function()". Blazor uses @onclick="CSharpMethod" - all interactivity is C#!

**Render mode required**: In .NET 9+, components need a render mode for interactivity. Without @rendermode, @onclick won't work! Static SSR has no interactivity.

**Hot Reload limitations**: Some changes require app restart. Adding new components or changing namespaces won't Hot Reload - restart the app.