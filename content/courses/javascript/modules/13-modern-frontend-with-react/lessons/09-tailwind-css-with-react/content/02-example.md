---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// Tailwind CSS with React

console.log('=== Tailwind CSS Fundamentals ===\n');

// Setup: npm install -D tailwindcss postcss autoprefixer
//        npx tailwindcss init -p

// 1. BASIC CLASSES
console.log('1. Basic Utility Classes\n');

let basicClasses = {
  spacing: {
    'p-4': 'padding: 1rem (16px)',
    'm-2': 'margin: 0.5rem (8px)',
    'px-6': 'padding-left/right: 1.5rem',
    'my-auto': 'margin-top/bottom: auto',
    'space-x-4': 'horizontal spacing between children'
  },
  colors: {
    'bg-blue-500': 'background-color: blue (medium)',
    'text-white': 'color: white',
    'border-gray-300': 'border-color: gray',
    'bg-gradient-to-r': 'linear gradient to right'
  },
  typography: {
    'text-xl': 'font-size: 1.25rem',
    'font-bold': 'font-weight: 700',
    'text-center': 'text-align: center',
    'uppercase': 'text-transform: uppercase'
  },
  layout: {
    'flex': 'display: flex',
    'grid': 'display: grid',
    'items-center': 'align-items: center',
    'justify-between': 'justify-content: space-between'
  }
};

for (let [category, classes] of Object.entries(basicClasses)) {
  console.log(`${category.toUpperCase()}:`);
  for (let [className, meaning] of Object.entries(classes)) {
    console.log(`  ${className.padEnd(20)} → ${meaning}`);
  }
  console.log('');
}

// 2. COMPONENT EXAMPLES
console.log('\n2. React Component Examples\n');

let buttonExample = `
// Button Component with Tailwind
function Button({ children, variant = 'primary' }) {
  const baseClasses = 'px-4 py-2 rounded font-medium transition-colors';
  
  const variants = {
    primary: 'bg-blue-500 text-white hover:bg-blue-600',
    secondary: 'bg-gray-200 text-gray-800 hover:bg-gray-300',
    danger: 'bg-red-500 text-white hover:bg-red-600'
  };
  
  return (
    <button className={\`\${baseClasses} \${variants[variant]}\`}>
      {children}
    </button>
  );
}

// Usage
<Button>Save</Button>
<Button variant="danger">Delete</Button>
`;
console.log(buttonExample);

let cardExample = `
// Card Component
function Card({ title, children }) {
  return (
    <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
      <h2 className="text-xl font-bold text-gray-800 mb-4">{title}</h2>
      <div className="text-gray-600">{children}</div>
    </div>
  );
}
`;
console.log(cardExample);

// 3. RESPONSIVE DESIGN
console.log('\n3. Responsive Design (Mobile-First)\n');

let responsiveExample = `
// Responsive breakpoints:
// sm:  640px and up
// md:  768px and up
// lg:  1024px and up
// xl:  1280px and up
// 2xl: 1536px and up

// Mobile: 1 column, Tablet: 2 columns, Desktop: 4 columns
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
  <Card>Item 1</Card>
  <Card>Item 2</Card>
  <Card>Item 3</Card>
  <Card>Item 4</Card>
</div>

// Hide on mobile, show on desktop
<nav className="hidden lg:flex">
  <Links />
</nav>

// Different padding at different sizes
<div className="p-4 md:p-6 lg:p-8">
  Content
</div>
`;
console.log(responsiveExample);

// 4. DARK MODE
console.log('\n4. Dark Mode Support\n');

let darkModeExample = `
// tailwind.config.js
module.exports = {
  darkMode: 'class',  // or 'media' for system preference
};

// Component with dark mode
<div className="bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
  <h1 className="text-gray-800 dark:text-gray-100">
    Supports both themes!
  </h1>
</div>

// Toggle dark mode
<html className="dark">  // Add/remove 'dark' class
`;
console.log(darkModeExample);

// 5. COMMON PATTERNS
console.log('\n5. Common UI Patterns\n');

let patterns = `
// Centered Container
<div className="max-w-4xl mx-auto px-4">
  Centered content
</div>

// Flex Row with Spacing
<div className="flex items-center justify-between gap-4">
  <Logo />
  <Nav />
</div>

// Form Input
<input className="w-full px-4 py-2 border border-gray-300 rounded-lg 
                  focus:ring-2 focus:ring-blue-500 focus:border-transparent
                  placeholder-gray-400" />

// Avatar
<img className="w-10 h-10 rounded-full object-cover" />

// Badge
<span className="px-2 py-1 text-xs font-medium bg-green-100 text-green-800 rounded-full">
  Active
</span>

// Loading Spinner
<div className="animate-spin h-8 w-8 border-4 border-blue-500 
                border-t-transparent rounded-full" />
`;
console.log(patterns);

console.log('\n=== Tailwind Setup Commands ===\n');
console.log('npm install -D tailwindcss postcss autoprefixer');
console.log('npx tailwindcss init -p');
console.log('');
console.log('// Add to tailwind.config.js');
console.log('content: ["./src/**/*.{js,jsx,ts,tsx}"]');
console.log('');
console.log('// Add to src/index.css');
console.log('@tailwind base;');
console.log('@tailwind components;');
console.log('@tailwind utilities;');
```
