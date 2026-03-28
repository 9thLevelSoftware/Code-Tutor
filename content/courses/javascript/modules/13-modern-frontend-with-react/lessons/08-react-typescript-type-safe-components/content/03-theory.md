---
type: "THEORY"
title: "Breaking Down the Syntax"
---

TypeScript + React essentials:

1. **Props Interface**:
   ```tsx
   interface ButtonProps {
     label: string;
     onClick: () => void;
     disabled?: boolean;  // Optional
     variant: 'primary' | 'secondary';  // Union type
   }
   
   function Button({ label, onClick, disabled = false }: ButtonProps) {
     return <button disabled={disabled} onClick={onClick}>{label}</button>;
   }
   ```

2. **Typed useState**:
   ```tsx
   // Simple types (inferred)
   const [count, setCount] = useState(0);
   
   // Complex types (explicit)
   const [user, setUser] = useState<User | null>(null);
   const [items, setItems] = useState<Item[]>([]);
   ```

3. **Common Event Types**:
   ```tsx
   // Input
   onChange: (e: React.ChangeEvent<HTMLInputElement>) => void
   
   // Form
   onSubmit: (e: React.FormEvent<HTMLFormElement>) => void
   
   // Button
   onClick: (e: React.MouseEvent<HTMLButtonElement>) => void
   
   // Keyboard
   onKeyDown: (e: React.KeyboardEvent<HTMLInputElement>) => void
   ```

4. **Typed useRef**:
   ```tsx
   const inputRef = useRef<HTMLInputElement>(null);
   const divRef = useRef<HTMLDivElement>(null);
   
   // Access with null check
   inputRef.current?.focus();
   ```

5. **Typed Callbacks**:
   ```tsx
   const handleSave = useCallback((id: number, data: User) => {
     // ...
   }, []);
   
   const total = useMemo<number>(() => {
     return items.reduce((sum, item) => sum + item.price, 0);
   }, [items]);
   ```

6. **Function Component Type**:
   ```tsx
   // Explicit return type (optional but helpful)
   const UserCard: React.FC<UserProps> = ({ name, email }) => {
     return <div>{name}</div>;
   };
   
   // Or simpler (preferred):
   function UserCard({ name, email }: UserProps): JSX.Element {
     return <div>{name}</div>;
   }
   ```