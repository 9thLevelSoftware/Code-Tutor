---
type: "ANALOGY"
title: "Version Control for Your Database Schema"
---

**MIGRATIONS = Database Schema Version Control**

Imagine you're developing a web app with your team:

**Week 1**: You create the `Users` table
**Week 2**: Teammate adds an `Email` column
**Week 3**: Another teammate creates `Orders` table with foreign key to `Users`
**Week 4**: You rename `Email` to `EmailAddress` and add an index

Without migrations: Chaos! Everyone's local database is different. Production crashes when you deploy.

With migrations: Each change is a script that can be applied anywhere - dev machines, test servers, production.

**How migrations work (like Git commits):**
• `20240115120000_InitialCreate.cs` - Creates Users table
• `20240122153000_AddUserEmail.cs` - Adds Email column
• `20240129104500_CreateOrdersTable.cs` - Adds Orders with relationship

Each migration:
• Has an `Up()` method (apply the change)
• Has a `Down()` method (rollback if needed)
• Gets timestamped so order is clear
• Can be applied to any environment

**BULK OPERATIONS (EF Core 7/8+):**

OLD way (like calling 1000 people one at a time):
```csharp
foreach (var product in outdatedProducts)
{
    context.Products.Remove(product);  // One DELETE per product
}
context.SaveChanges();  // 1000 round trips to database - SLOW!
```

NEW way (like sending one email to 1000 people):
```csharp
context.Products
    .Where(p => p.LastUpdated < DateTime.Now.AddYears(-1))
    .ExecuteDelete();  // Single DELETE statement - FAST!
```

Other bulk operations:
• `ExecuteUpdate()` - Update thousands of rows in one command
• `ExecuteDelete()` - Delete matching rows efficiently
• Perfect for: Data cleanup, price updates, status changes

Think: Migrations = 'Git for your database structure' | Bulk operations = 'SQL efficiency without the SQL hassle'