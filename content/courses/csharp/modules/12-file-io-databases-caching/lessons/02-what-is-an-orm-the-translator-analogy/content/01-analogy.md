---
type: "ANALOGY"
title: "Understanding the Concept"
---

Imagine you're at a restaurant in a foreign country where you don't speak the language:

WITHOUT A TRANSLATOR (raw SQL):
• You need to learn the local language first
• Write down your order phonetically
• Hope the waiter understands
• When food arrives, guess what each dish is
• One mistake = wrong meal or nothing at all
• In code: Write SQL strings, manually convert rows to objects, handle type conversions yourself

WITH A TRANSLATOR (ORM like Entity Framework Core):
• You speak normally in your language (C#)
• Translator converts to local language (SQL) automatically
• Order arrives correctly
• Food is presented how you expect
• You focus on what you WANT, not how to SAY it
• In code: Write C# LINQ queries, get objects back automatically, everything is type-safe

Real code comparison:

WITHOUT ORM (raw SQL):
```csharp
string sql = "SELECT * FROM Customers WHERE Age > 25";
var command = connection.CreateCommand();
command.CommandText = sql;
var reader = command.ExecuteReader();
while (reader.Read())
{
    var customer = new Customer 
    { 
        Id = (int)reader["Id"],
        Name = (string)reader["Name"]  // Hope this cast works!
    };
}
```
COMPLEX! SQL strings, manual mapping, error-prone, no IntelliSense!

WITH ORM (Entity Framework Core):
```csharp
var customers = dbContext.Customers
    .Where(c => c.Age > 25)
    .ToList();  // Returns List<Customer> automatically!
```
SIMPLE! C# LINQ, automatic mapping, type-safe, IntelliSense works!

Think: ORM = 'You speak C#, the ORM speaks SQL. You never have to learn the database dialect!'