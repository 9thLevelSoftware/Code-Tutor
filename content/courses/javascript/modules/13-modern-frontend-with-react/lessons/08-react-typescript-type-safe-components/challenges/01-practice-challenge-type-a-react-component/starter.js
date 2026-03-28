// TypeScript + React Simulation

// Define interfaces
interface Product {
  id: number;
  name: string;
  price: number;
  inStock: boolean;
}

interface ProductCardProps {
  product: Product;
  onAddToCart: (id: number) => void;
}

// Simulate a typed component
function ProductCard(props: ProductCardProps) {
  let { product, onAddToCart } = props;
  
  console.log(`[ProductCard] Rendering: ${product.name}`);
  console.log(`  Price: $${product.price.toFixed(2)}`);
  console.log(`  In Stock: ${product.inStock ? 'Yes' : 'No'}`);
  
  if (product.inStock) {
    console.log('  [Add to Cart] button enabled');
  } else {
    console.log('  [Out of Stock] button disabled');
  }
  
  return {
    addToCart: () => onAddToCart(product.id)
  };
}

// Test with valid data
console.log('=== Valid Usage ===\n');

let laptop: Product = {
  id: 1,
  name: 'MacBook Pro',
  price: 1999.99,
  inStock: true
};

let card = ProductCard({
  product: laptop,
  onAddToCart: (id) => console.log(`\nAdded product ${id} to cart!`)
});

card.addToCart();

// TypeScript would catch these errors:
console.log('\n=== Errors TypeScript Would Catch ===\n');
console.log('❌ Missing required prop:');
console.log('   ProductCard({ product: laptop })  // Error: missing onAddToCart');

console.log('\n❌ Wrong type:');
console.log('   let bad = { id: "1", name: "Item", price: "10", inStock: "yes" }');
console.log('   // Error: id should be number, price should be number, inStock should be boolean');

console.log('\n❌ Wrong function signature:');
console.log('   onAddToCart: (name: string) => void');
console.log('   // Error: expects (id: number) => void');