// Complete Profile Card with Tailwind CSS

console.log('═══════════════════════════════');
console.log('  Tailwind Profile Card Demo');
console.log('═══════════════════════════════\n');

// Define the component with all Tailwind classes
const ProfileCard = {
  // Main container
  container: [
    'bg-white',           // White background
    'rounded-xl',         // Large rounded corners
    'shadow-lg',          // Large shadow
    'p-8',                // Padding all sides
    'max-w-sm',           // Max width small
    'mx-auto',            // Center horizontally
    'hover:shadow-xl',    // Bigger shadow on hover
    'transition-shadow',  // Smooth transition
    'dark:bg-gray-800'    // Dark mode support
  ].join(' '),
  
  // Avatar section
  avatar: {
    wrapper: 'flex justify-center mb-6',
    image: [
      'w-28', 'h-28',           // Size
      'rounded-full',            // Circular
      'object-cover',            // Image fit
      'ring-4', 'ring-blue-100', // Border ring
      'dark:ring-blue-900'       // Dark mode ring
    ].join(' '),
    placeholder: 'w-28 h-28 rounded-full bg-gradient-to-br from-blue-400 to-purple-500 flex items-center justify-center text-white text-3xl font-bold'
  },
  
  // User information
  info: {
    name: 'text-2xl font-bold text-gray-800 text-center dark:text-white',
    title: 'text-blue-500 text-center font-medium mt-1',
    location: 'text-gray-400 text-sm text-center mt-1 flex items-center justify-center gap-1',
    bio: 'text-gray-600 text-center mt-4 leading-relaxed dark:text-gray-300'
  },
  
  // Stats section
  stats: {
    wrapper: 'flex justify-center gap-8 mt-6 pt-6 border-t border-gray-100 dark:border-gray-700',
    item: 'text-center',
    number: 'text-2xl font-bold text-gray-800 dark:text-white',
    label: 'text-xs text-gray-500 uppercase tracking-wide'
  },
  
  // Action buttons
  buttons: {
    wrapper: 'flex gap-3 mt-6',
    primary: [
      'flex-1',
      'bg-blue-500', 'text-white',
      'py-2.5', 'px-4',
      'rounded-lg',
      'font-medium',
      'hover:bg-blue-600',
      'active:scale-95',
      'transition-all',
      'flex', 'items-center', 'justify-center', 'gap-2'
    ].join(' '),
    secondary: [
      'flex-1',
      'bg-gray-100', 'text-gray-700',
      'py-2.5', 'px-4',
      'rounded-lg',
      'font-medium',
      'hover:bg-gray-200',
      'active:scale-95',
      'transition-all',
      'dark:bg-gray-700', 'dark:text-gray-200'
    ].join(' ')
  }
};

// Log the classes for each section
console.log('📦 CONTAINER');
console.log(`   ${ProfileCard.container}\n`);

console.log('🖼️  AVATAR');
console.log(`   Wrapper: ${ProfileCard.avatar.wrapper}`);
console.log(`   Image: ${ProfileCard.avatar.image}\n`);

console.log('ℹ️  USER INFO');
console.log(`   Name: ${ProfileCard.info.name}`);
console.log(`   Title: ${ProfileCard.info.title}`);
console.log(`   Bio: ${ProfileCard.info.bio}\n`);

console.log('📊 STATS');
console.log(`   Wrapper: ${ProfileCard.stats.wrapper}`);
console.log(`   Number: ${ProfileCard.stats.number}`);
console.log(`   Label: ${ProfileCard.stats.label}\n`);

console.log('🔘 BUTTONS');
console.log(`   Primary: ${ProfileCard.buttons.primary}`);
console.log(`   Secondary: ${ProfileCard.buttons.secondary}\n`);

// Visual representation
console.log('\n═══════════════════════════════');
console.log('     Visual Representation');
console.log('═══════════════════════════════\n');

console.log('┌─────────────────────────────────────┐');
console.log('│                                     │');
console.log('│            ╭─────────╮              │');
console.log('│            │   SJ    │              │');
console.log('│            ╰─────────╯              │');
console.log('│                                     │');
console.log('│          Sarah Johnson              │');
console.log('│         Product Designer            │');
console.log('│          📍 San Francisco           │');
console.log('│                                     │');
console.log('│    Creating beautiful, functional   │');
console.log('│    products that users love.        │');
console.log('│                                     │');
console.log('│  ─────────────────────────────────  │');
console.log('│                                     │');
console.log('│     234         89         12.4k    │');
console.log('│    POSTS      PROJECTS    FOLLOWERS │');
console.log('│                                     │');
console.log('│  ┌──────────┐  ┌──────────────────┐ │');
console.log('│  │  Follow  │  │     Message      │ │');
console.log('│  └──────────┘  └──────────────────┘ │');
console.log('│                                     │');
console.log('└─────────────────────────────────────┘');

console.log('\n\n═══════════════════════════════');
console.log('       Key Tailwind Classes');
console.log('═══════════════════════════════');
console.log('• Layout: flex, justify-center, items-center, gap-*');
console.log('• Spacing: p-*, m-*, mt-*, px-*');
console.log('• Colors: bg-*, text-*, ring-*');
console.log('• Typography: text-*, font-*');
console.log('• Borders: rounded-*, border-*');
console.log('• Effects: shadow-*, hover:*, transition-*');
console.log('• Dark Mode: dark:bg-*, dark:text-*');