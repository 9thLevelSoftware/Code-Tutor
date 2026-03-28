---
type: "ANALOGY"
title: "Type Narrowing: The Detective's Investigation"
---

Imagine you're a detective investigating a case. A witness describes the suspect: 'It was either a tall man in a blue jacket OR a short woman in a red coat.' This is like a union type - you know it's ONE of two possibilities, but not which one.

As a detective, you gather evidence to NARROW DOWN who it was:
- 'The suspect was wearing heels' -> Now you KNOW it's the woman (narrowing!)
- 'The suspect was over 6 feet tall' -> Now you KNOW it's the man
- 'The suspect had a badge' -> Only one of them is a police officer (property check)

Type narrowing works exactly the same way in TypeScript. When you have a union type like `string | number | null`, TypeScript doesn't know which type you actually have. But when you check evidence (using typeof, instanceof, property checks, or custom guards), TypeScript NARROWS the type within that code block. The detective's job is to eliminate possibilities until only one remains. TypeScript's job is to track what types are possible at each point in your code, eliminating options as you add checks.