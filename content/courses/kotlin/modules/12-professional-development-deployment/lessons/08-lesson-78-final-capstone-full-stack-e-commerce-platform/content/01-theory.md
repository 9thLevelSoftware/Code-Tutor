---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 90 minutes

This capstone project brings together everything you've learned to build a complete e-commerce platform. You'll create a KMP mobile app with shared business logic, a Ktor backend, and real-world features like authentication, payments, and order management.

**Project Architecture**

```
├── shared/                 # KMP shared module
│   ├── domain/            # Business logic, use cases
│   ├── data/              # Repositories, API clients
│   └── presentation/      # ViewModels, MVI
├── androidApp/            # Android UI (Compose)
├── iosApp/               # iOS UI (SwiftUI)
├── backend/              # Ktor server
│   ├── routes/           # API endpoints
│   ├── services/         # Business logic
│   └── persistence/      # Database with Exposed
```

**Core Features to Implement**

1. **User Authentication**
   - JWT-based auth
   - Secure token storage
   - Biometric login (platform-specific)

2. **Product Catalog**
   - Browse categories
   - Search and filters
   - Product details with images

3. **Shopping Cart**
   - Add/remove items
   - Persist cart locally (SQLDelight)
   - Sync with backend when online

4. **Checkout Flow**
   - Address management
   - Payment integration (Stripe)
   - Order confirmation

5. **Order History**
   - View past orders
   - Order status tracking
   - Reorder functionality

**Shared Domain Models**

```kotlin
// shared/src/commonMain/kotlin/domain/model/Product.kt
data class Product(
    val id: String,
    val name: String,
    val description: String,
    val price: Money,
    val images: List<String>,
    val category: Category,
    val inventory: Int
)

data class CartItem(
    val product: Product,
    val quantity: Int
) {
    val subtotal: Money
        get() = product.price * quantity
}

data class Order(
    val id: String,
    val items: List<CartItem>,
    val total: Money,
    val status: OrderStatus,
    val createdAt: Instant
)
```

**API Contract**

```kotlin
// Backend routes
fun Route.products() {
    get("/products") {
        val category = call.parameters["category"]
        val products = productService.getProducts(category)
        call.respond(products)
    }
    
    get("/products/{id}") {
        val id = call.parameters["id"]!!
        val product = productService.getProduct(id)
            ?: return@get call.respond(HttpStatusCode.NotFound)
        call.respond(product)
    }
}

fun Route.orders() {
    authenticate {
        post("/orders") {
            val request = call.receive<CreateOrderRequest>()
            val order = orderService.createOrder(
                userId = call.principal<UserId>()!!,
                items = request.items,
                shippingAddress = request.address
            )
            call.respond(HttpStatusCode.Created, order)
        }
    }
}
```

**Technology Stack**

| Layer | Technology |
|-------|-----------|
| UI | Compose Multiplatform / SwiftUI |
| State | MVI with StateFlow |
| Network | Ktor client + Kotlinx Serialization |
| Database | SQLDelight |
| Backend | Ktor + Exposed |
| Auth | JWT |
| Payments | Stripe SDK (platform-specific) |
| Images | Coil (Android) / AsyncImage (iOS) |

**Success Criteria**

- [ ] User can browse products on both Android and iOS
- [ ] Cart persists across app restarts
- [ ] Checkout completes successfully with test payments
- [ ] Orders appear in order history
- [ ] App works offline with sync when online
- [ ] All tests pass (unit, integration, UI)

This capstone demonstrates how KMP enables true code sharing while delivering native-quality experiences across platforms.
