// Theme Context Simulation

// Simulating React Context
let ThemeContext = {
  _value: null,
  Provider: function(value, children) {
    this._value = value;
    return children;
  }
};

function useTheme() {
  return ThemeContext._value;
}

// Theme Provider
let themeState = { theme: 'light' };

function toggleTheme() {
  themeState.theme = themeState.theme === 'light' ? 'dark' : 'light';
  console.log('[Theme Changed]', themeState.theme);
}

// Set up provider
ThemeContext.Provider({ theme: themeState.theme, toggleTheme });

// Components using context
function ThemeToggle() {
  let { theme, toggleTheme } = useTheme();
  console.log(`[ThemeToggle] Current: ${theme}`);
  console.log(`[ThemeToggle] Button: ${theme === 'light' ? '🌙 Dark Mode' : '☀️ Light Mode'}`);
  return { toggle: toggleTheme };
}

function ThemedBox() {
  let { theme } = useTheme();
  let styles = theme === 'light' 
    ? { bg: 'white', text: 'black' }
    : { bg: '#1a1a1a', text: 'white' };
  console.log(`[ThemedBox] Background: ${styles.bg}, Text: ${styles.text}`);
}

// Test the context
console.log('=== Theme Context Demo ===\n');
console.log('Initial state:');
ThemedBox();

let toggle = ThemeToggle();
console.log('\nClicking toggle button...');
toggle.toggle();

// Update context with new state
ThemeContext.Provider({ theme: themeState.theme, toggleTheme });

console.log('\nAfter toggle:');
ThemedBox();