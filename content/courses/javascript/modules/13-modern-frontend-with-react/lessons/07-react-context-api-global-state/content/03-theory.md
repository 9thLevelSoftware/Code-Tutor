---
type: "THEORY"
title: "Breaking Down the Syntax"
---

Context API step by step:

1. **Create Context**:
   ```jsx
   import { createContext } from 'react';
   
   const MyContext = createContext(defaultValue);
   // defaultValue used when no Provider found
   ```

2. **Create Provider Component**:
   ```jsx
   export function MyProvider({ children }) {
     const [state, setState] = useState(initialValue);
     
     return (
       <MyContext.Provider value={{ state, setState }}>
         {children}
       </MyContext.Provider>
     );
   }
   ```

3. **Create Custom Hook** (best practice!):
   ```jsx
   export function useMyContext() {
     const context = useContext(MyContext);
     if (!context) {
       throw new Error('useMyContext must be within MyProvider');
     }
     return context;
   }
   ```

4. **Wrap App with Provider**:
   ```jsx
   function App() {
     return (
       <MyProvider>
         <RestOfApp />
       </MyProvider>
     );
   }
   ```

5. **Use in Any Component**:
   ```jsx
   function DeepNestedComponent() {
     const { state, setState } = useMyContext();
     // Access state directly - no props needed!
   }
   ```

**Common Context Use Cases**:
- **AuthContext**: User login state, login/logout functions
- **ThemeContext**: Dark/light mode, toggle function
- **CartContext**: Shopping cart items, add/remove functions
- **LanguageContext**: Current language, translation function

**Multiple Contexts** (nest providers):
```jsx
function App() {
  return (
    <AuthProvider>
      <ThemeProvider>
        <CartProvider>
          <Router>...</Router>
        </CartProvider>
      </ThemeProvider>
    </AuthProvider>
  );
}
```