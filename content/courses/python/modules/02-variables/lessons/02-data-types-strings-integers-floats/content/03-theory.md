---
type: "THEORY"
title: "Syntax Breakdown"
---

Let's break down each data type and understand the rules:

### 1. Strings (str) - The Text Type

- **What they are:** Any text wrapped in quotes (single `'` or double `"`)

  ```python
  name = "Alice"     # Double quotes
  greeting = 'Hi!'   # Single quotes (works the same)
  message = "123"    # Even numbers in quotes are strings!
  ```

- **Key rule:** MUST have matching quotes at start and end

  ```python
  "Hello"  ✅ Correct
  'Hello'  ✅ Also correct
  "Hello'  ❌ Error - quotes don't match!
  ```

- **The `type()` function:** Tells you what type a value is

  ```python
  print(type("Alice"))  # Shows: <class 'str'>
  ```
  
  Think of `type()` as asking "What kind of container is this?"

### 2. Integers (int) - The Whole Number Type

- **What they are:** Whole numbers with NO decimal point and NO quotes

  ```python
  age = 25        # Positive integer
  count = 0       # Zero is an integer
  debt = -500     # Negative integer
  ```

- **Key rule:** NO quotes, NO decimal point

  ```python
  age = 25       ✅ Integer
  age = "25"     ❌ String (has quotes)
  age = 25.0     ❌ Float (has decimal point)
  ```

- **You can do math:** +, -, *, /, //, %, **

  ```python
  students = 20 + 5   # Addition: 25
  groups = 20 // 5    # Division (whole number): 4
  squared = 5 ** 2    # Exponent: 25
  ```

### 3. Floats (float) - The Decimal Number Type

- **What they are:** Numbers with a decimal point

  ```python
  price = 19.99
  pi = 3.14159
  temperature = -2.5
  exact_value = 5.0  # Even whole numbers with .0 are floats!
  ```

- **Key rule:** MUST have a decimal point (even if it's .0)

  ```python
  price = 19.99  ✅ Float
  price = 20     ❌ Integer (no decimal point)
  ```

- **Mixing int and float:** Result is always a float

  ```python
  result = 5 + 2.5   # 5 (int) + 2.5 (float) = 7.5 (float)
  print(type(result))  # <class 'float'>
  ```

- **IEEE 754 Standard:** Python uses 64-bit precision for floats (about 15-17 decimal digits of accuracy)

### 4. Booleans (bool) - The True/False Type

- **What they are:** Only TWO possible values: `True` or `False`

  ```python
  is_raining = True
  has_license = False
  ```

- **Key rules:**

  - Must be capitalized: `True` and `False` (not `true` or `false`)
  - NO quotes (with quotes they become strings)

  ```python
  is_student = True    ✅ Boolean
  is_student = true    ❌ Error (not capitalized)
  is_student = "True"  ❌ String (has quotes)
  ```

### 5. Mixing Types - What Works and What Doesn't

| Operation | Example | Result |
| :--- | :--- | :--- |
| String + String | `"Hello" + " World"` | ✅ `"Hello World"` |
| Integer + Integer | `5 + 3` | ✅ `8` |
| Float + Integer | `5.5 + 2` | ✅ `7.5` |
| String * Integer | `"Ha" * 3` | ✅ `"HaHaHa"` |
| String + Integer | `"Age: " + 25` | ❌ TypeError! |

**The golden rule:** You can't directly combine strings and numbers with `+`. You need to convert them first (we'll learn this next lesson!).
