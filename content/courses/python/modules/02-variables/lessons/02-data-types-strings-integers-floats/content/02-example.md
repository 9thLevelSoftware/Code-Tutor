---
type: "EXAMPLE"
title: "Code Example"
---

**Expected Output:**
```
STRINGS (text):
name = Alice
Type: <class 'str'>

INTEGERS (whole numbers):
age = 25
Type: <class 'int'>

FLOATS (decimal numbers):
price = 19.99
Type: <class 'float'>

BOOLEANS (True/False):
is_student = True
Type: <class 'bool'>

MIXING TYPES:
String + String: Hello World
Integer + Integer: 15
Float + Integer: 5.5
String * Integer: HaHaHa
```

```python
# Let's explore Python's main data types!

# STRING - text wrapped in quotes
name = "Alice"
greeting = 'Hello, World!'
address = "123 Main Street"

print("STRINGS (text):")
print(f"name = {name}")
print(f"Type: {type(name)}")
print()

# INTEGER - whole numbers (no quotes, no decimal point)
age = 25
students = 100
temperature = -5

print("INTEGERS (whole numbers):")
print(f"age = {age}")
print(f"Type: {type(age)}")
print()

# FLOAT - numbers with decimal points
price = 19.99
pi = 3.14159
temperature_precise = 98.6

print("FLOATS (decimal numbers):")
print(f"price = {price}")
print(f"Type: {type(price)}")
print()

# BOOLEAN - True or False (capital T and F, no quotes)
is_student = True
has_license = False

print("BOOLEANS (True/False):")
print(f"is_student = {is_student}")
print(f"Type: {type(is_student)}")
print()

# What happens when you mix types?
print("MIXING TYPES:")
print(f"String + String: {'Hello' + ' ' + 'World'}")
print(f"Integer + Integer: {10 + 5}")
print(f"Float + Integer: {3.5 + 2}")
print(f"String * Integer: {'Ha' * 3}")
# print(f"String + Integer: {'Age: ' + 25}")  # This would cause an ERROR!
```
