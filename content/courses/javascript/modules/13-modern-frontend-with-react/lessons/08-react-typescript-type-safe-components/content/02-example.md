---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// React + TypeScript Fundamentals

console.log('=== React + TypeScript ===\n');

// Setup: npm create vite@latest my-app -- --template react-ts

// 1. TYPING PROPS
console.log('1. Typing Component Props\n');

let userCardExample = `
// Define props interface
interface UserCardProps {
  name: string;
  email: string;
  age: number;
  isAdmin?: boolean;  // Optional prop
  onEdit: (id: number) => void;  // Function prop
}

// Use interface with component
function UserCard({ name, email, age, isAdmin = false, onEdit }: UserCardProps) {
  return (
    <div className="card">
      <h2>{name} {isAdmin && '👑'}</h2>
      <p>{email}</p>
      <p>Age: {age}</p>
      <button onClick={() => onEdit(1)}>Edit</button>
    </div>
  );
}

// TypeScript catches errors:
<UserCard name="Alice" />  // ❌ Error: missing email, age, onEdit
<UserCard name="Alice" email="a@b.com" age="25" onEdit={() => {}} />  // ❌ age should be number
<UserCard name="Alice" email="a@b.com" age={25} onEdit={() => {}} />  // ✅ Correct!
`;
console.log(userCardExample);

// 2. TYPING STATE
console.log('\n2. Typing useState\n');

let stateExample = `
// Explicit type for complex state
interface User {
  id: number;
  name: string;
  email: string;
}

function UserProfile() {
  // Type inferred from initial value
  const [count, setCount] = useState(0);  // number
  const [name, setName] = useState('');   // string
  
  // Explicit type for complex objects
  const [user, setUser] = useState<User | null>(null);
  
  // Array of objects
  const [users, setUsers] = useState<User[]>([]);
  
  // Union types
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  
  return (
    <div>
      {user?.name}  // Safe access with optional chaining
      {status === 'loading' && <Spinner />}
    </div>
  );
}
`;
console.log(stateExample);

// 3. TYPING EVENTS
console.log('\n3. Typing Event Handlers\n');

let eventExample = `
function SearchForm() {
  const [query, setQuery] = useState('');
  
  // Input change event
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setQuery(e.target.value);
  };
  
  // Form submit event
  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    console.log('Searching:', query);
  };
  
  // Button click event
  const handleClick = (e: React.MouseEvent<HTMLButtonElement>) => {
    console.log('Button clicked');
  };
  
  return (
    <form onSubmit={handleSubmit}>
      <input value={query} onChange={handleChange} />
      <button type="submit" onClick={handleClick}>Search</button>
    </form>
  );
}
`;
console.log(eventExample);

// 4. TYPING CHILDREN
console.log('\n4. Typing Children Prop\n');

let childrenExample = `
// For components that wrap other components
interface CardProps {
  title: string;
  children: React.ReactNode;  // Any valid React child
}

function Card({ title, children }: CardProps) {
  return (
    <div className="card">
      <h2>{title}</h2>
      {children}
    </div>
  );
}

// Usage
<Card title="Profile">
  <p>User content here</p>
  <UserAvatar />
</Card>
`;
console.log(childrenExample);

// 5. TYPING CONTEXT
console.log('\n5. Typing Context API\n');

let contextExample = `
// contexts/AuthContext.tsx
interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}

// Now TypeScript knows exactly what useAuth returns!
function Navbar() {
  const { user, logout } = useAuth();  // Fully typed!
  //     ^^^^ TypeScript knows user is User | null
}
`;
console.log(contextExample);

// 6. COMMON PATTERNS
console.log('\n6. Common TypeScript + React Patterns\n');

let patterns = `
// Generic Components
interface ListProps<T> {
  items: T[];
  renderItem: (item: T) => React.ReactNode;
}

function List<T>({ items, renderItem }: ListProps<T>) {
  return <ul>{items.map(renderItem)}</ul>;
}

// Usage with type inference
<List 
  items={[{ id: 1, name: 'Alice' }]} 
  renderItem={(user) => <li key={user.id}>{user.name}</li>}
/>

// Discriminated Unions for State
type RequestState<T> = 
  | { status: 'idle' }
  | { status: 'loading' }
  | { status: 'success'; data: T }
  | { status: 'error'; error: string };

function UserList() {
  const [state, setState] = useState<RequestState<User[]>>({ status: 'idle' });
  
  if (state.status === 'loading') return <Spinner />;
  if (state.status === 'error') return <Error message={state.error} />;
  if (state.status === 'success') return <List items={state.data} />;
  return <button onClick={fetch}>Load Users</button>;
}
`;
console.log(patterns);

console.log('\n=== Quick Reference ===\n');
console.log('Props:     interface XProps { prop: type }');
console.log('State:     useState<Type>(initial)');
console.log('Events:    (e: React.ChangeEvent<HTMLInputElement>) => void');
console.log('Children:  React.ReactNode');
console.log('Ref:       useRef<HTMLInputElement>(null)');
console.log('Context:   createContext<Type | null>(null)');
```
