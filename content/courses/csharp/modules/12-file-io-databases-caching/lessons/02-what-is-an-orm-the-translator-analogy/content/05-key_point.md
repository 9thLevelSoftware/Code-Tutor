---
type: "KEY_POINT"
title: "ORM Bridges Objects and Tables"
---

## Key Takeaways

- **An ORM maps C# classes to database tables** — your `Product` class becomes the `Products` table automatically. Properties become columns with appropriate SQL types. One object instance equals one row in the table. This mapping eliminates the need to write SQL table schemas by hand.

- **Write LINQ, get SQL** — `context.Products.Where(p => p.Price > 10)` generates `SELECT * FROM Products WHERE Price > 10`. You work with objects and expressions, the ORM handles SQL translation, parameterization, and execution. This abstraction prevents SQL injection and handles database dialect differences.

- **`DbContext` is your database session** — it tracks entity changes, generates SQL, manages the connection pool, and coordinates transactions. Register it with Dependency Injection using `AddDbContext<T>()` in ASP.NET Core. The context follows the Unit of Work pattern, batching changes until `SaveChanges()` is called.

- **Always call `SaveChanges()` to persist data** — modifying entity properties only updates in-memory objects. No database changes occur until you explicitly call `context.SaveChanges()`. This method starts a transaction, generates INSERT/UPDATE/DELETE statements for all tracked changes, and commits atomically.

- **Use `AsNoTracking()` for read-only queries** — EF Core tracks entities by default for change detection. For pure display scenarios where you won't modify data, `AsNoTracking()` eliminates the memory overhead of the change tracker and improves performance significantly.

- **Navigation properties define relationships** — `Order.Customer` and `Customer.Orders` establish foreign key relationships without manual join SQL. Configure these in `OnModelCreating()` or using data annotations. Lazy loading fetches related data automatically; eager loading with `Include()` optimizes query performance by reducing round trips.
