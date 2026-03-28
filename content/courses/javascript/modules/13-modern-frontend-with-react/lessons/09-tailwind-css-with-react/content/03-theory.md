---
type: "THEORY"
title: "Breaking Down the Syntax"
---

Tailwind class naming patterns:

1. **Color Scale** (50-950):
   ```jsx
   // Lighter → Darker
   bg-blue-50   // Very light blue
   bg-blue-100
   bg-blue-500  // Medium blue (default)
   bg-blue-900  // Very dark blue
   
   text-gray-700  // Dark gray text
   border-red-300  // Light red border
   ```

2. **Spacing Scale** (0-96):
   ```jsx
   // 1 unit = 0.25rem = 4px
   p-0   // 0px
   p-1   // 4px
   p-2   // 8px
   p-4   // 16px
   p-8   // 32px
   
   // Directional
   pt-4  // padding-top
   pr-4  // padding-right
   pb-4  // padding-bottom
   pl-4  // padding-left
   px-4  // padding left + right
   py-4  // padding top + bottom
   ```

3. **State Modifiers**:
   ```jsx
   hover:bg-blue-600    // On hover
   focus:ring-2         // On focus
   active:scale-95      // On click
   disabled:opacity-50  // When disabled
   group-hover:visible  // When parent hovered
   ```

4. **Responsive Prefixes**:
   ```jsx
   // Mobile-first: no prefix = all sizes
   w-full           // width: 100% on all
   md:w-1/2         // width: 50% on medium+
   lg:w-1/3         // width: 33% on large+
   
   hidden lg:block  // Hidden until large screens
   ```

5. **Flexbox Shortcuts**:
   ```jsx
   <div className="flex items-center justify-between gap-4">
   //              ^^^^  ^^^^^^^^^^^^^  ^^^^^^^^^^^^^^^  ^^^^^
   //              display  align-items  justify-content  gap
   ```

6. **Grid Shortcuts**:
   ```jsx
   <div className="grid grid-cols-3 gap-4">
   //              ^^^^  ^^^^^^^^^^  ^^^^^
   //              display  3 columns  gap
   ```

7. **Common Combinations**:
   ```jsx
   // Card
   "bg-white rounded-lg shadow-md p-6"
   
   // Button
   "bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
   
   // Input
   "border border-gray-300 rounded px-3 py-2 focus:ring-2"
   
   // Centered container
   "max-w-4xl mx-auto px-4"
   ```