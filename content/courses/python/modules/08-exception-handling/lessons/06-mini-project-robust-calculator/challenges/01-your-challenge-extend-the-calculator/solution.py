import math

# Custom exception for invalid number operations
class InvalidNumberError(ValueError):
    """Raised when a number is invalid for the operation."""
    pass

class RobustCalculator:
    """Calculator with scientific functions and history."""
    
    def __init__(self):
        self.history = []
    
    def _record(self, operation, result):
        """Record an operation in history."""
        self.history.append({'operation': operation, 'result': result})
    
    def logarithm(self, number, base=10):
        """Calculate logarithm with validation."""
        # Step 1: Validate number
        if not isinstance(number, (int, float)):
            raise InvalidNumberError(f"Number must be numeric, got {type(number).__name__}")
        if number <= 0:
            raise InvalidNumberError("Logarithm requires a positive number")
        
        # Step 2: Validate base
        if not isinstance(base, (int, float)):
            raise InvalidNumberError(f"Base must be numeric, got {type(base).__name__}")
        if base <= 0 or base == 1:
            raise InvalidNumberError("Base must be positive and not equal to 1")
        
        # Step 3: Calculate and record
        result = math.log(number, base)
        self._record(f"log{base}({number})", result)
        return result
    
    def sine(self, angle_degrees):
        """Calculate sine (angle in degrees)."""
        # Step 1: Validate input
        if not isinstance(angle_degrees, (int, float)):
            raise InvalidNumberError(f"Angle must be numeric, got {type(angle_degrees).__name__}")
        
        # Step 2: Convert to radians and calculate
        angle_radians = math.radians(angle_degrees)
        result = math.sin(angle_radians)
        
        # Step 3: Record and return
        self._record(f"sin({angle_degrees}deg)", result)
        return result
    
    def cosine(self, angle_degrees):
        """Calculate cosine (angle in degrees)."""
        # Step 1: Validate input
        if not isinstance(angle_degrees, (int, float)):
            raise InvalidNumberError(f"Angle must be numeric, got {type(angle_degrees).__name__}")
        
        # Step 2: Convert to radians and calculate
        angle_radians = math.radians(angle_degrees)
        result = math.cos(angle_radians)
        
        # Step 3: Record and return
        self._record(f"cos({angle_degrees}deg)", result)
        return result
    
    def show_history(self):
        """Display calculation history."""
        for i, entry in enumerate(self.history, 1):
            print(f"{i}. {entry['operation']} = {entry['result']:.6f}")

# Test the calculator
calc = RobustCalculator()

print("=== Scientific Calculator Tests ===")

# Logarithm tests
print(f"log10(100) = {calc.logarithm(100)}")
print(f"log2(8) = {calc.logarithm(8, 2)}")

# Trigonometry tests
print(f"sin(90) = {calc.sine(90)}")
print(f"cos(0) = {calc.cosine(0)}")
print(f"sin(45) = {calc.sine(45)}")

# Error handling test
try:
    calc.logarithm(-5)
except InvalidNumberError as e:
    print(f"Error: {e}")

# Show history
print("\n=== Calculation History ===")
calc.show_history()