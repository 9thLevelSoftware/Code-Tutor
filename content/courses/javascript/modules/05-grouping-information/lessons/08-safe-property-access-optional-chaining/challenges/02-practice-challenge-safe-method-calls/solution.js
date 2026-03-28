let calculator = {
  add: (a, b) => a + b,
  multiply: (a, b) => a * b
};

let sum = calculator.add?.(5, 3);
let product = calculator.multiply?.(4, 2);
let difference = calculator.subtract?.(10, 4);
let quotient = calculator.divide?.(20, 5);

console.log('Sum:', sum);  // 8
console.log('Product:', product);  // 8
console.log('Difference:', difference ?? 'Method not available');  // 'Method not available'
console.log('Quotient:', quotient ?? 'Method not available');  // 'Method not available'