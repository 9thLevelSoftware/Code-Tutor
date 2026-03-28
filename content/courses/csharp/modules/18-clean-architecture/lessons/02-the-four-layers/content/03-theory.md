---
type: "THEORY"
title: "Dependency Rule - Dependencies Point Inward Only"
---

## The Dependency Rule: The Heart of Clean Architecture

The Dependency Rule is the single most important principle in Clean Architecture. It states that source code dependencies must point INWARD - toward higher-level policies and away from outer-layer details.

**VISUALIZING THE RULE:**

Imagine concentric circles, with Domain at the center:

```
         ┌────────────────────────────────────────┐
         │          PRESENTATION (API)             │
         │   ┌────────────────────────────────┐   │
         │   │      INFRASTRUCTURE (EF)       │   │
         │   │   ┌────────────────────────┐   │   │
         │   │   │    APPLICATION         │   │   │
         │   │   │   ┌────────────────┐   │   │   │
         │   │   │   │    DOMAIN      │   │   │   │
         │   │   │   │   (Center)     │   │   │   │
         │   │   │   └────────────────┘   │   │   │
         │   │   └────────────────────────┘   │   │
         │   └────────────────────────────────┘   │
         └────────────────────────────────────────┘
                    ← Dependencies point inward
```

**WHAT 'INWARD' MEANS:**

1. **Domain Layer** (innermost) - Has ZERO dependencies on any other project. It doesn't reference Application, Infrastructure, or Presentation. It's completely self-contained.

2. **Application Layer** - Only references Domain. It can use entities and value objects from Domain, but knows nothing about Entity Framework or ASP.NET Core.

3. **Infrastructure Layer** - References Application (and transitively Domain). It implements the interfaces defined in Application.

4. **Presentation Layer** - References Infrastructure (and transitively all inner layers). It wires everything together with dependency injection.

**WHY THIS MATTERS:**

1. **Isolation**: The Domain layer can be tested without any external dependencies. No database needed, no HTTP mocking - just pure unit tests.

2. **Replaceability**: Want to switch from SQL Server to MongoDB? Only Infrastructure changes. The business logic in Domain and Application remains untouched.

3. **Framework Independence**: ASP.NET Core could be replaced with a different framework. Domain and Application wouldn't know or care.

4. **Parallel Development**: Teams can work on different layers simultaneously without stepping on each other's toes.

**THE DEPENDENCY INVERSION TRICK:**

How does Application use a database without knowing about Entity Framework? Through interfaces!

- Application defines: `interface IProductRepository { Task<Product> GetById(int id); }`
- Infrastructure implements: `class ProductRepository : IProductRepository { /* uses EF */ }`
- At runtime, DI container wires them together

This is the Dependency Inversion Principle (the D in SOLID) in action. High-level modules (Application) don't depend on low-level modules (Infrastructure). Both depend on abstractions (interfaces).