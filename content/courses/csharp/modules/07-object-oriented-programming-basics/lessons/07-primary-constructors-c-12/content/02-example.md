---
type: "EXAMPLE"
title: "Code Example"
---

This example demonstrates the concepts in action.

```csharp
// OLD WAY - lots of boilerplate!
class PersonOld
{
    private readonly string _name;
    private readonly int _age;
    
    public PersonOld(string name, int age)
    {
        _name = name;
        _age = age;
    }
    
    public void Introduce()
    {
        Console.WriteLine($"Hi, I'm {_name}, {_age} years old.");
    }
}

// NEW WAY - Primary Constructor (C# 12+)!
public class Person(string name, int age)
{
    // Parameters are available everywhere in the class!
    public void Introduce()
    {
        Console.WriteLine($"Hi, I'm {name}, {age} years old.");
    }
    
    public string GetName() => name;
    public int GetAge() => age;
}

// Usage - same as before!
var person = new Person("Alice", 30);
person.Introduce();  // Hi, I'm Alice, 30 years old.

// Works with inheritance too!
public class Employee(string name, int age, string department) 
    : Person(name, age)  // Pass to base class
{
    public void ShowDepartment()
    {
        Console.WriteLine($"{name} works in {department}");
    }
}

var emp = new Employee("Bob", 25, "Engineering");
emp.Introduce();        // From Person
emp.ShowDepartment();   // Bob works in Engineering
```

---

## C# 13 Preview: Using `field` with Primary Constructors

```csharp
// C# 13 (preview) - Using 'field' keyword for cleaner properties
public class Product(string name, decimal price)
{
    // No explicit backing field needed!
    public string Name
    {
        get => field;
        set => field = !string.IsNullOrWhiteSpace(value) 
            ? value 
            : throw new ArgumentException("Name cannot be empty");
    }
    
    public decimal Price
    {
        get => field;
        init => field = value > 0 
            ? value 
            : throw new ArgumentOutOfRangeException("Price must be positive");
    }
    
    // Primary constructor parameters still available
    public string GetInfo() => $"{name} costs ${price}";
}

// Usage
var product = new Product("Laptop", 999.99m);
Console.WriteLine(product.Name);  // "Laptop"
product.Name = "Gaming Laptop";     // Valid update
// product.Price = 899.99m;         // Error: Price is init-only
```
