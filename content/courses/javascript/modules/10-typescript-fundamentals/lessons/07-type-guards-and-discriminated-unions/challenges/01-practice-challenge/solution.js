// 1. Payment method interfaces with discriminant 'kind'
interface CreditCard {
  kind: 'credit';
  cardNumber: string;
  cvv: string;
}

interface PayPal {
  kind: 'paypal';
  email: string;
}

interface BankTransfer {
  kind: 'bank';
  accountNumber: string;
  routingNumber: string;
}

// 2. Union type of all payment methods
type PaymentMethod = CreditCard | PayPal | BankTransfer;

// 3. Process payment with type-safe switch
function processPayment(payment: PaymentMethod, amount: number): void {
  console.log(`Processing payment of $${amount}`);
  
  switch (payment.kind) {
    case 'credit':
      // TypeScript knows: payment is CreditCard
      let lastFour = payment.cardNumber.slice(-4);
      console.log(`Processing credit card ending in ${lastFour}`);
      break;
      
    case 'paypal':
      // TypeScript knows: payment is PayPal
      console.log(`Processing PayPal payment for ${payment.email}`);
      break;
      
    case 'bank':
      // TypeScript knows: payment is BankTransfer
      let maskedAccount = payment.accountNumber.slice(-4);
      console.log(`Processing bank transfer from account ${maskedAccount}`);
      break;
  }
}

// 4. Test with all payment types
let creditPayment: CreditCard = {
  kind: 'credit',
  cardNumber: '4111111111111234',
  cvv: '123'
};

let paypalPayment: PayPal = {
  kind: 'paypal',
  email: 'user@example.com'
};

let bankPayment: BankTransfer = {
  kind: 'bank',
  accountNumber: '12345678',
  routingNumber: '987654321'
};

processPayment(creditPayment, 100);
// Processing payment of $100
// Processing credit card ending in 1234

processPayment(paypalPayment, 50);
// Processing payment of $50
// Processing PayPal payment for user@example.com

processPayment(bankPayment, 200);
// Processing payment of $200
// Processing bank transfer from account 5678