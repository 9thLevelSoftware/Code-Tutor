---
type: "WARNING"
title: "Common Pitfalls"
---

Common event handling mistakes:

1. **Calling function immediately instead of passing reference**:
   ```jsx
   // WRONG! (calls immediately on render)
   <button onClick={handleClick()}>Click</button>
   
   // CORRECT! (passes function reference)
   <button onClick={handleClick}>Click</button>
   
   // Or use arrow function:
   <button onClick={() => handleClick()}>Click</button>
   ```

2. **Forgetting e.preventDefault() on forms**:
   ```jsx
   function handleSubmit(e) {
     // WRONG! (page reloads)
     console.log('Submitted');
     
     // CORRECT!
     e.preventDefault();  // Stop page reload
     console.log('Submitted');
   }
   ```

3. **Wrong event property for inputs**:
   ```jsx
   // Text input
   <input onChange={(e) => setValue(e.target.value)} />
   //                                   ^^^^^ value for text
   
   // Checkbox
   <input type="checkbox" onChange={(e) => setChecked(e.target.checked)} />
   //                                                   ^^^^^^^ checked for checkbox
   ```

4. **Not binding 'this' in class components**:
   ```jsx
   // Old class component issue (not relevant for function components)
   class MyComponent extends React.Component {
     handleClick() {
       this.setState(...);  // 'this' is undefined!
     }
     
     // Fix 1: Bind in constructor
     constructor() {
       this.handleClick = this.handleClick.bind(this);
     }
     
     // Fix 2: Arrow function
     handleClick = () => {
       this.setState(...);
     }
   }
   ```

5. **Trying to pass arguments incorrectly**:
   ```jsx
   // WRONG!
   <button onClick={handleClick(id)}>  // Calls immediately!
   
   // CORRECT!
   <button onClick={() => handleClick(id)}>Delete</button>
   
   // Or:
   <button onClick={handleClick.bind(null, id)}>Delete</button>
   ```

6. **Uncontrolled vs controlled inputs confusion**:
   ```jsx
   // Uncontrolled (React doesn't control value)
   <input defaultValue="Initial" />
   
   // Controlled (React controls value via state)
   const [text, setText] = useState('Initial');
   <input
     value={text}                          // Must have value
     onChange={(e) => setText(e.target.value)}  // Must have onChange
   />
   ```

7. **Multiple handlers without arrow functions**:
   ```jsx
   // WRONG! (both call immediately)
   <div
     onMouseEnter={handleEnter()}
     onMouseLeave={handleLeave()}
   >
   
   // CORRECT!
   <div
     onMouseEnter={handleEnter}
     onMouseLeave={handleLeave}
   >
   
   // Or with arguments:
   <div
     onMouseEnter={() => handleEnter(id)}
     onMouseLeave={() => handleLeave(id)}
   >
   ```