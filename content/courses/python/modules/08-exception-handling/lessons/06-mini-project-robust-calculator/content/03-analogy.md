---
type: "ANALOGY"
title: "Code Architecture Breakdown"
---

**Exception Hierarchy:**
```
CalculatorError (base)
├── InvalidOperationError
├── InvalidNumberError
├── DivisionByZeroError
└── NegativeSquareRootError
```

Benefits:
- Can catch all calculator errors with `except CalculatorError`
- Can catch specific errors separately
- Clear error types for debugging

**Validation Pattern (used throughout):**
```python
def operation(self, param):
    # 1. Validate input
    param = self._validate_number(param, "param_name")
    
    # 2. Check business rules
    if param == 0:
        raise DivisionByZeroError("...")
    
    # 3. Perform operation
    result = do_calculation(param)
    
    # 4. Record for history
    self._record_operation("...", result)
    
    # 5. Return result
    return result
```

**Error Handling in REPL:**
```python
while True:
    try:
        # Get user input
        # Validate
        # Execute operation
        # Display result
    except SpecificError1 as e:
        # Handle gracefully
    except SpecificError2 as e:
        # Handle gracefully
    except Exception as e:
        # Unexpected errors
    finally:
        # Always cleanup
```

Benefits:
- User never sees crashes
- Specific errors get specific messages
- Unexpected errors are logged
- Calculator keeps running

**Defensive Programming Checklist (applied throughout):**

✅ **Every input validated:**
- Type check: `isinstance(value, (int, float))`
- Range check: `if b == 0: raise DivisionByZeroError`
- Special values: Check for infinity, NaN

✅ **Clear error messages:**
- What went wrong: "Cannot divide by zero"
- What was expected: "Please enter a non-zero divisor"
- How to fix it: Actionable guidance

✅ **Edge cases handled:**
- Division by zero
- Square root of negative
- Overflow (numbers too large)
- Invalid types
- Empty input

✅ **Graceful degradation:**
- Error occurs → Show message → Let user try again
- History preserved even after errors
- Memory preserved

**Why This Is Production-Ready:**

1. **Never crashes** - all errors caught
2. **Clear feedback** - users know what went wrong
3. **Maintainable** - clear structure, documented
4. **Testable** - each function isolated
5. **Extensible** - easy to add new operations
6. **User-friendly** - helpful messages, history, memory

This is how professional developers write robust code.