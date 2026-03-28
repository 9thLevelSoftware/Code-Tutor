---
type: "THEORY"
title: "Breaking Down the Syntax"
---

Event handling explained:

1. **Basic onClick**:
   ```jsx
   function MyButton() {
     function handleClick() {
       console.log('Clicked!');
     }
     
     return <button onClick={handleClick}>Click Me</button>;
     //                      ^^^^^^^^^^^^
     //                      Function REFERENCE (no parentheses!)
   }
   ```

2. **Inline Arrow Function**:
   ```jsx
   <button onClick={() => console.log('Clicked!')}>Click</button>
   
   // With state update:
   <button onClick={() => setCount(count + 1)}>+1</button>
   ```

3. **Event Object** (e or event):
   ```jsx
   function handleClick(event) {
     console.log('Clicked element:', event.target);
     console.log('Click position:', event.clientX, event.clientY);
   }
   
   <button onClick={handleClick}>Click</button>
   ```

4. **Form Events** (onChange, onSubmit):
   ```jsx
   function Form() {
     const [name, setName] = useState('');
     
     function handleSubmit(e) {
       e.preventDefault();  // DON'T reload page!
       console.log('Submitted:', name);
     }
     
     return (
       <form onSubmit={handleSubmit}>
         <input
           value={name}
           onChange={(e) => setName(e.target.value)}
           //                        ^^^^^^^^^^^^^
           //                        Get input value
         />
         <button type="submit">Submit</button>
       </form>
     );
   }
   ```

5. **Passing Arguments** to event handlers:
   ```jsx
   // WRONG!
   <button onClick={handleClick(id)}>  // Calls immediately!
   
   // CORRECT!
   <button onClick={() => handleClick(id)}>  // Calls when clicked
   <button onClick={handleClick.bind(null, id)}>  // Alternative
   ```

6. **Multiple Events**:
   ```jsx
   <div
     onClick={handleClick}
     onMouseEnter={handleEnter}
     onMouseLeave={handleLeave}
   >
     Hover and click me!
   </div>
   ```

7. **Controlled Inputs** (input value tied to state):
   ```jsx
   const [text, setText] = useState('');
   
   <input
     value={text}              // Controlled by state
     onChange={(e) => setText(e.target.value)}
   />
   ```

8. **Checkbox Events**:
   ```jsx
   const [checked, setChecked] = useState(false);
   
   <input
     type="checkbox"
     checked={checked}
     onChange={(e) => setChecked(e.target.checked)}
     //                            ^^^^^^^^^^^^^
     //                            For checkboxes!
   />
   ```

9. **Keyboard Events**:
   ```jsx
   function handleKeyDown(e) {
     if (e.key === 'Enter') {
       submit();
     }
     if (e.key === 'Escape') {
       cancel();
     }
   }
   
   <input onKeyDown={handleKeyDown} />
   ```

10. **Prevent Default & Stop Propagation**:
   ```jsx
   function handleSubmit(e) {
     e.preventDefault();     // Don't submit form
     e.stopPropagation();    // Don't bubble up to parent
     // Your code here
   }
   ```