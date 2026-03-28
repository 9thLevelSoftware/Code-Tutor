---
type: "WARNING"
title: "Common Pitfalls"
---

Common Tailwind mistakes:

1. **Not purging unused styles in production**:
   ```javascript
   // tailwind.config.js - MUST configure content
   module.exports = {
     content: [
       './src/**/*.{js,jsx,ts,tsx}',  // Scan these files
       './public/index.html'
     ],
   };
   // Without this: 3MB CSS file!
   // With this: ~10KB CSS file
   ```

2. **Dynamic class names don't work**:
   ```jsx
   // WRONG! Tailwind can't detect dynamic classes
   <div className={`bg-${color}-500`}>  // Won't work!
   
   // CORRECT! Use complete class names
   const colorClasses = {
     blue: 'bg-blue-500',
     red: 'bg-red-500',
     green: 'bg-green-500'
   };
   <div className={colorClasses[color]}>
   ```

3. **Class order doesn't matter (mostly)**:
   ```jsx
   // These are the same:
   "px-4 py-2 bg-blue-500"
   "bg-blue-500 py-2 px-4"
   
   // BUT: later classes don't override earlier ones!
   "text-red-500 text-blue-500"  // Both applied, browser decides
   ```

4. **Forgetting mobile-first**:
   ```jsx
   // WRONG thinking: "hide on large screens"
   <div className="lg:hidden">  // Hidden on lg+, visible on mobile
   
   // CORRECT thinking: mobile-first
   <div className="block lg:hidden">  // Same, but clearer intent
   <div className="hidden lg:block">  // Hidden mobile, visible lg+
   ```

5. **Long class strings (use extraction)**:
   ```jsx
   // MESSY
   <button className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 focus:ring-2 focus:ring-blue-300 disabled:opacity-50 transition-colors">
   
   // BETTER: Extract to variable or component
   const btnPrimary = 'px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600';
   <button className={btnPrimary}>
   
   // BEST: Use @apply in CSS (sparingly)
   .btn-primary {
     @apply px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600;
   }
   ```

6. **Not using the official plugins**:
   ```bash
   # Forms plugin - better form styling
   npm install @tailwindcss/forms
   
   # Typography plugin - prose styling
   npm install @tailwindcss/typography
   ```