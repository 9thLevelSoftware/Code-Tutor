---
type: "THEORY"
title: "Syntax Breakdown"
---

## Breaking Down the Syntax

**`@rendermode InteractiveServer`**: .NET 9 syntax for render mode. C# runs on server. UI updates over SignalR connection. Low initial payload, requires connection.

**`@rendermode InteractiveWebAssembly`**: C# compiles to WebAssembly, runs in browser. Large download, but fully client-side. Works offline after load!

**`@rendermode InteractiveAuto`**: NEW in .NET 9! Starts with Server (fast), downloads WASM in background, seamlessly switches. Best user experience!

**`Static SSR`**: The default when no @rendermode is specified. No interactivity, pure HTML. Like traditional web pages. Fast, SEO-friendly. Use for content pages, blogs, documentation.