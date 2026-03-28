---
type: "EXAMPLE"
title: "ShopFlow Solution Structure"
---

Here's how ShopFlow's Clean Architecture solution is organized as separate .NET projects, showing the complete folder structure for a real-world e-commerce application.

```csharp
// ===== SHOPFLOW CLEAN ARCHITECTURE - COMPLETE SOLUTION STRUCTURE =====

/*
ShopFlow.sln
│
├── src/
│   │
│   ├── ShopFlow.Domain/                    ← INNERMOST LAYER (The Core)
│   │   ├── Entities/
│   │   │   ├── Product.cs                   # Core business entity
│   │   │   ├── Order.cs                     # Aggregate root
│   │   │   ├── OrderItem.cs                 # Part of Order aggregate
│   │   │   ├── Customer.cs                  # Customer entity
│   │   │   └── Category.cs                  # Product categorization
│   │   │
│   │   ├── ValueObjects/
│   │   │   ├── Money.cs                     # Immutable money representation
│   │   │   ├── Address.cs                   # Shipping/billing address
│   │   │   ├── Email.cs                     # Validated email address
│   │   │   ├── PhoneNumber.cs               # Validated phone
│   │   │   └── Quantity.cs                  # Non-negative quantity
│   │   │
│   │   ├── Enums/
│   │   │   ├── OrderStatus.cs               # Pending, Processing, Shipped, etc.
│   │   │   ├── PaymentStatus.cs             # Unpaid, Paid, Refunded
│   │   │   └── ProductStatus.cs             # Active, Discontinued, OutOfStock
│   │   │
│   │   ├── Events/
│   │   │   ├── OrderPlacedEvent.cs          # Domain event
│   │   │   ├── PaymentReceivedEvent.cs      # Domain event
│   │   │   └── InventoryLowEvent.cs         # Domain event
│   │   │
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs           # Base domain exception
│   │   │   ├── InsufficientStockException.cs
│   │   │   └── InvalidOrderStateException.cs
│   │   │
│   │   └── ShopFlow.Domain.csproj           # NO PROJECT REFERENCES!
│   │
│   ├── ShopFlow.Application/               ← USE CASE LAYER
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IUnitOfWork.cs           # Transaction boundary
│   │   │   │   ├── ICurrentUserService.cs   # Current user context
│   │   │   │   └── IDateTimeService.cs      # Abstracting DateTime.Now
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs    # MediatR pipeline
│   │   │   │   └── LoggingBehavior.cs       # Cross-cutting concerns
│   │   │   └── Mappings/
│   │   │       └── MappingProfile.cs        # AutoMapper configuration
│   │   │
│   │   ├── Products/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateProduct/
│   │   │   │   │   ├── CreateProductCommand.cs
│   │   │   │   │   ├── CreateProductHandler.cs
│   │   │   │   │   └── CreateProductValidator.cs
│   │   │   │   └── UpdateProduct/
│   │   │   │       ├── UpdateProductCommand.cs
│   │   │   │       └── UpdateProductHandler.cs
│   │   │   ├── Queries/
│   │   │   │   ├── GetProduct/
│   │   │   │   │   ├── GetProductQuery.cs
│   │   │   │   │   ├── GetProductHandler.cs
│   │   │   │   │   └── ProductDto.cs
│   │   │   │   └── GetProducts/
│   │   │   │       ├── GetProductsQuery.cs
│   │   │   │       └── GetProductsHandler.cs
│   │   │   └── IProductRepository.cs        # Repository interface
│   │   │
│   │   ├── Orders/
│   │   │   ├── Commands/
│   │   │   │   └── CreateOrder/
│   │   │   │       ├── CreateOrderCommand.cs
│   │   │   │       └── CreateOrderHandler.cs
│   │   │   ├── Queries/
│   │   │   │   └── GetOrderById/
│   │   │   │       ├── GetOrderQuery.cs
│   │   │   │       └── OrderDto.cs
│   │   │   ├── IOrderRepository.cs
│   │   │   └── IPaymentGateway.cs           # External service interface
│   │   │
│   │   ├── Notifications/
│   │   │   └── IEmailService.cs             # Email service interface
│   │   │
│   │   └── ShopFlow.Application.csproj      # References: ShopFlow.Domain
│   │
│   ├── ShopFlow.Infrastructure/            ← EXTERNAL CONCERNS
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs              # EF Core DbContext
│   │   │   ├── Configurations/
│   │   │   │   ├── ProductConfiguration.cs  # Fluent API config
│   │   │   │   ├── OrderConfiguration.cs
│   │   │   │   └── CustomerConfiguration.cs
│   │   │   ├── Migrations/
│   │   │   │   └── 20240115_InitialCreate.cs
│   │   │   └── UnitOfWork.cs                # IUnitOfWork implementation
│   │   │
│   │   ├── Repositories/
│   │   │   ├── ProductRepository.cs         # IProductRepository impl
│   │   │   ├── OrderRepository.cs           # IOrderRepository impl
│   │   │   └── RepositoryBase.cs            # Generic repository
│   │   │
│   │   ├── Services/
│   │   │   ├── EmailService.cs              # IEmailService impl (SendGrid)
│   │   │   ├── DateTimeService.cs           # IDateTimeService impl
│   │   │   └── CurrentUserService.cs        # ICurrentUserService impl
│   │   │
│   │   ├── ExternalServices/
│   │   │   ├── StripePaymentGateway.cs      # IPaymentGateway impl
│   │   │   └── TwilioSmsService.cs          # SMS notifications
│   │   │
│   │   ├── DependencyInjection.cs           # Extension method for DI
│   │   │
│   │   └── ShopFlow.Infrastructure.csproj   # References: ShopFlow.Application
│   │
│   └── ShopFlow.API/                        ← OUTERMOST LAYER (Presentation)
│       ├── Controllers/
│       │   ├── ProductsController.cs        # REST endpoints
│       │   ├── OrdersController.cs
│       │   └── CustomersController.cs
│       │
│       ├── Endpoints/                       # Minimal API alternative
│       │   ├── ProductEndpoints.cs
│       │   └── OrderEndpoints.cs
│       │
│       ├── Middleware/
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   └── RequestLoggingMiddleware.cs
│       │
│       ├── Filters/
│       │   └── ValidationFilter.cs
│       │
│       ├── Program.cs                       # Application entry point + DI
│       ├── appsettings.json
│       │
│       └── ShopFlow.API.csproj              # References: ShopFlow.Infrastructure
│
└── tests/
    ├── ShopFlow.Domain.Tests/               # Unit tests for domain logic
    ├── ShopFlow.Application.Tests/          # Unit tests with mocked repos
    ├── ShopFlow.Infrastructure.Tests/       # Integration tests with DB
    └── ShopFlow.API.Tests/                  # API integration tests
*/

// KEY INSIGHT: Each project only references the layer directly below it!
// Domain → (no references)
// Application → Domain
// Infrastructure → Application
// API → Infrastructure
```
