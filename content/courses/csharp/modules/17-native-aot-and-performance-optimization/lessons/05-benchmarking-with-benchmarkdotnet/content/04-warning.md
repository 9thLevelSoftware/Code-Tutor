---
type: "WARNING"
title: "Common Pitfalls"
---

## Benchmarking Gotchas

**Debug Mode Invalidates Results**: Running benchmarks in Debug mode produces meaningless results due to disabled optimizations. ALWAYS use `dotnet run -c Release`. Debug performance can be 10-100x slower than Release.

**Dead Code Elimination**: If your benchmark method doesn't return a value or use its result, the JIT may optimize away the entire computation! Always return results or use `[Benchmark]` methods that consume their outputs.

**Micro-Benchmark Limitations**: Extremely fast operations (nanoseconds) are hard to measure accurately. Results may have high variance. Consider testing with realistic workloads rather than micro-optimizations.

**Environment Interference**: Background processes, thermal throttling, and power management affect results. Run benchmarks on a quiet system, preferably with multiple iterations to detect anomalies.

**Comparing Across Runtimes**: When comparing .NET 9 vs .NET 10, use separate jobs with explicit runtime configuration. BenchmarkDotNet supports `[Config]` attributes for multi-runtime comparisons.

**Visual Studio Integration**: Visual Studio 2022+ can analyze BenchmarkDotNet results directly. Use the Profiling tools to correlate benchmark results with CPU and memory profiling data.