// Profile Card with Tailwind CSS

console.log('=== Profile Card Component ===\n');

// Simulate the component structure with classes
let profileCard = {
  container: 'bg-white rounded-xl shadow-lg p-6 max-w-sm mx-auto',
  
  avatar: {
    wrapper: 'flex justify-center mb-4',
    image: 'w-24 h-24 rounded-full bg-gray-200'
  },
  
  info: {
    name: 'text-xl font-bold text-gray-800 text-center',
    title: 'text-gray-500 text-center mb-2',
    bio: 'text-gray-600 text-center text-sm'
  },
  
  buttons: {
    wrapper: 'flex gap-3 mt-6',
    follow: 'flex-1 bg-blue-500 text-white py-2 rounded-lg hover:bg-blue-600 transition-colors',
    message: 'flex-1 bg-gray-200 text-gray-800 py-2 rounded-lg hover:bg-gray-300 transition-colors'
  }
};

// Display the structure
console.log('CARD CONTAINER:');
console.log(`  className="${profileCard.container}"`);

console.log('\nAVATAR:');
console.log(`  wrapper: "${profileCard.avatar.wrapper}"`);
console.log(`  image: "${profileCard.avatar.image}"`);

console.log('\nUSER INFO:');
console.log(`  name: "${profileCard.info.name}"`);
console.log(`  title: "${profileCard.info.title}"`);
console.log(`  bio: "${profileCard.info.bio}"`);

console.log('\nBUTTONS:');
console.log(`  wrapper: "${profileCard.buttons.wrapper}"`);
console.log(`  follow: "${profileCard.buttons.follow}"`);
console.log(`  message: "${profileCard.buttons.message}"`);

// Render simulation
console.log('\n\n=== Rendered Output (Simulated) ===\n');
console.log('┌────────────────────────────────┐');
console.log('│                                │');
console.log('│         ┌──────────┐           │');
console.log('│         │  Avatar  │           │');
console.log('│         └──────────┘           │');
console.log('│                                │');
console.log('│         Sarah Johnson          │');
console.log('│      Product Designer          │');
console.log('│                                │');
console.log('│  Creating user-centered        │');
console.log('│  experiences for startups.     │');
console.log('│                                │');
console.log('│  [Follow]       [Message]      │');
console.log('│                                │');
console.log('└────────────────────────────────┘');