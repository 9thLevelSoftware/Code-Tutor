---
type: "THEORY"
title: "Comparison Operators - How to Compare Things"
---

**Estimated Time**: 45 minutes
**Difficulty**: Beginner

## Introduction

Comparison operators are the decision-makers of code. They evaluate relationships between values and return `true` or `false`, enabling programs to branch, loop, and respond intelligently to data. Understanding these operators is essential for any JavaScript developer.

In this lesson, you'll learn:

1. **Equality Operators**: `==` vs. `===` and why strict equality matters
2. **Relational Operators**: `<`, `>`, `<=`, `>=` for ordering comparisons
3. **Type Coercion**: How JavaScript implicitly converts values during comparison
4. **Best Practices**: Writing comparison logic that behaves predictably

## The Equality Operator Trap

JavaScript's `==` performs type coercion, leading to surprising results:

```javascript
0 == "0"      // true (string converted to number)
0 == []       // true (array converted to empty string, then 0)
"" == false   // true (both convert to 0)
null == undefined // true
```

**Always use `===` (strict equality)** which checks both value AND type:

```javascript
0 === "0"     // false (number !== string)
0 === []      // false (number !== object)
null === undefined // false
```

## Comparison Operators Reference

| Operator | Meaning | Example | Result |
|----------|---------|---------|--------|
| `===` | Strict equal | `5 === 5` | `true` |
| `!==` | Strict not equal | `5 !== "5"` | `true` |
| `>` | Greater than | `10 > 5` | `true` |
| `<` | Less than | `3 < 7` | `true` |
| `>=` | Greater or equal | `5 >= 5` | `true` |
| `<=` | Less or equal | `4 <= 5` | `true` |

## String Comparison

JavaScript compares strings lexicographically (dictionary order):

```javascript
"apple" < "banana"   // true (a comes before b)
"Zebra" < "apple"    // true (uppercase Z < lowercase a)
"10" < "2"           // true ("1" < "2" in string comparison)
```

## Real-World Context

Comparison operators power every conditional in your applications:
- **Form validation**: Checking if inputs meet requirements
- **Search and filter**: Finding items that match criteria
- **Game logic**: Determining win/lose conditions
- **Data sorting**: Ordering arrays by values

Bugs from `==` vs `===` confusion have caused production outages at major companies. Facebook's codebase famously has lint rules banning `==` entirely. Understanding these operators prevents subtle bugs that only appear in edge cases.

## Best Practices

1. **Always use `===` and `!==`**—disable `==` in your linter
2. **Be careful comparing objects**—`{} === {}` is `false` (different references)
3. **Use `Object.is()`** for special cases (NaN equality, signed zeros)
4. **Convert strings to numbers** before numeric comparison if needed

Master comparisons, master decision-making in code. It has proper frontmatter so the loader will not fail to parse it.
