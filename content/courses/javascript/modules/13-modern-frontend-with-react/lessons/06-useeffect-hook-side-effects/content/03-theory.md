---
type: "THEORY"
title: "Breaking Down the Syntax"
---

useEffect Hook explained:

1. **Basic useEffect** (runs after every render):
   ```jsx
   import { useEffect } from 'react';
   
   function MyComponent() {
     useEffect(() => {
       console.log('Component rendered!');
     });
     // NO dependency array = runs after EVERY render
   }
   ```

2. **Run once on mount** (empty dependency array):
   ```jsx
   useEffect(() => {
     console.log('Component mounted!');
   }, []);  // Empty array = run ONCE on mount
   ```

3. **Run when specific values change**:
   ```jsx
   useEffect(() => {
     console.log('Count changed:', count);
   }, [count]);  // Runs when count changes
   ```

4. **Cleanup function** (returned from effect):
   ```jsx
   useEffect(() => {
     // Setup
     const timer = setInterval(() => {
       console.log('Tick');
     }, 1000);
     
     // Cleanup (runs before next effect OR on unmount)
     return () => {
       clearInterval(timer);
       console.log('Timer cleaned up');
     };
   }, []);
   ```

5. **Fetching data**:
   ```jsx
   useEffect(() => {
     async function fetchData() {
       const res = await fetch('/api/users');
       const data = await res.json();
       setUsers(data);
     }
     
     fetchData();
   }, []);  // Fetch once on mount
   ```

6. **Multiple dependencies**:
   ```jsx
   useEffect(() => {
     // Runs when userId OR filter changes
     fetchUserPosts(userId, filter);
   }, [userId, filter]);
   ```

7. **Event listeners**:
   ```jsx
   useEffect(() => {
     function handleScroll() {
       setScrollY(window.scrollY);
     }
     
     window.addEventListener('scroll', handleScroll);
     
     return () => {
       window.removeEventListener('scroll', handleScroll);
     };
   }, []);
   ```

8. **Local Storage**:
   ```jsx
   useEffect(() => {
     localStorage.setItem('user', JSON.stringify(user));
   }, [user]);  // Save when user changes
   ```

9. **Document Title**:
   ```jsx
   useEffect(() => {
     document.title = `Messages (${unreadCount})`;
   }, [unreadCount]);
   ```

10. **Conditional Effect**:
   ```jsx
   useEffect(() => {
     if (isLoggedIn) {
       // Only run if logged in
       connectToChat();
       
       return () => disconnectFromChat();
     }
   }, [isLoggedIn]);
   ```