---
type: "KEY_POINT"
title: "The Dependency Rule"
---

## Key Takeaways

- **Source code dependencies must point inward only** -- Infrastructure references Application; Application references Domain; Domain references nothing. Violations of this rule couple your business logic to frameworks.

- **Interfaces live in inner layers, implementations in outer layers** -- `IProductRepository` is defined in Application. `ProductRepository` (using EF Core) lives in Infrastructure. This inversion lets you test without databases.

- **Each layer is a separate .csproj** -- `ShopFlow.Domain`, `ShopFlow.Application`, `ShopFlow.Infrastructure`, `ShopFlow.Api`. Project references enforce the dependency rule at compile time.
