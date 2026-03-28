---
type: "WARNING"
title: "Common Pitfalls"
---

Common useEffect + fetch mistakes:

1. **Forgetting dependency array**:
   ```jsx
   // WRONG! Infinite loop!
   useEffect(() => {
     fetch('/api/users')
       .then(res => res.json())
       .then(data => setUsers(data)); // State update triggers re-render
   }); // No dependency array → runs every render → infinite loop!
   
   // CORRECT!
   useEffect(() => {
     fetch('/api/users')
       .then(res => res.json())
       .then(data => setUsers(data));
   }, []); // Empty array → run once
   ```

2. **Using async directly in useEffect**:
   ```jsx
   // WRONG!
   useEffect(async () => {  // ← Can't do this!
     const res = await fetch('/api/users');
   }, []);
   
   // CORRECT!
   useEffect(() => {
     async function fetchData() {  // Define async function inside
       const res = await fetch('/api/users');
     }
     fetchData();  // Call it
   }, []);
   ```

3. **Not handling loading/error states**:
   ```jsx
   // WRONG! (bad UX)
   const [users, setUsers] = useState([]);
   useEffect(() => {
     fetch('/api/users')
       .then(res => res.json())
       .then(data => setUsers(data));
   }, []);
   return <div>{users.map(...)}</div>; // Empty while loading!
   
   // CORRECT!
   const [users, setUsers] = useState([]);
   const [loading, setLoading] = useState(true);
   
   useEffect(() => {
     fetch('/api/users')
       .then(res => res.json())
       .then(data => {
         setUsers(data);
         setLoading(false);
       });
   }, []);
   
   if (loading) return <div>Loading...</div>;
   return <div>{users.map(...)}</div>;
   ```

4. **Not checking response.ok**:
   ```jsx
   // WRONG! (doesn't catch HTTP errors)
   const res = await fetch('/api/users');
   const data = await res.json(); // Fails if 404/500!
   
   // CORRECT!
   const res = await fetch('/api/users');
   if (!res.ok) {
     throw new Error(`HTTP ${res.status}: ${res.statusText}`);
   }
   const data = await res.json();
   ```

5. **Missing cleanup (memory leaks)**:
   ```jsx
   // WRONG! (can cause "Can't perform state update on unmounted component")
   useEffect(() => {
     fetch('/api/users')
       .then(res => res.json())
       .then(data => setUsers(data)); // Component might unmount before this!
   }, []);
   
   // CORRECT!
   useEffect(() => {
     let cancelled = false;
     
     fetch('/api/users')
       .then(res => res.json())
       .then(data => {
         if (!cancelled) {  // Only update if still mounted
           setUsers(data);
         }
       });
     
     return () => {
       cancelled = true;  // Cleanup
     };
   }, []);
   ```

6. **Wrong dependencies**:
   ```jsx
   function UserProfile({ userId }) {
     const [user, setUser] = useState(null);
     
     // WRONG! (doesn't re-fetch when userId changes)
     useEffect(() => {
       fetch(`/api/users/${userId}`)
         .then(res => res.json())
         .then(data => setUser(data));
     }, []); // Should include userId!
     
     // CORRECT!
     useEffect(() => {
       fetch(`/api/users/${userId}`)
         .then(res => res.json())
         .then(data => setUser(data));
     }, [userId]); // Re-fetch when userId changes
   }
   ```