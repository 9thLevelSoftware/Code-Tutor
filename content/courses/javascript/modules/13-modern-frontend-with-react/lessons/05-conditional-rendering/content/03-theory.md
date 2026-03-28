---
type: "THEORY"
title: "Breaking Down the Syntax"
---

Conditional rendering patterns:

1. **Ternary Operator** (? :):
   ```jsx
   function Greeting({ isLoggedIn }) {
     return (
       <div>
         {isLoggedIn ? (
           <h1>Welcome back!</h1>
         ) : (
           <h1>Please log in</h1>
         )}
       </div>
     );
   }
   ```

2. **Logical AND** (&&):
   ```jsx
   function Notifications({ count }) {
     return (
       <div>
         <h2>Dashboard</h2>
         {count > 0 && <p>You have {count} notifications</p>}
       </div>
     );
   }
   // Shows paragraph ONLY if count > 0
   ```

3. **Early Return**:
   ```jsx
   function UserProfile({ user }) {
     if (!user) {
       return <p>Loading...</p>;
     }
     
     if (user.error) {
       return <p>Error: {user.error}</p>;
     }
     
     return (
       <div>
         <h1>{user.name}</h1>
         <p>{user.email}</p>
       </div>
     );
   }
   ```

4. **Variable Assignment**:
   ```jsx
   function Dashboard({ isLoggedIn }) {
     let content;
     
     if (isLoggedIn) {
       content = <UserDashboard />;
     } else {
       content = <LoginPrompt />;
     }
     
     return <div>{content}</div>;
   }
   ```

5. **Null for Hiding**:
   ```jsx
   function Alert({ message, show }) {
     if (!show) return null;  // Render nothing
     
     return <div className="alert">{message}</div>;
   }
   ```

6. **Conditional CSS Classes**:
   ```jsx
   function Button({ isActive }) {
     return (
       <button className={isActive ? 'btn-active' : 'btn-inactive'}>
         {isActive ? 'Active' : 'Inactive'}
       </button>
     );
   }
   ```

7. **Multiple Conditions (Switch)**:
   ```jsx
   function StatusDisplay({ status }) {
     switch(status) {
       case 'loading':
         return <Spinner />;
       case 'error':
         return <ErrorMessage />;
       case 'success':
         return <SuccessMessage />;
       default:
         return null;
     }
   }
   ```

8. **Conditional Props**:
   ```jsx
   <button
     className={isActive ? 'active' : 'inactive'}
     disabled={isLoading}
     style={{ color: hasError ? 'red' : 'black' }}
   >
     {isLoading ? 'Loading...' : 'Submit'}
   </button>
   ```

9. **List Filtering**:
   ```jsx
   function TodoList({ todos, filter }) {
     const filteredTodos = filter === 'completed'
       ? todos.filter(t => t.completed)
       : filter === 'active'
       ? todos.filter(t => !t.completed)
       : todos;
     
     return (
       <ul>
         {filteredTodos.map(todo => <li key={todo.id}>{todo.text}</li>)}
       </ul>
     );
   }
   ```