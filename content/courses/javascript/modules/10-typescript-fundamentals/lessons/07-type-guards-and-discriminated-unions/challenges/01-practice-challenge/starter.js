// 1. Create the payment method interfaces
interface CreditCard {
  // Add properties
}

interface PayPal {
  // Add properties
}

interface BankTransfer {
  // Add properties
}

// 2. Create the union type
type PaymentMethod = any;  // Fix this

// 3. Create the processPayment function
function processPayment(payment, amount) {
  // Use switch statement on payment.kind
  console.log('Processing payment of $' + amount);
}

// 4. Test with all payment types
let creditPayment = { kind: 'credit', cardNumber: '4111111111111234', cvv: '123' };
let paypalPayment = { kind: 'paypal', email: 'user@example.com' };
let bankPayment = { kind: 'bank', accountNumber: '12345678', routingNumber: '987654321' };

processPayment(creditPayment, 100);
processPayment(paypalPayment, 50);
processPayment(bankPayment, 200);