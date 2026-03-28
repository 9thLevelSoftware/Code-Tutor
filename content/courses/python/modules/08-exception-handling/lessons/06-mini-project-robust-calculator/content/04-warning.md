---
type: "WARNING"
title: "Common Pitfalls"
---

### Watch Out For These Common Mistakes:

**1. Catching Too Broadly in Calculator Functions**
```python
# WRONG - Hides real bugs in your code
def calculate(a, op, b):
    try:
        if op == '+':
            return a + b
        # ... more operations
    except Exception:
        return "Error"  # What error? Bug hidden!

# CORRECT - Catch specific, expected exceptions
def calculate(a, op, b):
    try:
        if op == '+':
            return a + b
        elif op == '/':
            return a / b
    except ZeroDivisionError:
        return "Error: Division by zero"
    except TypeError:
        return "Error: Invalid operand types"
```

**2. Not Validating Operator Before Calculation**
```python
# WRONG - Invalid operator causes silent failure
def calculate(a, op, b):
    if op == '+':
        return a + b
    elif op == '-':
        return a - b
    # What if op is '%'? Returns None silently!

# CORRECT - Validate operator, fail explicitly
def calculate(a, op, b):
    valid_ops = {'+', '-', '*', '/'}
    if op not in valid_ops:
        raise ValueError(f"Invalid operator: {op}")
    # ... rest of logic
```

**3. Returning Mixed Types (Error Strings vs Numbers)**
```python
# WRONG - Caller can't distinguish error from result
def divide(a, b):
    if b == 0:
        return "Error"  # String mixed with number returns
    return a / b

result = divide(10, 0)
print(result + 1)  # TypeError: can't add str and int!

# CORRECT - Use exceptions for errors
def divide(a, b):
    if b == 0:
        raise ZeroDivisionError("Cannot divide by zero")
    return a / b
```

**4. Not Handling Floating Point Edge Cases**
```python
# WRONG - Doesn't handle special float values
def divide(a, b):
    return a / b

print(divide(1.0, 0.0))   # Returns inf, not error!
print(divide(0.0, 0.0))   # Returns nan, not error!

# CORRECT - Check for special values
import math
def divide(a, b):
    result = a / b
    if math.isinf(result) or math.isnan(result):
        raise ValueError("Result is not a valid number")
    return result
```

**5. Losing Precision in Display**
```python
# WRONG - Showing too many decimal places
def calculate(a, op, b):
    result = a / b
    return result  # Returns 0.30000000000000004 for 0.1 + 0.2

# CORRECT - Round for display, keep precision internally
def calculate(a, op, b):
    result = a / b
    return round(result, 10)  # Clean display value
```