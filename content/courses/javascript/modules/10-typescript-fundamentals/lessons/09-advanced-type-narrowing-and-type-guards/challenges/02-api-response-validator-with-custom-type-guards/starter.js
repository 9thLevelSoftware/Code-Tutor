// 1. User interface
interface User {
  // Define: id, name, email, role ('admin' | 'user')
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
  // Check all properties exist and have correct types
  // Don't forget to validate role is 'admin' or 'user'
}

// 4. Type guard for ApiResponse
function isApiResponse(value: unknown): value is ApiResponse {
  // Check success property and validate accordingly
}

// 5. Safe processor function
function processApiResponse(data: unknown): void {
  if (!isApiResponse(data)) {
    console.log('Invalid API response format');
    return;
  }
  
  // Now data is ApiResponse - handle success and error cases
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
processApiResponse(validError);
processApiResponse(invalidData);