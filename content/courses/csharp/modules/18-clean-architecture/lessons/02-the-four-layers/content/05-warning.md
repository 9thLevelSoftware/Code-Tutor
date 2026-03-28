---
type: "WARNING"
title: "Common Pitfalls"
---

## Layer Boundary Mistakes

**Leaking Infrastructure Into Application**: The most common violation is letting Entity Framework types (DbContext, IQueryable) leak into the Application layer. If your Application service returns `IQueryable<Product>`, the Presentation layer now depends on EF Core. Return concrete collections like `IReadOnlyList<T>` instead.

**Shared DTOs Across Layers**: Creating a single DTO used by all layers couples them together. A change to the API response format shouldn't require changes in the Application layer. Keep separate models: Domain entities, Application DTOs, and API response models.

**Infrastructure Layer Becoming a Dumping Ground**: When unsure where code belongs, developers default to Infrastructure. Over time it accumulates business logic, validation, and orchestration that should live in Application or Domain. If code doesn't touch an external system (database, API, file system), it probably doesn't belong in Infrastructure.

**Circular Dependencies**: Application defines interfaces, Infrastructure implements them, but if Application accidentally references Infrastructure types (even through transitive dependencies), the dependency rule is broken. The compiler won't catch semantic violations -- only project reference violations. Review dependencies regularly.

**Too Many Layers for Simple Operations**: A simple "get product by ID" doesn't need Command/Query objects, handlers, validators, and mappers. It's okay to have thin pass-through layers for simple operations. Not every endpoint needs the full CQRS ceremony.
