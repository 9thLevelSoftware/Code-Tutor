class Vehicle:
    def __init__(self, brand, year):
        # TODO: Set brand and year
        pass
    
    def start(self):
        # TODO: Return start message
        pass
    
    def stop(self):
        # TODO: Return stop message
        pass
    
    def info(self):
        # TODO: Return basic info
        pass

class Car(Vehicle):
    def __init__(self, brand, year, num_doors):
        # TODO: Call parent __init__ and set num_doors
        pass
    
    def info(self):
        # TODO: Override with car-specific info
        pass

class Motorcycle(Vehicle):
    def __init__(self, brand, year, has_sidecar=False):
        # TODO: Call parent __init__ and set has_sidecar
        pass
    
    def info(self):
        # TODO: Override with motorcycle-specific info
        pass

# TODO: Create vehicles and test methods
