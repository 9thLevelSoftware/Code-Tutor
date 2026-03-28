// 1. Define shape interfaces with discriminant 'kind'
interface Circle {
  kind: 'circle';
  radius: number;
}

interface Rectangle {
  kind: 'rectangle';
  width: number;
  height: number;
}

interface Triangle {
  kind: 'triangle';
  base: number;
  height: number;
}

// 2. Union type
type Shape = Circle | Rectangle | Triangle;

// Helper for exhaustiveness checking
function assertNever(x: never): never {
  throw new Error(`Unexpected shape: ${JSON.stringify(x)}`);
}

// 3. Calculate area with exhaustiveness checking
function calculateArea(shape: Shape): number {
  switch (shape.kind) {
    case 'circle':
      return Math.PI * shape.radius ** 2;
    case 'rectangle':
      return shape.width * shape.height;
    case 'triangle':
      return 0.5 * shape.base * shape.height;
    default:
      return assertNever(shape);
  }
}

// 4. Describe the shape
function describeShape(shape: Shape): string {
  switch (shape.kind) {
    case 'circle':
      return `A circle with radius ${shape.radius}`;
    case 'rectangle':
      return `A ${shape.width}x${shape.height} rectangle`;
    case 'triangle':
      return `A triangle with base ${shape.base} and height ${shape.height}`;
    default:
      return assertNever(shape);
  }
}

// 5. Test all shapes
let circle: Circle = { kind: 'circle', radius: 5 };
let rect: Rectangle = { kind: 'rectangle', width: 10, height: 4 };
let tri: Triangle = { kind: 'triangle', base: 6, height: 8 };

console.log(describeShape(circle), '- Area:', calculateArea(circle).toFixed(2));
// A circle with radius 5 - Area: 78.54

console.log(describeShape(rect), '- Area:', calculateArea(rect));
// A 10x4 rectangle - Area: 40

console.log(describeShape(tri), '- Area:', calculateArea(tri));
// A triangle with base 6 and height 8 - Area: 24