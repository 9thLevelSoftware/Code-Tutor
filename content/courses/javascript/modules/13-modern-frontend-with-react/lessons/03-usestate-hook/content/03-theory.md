---
type: "THEORY"
title: "Breaking Down the Syntax"
---

useState Hook explained:

1. **Basic useState**:
   ```jsx
   import { useState } from 'react';
   
   function Counter() {
     const [count, setCount] = useState(0);
     //      ^^^^^  ^^^^^^^^          ^^^
     //      state  setter           initial value
     
     return (
       <div>
         <p>{count}</p>
         <button onClick={() => setCount(count + 1)}>+1</button>
       </div>
     );
   }
   ```

2. **Array Destructuring** (what [count, setCount] means):
   ```jsx
   // useState returns an array: [value, setter]
   const stateArray = useState(0);  // [0, function]
   const count = stateArray[0];     // Get value
   const setCount = stateArray[1];  // Get setter
   
   // Shorthand (array destructuring):
   const [count, setCount] = useState(0);  // Same thing!
   ```

3. **Initial Value**:
   ```jsx
   const [count, setCount] = useState(0);      // Number
   const [name, setName] = useState('Alice');  // String
   const [isOpen, setIsOpen] = useState(false); // Boolean
   const [items, setItems] = useState([]);     // Array
   const [user, setUser] = useState({ name: 'Alice' }); // Object
   ```

4. **Updating State**:
   ```jsx
   // Simple value
   setCount(5);           // Set to 5
   setCount(count + 1);   // Increment
   
   // Using previous value (safer)
   setCount(prevCount => prevCount + 1);
   ```

5. **Multiple State Variables**:
   ```jsx
   function Form() {
     const [name, setName] = useState('');
     const [email, setEmail] = useState('');
     const [age, setAge] = useState(0);
     
     // Each is independent
   }
   ```

6. **State with Objects** (must spread!):
   ```jsx
   const [user, setUser] = useState({ name: 'Alice', age: 25 });
   
   // WRONG!
   user.age = 26;         // Don't mutate!
   setUser(user);         // React won't detect change
   
   // CORRECT!
   setUser({ ...user, age: 26 });  // Create new object
   ```

7. **State with Arrays**:
   ```jsx
   const [items, setItems] = useState([1, 2, 3]);
   
   // Add item
   setItems([...items, 4]);
   setItems([newItem, ...items]);  // Add to beginning
   
   // Remove item
   setItems(items.filter((item, index) => index !== 0));
   
   // Update item
   setItems(items.map((item, i) => i === 1 ? newValue : item));
   ```

8. **Lazy Initial State** (expensive calculation):
   ```jsx
   // WRONG! (runs every render)
   const [data, setData] = useState(expensiveCalculation());
   
   // CORRECT! (runs once)
   const [data, setData] = useState(() => expensiveCalculation());
   ```