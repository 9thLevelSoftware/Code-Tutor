---
type: "WARNING"
title: "Common Pitfalls"
---

## Watch Out For These Issues!

**Forgetting 'return':** If your method has a return type (not void), EVERY code path MUST return a value. The compiler error may appear on the closing brace, not where you missed the return. Use guard clauses at the top for early returns, and always ensure your logic covers all branches. The `return` statement both exits the method immediately AND sends the value back to the caller.

**Missing parentheses:** `player.Attack` retrieves a MethodInfo reference to the method itself. `player.Attack()` actually INVOKES it. Forgetting parentheses is one of the most common beginner errors. The compiler error "Method name expected" or unexpected behavior usually indicates this mistake. When you see a method behaving like a property, check for missing parentheses.

**Return type mismatch:** If your method signature declares `int`, returning `"hello"` causes a compile-time error. C# is strongly typed - the return value must be compatible with the declared return type. This includes not returning null for value types (unless using nullable int?). The compiler checks this statically before your program ever runs.

**Ignoring return values:** `calc.Add(5, 3);` compiles but throws away the result! The calculation happens, but the value disappears. Always capture return values in variables: `int sum = calc.Add(5, 3);`. If you genuinely don't need the return value, consider whether the method should be void instead. Ignoring return values is a code smell indicating potential logic errors.

**Modifying parameters unexpectedly:** Methods receive copies of value types and references to reference types. Modifying a value type parameter doesn't affect the original variable. But modifying a reference type's properties DOES affect the original object. This distinction causes subtle bugs - document clearly when methods mutate their parameters or return new instances versus modifying in place.

**Recursive method without base case:** A method that calls itself without a condition to stop will cause a `StackOverflowException`. Always ensure recursive methods have a clear base case that stops the recursion, and verify that every recursive call moves closer to that base case. Stack space is finite - deep recursion can crash your application.