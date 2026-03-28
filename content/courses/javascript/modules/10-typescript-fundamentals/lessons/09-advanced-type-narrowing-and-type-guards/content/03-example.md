---
type: "EXAMPLE"
title: "instanceof Guards - Class and Constructor Checking"
---

The instanceof operator checks if an object was created from a specific constructor. It's perfect for class hierarchies and built-in types:

```typescript
// instanceof with built-in types
function handleError(error: Error | string): string {
  if (error instanceof Error) {
    // TypeScript knows: error is Error
    // Access Error properties: message, name, stack
    return `Error [${error.name}]: ${error.message}`;
  }
  
  // TypeScript knows: error is string
  return `String error: ${error}`;
}

console.log(handleError(new Error('Something failed')));
// 'Error [Error]: Something failed'

console.log(handleError('Connection timeout'));
// 'String error: Connection timeout'

// instanceof with custom classes
class Dog {
  name: string;
  constructor(name: string) {
    this.name = name;
  }
  bark(): void {
    console.log(`${this.name} says: Woof!`);
  }
}

class Cat {
  name: string;
  constructor(name: string) {
    this.name = name;
  }
  meow(): void {
    console.log(`${this.name} says: Meow!`);
  }
}

class Bird {
  name: string;
  constructor(name: string) {
    this.name = name;
  }
  chirp(): void {
    console.log(`${this.name} says: Chirp!`);
  }
}

type Pet = Dog | Cat | Bird;

function makeSound(pet: Pet): void {
  if (pet instanceof Dog) {
    // TypeScript knows: pet is Dog
    pet.bark();
  } else if (pet instanceof Cat) {
    // TypeScript knows: pet is Cat
    pet.meow();
  } else {
    // TypeScript knows: pet is Bird
    pet.chirp();
  }
}

makeSound(new Dog('Buddy'));   // 'Buddy says: Woof!'
makeSound(new Cat('Whiskers')); // 'Whiskers says: Meow!'
makeSound(new Bird('Tweety'));  // 'Tweety says: Chirp!'

// instanceof with Error subclasses
class ValidationError extends Error {
  field: string;
  constructor(message: string, field: string) {
    super(message);
    this.name = 'ValidationError';
    this.field = field;
  }
}

class NetworkError extends Error {
  statusCode: number;
  constructor(message: string, statusCode: number) {
    super(message);
    this.name = 'NetworkError';
    this.statusCode = statusCode;
  }
}

function handleAppError(error: Error): void {
  if (error instanceof ValidationError) {
    // TypeScript knows: error is ValidationError
    console.log(`Validation failed on field '${error.field}': ${error.message}`);
  } else if (error instanceof NetworkError) {
    // TypeScript knows: error is NetworkError
    console.log(`Network error (${error.statusCode}): ${error.message}`);
  } else {
    // Generic Error
    console.log(`Unknown error: ${error.message}`);
  }
}

handleAppError(new ValidationError('Required', 'email'));
// 'Validation failed on field 'email': Required'

handleAppError(new NetworkError('Server unavailable', 503));
// 'Network error (503): Server unavailable'

// LIMITATION: instanceof doesn't work with plain objects or interfaces!
interface User {
  name: string;
  email: string;
}

let user: User = { name: 'Alice', email: 'alice@test.com' };
// user instanceof User; // ERROR! User is not a runtime value
// Interfaces don't exist at runtime - use 'in' operator or custom guards instead
```
