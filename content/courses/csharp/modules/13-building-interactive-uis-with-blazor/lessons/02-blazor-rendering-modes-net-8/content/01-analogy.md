---
type: "ANALOGY"
title: "Understanding the Concept"
---

Imagine watching a movie:

STREAMING (Blazor Server):
• Movie plays on Netflix server
• Only video frames sent to your screen
• Fast startup, low download
• Need constant internet connection

DOWNLOADED (Blazor WebAssembly):
• Download entire movie to device
• Plays offline on your device
• Large download, but works offline
• Device does all the work

SMART MODE (Blazor Auto - .NET 9+):
• Starts streaming immediately (fast!)
• Downloads in background
• Switches to local playback when ready
• Best of both worlds!

.NET 9 UNIFIED RENDERING:
• Static SSR (Server-Side Rendering) - No interactivity, SEO-friendly
• Interactive Server - C# on server via SignalR
• Interactive WebAssembly - C# in browser via WASM
• Interactive Auto - Best of Server + WASM!

.NET 9 improvements:
• 25% faster WebAssembly startup
• Better reconnection experience
• [ExcludeFromInteractiveRouting] for static pages
• WebSocket compression for Server mode

Choose per component or page!

Think: Rendering mode = 'WHERE does C# code run - server or browser?'