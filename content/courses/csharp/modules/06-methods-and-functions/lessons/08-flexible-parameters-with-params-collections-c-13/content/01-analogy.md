---
type: "ANALOGY"
title: "The Pizza Shop's Flexible Ordering System"
---

Imagine a pizza shop that has revolutionized how customers can place topping orders. In the old days, you had exactly two rigid choices: either stand at the counter and verbally list every topping one by one ("pepperoni, mushrooms, olives, onions"), or you had to write everything down on a specific paper form the shop provided and hand it over. There was no middle ground, no flexibility.

C# 13 introduces a modern, flexible ordering system that accommodates every customer's preference. Now you can walk into the pizza shop and order in multiple convenient ways that suit your style.

First, you can still do it the traditional way – simply stand there and rattle off toppings: "Pepperoni and mushrooms please!" The cashier understands and processes it immediately. This is the classic `params` approach where you list arguments individually.

Second, you can hand over a pre-written list: "Here's my usual order on this napkin." The shop accepts your personal list, whether it's on a sticky note, a phone memo, or any piece of paper. With C# 13's enhanced `params`, you can pass existing collections directly – arrays, lists, spans – whatever you already have.

Third, you can now use the shop's new digital kiosk that accepts any format: "Tap your toppings on the screen, or scan your saved preference card, or just speak into the microphone." C# 13's `params` works with `IEnumerable<T>`, `ReadOnlySpan<T>`, `IReadOnlyList<T>`, and more – the method accepts whatever collection type fits your scenario.

The beauty is that the pizza shop (your method) doesn't care HOW you provide the toppings – it just receives them and makes the pizza. The customer (caller) gets maximum convenience, and the chef (method implementation) handles everything uniformly.

This flexibility shines in real-world scenarios. Logging frameworks can accept a single message, multiple arguments, or an existing collection of log entries. Math utilities can take individual numbers or a pre-built dataset. UI rendering methods can accept inline color values or a theme configuration collection.

Best of all, for performance-critical scenarios, `params ReadOnlySpan<T>` keeps everything on the stack – like ordering from a drive-thru window without ever parking and coming inside. Zero allocations, maximum speed, complete flexibility. That's the C# 13 `params` revolution.