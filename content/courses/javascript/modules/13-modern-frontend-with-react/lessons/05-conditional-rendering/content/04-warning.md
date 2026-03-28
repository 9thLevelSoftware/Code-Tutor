---
type: "WARNING"
title: "Common Pitfalls"
---

Common conditional rendering mistakes:

1. **Using if statement inside JSX**:
   ```jsx
   // WRONG! Can't use if in JSX
   return (
     <div>
       {if (isLoggedIn) { <p>Welcome</p> }}  // Syntax error!
     </div>
   );
   
   // CORRECT! Use ternary
   return (
     <div>
       {isLoggedIn ? <p>Welcome</p> : <p>Login</p>}
     </div>
   );
   
   // Or if/else before return:
   if (isLoggedIn) {
     return <div><p>Welcome</p></div>;
   }
   return <div><p>Login</p></div>;
   ```

2. **Wrong use of && operator**:
   ```jsx
   // WRONG! Renders "0" when count is 0
   {count && <p>You have {count} items</p>}
   
   // CORRECT! Use explicit boolean
   {count > 0 && <p>You have {count} items</p>}
   ```

3. **Forgetting the else in ternary**:
   ```jsx
   // WRONG!
   {isLoggedIn ? <Dashboard /> }  // Syntax error!
   
   // CORRECT!
   {isLoggedIn ? <Dashboard /> : null}
   {isLoggedIn ? <Dashboard /> : <Login />}
   
   // Or use &&:
   {isLoggedIn && <Dashboard />}
   ```

4. **Nested ternaries (hard to read)**:
   ```jsx
   // WRONG! (too complex)
   {isLoading ? <Spinner /> : hasError ? <Error /> : hasData ? <Data /> : <Empty />}
   
   // BETTER! Use if/else before return
   if (isLoading) return <Spinner />;
   if (hasError) return <Error />;
   if (hasData) return <Data />;
   return <Empty />;
   ```

5. **Rendering undefined or false**:
   ```jsx
   // WRONG! Renders "false" or "undefined" as text
   <div>{isLoggedIn}</div>  // Renders "true" or "false"
   <div>{userData}</div>    // Might render "undefined"
   
   // CORRECT!
   <div>{isLoggedIn ? 'Yes' : 'No'}</div>
   <div>{userData?.name || 'N/A'}</div>
   ```

6. **String conditions** (always truthy!):
   ```jsx
   // WRONG! String "false" is truthy!
   const showBanner = "false";  // String, not boolean
   {showBanner && <Banner />}   // Always shows!
   
   // CORRECT!
   const showBanner = false;    // Boolean
   {showBanner && <Banner />}   // Works correctly
   ```

7. **Not handling all states** (loading, error, data):
   ```jsx
   // WRONG! Missing loading and error states
   function UserProfile({ user }) {
     return <div>{user.name}</div>;  // Crashes if user is null!
   }
   
   // CORRECT!
   function UserProfile({ user, loading, error }) {
     if (loading) return <Spinner />;
     if (error) return <Error message={error} />;
     if (!user) return <p>No user found</p>;
     return <div>{user.name}</div>;
   }
   ```