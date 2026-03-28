---
type: "EXAMPLE"
title: "Code Example: Unit Testing with pytest"
---

**pytest features:**

**1. Simple assertions:**
```python
assert result == expected
assert value > 0
assert item in list
```

**2. Testing exceptions:**
```python
with pytest.raises(ValueError):
    function_that_fails()
```

**3. Fixtures (setup/teardown):**
```python
@pytest.fixture
def database():
    db = create_db()
    yield db  # Test runs here
    db.close()
```

**4. Parametrized tests:**
```python
@pytest.mark.parametrize("input,expected", [
    (1, 2),
    (2, 4),
])
def test_double(input, expected):
    assert double(input) == expected
```

**Test naming:**
- Prefix with `test_`
- Descriptive names
- One test = one concept

```python
# Example functions to test

def add(a, b):
    """Add two numbers"""
    return a + b

def divide(a, b):
    """Divide two numbers"""
    if b == 0:
        raise ValueError("Cannot divide by zero")
    return a / b

def is_palindrome(text):
    """Check if text is a palindrome"""
    cleaned = ''.join(c.lower() for c in text if c.isalnum())
    return cleaned == cleaned[::-1]

class Calculator:
    """Simple calculator class"""
    
    def __init__(self):
        self.history = []
    
    def add(self, a, b):
        result = a + b
        self.history.append(f"{a} + {b} = {result}")
        return result
    
    def get_history(self):
        return self.history.copy()

print("=== Writing Tests with pytest ===")

# Test file: test_calculator.py
test_code = '''
import pytest
from calculator import add, divide, is_palindrome, Calculator

# Basic unit tests
def test_add_positive_numbers():
    """Test adding positive numbers"""
    assert add(2, 3) == 5
    assert add(10, 20) == 30

def test_add_negative_numbers():
    """Test adding negative numbers"""
    assert add(-1, -1) == -2
    assert add(-5, 5) == 0

def test_divide_normal():
    """Test normal division"""
    assert divide(10, 2) == 5
    assert divide(9, 3) == 3

def test_divide_by_zero():
    """Test division by zero raises error"""
    with pytest.raises(ValueError, match="Cannot divide by zero"):
        divide(10, 0)

def test_is_palindrome_simple():
    """Test simple palindromes"""
    assert is_palindrome("racecar") == True
    assert is_palindrome("hello") == False

def test_is_palindrome_case_insensitive():
    """Test case insensitivity"""
    assert is_palindrome("RaceCar") == True
    assert is_palindrome("A man a plan a canal Panama") == True

# Test with fixtures
@pytest.fixture
def calculator():
    """Fixture providing a fresh Calculator instance"""
    return Calculator()

def test_calculator_add(calculator):
    """Test calculator add method"""
    result = calculator.add(5, 3)
    assert result == 8

def test_calculator_history(calculator):
    """Test calculator maintains history"""
    calculator.add(5, 3)
    calculator.add(10, 2)
    history = calculator.get_history()
    assert len(history) == 2
    assert "5 + 3 = 8" in history

# Parametrized tests (test multiple inputs)
@pytest.mark.parametrize("a,b,expected", [
    (2, 3, 5),
    (10, 20, 30),
    (-1, 1, 0),
    (0, 0, 0),
    (100, 200, 300)
])
def test_add_parametrized(a, b, expected):
    """Test add with multiple parameter sets"""
    assert add(a, b) == expected

# Test error messages
def test_divide_error_message():
    """Test error message is helpful"""
    try:
        divide(10, 0)
        assert False, "Should have raised ValueError"
    except ValueError as e:
        assert "Cannot divide by zero" in str(e)
'''

print("Test file example:")
print(test_code)

print("\n=== Running Tests ===")
print("""
Command: pytest test_calculator.py

Output:
========================= test session starts =========================
collected 10 items

test_calculator.py ..........                                   [100%]

========================= 10 passed in 0.03s =========================

Options:
  pytest -v                    # Verbose output
  pytest -k "palindrome"       # Run tests matching name
  pytest --cov=src             # Show code coverage
  pytest -x                    # Stop on first failure
  pytest --pdb                 # Drop into debugger on failure
""")

print("\n=== Test Organization ===")

project_structure = """
my_project/
├── src/
│   ├── __init__.py
│   ├── calculator.py
│   ├── models.py
│   └── services.py
│
├── tests/
│   ├── __init__.py
│   ├── conftest.py          # Shared fixtures
│   ├── test_calculator.py   # Test calculator module
│   ├── test_models.py       # Test models
│   └── test_services.py     # Test services
│
├── pytest.ini               # pytest configuration
└── requirements-test.txt    # Test dependencies
"""

print(project_structure)
```
