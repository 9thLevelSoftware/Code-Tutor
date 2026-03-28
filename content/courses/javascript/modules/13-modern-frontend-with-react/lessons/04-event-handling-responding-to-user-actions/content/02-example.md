---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// React Event Handling

console.log('=== Event Handling in React ===\n');

// CONCEPT: Event Listeners
let button = {
  label: 'Click Me',
  clickCount: 0,
  
  // Event handler function
  handleClick: function() {
    this.clickCount++;
    console.log(`[Event] Button clicked! Total clicks: ${this.clickCount}`);
  }
};

// Simulate user clicking button
console.log('Button label:', button.label);
console.log('\nUser clicks button 3 times:\n');
button.handleClick();
button.handleClick();
button.handleClick();

// COMMON EVENT TYPES
console.log('\n=== Common React Events ===\n');

let events = {
  'onClick': 'Button clicks, div clicks, any element click',
  'onChange': 'Input field changes (text input, checkbox, select)',
  'onSubmit': 'Form submission',
  'onMouseEnter': 'Mouse cursor enters element',
  'onMouseLeave': 'Mouse cursor leaves element',
  'onKeyDown': 'Key pressed down',
  'onKeyUp': 'Key released',
  'onFocus': 'Element receives focus (clicked or tabbed to)',
  'onBlur': 'Element loses focus'
};

for (let [event, description] of Object.entries(events)) {
  console.log(`${event.padEnd(15)} - ${description}`);
}

// REACT SYNTAX
console.log('\n\n=== Event Handler Syntax ===\n');

console.log('// Method 1: Inline arrow function');
console.log('<button onClick={() => console.log("Clicked!")}>Click</button>\n');

console.log('// Method 2: Named function reference');
console.log('function handleClick() {');
console.log('  console.log("Clicked!");');
console.log('}');
console.log('<button onClick={handleClick}>Click</button>\n');

console.log('// Method 3: With event object');
console.log('function handleClick(event) {');
console.log('  console.log("Button:", event.target);');
console.log('}');
console.log('<button onClick={handleClick}>Click</button>\n');

// EVENT OBJECT
console.log('=== The Event Object ===\n');

let simulatedEvent = {
  target: { tagName: 'BUTTON', textContent: 'Click Me', value: '' },
  type: 'click',
  preventDefault: function() {
    console.log('[Event] Default action prevented');
  },
  stopPropagation: function() {
    console.log('[Event] Event propagation stopped');
  }
};

function handleEvent(event) {
  console.log('Event type:', event.type);
  console.log('Target element:', event.target.tagName);
  console.log('Button text:', event.target.textContent);
}

console.log('Simulating click event:\n');
handleEvent(simulatedEvent);

// FORM HANDLING
console.log('\n\n=== Form Event Handling ===\n');

let form = {
  state: { name: '', email: '' },
  
  handleNameChange: function(event) {
    this.state.name = event.target.value;
    console.log('[Input] Name:', this.state.name);
  },
  
  handleEmailChange: function(event) {
    this.state.email = event.target.value;
    console.log('[Input] Email:', this.state.email);
  },
  
  handleSubmit: function(event) {
    event.preventDefault();
    console.log('[Submit] Form data:', this.state);
  }
};

console.log('User types in name field:');
form.handleNameChange({ target: { value: 'Alice' } });
form.handleNameChange({ target: { value: 'Alice J' } });
form.handleNameChange({ target: { value: 'Alice Johnson' } });

console.log('\nUser types in email field:');
form.handleEmailChange({ target: { value: 'alice@example.com' } });

console.log('\nUser submits form:');
form.handleSubmit({ preventDefault: () => console.log('[Browser] Default submit prevented') });

// REAL REACT EXAMPLE
console.log('\n\n=== Complete React Example ===\n');

console.log('import { useState } from "react";\n');

console.log('function LoginForm() {');
console.log('  const [email, setEmail] = useState("");');
console.log('  const [password, setPassword] = useState("");\n');

console.log('  function handleSubmit(event) {');
console.log('    event.preventDefault();  // Don\'t reload page');
console.log('    console.log("Login:", email, password);');
console.log('  }\n');

console.log('  return (');
console.log('    <form onSubmit={handleSubmit}>');
console.log('      <input');
console.log('        type="email"');
console.log('        value={email}');
console.log('        onChange={(e) => setEmail(e.target.value)}');
console.log('      />');
console.log('      <input');
console.log('        type="password"');
console.log('        value={password}');
console.log('        onChange={(e) => setPassword(e.target.value)}');
console.log('      />');
console.log('      <button type="submit">Login</button>');
console.log('    </form>');
console.log('  );');
console.log('}');

// EVENT PATTERNS
console.log('\n\n=== Common Event Patterns ===\n');

let patterns = [
  {    pattern: 'Button Click',
    code: '<button onClick={handleClick}>Click</button>'
  },
  {
    pattern: 'Input Change',
    code: '<input onChange={(e) => setValue(e.target.value)} />'
  },
  {
    pattern: 'Form Submit',
    code: '<form onSubmit={handleSubmit}>...</form>'
  },
  {
    pattern: 'Checkbox Toggle',
    code: '<input type="checkbox" onChange={(e) => setChecked(e.target.checked)} />'
  },
  {
    pattern: 'Keyboard Event',
    code: '<input onKeyDown={(e) => e.key === "Enter" && submit()} />'
  },
  {
    pattern: 'Mouse Hover',
    code: '<div onMouseEnter={handleEnter} onMouseLeave={handleLeave}>'
  }
];

patterns.forEach(p => {
  console.log(`${p.pattern}:`);
  console.log(`  ${p.code}\n`);
});
```
