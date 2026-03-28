---
type: "THEORY"
title: "Breaking Down the Syntax"
---

useEffect with fetch explained:

1. **Basic useEffect with fetch**:
   ```jsx
   import { useEffect, useState } from 'react';
   
   function UserList() {
     const [users, setUsers] = useState([]);
     
     useEffect(() => {
       fetch('http://localhost:4000/api/users')
         .then(res => res.json())
         .then(data => setUsers(data));
     }, []); // Empty array = run once on mount
     
     return <div>{users.map(u => <div key={u.id}>{u.name}</div>)}</div>;
   }
   ```

2. **With async/await** (recommended):
   ```jsx
   useEffect(() => {
     async function fetchUsers() {
       const response = await fetch('http://localhost:4000/api/users');
       const data = await response.json();
       setUsers(data);
     }
     
     fetchUsers();
   }, []);
   ```

3. **With loading and error states** (best practice):
   ```jsx
   const [users, setUsers] = useState([]);
   const [loading, setLoading] = useState(true);
   const [error, setError] = useState(null);
   
   useEffect(() => {
     async function fetchUsers() {
       try {
         setLoading(true);
         const res = await fetch('http://localhost:4000/api/users');
         
         if (!res.ok) {
           throw new Error(`HTTP ${res.status}`);
         }
         
         const data = await res.json();
         setUsers(data);
       } catch (err) {
         setError(err.message);
       } finally {
         setLoading(false);
       }
     }
     
     fetchUsers();
   }, []);
   
   if (loading) return <div>Loading...</div>;
   if (error) return <div>Error: {error}</div>;
   return <div>...</div>;
   ```

4. **Dependency array - fetch when value changes**:
   ```jsx
   const [userId, setUserId] = useState(1);
   const [user, setUser] = useState(null);
   
   useEffect(() => {
     fetch(`http://localhost:4000/api/users/${userId}`)
       .then(res => res.json())
       .then(data => setUser(data));
   }, [userId]); // Re-fetch when userId changes
   ```

5. **Cleanup function** (abort requests):
   ```jsx
   useEffect(() => {
     const controller = new AbortController();
     
     fetch('http://localhost:4000/api/users', {
       signal: controller.signal
     })
       .then(res => res.json())
       .then(data => setUsers(data))
       .catch(err => {
         if (err.name !== 'AbortError') {
           setError(err.message);
         }
       });
     
     // Cleanup: abort fetch if component unmounts
     return () => controller.abort();
   }, []);
   ```

6. **POST request in useEffect**:
   ```jsx
   useEffect(() => {
     async function createUser() {
       const res = await fetch('http://localhost:4000/api/users', {
         method: 'POST',
         headers: { 'Content-Type': 'application/json' },
         body: JSON.stringify({ name: 'Alice', email: 'alice@example.com' })
       });
       const newUser = await res.json();
       console.log('Created:', newUser);
     }
     
     createUser();
   }, []);
   ```