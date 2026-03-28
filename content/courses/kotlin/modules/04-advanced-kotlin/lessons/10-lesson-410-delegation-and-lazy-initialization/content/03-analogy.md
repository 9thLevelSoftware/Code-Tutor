---
type: "ANALOGY"
title: "Property Management: A Real-World Delegation Analogy"
---

**The Property Owner and the Management Company**

Imagine you own several apartment buildings but live in another city. You can't personally handle every tenant request, maintenance issue, or rent collection. What do you do?

**Without Delegation: The Overwhelmed Owner**

You try to manage everything yourself:
- Answer every phone call from tenants
- Coordinate every plumbing repair
- Collect rent from each unit
- Advertise vacant apartments
- Screen potential tenants

You're constantly forwarding requests, keeping records, and acting as a middleman. This is like Java code without delegation—endless boilerplate forwarding methods that add no real value.

```kotlin
// Without delegation - tedious forwarding
class RemoteOwner : PropertyManager {
    val localManager = LocalManager()
    
    override fun collectRent(unit: String): Payment {
        return localManager.collectRent(unit)  // Just forwarding!
    }
    
    override fun handleMaintenance(request: Request) {
        localManager.handleMaintenance(request)  // Just forwarding!
    }
    
    override fun screenTenant(applicant: Applicant): Boolean {
        return localManager.screenTenant(applicant)  // Just forwarding!
    }
    // ... dozens more methods
}
```

**With Delegation: The Smart Owner**

You hire a property management company and *delegate* all day-to-day operations to them:
- They handle tenant relations
- They coordinate repairs
- They collect and deposit rent
- You only get involved for major decisions

This is exactly how Kotlin's `by` keyword works—automatic delegation with the ability to override when needed.

```kotlin
// With delegation - clean and focused
class RemoteOwner(manager: PropertyManager) : PropertyManager by manager {
    override fun approveMajorExpense(expense: Expense): Boolean {
        // Only handle what matters - major decisions
        return expense.amount < 10000
    }
}
```

**The Benefits**

1. **No Boilerplate**: The management company handles routine tasks automatically
2. **Focused Attention**: You intervene only where you add value
3. **Flexible**: You can swap management companies without changing your role
4. **Composability**: You can delegate different buildings to different managers

**Multiple Delegation: The Portfolio Owner**

As your real estate empire grows, you might delegate different responsibilities to different specialists:
- Property Manager A handles residential buildings
- Property Manager B handles commercial properties
- A marketing firm handles advertising
- An accounting firm handles finances

Kotlin supports this too—you can delegate multiple interfaces to different objects:

```kotlin
class PortfolioOwner(
    residential: ResidentialManager,
    commercial: CommercialManager,
    marketing: MarketingService
) : ResidentialManager by residential,
    CommercialManager by commercial,
    MarketingCapable by marketing
```

**Real-World Programming Connection**

Just as property delegation lets owners focus on high-value decisions while experts handle operations, Kotlin's delegation lets developers:
- Focus business logic in their classes
- Delegate infrastructure concerns (logging, caching, validation) to specialized delegates
- Swap implementations without changing business logic
- Compose behaviors rather than inheriting rigid hierarchies

This is the power of composition over inheritance—and Kotlin makes it as easy as saying `by`.
