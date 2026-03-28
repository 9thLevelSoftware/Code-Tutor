---
type: "EXAMPLE"
title: "Complete Implementation: Robust Calculator"
---

This production-ready calculator demonstrates:

**1. Custom Exception Hierarchy:**
- Base `CalculatorError` for all calculator errors
- Specific exceptions for each error type
- Clear, descriptive error messages

**2. Comprehensive Validation:**
- Type checking with `isinstance()`
- Range validation (no infinity, NaN)
- Defensive programming throughout

**3. Error Handling Patterns:**
- Try/except for risky operations
- Finally for cleanup (logging)
- EAFP approach where appropriate

**4. User-Friendly Features:**
- Clear error messages
- Calculation history
- Memory storage
- Interactive REPL

**5. Production-Level Code:**
- Type hints for documentation
- Docstrings for all methods
- Proper exception hierarchy
- Edge case handling

```python
"""Robust Calculator with Comprehensive Error Handling

A production-ready calculator that handles all possible errors gracefully.
Demonstrates: custom exceptions, validation, defensive programming, EAFP.
"""

import math
from typing import List, Tuple

# ============================================================================
# Custom Exception Classes
# ============================================================================

class CalculatorError(Exception):
    """Base exception for all calculator errors."""
    pass

class InvalidOperationError(CalculatorError):
    """Raised when an invalid operation is requested."""
    pass

class InvalidNumberError(CalculatorError):
    """Raised when input cannot be converted to a number."""
    pass

class DivisionByZeroError(CalculatorError):
    """Raised when attempting to divide by zero."""
    pass

class NegativeSquareRootError(CalculatorError):
    """Raised when attempting square root of negative number."""
    pass

# ============================================================================
# Calculator Class
# ============================================================================

class RobustCalculator:
    """Calculator with comprehensive error handling."""
    
    def __init__(self):
        """Initialize calculator with memory and history."""
        self.memory = 0.0
        self.history: List[Tuple[str, float]] = []
        self.operations = {
            'add': self.add,
            'subtract': self.subtract,
            'multiply': self.multiply,
            'divide': self.divide,
            'power': self.power,
            'sqrt': self.square_root,
            'modulo': self.modulo,
        }
    
    def _validate_number(self, value, param_name="value"):
        """Validate that value is a number.
        
        Args:
            value: Value to validate
            param_name: Name for error messages
            
        Returns:
            float: Validated number
            
        Raises:
            InvalidNumberError: If value is not a number
        """
        if not isinstance(value, (int, float)):
            raise InvalidNumberError(
                f"{param_name} must be a number, got {type(value).__name__}"
            )
        
        # Check for infinity and NaN
        if math.isinf(value):
            raise InvalidNumberError(f"{param_name} cannot be infinity")
        if math.isnan(value):
            raise InvalidNumberError(f"{param_name} cannot be NaN")
        
        return float(value)
    
    def _record_operation(self, operation: str, result: float):
        """Record operation in history."""
        self.history.append((operation, result))
        # Keep only last 10 operations
        if len(self.history) > 10:
            self.history.pop(0)
    
    def add(self, a, b):
        """Add two numbers."""
        a = self._validate_number(a, "first number")
        b = self._validate_number(b, "second number")
        result = a + b
        self._record_operation(f"{a} + {b}", result)
        return result
    
    def subtract(self, a, b):
        """Subtract b from a."""
        a = self._validate_number(a, "first number")
        b = self._validate_number(b, "second number")
        result = a - b
        self._record_operation(f"{a} - {b}", result)
        return result
    
    def multiply(self, a, b):
        """Multiply two numbers."""
        a = self._validate_number(a, "first number")
        b = self._validate_number(b, "second number")
        result = a * b
        self._record_operation(f"{a} * {b}", result)
        return result
    
    def divide(self, a, b):
        """Divide a by b.
        
        Raises:
            DivisionByZeroError: If b is zero
        """
        a = self._validate_number(a, "dividend")
        b = self._validate_number(b, "divisor")
        
        if b == 0:
            raise DivisionByZeroError(
                "Cannot divide by zero. Please enter a non-zero divisor."
            )
        
        result = a / b
        self._record_operation(f"{a} / {b}", result)
        return result
    
    def power(self, base, exponent):
        """Raise base to exponent."""
        base = self._validate_number(base, "base")
        exponent = self._validate_number(exponent, "exponent")
        
        try:
            result = base ** exponent
            # Check for overflow
            if math.isinf(result):
                raise InvalidNumberError(
                    f"Result too large: {base}^{exponent} causes overflow"
                )
            self._record_operation(f"{base} ^ {exponent}", result)
            return result
        except OverflowError:
            raise InvalidNumberError(
                f"Result too large: {base}^{exponent} causes overflow"
            )
    
    def square_root(self, number):
        """Calculate square root.
        
        Raises:
            NegativeSquareRootError: If number is negative
        """
        number = self._validate_number(number, "number")
        
        if number < 0:
            raise NegativeSquareRootError(
                f"Cannot calculate square root of negative number: {number}. "
                f"Use complex numbers for this operation."
            )
        
        result = math.sqrt(number)
        self._record_operation(f"sqrt({number})", result)
        return result
    
    def modulo(self, a, b):
        """Calculate a modulo b.
        
        Raises:
            DivisionByZeroError: If b is zero
        """
        a = self._validate_number(a, "dividend")
        b = self._validate_number(b, "divisor")
        
        if b == 0:
            raise DivisionByZeroError(
                "Cannot perform modulo with zero divisor"
            )
        
        result = a % b
        self._record_operation(f"{a} % {b}", result)
        return result
    
    def store_memory(self, value):
        """Store value in memory."""
        value = self._validate_number(value, "memory value")
        self.memory = value
        return value
    
    def recall_memory(self):
        """Recall value from memory."""
        return self.memory
    
    def clear_memory(self):
        """Clear memory."""
        self.memory = 0.0
    
    def show_history(self):
        """Show calculation history."""
        if not self.history:
            return "No calculation history"
        
        result = "\nCalculation History (last 10):\n"
        result += "-" * 40 + "\n"
        for i, (operation, value) in enumerate(self.history, 1):
            result += f"{i}. {operation} = {value}\n"
        return result
    
    def clear_history(self):
        """Clear calculation history."""
        self.history.clear()

# ============================================================================
# Interactive Calculator REPL
# ============================================================================

def safe_float_input(prompt):
    """Safely get a float from user.
    
    Args:
        prompt: Prompt to display
        
    Returns:
        float: Validated number
        
    Raises:
        InvalidNumberError: If input is not a valid number
        ValueError: If input is empty
    """
    user_input = input(prompt).strip()
    
    if not user_input:
        raise ValueError("Input cannot be empty")
    
    try:
        return float(user_input)
    except ValueError:
        raise InvalidNumberError(
            f"'{user_input}' is not a valid number. Please enter a numeric value."
        )

def run_calculator():
    """Run the interactive calculator."""
    calc = RobustCalculator()
    
    print("="*50)
    print("  ROBUST CALCULATOR - Error Handling Demo")
    print("="*50)
    print("\nAvailable operations:")
    print("  add, subtract, multiply, divide")
    print("  power, sqrt, modulo")
    print("  memory, recall, clear-memory")
    print("  history, clear-history")
    print("  quit\n")
    
    while True:
        try:
            # Get operation
            print("-" * 50)
            operation = input("\nEnter operation (or 'quit' to exit): ").strip().lower()
            
            if not operation:
                print("❌ Please enter an operation")
                continue
            
            if operation in ('quit', 'exit', 'q'):
                print("\n👋 Thanks for using Robust Calculator!")
                break
            
            # Handle special operations
            if operation == 'history':
                print(calc.show_history())
                continue
            
            if operation == 'clear-history':
                calc.clear_history()
                print("✓ History cleared")
                continue
            
            if operation == 'recall':
                print(f"Memory: {calc.recall_memory()}")
                continue
            
            if operation == 'clear-memory':
                calc.clear_memory()
                print("✓ Memory cleared")
                continue
            
            # Operations requiring one operand
            if operation == 'sqrt':
                num = safe_float_input("Enter number: ")
                result = calc.square_root(num)
                print(f"\n✓ sqrt({num}) = {result}")
                continue
            
            if operation == 'memory':
                value = safe_float_input("Enter value to store: ")
                calc.store_memory(value)
                print(f"✓ Stored {value} in memory")
                continue
            
            # Operations requiring two operands
            if operation in calc.operations:
                a = safe_float_input("Enter first number: ")
                b = safe_float_input("Enter second number: ")
                
                result = calc.operations[operation](a, b)
                print(f"\n✓ Result: {result}")
            else:
                raise InvalidOperationError(
                    f"Unknown operation '{operation}'. "
                    f"Type an operation name from the list above."
                )
        
        except InvalidNumberError as e:
            print(f"\n❌ Invalid Number: {e}")
        except DivisionByZeroError as e:
            print(f"\n❌ Division Error: {e}")
        except NegativeSquareRootError as e:
            print(f"\n❌ Math Error: {e}")
        except InvalidOperationError as e:
            print(f"\n❌ Invalid Operation: {e}")
        except ValueError as e:
            print(f"\n❌ Input Error: {e}")
        except CalculatorError as e:
            print(f"\n❌ Calculator Error: {e}")
        except KeyboardInterrupt:
            print("\n\n👋 Calculator interrupted. Goodbye!")
            break
        except Exception as e:
            print(f"\n❌ Unexpected Error: {e}")
            print("Please report this bug!")
        finally:
            # Always runs - could be used for logging
            pass

# ============================================================================
# Demo and Tests
# ============================================================================

print("=" * 60)
print("DEMO: Testing Robust Calculator")
print("=" * 60)

calc = RobustCalculator()

print("\n1. Valid Operations:")
print(f"   10 + 5 = {calc.add(10, 5)}")
print(f"   20 - 8 = {calc.subtract(20, 8)}")
print(f"   6 * 7 = {calc.multiply(6, 7)}")
print(f"   15 / 3 = {calc.divide(15, 3)}")
print(f"   2 ^ 8 = {calc.power(2, 8)}")
print(f"   sqrt(16) = {calc.square_root(16)}")

print("\n2. Error Handling:")

try:
    calc.divide(10, 0)
except DivisionByZeroError as e:
    print(f"   ✓ Caught division by zero: {e}")

try:
    calc.square_root(-4)
except NegativeSquareRootError as e:
    print(f"   ✓ Caught negative sqrt: {e}")

try:
    calc.add("hello", 5)
except InvalidNumberError as e:
    print(f"   ✓ Caught invalid number: {e}")

try:
    calc.power(10, 1000)  # Huge number
except InvalidNumberError as e:
    print(f"   ✓ Caught overflow: {e}")

print("\n3. Memory and History:")
calc.store_memory(42)
print(f"   Stored: {calc.recall_memory()}")
print(f"   {calc.show_history()}")

print("\n✓ All error handling working correctly!")
print("\nTo run interactive calculator, call run_calculator()")
```
