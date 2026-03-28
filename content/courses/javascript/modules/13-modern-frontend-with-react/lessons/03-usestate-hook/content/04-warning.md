---
type: "WARNING"
title: "Common Pitfalls"
---

Common useState mistakes:

1. **Mutating state directly**:
   ```jsx
   // WRONG!
   const [user, setUser] = useState({ name: 'Alice' });
   user.name = 'Bob';     // Don't mutate!
   setUser(user);         // React won't re-render
   
   // CORRECT!
   setUser({ ...user, name: 'Bob' });  // New object
   ```

2. **Forgetting to use setter**:
   ```jsx
   const [count, setCount] = useState(0);
   
   // WRONG!
   count = count + 1;     // Won't work!
   
   // CORRECT!
   setCount(count + 1);   // Use setter
   ```

3. **Using state immediately after setting**:
   ```jsx
   const [count, setCount] = useState(0);
   
   function increment() {
     setCount(count + 1);
     console.log(count);    // Still 0! (state updates are async)
   }
   
   // Use useEffect or callback to see new value:
   setCount(prevCount => {
     console.log('Will be:', prevCount + 1);
     return prevCount + 1;
   });
   ```

4. **Multiple setStates based on current state**:
   ```jsx
   // WRONG! (race condition)
   setCount(count + 1);
   setCount(count + 1);
   setCount(count + 1);
   // Count only increases by 1!
   
   // CORRECT!
   setCount(c => c + 1);
   setCount(c => c + 1);
   setCount(c => c + 1);
   // Count increases by 3!
   ```

5. **Mutating arrays**:
   ```jsx
   const [items, setItems] = useState([1, 2, 3]);
   
   // WRONG!
   items.push(4);         // Mutates!
   setItems(items);       // Won't re-render
   
   // CORRECT!
   setItems([...items, 4]);  // New array
   
   // Other array operations:
   setItems(items.filter(i => i !== 2));    // Remove
   setItems(items.map(i => i === 2 ? 5 : i)); // Update
   ```

6. **Forgetting initial value**:
   ```jsx
   // WRONG!
   const [count, setCount] = useState();  // undefined!
   
   // CORRECT!
   const [count, setCount] = useState(0);  // Start at 0
   ```

7. **Using if/loops in component body with useState**:
   ```jsx
   // WRONG! (hooks must be at top level)
   function MyComponent({ condition }) {
     if (condition) {
       const [state, setState] = useState(0);  // Error!
     }
   }
   
   // CORRECT!
   function MyComponent({ condition }) {
     const [state, setState] = useState(0);  // Always call
     
     if (condition) {
       // Use state here
     }
   }
   ```