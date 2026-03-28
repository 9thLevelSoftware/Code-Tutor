---
type: "EXAMPLE"
title: "Code Example"
---

This example demonstrates the concepts in action.

```csharp
// .NET 9 RENDERING MODES

// 1. STATIC SERVER-SIDE RENDERING (SSR)
// Default when NO @rendermode specified - no interactivity, fast SEO
@page "/products"
@* No @rendermode = Static SSR (server renders HTML, no interactivity) *@

<h3>Product List</h3>
@foreach (var product in products)
{
    <p>@product.Name</p>
}

// 2. INTERACTIVE SERVER
// C# runs on server, UI updates via SignalR
@page "/counter"
@rendermode InteractiveServer

<button @onclick="IncrementCount">Count: @count</button>

@code {
    private int count = 0;
    private void IncrementCount() => count++;
}

// 3. INTERACTIVE WEBASSEMBLY
// C# runs in browser via WebAssembly
@page "/calculator"
@rendermode InteractiveWebAssembly

<button @onclick="Calculate">Calculate</button>

// 4. INTERACTIVE AUTO (.NET 9 - BEST!)
// Starts with Server, switches to WASM when ready
@page "/dashboard"
@rendermode InteractiveAuto

<RealTimeChart />  // Server initially, then WASM

// COMPARISON TABLE
/*
                    Server      WebAssembly     Auto (.NET 9)
C# runs on:         Server      Browser         Both
Initial load:       Fast        Slow            Fast
Offline:            No          Yes             Yes*
Scalability:        Lower       Higher          Best
Latency:            Network     None            Hybrid
Best for:           Forms       SPAs            Everything
*/

// CONFIGURING IN PROGRAM.CS (.NET 9)
var builder = WebApplication.CreateBuilder(args);

// Add Blazor components with all render modes
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Map Blazor with all render modes enabled
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveAutoRenderMode();
```
