---
type: "EXAMPLE"
title: "Complete Example: Order Service"
---


Putting it all together:



```kotlin
import arrow.core.*
import arrow.core.raise.*

// Error types
sealed interface OrderError {
    data class NotFound(val id: Long) : OrderError
    data class InvalidStatus(val message: String) : OrderError
    data class PaymentFailed(val reason: String) : OrderError
    data class InsufficientStock(val productId: Long) : OrderError
}

// Service with Raise-based implementation
class OrderService(
    private val orderRepository: OrderRepository,
    private val inventoryService: InventoryService,
    private val paymentService: PaymentService
) {
    // Public API returns Either
    suspend fun processOrder(orderId: Long): Either<OrderError, Order> = either {
        processOrderInternal(orderId)
    }
    
    // Internal implementation uses Raise
    context(raise: Raise<OrderError>)
    private suspend fun processOrderInternal(orderId: Long): Order {
        // Get order
        val order = orderRepository.findById(orderId)
            ?: raise.raise(OrderError.NotFound(orderId))

        // Validate status
        raise.ensure(order.status == OrderStatus.PENDING) {
            OrderError.InvalidStatus("Order is ${order.status}, expected PENDING")
        }

        // Check inventory
        for (item in order.items) {
            val available = inventoryService.getStock(item.productId)
            raise.ensure(available >= item.quantity) {
                OrderError.InsufficientStock(item.productId)
            }
        }

        // Reserve inventory
        inventoryService.reserve(order.items)

        // Process payment
        val paymentResult = paymentService.charge(order.customerId, order.total)
        raise.ensure(paymentResult.success) {
            // Rollback inventory reservation on payment failure
            inventoryService.release(order.items)
            OrderError.PaymentFailed(paymentResult.message)
        }

        // Update order
        return orderRepository.save(
            order.copy(status = OrderStatus.PROCESSING, paymentId = paymentResult.id)
        )
    }
}

// Controller using the service
class OrderController(private val orderService: OrderService) {
    
    suspend fun processOrder(orderId: Long): Response = 
        orderService.processOrder(orderId).fold(
            ifLeft = { error ->
                when (error) {
                    is OrderError.NotFound -> 
                        Response.notFound("Order $orderId not found")
                    is OrderError.InvalidStatus -> 
                        Response.badRequest(error.message)
                    is OrderError.PaymentFailed -> 
                        Response.paymentRequired(error.reason)
                    is OrderError.InsufficientStock -> 
                        Response.conflict("Product ${error.productId} out of stock")
                }
            },
            ifRight = { order ->
                Response.ok(order)
            }
        )
}
```
