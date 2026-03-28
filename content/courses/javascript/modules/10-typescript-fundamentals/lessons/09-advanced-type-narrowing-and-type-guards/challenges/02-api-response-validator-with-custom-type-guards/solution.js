// 1. User interface
interface User {
  id: number;
  name: string;
  email: string;
  role: 'admin' | 'user';
}

// 2. API Response types (discriminated union)
interface SuccessResponse {
  success: true;
  data: User;
}

interface ErrorResponse {
  success: false;
  error: string;
  code: number;
}

type ApiResponse = SuccessResponse | ErrorResponse;

// 3. Type guard for User
function isUser(value: unknown): value is User {
  if (typeof value !== 'object' || value === null) {
    return false;
  }
  
  const obj = value as Record<string, unknown>;
  
  return (
    typeof obj.id === 'number' &&
    typeof obj.name === 'string' &&
    typeof obj.email === 'string' &&
    (obj.role === 'admin' || obj.role === 'user')
  );
}

// 4. Type guard for ApiResponse
function isApiResponse(value: unknown): value is ApiResponse {
  if (typeof value !== 'object' || value === null) {
    return false;
  }
  
  const obj = value as Record<string, unknown>;
  
  if (obj.success === true) {
    // Validate SuccessResponse
    return isUser(obj.data);
  } else if (obj.success === false) {
    // Validate ErrorResponse
    return (
      typeof obj.error === 'string' &&
      typeof obj.code === 'number'
    );
  }
  
  return false;
}

// 5. Safe processor function
function processApiResponse(data: unknown): void {
  if (!isApiResponse(data)) {
    console.log('Invalid API response format');
    return;
  }
  
  // Now data is ApiResponse
  if (data.success) {
    // TypeScript knows: data is SuccessResponse
    console.log(`Success! User: ${data.data.name} (${data.data.email})`);
    console.log(`Role: ${data.data.role}`);
  } else {
    // TypeScript knows: data is ErrorResponse
    console.log(`Error ${data.code}: ${data.error}`);
  }
}

// Test with various data
let validSuccess = {
  success: true,
  data: { id: 1, name: 'Alice', email: 'alice@test.com', role: 'admin' }
};

let validError = {
  success: false,
  error: 'User not found',
  code: 404
};

let invalidData = { message: 'wrong format' };

processApiResponse(validSuccess);
// Success! User: Alice (alice@test.com)
// Role: admin

processApiResponse(validError);
// Error 404: User not found

processApiResponse(invalidData);
// Invalid API response format