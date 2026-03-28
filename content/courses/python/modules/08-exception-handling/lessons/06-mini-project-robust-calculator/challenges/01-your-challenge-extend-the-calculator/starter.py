import math

class RobustCalculator:
    # ... (previous code) ...
    
    def logarithm(self, number, base=10):
        """Calculate logarithm.
        
        Args:
            number: Number to calculate log of
            base: Logarithm base (default 10)
            
        Raises:
            InvalidNumberError: If number <= 0 or base <= 0
        """
        # TODO: Validate number and base
        # TODO: Check number > 0 (can't log negative or zero)
        # TODO: Check base > 0 and base != 1
        # TODO: Calculate using math.log(number, base)
        # TODO: Record in history
        # TODO: Return result
        pass
    
    def sine(self, angle_degrees):
        """Calculate sine (angle in degrees).
        
        Args:
            angle_degrees: Angle in degrees
            
        Returns:
            float: Sine of angle
        """
        # TODO: Validate angle
        # TODO: Convert to radians: math.radians(angle_degrees)
        # TODO: Calculate: math.sin(angle_radians)
        # TODO: Record in history
        # TODO: Return result
        pass
    
    def cosine(self, angle_degrees):
        """Calculate cosine (angle in degrees)."""
        # TODO: Similar to sine
        pass