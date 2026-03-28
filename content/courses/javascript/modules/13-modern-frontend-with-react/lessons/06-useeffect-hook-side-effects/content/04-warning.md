---
type: "WARNING"
title: "Common Pitfalls"
---

Common useEffect mistakes:

1. **Forgetting dependency array**:
   ```jsx
   // WRONG! Runs after EVERY render
   useEffect(() => {
     fetchData();  // Infinite loop if it updates state!
   });
   
   // CORRECT!
   useEffect(() => {
     fetchData();
   }, []);  // Run once on mount
   ```

2. **Using async directly in useEffect**:
   ```jsx
   // WRONG!
   useEffect(async () => {  // Can't make effect async!
     const data = await fetch('/api/users');
   }, []);
   
   // CORRECT!
   useEffect(() => {
     async function fetchData() {
       const data = await fetch('/api/users');
     }
     fetchData();
   }, []);
   ```

3. **Missing dependencies**:
   ```jsx
   // WRONG! (missing 'count' in dependencies)
   useEffect(() => {
     console.log('Count is:', count);
   }, []);  // Should include [count]!
   
   // CORRECT!
   useEffect(() => {
     console.log('Count is:', count);
   }, [count]);  // Re-run when count changes
   ```

4. **Not cleaning up**:
   ```jsx
   // WRONG! (memory leak)
   useEffect(() => {
     const timer = setInterval(() => setCount(c => c + 1), 1000);
     // No cleanup!
   }, []);
   
   // CORRECT!
   useEffect(() => {
     const timer = setInterval(() => setCount(c => c + 1), 1000);
     return () => clearInterval(timer);  // Cleanup!
   }, []);
   ```

5. **Infinite loops**:
   ```jsx
   // WRONG! Infinite loop
   useEffect(() => {
     setCount(count + 1);  // Updates state → re-render → effect runs → updates state...
   }, [count]);  // Depends on count!
   
   // CORRECT! (if you really need this pattern)
   useEffect(() => {
     if (count < 10) {  // Add condition
       setCount(count + 1);
     }
   }, [count]);
   ```

6. **Stale closures**:
   ```jsx
   // WRONG! (captures old 'count')
   useEffect(() => {
     const timer = setInterval(() => {
       setCount(count + 1);  // Always uses initial count!
     }, 1000);
     return () => clearInterval(timer);
   }, []);  // Empty deps = count never updates
   
   // CORRECT!
   useEffect(() => {
     const timer = setInterval(() => {
       setCount(c => c + 1);  // Use functional update
     }, 1000);
     return () => clearInterval(timer);
   }, []);  // No dependency on count needed
   ```

7. **Multiple effects for unrelated logic** (good practice!):
   ```jsx
   // WRONG! (mixing concerns)
   useEffect(() => {
     fetchUsers();  // Unrelated
     document.title = 'Users';  // to
     window.addEventListener('resize', handleResize);  // each other
   }, []);
   
   // CORRECT! (separate effects)
   useEffect(() => {
     fetchUsers();
   }, []);
   
   useEffect(() => {
     document.title = 'Users';
   }, []);
   
   useEffect(() => {
     window.addEventListener('resize', handleResize);
     return () => window.removeEventListener('resize', handleResize);
   }, []);
   ```