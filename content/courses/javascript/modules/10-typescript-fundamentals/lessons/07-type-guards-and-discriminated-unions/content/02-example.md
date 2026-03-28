---
type: "EXAMPLE"
title: "Narrowing the Type"
---

```typescript
// 1. Using 'typeof' (For primitives)
function printId(id: number | string) {
    if (typeof id === "string") {
        // TypeScript knows 'id' is a string here!
        console.log(`ID: ${id.toUpperCase()}`);
    } else {
        // TypeScript knows 'id' is a number here!
        console.log(`ID: ${id.toFixed(2)}`);
    }
}

// 2. Discriminated Unions (The Professional Way)
interface SuccessResponse {
    status: "success"; // The Discriminant
    data: string[];
}

interface ErrorResponse {
    status: "error"; // The Discriminant
    message: string;
}

type APIResponse = SuccessResponse | ErrorResponse;

function handleResponse(res: APIResponse) {
    if (res.status === "success") {
        // Only 'data' is available here
        console.log(res.data.length);
    } else {
        // Only 'message' is available here
        console.log(res.message);
    }
}

// 3. User-Defined Type Guards (The 'is' keyword)
function isString(value: unknown): value is string {
    return typeof value === "string";
}

let input: unknown = "Hello";
if (isString(input)) {
    console.log(input.length); // Works because of 'value is string'
}
```