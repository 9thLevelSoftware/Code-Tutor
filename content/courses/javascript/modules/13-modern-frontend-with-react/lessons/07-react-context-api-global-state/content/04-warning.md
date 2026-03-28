---
type: "WARNING"
title: "Common Pitfalls"
---

Common Context mistakes:

1. **Using Context for everything** (overuse):
   ```jsx
   // WRONG! Don't use context for local state
   const ButtonContext = createContext();
   // Button click count doesn't need global state!
   
   // CORRECT! Only use for truly global state:
   // - Auth, Theme, Language, Shopping Cart
   ```

2. **Forgetting the Provider**:
   ```jsx
   // WRONG! useContext outside Provider returns undefined
   function App() {
     return <UserProfile />;  // No Provider!
   }
   
   function UserProfile() {
     const user = useContext(UserContext);  // undefined!
   }
   
   // CORRECT!
   function App() {
     return (
       <UserContext.Provider value={user}>
         <UserProfile />
       </UserContext.Provider>
     );
   }
   ```

3. **Not memoizing context value** (performance issue):
   ```jsx
   // WRONG! New object every render = all consumers re-render
   <MyContext.Provider value={{ user, setUser }}>
   
   // CORRECT! Memoize the value
   const value = useMemo(() => ({ user, setUser }), [user]);
   <MyContext.Provider value={value}>
   ```

4. **Putting too much in one context**:
   ```jsx
   // WRONG! Everything in one context
   const AppContext = createContext();
   // value = { user, theme, cart, language, notifications... }
   // Changing theme re-renders cart components!
   
   // CORRECT! Separate contexts
   <AuthProvider>
     <ThemeProvider>
       <CartProvider>
   ```

5. **Not creating custom hook** (error-prone):
   ```jsx
   // WRONG! Raw useContext everywhere
   const value = useContext(AuthContext);
   // No error if used outside provider!
   
   // CORRECT! Custom hook with error checking
   function useAuth() {
     const context = useContext(AuthContext);
     if (!context) {
       throw new Error('useAuth must be within AuthProvider');
     }
     return context;
   }
   ```

**When NOT to use Context**:
- State only used by 1-2 nearby components (just pass props)
- Frequently changing data (use state management like Zustand)
- Server state (use React Query or SWR instead)