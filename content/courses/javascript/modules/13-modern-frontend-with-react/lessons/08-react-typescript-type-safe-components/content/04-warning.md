---
type: "WARNING"
title: "Common Pitfalls"
---

Common TypeScript + React mistakes:

1. **Forgetting null checks with state**:
   ```tsx
   // WRONG! user could be null
   const [user, setUser] = useState<User | null>(null);
   return <div>{user.name}</div>;  // ❌ Error!
   
   // CORRECT! Check for null
   return <div>{user?.name}</div>;  // ✅ Optional chaining
   // OR
   if (!user) return <Loading />;
   return <div>{user.name}</div>;  // ✅ TypeScript knows user exists
   ```

2. **Wrong event type**:
   ```tsx
   // WRONG! Using wrong element type
   const handleChange = (e: React.ChangeEvent<HTMLButtonElement>) => {
     console.log(e.target.value);  // Buttons don't have value!
   };
   
   // CORRECT! Match element type
   const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
     console.log(e.target.value);  // ✅
   };
   ```

3. **Using `any` to avoid errors**:
   ```tsx
   // WRONG! Defeats the purpose of TypeScript
   const [data, setData] = useState<any>(null);
   
   // CORRECT! Define proper types
   interface ApiResponse {
     users: User[];
     total: number;
   }
   const [data, setData] = useState<ApiResponse | null>(null);
   ```

4. **Overusing React.FC**:
   ```tsx
   // Less preferred (implicit children, verbose)
   const Card: React.FC<CardProps> = ({ title }) => { ... };
   
   // Preferred (simpler, explicit)
   function Card({ title }: CardProps) { ... }
   ```

5. **Not typing async functions**:
   ```tsx
   // WRONG! Return type unclear
   const fetchUsers = async () => {
     const res = await fetch('/api/users');
     return res.json();  // What type is this?
   };
   
   // CORRECT! Explicit return type
   const fetchUsers = async (): Promise<User[]> => {
     const res = await fetch('/api/users');
     return res.json();
   };
   ```

**Setup TypeScript + React**:
```bash
# New project
npm create vite@latest my-app -- --template react-ts

# Add to existing project
npm install typescript @types/react @types/react-dom
npx tsc --init
```