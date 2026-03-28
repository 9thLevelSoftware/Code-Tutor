// Complete TypeScript + React Type Safety Demo

console.log('═══════════════════════════════');
console.log('  TypeScript + React Demo');
console.log('═══════════════════════════════\n');

// 1. Define interfaces
interface Product {
  id: number;
  name: string;
  price: number;
  inStock: boolean;
  category?: string;  // Optional property
}

interface ProductCardProps {
  product: Product;
  onAddToCart: (id: number) => void;
  showDetails?: boolean;  // Optional prop with default
}

// Type for cart state
interface CartItem {
  product: Product;
  quantity: number;
}

// 2. Simulate typed component
function ProductCard({ product, onAddToCart, showDetails = true }: ProductCardProps) {
  console.log(`\n┌─ ProductCard ──────────────────┐`);
  console.log(`│ ${product.name.padEnd(30)} │`);
  console.log(`├────────────────────────────────┤`);
  
  if (showDetails) {
    console.log(`│ Price: $${product.price.toFixed(2).padEnd(22)} │`);
    console.log(`│ Stock: ${(product.inStock ? '✓ Available' : '✗ Out of Stock').padEnd(23)} │`);
    if (product.category) {
      console.log(`│ Category: ${product.category.padEnd(20)} │`);
    }
  }
  
  console.log(`│                                │`);
  if (product.inStock) {
    console.log(`│  [🛒 Add to Cart]              │`);
  } else {
    console.log(`│  [Notify When Available]       │`);
  }
  console.log(`└────────────────────────────────┘`);
  
  return {
    handleClick: () => {
      if (product.inStock) {
        onAddToCart(product.id);
      } else {
        console.log(`[Notification] Will notify when ${product.name} is back!`);
      }
    }
  };
}

// 3. Simulate cart with typed state
let cart: CartItem[] = [];

function addToCart(productId: number, products: Product[]): void {
  const product = products.find(p => p.id === productId);
  if (!product) {
    console.log(`[Error] Product ${productId} not found`);
    return;
  }
  
  const existing = cart.find(item => item.product.id === productId);
  if (existing) {
    existing.quantity++;
    console.log(`[Cart] Updated ${product.name} quantity to ${existing.quantity}`);
  } else {
    cart.push({ product, quantity: 1 });
    console.log(`[Cart] Added ${product.name} to cart`);
  }
}

// 4. Test with typed data
const products: Product[] = [
  { id: 1, name: 'MacBook Pro 16"', price: 2499.99, inStock: true, category: 'Laptops' },
  { id: 2, name: 'iPhone 15 Pro', price: 999.99, inStock: true, category: 'Phones' },
  { id: 3, name: 'AirPods Max', price: 549.99, inStock: false, category: 'Audio' }
];

console.log('=== Rendering Product Cards ===');

products.forEach(product => {
  const card = ProductCard({
    product,
    onAddToCart: (id) => addToCart(id, products)
  });
  card.handleClick();
});

console.log('\n\n=== Cart Summary ===');
console.log(`Total items: ${cart.length}`);
const total = cart.reduce((sum, item) => sum + (item.product.price * item.quantity), 0);
console.log(`Total: $${total.toFixed(2)}`);

console.log('\n\n=== TypeScript Error Prevention ===\n');

const errorExamples = [
  {
    code: 'ProductCard({ product: laptop })',
    error: 'Property onAddToCart is missing',
    fixed: 'ProductCard({ product: laptop, onAddToCart: (id) => {} })'
  },
  {
    code: '{ id: "1", name: "Item", price: "10" }',
    error: 'Type string is not assignable to type number',
    fixed: '{ id: 1, name: "Item", price: 10, inStock: true }'
  },
  {
    code: 'onAddToCart: (name: string) => {}',
    error: 'Parameter name incompatible with id: number',
    fixed: 'onAddToCart: (id: number) => {}'
  },
  {
    code: 'product.nonExistent',
    error: 'Property nonExistent does not exist on type Product',
    fixed: 'Use only defined properties: id, name, price, inStock'
  }
];

errorExamples.forEach((ex, i) => {
  console.log(`${i + 1}. ❌ ${ex.code}`);
  console.log(`   Error: ${ex.error}`);
  console.log(`   ✅ Fix: ${ex.fixed}\n`);
});

console.log('═══════════════════════════════');
console.log('  Benefits of TypeScript + React');
console.log('═══════════════════════════════');
console.log('✓ Catch errors at compile time, not runtime');
console.log('✓ IDE autocomplete for props and state');
console.log('✓ Self-documenting code with interfaces');
console.log('✓ Refactor with confidence');
console.log('✓ Better team collaboration');