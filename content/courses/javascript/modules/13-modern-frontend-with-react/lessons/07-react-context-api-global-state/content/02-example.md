---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// React Context API - Global State

console.log('=== React Context API ===\n');

// THE PROBLEM: Prop Drilling
console.log('PROBLEM: Prop Drilling\n');
console.log('App → Layout → Header → UserMenu → UserName');
console.log('Pass user prop through EVERY component? Tedious!\n');

let propDrillingExample = `
// Without Context (prop drilling nightmare)
function App() {
  const [user, setUser] = useState({ name: 'Alice' });
  return <Layout user={user} />;  // Pass to Layout
}

function Layout({ user }) {
  return <Header user={user} />;  // Pass to Header
}

function Header({ user }) {
  return <UserMenu user={user} />;  // Pass to UserMenu
}

function UserMenu({ user }) {
  return <span>{user.name}</span>;  // Finally use it!
}
`;
console.log(propDrillingExample);

// THE SOLUTION: Context
console.log('\nSOLUTION: Context API\n');

// Simulating React Context
let UserContext = {
  _value: null,
  Provider: function(props) {
    this._value = props.value;
    console.log('[Provider] Broadcasting:', props.value);
    return props.children;
  }
};

function useContext(context) {
  console.log('[useContext] Receiving:', context._value);
  return context._value;
}

// Create and use context
console.log('// Step 1: Create Context');
console.log('const UserContext = createContext(null);\n');

console.log('// Step 2: Provide value at top level');
console.log('function App() {');
console.log('  const [user, setUser] = useState({ name: "Alice" });');
console.log('  return (');
console.log('    <UserContext.Provider value={{ user, setUser }}>');
console.log('      <Layout />  // No props needed!');
console.log('    </UserContext.Provider>');
console.log('  );');
console.log('}\n');

console.log('// Step 3: Consume anywhere with useContext');
console.log('function UserMenu() {');
console.log('  const { user } = useContext(UserContext);  // Direct access!');
console.log('  return <span>{user.name}</span>;');
console.log('}\n');

// Simulate the flow
console.log('=== Simulating Context Flow ===\n');

UserContext.Provider({ value: { name: 'Alice', role: 'admin' } });
let userData = useContext(UserContext);
console.log('UserMenu displays:', userData.name);

// Real-world example: Auth Context
console.log('\n\n=== Real Example: Auth Context ===\n');

let authContextCode = `
// contexts/AuthContext.jsx
import { createContext, useContext, useState } from 'react';

// 1. Create context
const AuthContext = createContext(null);

// 2. Create provider component
export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  const login = async (email, password) => {
    const res = await fetch('/api/login', {
      method: 'POST',
      body: JSON.stringify({ email, password })
    });
    const data = await res.json();
    setUser(data.user);
  };

  const logout = () => {
    setUser(null);
    // Clear cookies/tokens
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

// 3. Create custom hook for easy access
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
`;
console.log(authContextCode);

console.log('// Usage in App.jsx');
console.log('import { AuthProvider } from "./contexts/AuthContext";\n');
console.log('function App() {');
console.log('  return (');
console.log('    <AuthProvider>');
console.log('      <Router>');
console.log('        <Routes>...</Routes>');
console.log('      </Router>');
console.log('    </AuthProvider>');
console.log('  );');
console.log('}\n');

console.log('// Usage in any component');
console.log('function Navbar() {');
console.log('  const { user, logout } = useAuth();');
console.log('  return user ? (');
console.log('    <button onClick={logout}>Logout {user.name}</button>');
console.log('  ) : (');
console.log('    <Link to="/login">Login</Link>');
console.log('  );');
console.log('}');

// Theme Context example
console.log('\n\n=== Real Example: Theme Context ===\n');

let ThemeContext = {
  _value: { theme: 'light', toggleTheme: () => {} }
};

console.log('// contexts/ThemeContext.jsx');
console.log('export function ThemeProvider({ children }) {');
console.log('  const [theme, setTheme] = useState("light");');
console.log('');
console.log('  const toggleTheme = () => {');
console.log('    setTheme(prev => prev === "light" ? "dark" : "light");');
console.log('  };');
console.log('');
console.log('  return (');
console.log('    <ThemeContext.Provider value={{ theme, toggleTheme }}>');
console.log('      {children}');
console.log('    </ThemeContext.Provider>');
console.log('  );');
console.log('}\n');

console.log('// Any component can toggle theme!');
console.log('function ThemeToggle() {');
console.log('  const { theme, toggleTheme } = useTheme();');
console.log('  return (');
console.log('    <button onClick={toggleTheme}>');
console.log('      {theme === "light" ? "🌙" : "☀️"}');
console.log('    </button>');
console.log('  );');
console.log('}');
```
