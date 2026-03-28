---
type: "THEORY"
title: "Syntax Breakdown"
---

## Breaking Down the Syntax

**`public class Person(string name, int age)`**: Parameters go right after the class name in parentheses. No need to declare fields or write constructor body!

**`Parameters are captured`**: The parameters (name, age) are available in ALL instance members - methods, properties, initializers. They act like private fields.

**`: Person(name, age)`**: In derived classes, pass primary constructor parameters to base class constructor using this syntax after the class declaration.

**`No required fields`**: Parameters are captured but NOT automatically properties. If you need a public property, you still declare it: `public string Name { get; } = name;`

**`Validation`**: You can still validate by using the parameters in field initializers: `private readonly string _name = name ?? throw new ArgumentNullException(nameof(name));`

---

## 🆕 C# 13 Preview: The `field` Keyword

**Note**: C# 13 introduces a preview feature that pairs well with primary constructors - the `field` contextual keyword.

**What it does**: The `field` keyword lets you access the compiler-generated backing field within property accessors, eliminating the need to declare explicit backing fields.

```csharp
public class Person(string name, int age)
{
    // Using 'field' keyword in C# 13 (preview feature)
    public string Name
    {
        get => field;           // Access backing field directly
        set => field = value ?? throw new ArgumentNullException();
    }
    
    public int Age
    {
        get => field;
        set => field = value > 0 ? value : throw new ArgumentOutOfRangeException();
    }
}
```

**Benefits**:
- Less boilerplate - no need to declare `_name`, `_age` fields
- Cleaner code when you need validation or logic in property accessors
- Works seamlessly with primary constructor parameters

**Important**: The `field` keyword is a preview feature in C# 13. Enable it in your project file with `<EnablePreviewFeatures>true</EnablePreviewFeatures>`.