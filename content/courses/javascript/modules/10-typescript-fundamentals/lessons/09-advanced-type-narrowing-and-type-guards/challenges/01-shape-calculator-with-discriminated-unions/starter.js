// 1. Define shape interfaces with discriminant 'kind'
interface Circle {
  // kind: 'circle' and radius
}

interface Rectangle {
  // kind: 'rectangle', width, and height
}

interface Triangle {
  // kind: 'triangle', base, and height
}

// 2. Union type
type Shape = Circle | Rectangle | Triangle;

// Helper for exhaustiveness checking
function assertNever(x: never): never {
  throw new Error(`Unexpected shape: ${JSON.stringify(x)}`);
}

// 3. Calculate area with exhaustiveness checking
function calculateArea(shape: Shape): number {
  // Use switch on shape.kind
  // Circle: Math.PI * r^2
  // Rectangle: width * height
  // Triangle: 0.5 * base * height
}

// 4. Describe the shape
function describeShape(shape: Shape): string {
  // Return descriptions like:
  // 'A circle with radius 5'
  // 'A 10x4 rectangle'
  // 'A triangle with base 6 and height 8'
}

// 5. Test all shapes
let circle: Circle = { kind: 'circle', radius: 5 };
let rect: Rectangle = { kind: 'rectangle', width: 10, height: 4 };
let tri: Triangle = { kind: 'triangle', base: 6, height: 8 };

console.log(describeShape(circle), '- Area:', calculateArea(circle).toFixed(2));
console.log(describeShape(rect), '- Area:', calculateArea(rect));
console.log(describeShape(tri), '- Area:', calculateArea(tri));