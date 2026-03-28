// C# 13 Primary Constructor and field keyword test
using System;

// Test primary constructor with class (not just records)
public class Person(string name, int age)
{
    public void Introduce()
    {
        Console.WriteLine($"Hi, I'm {name}, {age} years old.");
    }

    public string GetName() => name;
    public int GetAge() => age;
}

// Test inheritance with primary constructor
public class Employee(string name, int age, string department)
    : Person(name, age)
{
    public void ShowDepartment()
    {
        Console.WriteLine($"{name} works in {department}");
    }
}

// Test field keyword with primary constructor
public class Product(string name, decimal price)
{
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

    public string GetInfo() => $"{name} costs ${price}";
}

class Program
{
    static void Main()
    {
        // Test Person
        var person = new Person("Alice", 30);
        person.Introduce();

        // Test Employee
        var emp = new Employee("Bob", 25, "Engineering");
        emp.Introduce();
        emp.ShowDepartment();

        // Test Product with field keyword
        var product = new Product("Laptop", 999.99m);
        Console.WriteLine(product.Name);
        product.Name = "Gaming Laptop";
        Console.WriteLine(product.GetInfo());

        Console.WriteLine("All tests passed!");
    }
}
