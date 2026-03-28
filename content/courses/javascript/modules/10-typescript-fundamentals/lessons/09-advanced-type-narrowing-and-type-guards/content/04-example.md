---
type: "EXAMPLE"
title: "The 'in' Operator - Property Existence Checks"
---

The 'in' operator checks if a property exists in an object. It's essential for narrowing between object types with different shapes:

```typescript
// Basic 'in' operator usage
interface Admin {
  name: string;
  role: 'admin';
  permissions: string[];
}

interface RegularUser {
  name: string;
  role: 'user';
  subscription: 'free' | 'premium';
}

type User = Admin | RegularUser;

function displayUserInfo(user: User): void {
  // Both types have 'name' and 'role'
  console.log(`Name: ${user.name}`);
  console.log(`Role: ${user.role}`);
  
  // Use 'in' to check for type-specific properties
  if ('permissions' in user) {
    // TypeScript knows: user is Admin
    console.log(`Permissions: ${user.permissions.join(', ')}`);
  }
  
  if ('subscription' in user) {
    // TypeScript knows: user is RegularUser
    console.log(`Subscription: ${user.subscription}`);
  }
}

let admin: Admin = {
  name: 'Alice',
  role: 'admin',
  permissions: ['create', 'read', 'update', 'delete']
};

let regularUser: RegularUser = {
  name: 'Bob',
  role: 'user',
  subscription: 'premium'
};

displayUserInfo(admin);
// Name: Alice
// Role: admin
// Permissions: create, read, update, delete

displayUserInfo(regularUser);
// Name: Bob
// Role: user
// Subscription: premium

// 'in' with optional properties
interface BasicProfile {
  username: string;
}

interface DetailedProfile {
  username: string;
  avatar?: string;  // Optional in DetailedProfile
  bio?: string;
}

type Profile = BasicProfile | DetailedProfile;

function renderProfile(profile: Profile): void {
  console.log(`Username: ${profile.username}`);
  
  // Check for optional properties
  if ('bio' in profile && profile.bio) {
    console.log(`Bio: ${profile.bio}`);
  }
  
  if ('avatar' in profile && profile.avatar) {
    console.log(`Avatar: ${profile.avatar}`);
  }
}

renderProfile({ username: 'alice' });
// Username: alice

renderProfile({ username: 'bob', bio: 'Developer', avatar: 'bob.png' });
// Username: bob
// Bio: Developer
// Avatar: bob.png

// 'in' with discriminated unions (even better!)
interface SuccessResponse {
  status: 'success';
  data: { id: number; name: string };
}

interface ErrorResponse {
  status: 'error';
  error: { code: number; message: string };
}

type ApiResponse = SuccessResponse | ErrorResponse;

function handleResponse(response: ApiResponse): void {
  // Using 'in' to narrow
  if ('data' in response) {
    // TypeScript knows: response is SuccessResponse
    console.log(`Success! User: ${response.data.name}`);
  } else {
    // TypeScript knows: response is ErrorResponse
    console.log(`Error ${response.error.code}: ${response.error.message}`);
  }
  
  // OR use the discriminant (even better for this pattern!)
  if (response.status === 'success') {
    console.log(`Got user ID: ${response.data.id}`);
  }
}

handleResponse({ status: 'success', data: { id: 1, name: 'Alice' } });
// Success! User: Alice
// Got user ID: 1

handleResponse({ status: 'error', error: { code: 404, message: 'Not found' } });
// Error 404: Not found
```
