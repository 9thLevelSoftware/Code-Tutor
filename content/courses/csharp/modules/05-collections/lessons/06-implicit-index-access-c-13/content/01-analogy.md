---
type: "ANALOGY"
title: "The Theater with Two Entrances"
---

Imagine attending a play at a grand theater with 100 numbered seats. You have a ticket for "Seat 1," which clearly means the first row near the stage. But what if you want to sit near the exit for a quick departure? You'd need to count backwards from 100 to find the right seat number. Now imagine you want to tell a friend to meet you at "the last seat" or "third from the end" – describing this with forward counting becomes tedious and error-prone.

This is exactly how arrays worked before C# 8. All indexing started from the front (0). Getting the last element required calculating `array[array.Length - 1]`. Getting the third from the end meant `array[array.Length - 3]`. Every time you wanted to reference positions from the back, you performed mental arithmetic and hoped you didn't make an off-by-one error.

C# 8 introduced the "from end" operator `^`, which was like adding a second entrance to our theater – one at the front and one at the back. Now `^1` means "the last seat" regardless of theater size. `^3` means "third from the end." The counting starts from the back entrance instead of the front.

But there was a limitation: this second entrance only worked after the theater was built. You couldn't use `^` notation while constructing the seating chart during initialization – it was only for accessing existing seats, not defining them.

C# 13 removes this restriction. Now you can use the back entrance even during construction. Want to assign VIP seating to the last three rows while building your array? Simply write `new string[10] { [^1] = "VIP", [^2] = "VIP", [^3] = "VIP" }`. The initialization syntax now accepts `^` indices, letting you reference positions from the end while setting up the collection.

This is particularly valuable when collection sizes are dynamic or unknown at compile time. Instead of calculating forward indices based on length, you simply declare intent: "put this at the end," "put that second-to-last." The code becomes more readable because it expresses what you mean rather than how to calculate it.

Think of it as finally being able to say "fill in the last three seats with VIP tickets" while the seating chart is being drawn, not just after all seats are built!