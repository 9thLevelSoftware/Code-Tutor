// Complete Theme Context System

console.log('═══════════════════════════════');
console.log('   Theme Context Demo');
console.log('═══════════════════════════════\n');

// Simulating React's createContext and useContext
function createContext(defaultValue) {
  return {
    _value: defaultValue,
    Provider: function(props) {
      this._value = props.value;
      console.log('[Context] Provider mounted with:', props.value.theme);
      if (props.children) props.children();
    }
  };
}

function useContext(context) {
  if (!context._value) {
    throw new Error('useContext must be used within a Provider!');
  }
  return context._value;
}

// 1. Create Theme Context
const ThemeContext = createContext({ theme: 'light', toggleTheme: () => {} });

// 2. Theme Provider (manages state)
let themeState = {
  theme: 'light',
  listeners: [],
  
  toggle() {
    this.theme = this.theme === 'light' ? 'dark' : 'light';
    console.log(`\n[State] Theme changed to: ${this.theme}`);
    this.notify();
  },
  
  subscribe(fn) {
    this.listeners.push(fn);
  },
  
  notify() {
    this.listeners.forEach(fn => fn(this.theme));
  }
};

function ThemeProvider(children) {
  const value = {
    theme: themeState.theme,
    toggleTheme: () => themeState.toggle()
  };
  
  ThemeContext.Provider({ value, children });
}

// 3. Custom Hook (best practice!)
function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within ThemeProvider');
  }
  return context;
}

// 4. Components using the context
function Header() {
  const { theme } = useTheme();
  console.log(`\n[Header] Rendering with ${theme} theme`);
  console.log(`  Logo: ${theme === 'light' ? '🌞 MyApp' : '🌙 MyApp'}`);
}

function ThemeToggleButton() {
  const { theme, toggleTheme } = useTheme();
  console.log(`\n[ThemeToggle] Button: ${theme === 'light' ? '🌙 Switch to Dark' : '☀️ Switch to Light'}`);
  return { onClick: toggleTheme };
}

function Card() {
  const { theme } = useTheme();
  const styles = {
    light: { bg: '#ffffff', text: '#000000', border: '#e0e0e0' },
    dark: { bg: '#2d2d2d', text: '#ffffff', border: '#404040' }
  };
  const s = styles[theme];
  console.log(`\n[Card] Styled with ${theme} theme`);
  console.log(`  Background: ${s.bg}`);
  console.log(`  Text: ${s.text}`);
  console.log(`  Border: ${s.border}`);
}

function Footer() {
  const { theme } = useTheme();
  console.log(`\n[Footer] © 2024 MyApp (${theme} mode)`);
}

// 5. App with all components
function App() {
  Header();
  Card();
  let toggleBtn = ThemeToggleButton();
  Footer();
  return { toggleBtn };
}

// Run the demo
console.log('─── Initial Render (Light Theme) ───');
let app;
ThemeProvider(() => {
  app = App();
});

console.log('\n─── User Clicks Theme Toggle ───');
themeState.toggle();

// Re-render with new theme
console.log('\n─── Re-render (Dark Theme) ───');
ThemeProvider(() => {
  app = App();
});

console.log('\n─── Toggle Again ───');
themeState.toggle();

ThemeProvider(() => {
  app = App();
});

console.log('\n\n═══════════════════════════════');
console.log('   Context API Benefits');
console.log('═══════════════════════════════');
console.log('✓ No prop drilling through intermediate components');
console.log('✓ Any component can access theme directly');
console.log('✓ Single source of truth for theme state');
console.log('✓ Easy to add new themed components');