---
type: "THEORY"
title: "Best Practices Summary"
---


### Do

1. **Define clear error hierarchies** - Sealed interfaces for each domain
2. **Use error accumulation for input validation** - Collect all errors at once with `zipOrAccumulate`
3. **Use Either for business logic** - Short-circuit on first error
4. **Handle errors at boundaries** - Controllers, CLI, etc.
5. **Test both success and failure paths** - Errors are first-class
6. **Keep error messages user-friendly** - Map to presentation layer

### Don't

1. **Don't use generic Throwable** - Lose type safety
2. **Don't ignore Either/Result** - Always handle the error case
3. **Don't mix exceptions and Either randomly** - Choose at boundaries
4. **Don't over-engineer** - Simple nullables are fine for simple cases
5. **Don't forget logging** - Errors still need observability

---

