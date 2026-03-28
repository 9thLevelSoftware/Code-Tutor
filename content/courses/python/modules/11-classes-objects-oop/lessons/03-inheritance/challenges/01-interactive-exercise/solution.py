class Vehicle:
    def __init__(self, brand, year):
        self.brand = brand
        self.year = year
    
    def start(self):
        return f"{self.brand} is starting..."
    
    def stop(self):
        return f"{self.brand} is stopping..."
    
    def info(self):
        return f"{self.brand} ({self.year})"

class Car(Vehicle):
    def __init__(self, brand, year, num_doors):
        super().__init__(brand, year)
        self.num_doors = num_doors
    
    def info(self):
        return f"{self.brand} Car ({self.year}) - {self.num_doors} doors"
    
    def honk(self):
        return "Beep beep!"

class Motorcycle(Vehicle):
    def __init__(self, brand, year, has_sidecar=False):
        super().__init__(brand, year)
        self.has_sidecar = has_sidecar
    
    def info(self):
        sidecar = "with sidecar" if self.has_sidecar else "no sidecar"
        return f"{self.brand} Motorcycle ({self.year}) - {sidecar}"
    
    def wheelie(self):
        return "Doing a wheelie!"

# Test the hierarchy
print("=== Creating Vehicles ===")
car = Car("Toyota", 2023, 4)
motorcycle = Motorcycle("Harley", 2022, has_sidecar=True)

print("\n=== Testing Inherited Methods ===")
print(car.start())
print(motorcycle.start())
print(car.stop())
print(motorcycle.stop())

print("\n=== Testing Overridden Methods ===")
print(car.info())
print(motorcycle.info())

print("\n=== Testing Child-Specific Methods ===")
print(car.honk())
print(motorcycle.wheelie())

print("\n=== Checking Inheritance ===")
print(f"Is car a Vehicle? {isinstance(car, Vehicle)}")
print(f"Is motorcycle a Vehicle? {isinstance(motorcycle, Vehicle)}")
print(f"Is Car a subclass of Vehicle? {issubclass(Car, Vehicle)}")
