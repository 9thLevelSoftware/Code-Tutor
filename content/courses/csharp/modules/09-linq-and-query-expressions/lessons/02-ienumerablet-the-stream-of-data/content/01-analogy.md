---
type: "ANALOGY"
title: "Understanding the Concept"
---

Imagine watching a movie on a streaming service like Netflix or Spotify:

WITHOUT STREAMING (List<T>):
• You must download the ENTIRE movie first (might take 30 minutes!)
• Only then can you start watching
• If the movie is 4K and 3 hours long, you need HUGE storage space

WITH STREAMING (IEnumerable<T>):
• Press play → First frame appears in 2 seconds
• Movie streams ONE FRAME AT A TIME as you watch
• You don't need to store the whole movie
• You can stop anytime without downloading the rest

That's IEnumerable<T> - it represents a SEQUENCE of items:
• 'T' is the type: IEnumerable<int>, IEnumerable<string>
• Items are accessed ONE AT A TIME (via foreach)
• Doesn't load everything into memory at once
• LINQ methods return IEnumerable<T>

Real-world scenarios:
• Reading a 10-million-row log file - stream line by line instead of loading all
• Processing sensor data - handle each reading as it arrives
• Search results - show first 20 while fetching the rest in background

Why use it?
• MEMORY EFFICIENT: Query 1 million items without loading them all
• LAZY EVALUATION: Computation happens only when needed
• FLEXIBLE: Works with any collection type

List<T> implements IEnumerable<T>, arrays implement it, LINQ results are IEnumerable<T>.

Think: IEnumerable<T> = 'Streaming your data instead of downloading it all first.'