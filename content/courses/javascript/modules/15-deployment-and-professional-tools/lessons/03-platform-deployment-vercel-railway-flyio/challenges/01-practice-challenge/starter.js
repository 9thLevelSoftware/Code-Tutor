// React app deployment simulation

const frontendApp = {
  name: 'My React App',
  
  env: {
    development: {
      API_URL: 'http://localhost:3000',
      NODE_ENV: 'development'
    },
    production: {
      API_URL: 'https://my-api.onrender.com',
      NODE_ENV: 'production'
    }
  },
  
  build(environment) {
    console.log(`\n🔨 Building for ${environment}...\n`);
    
    const config = this.env[environment];
    console.log('Environment variables:');
    console.log(`  VITE_API_URL=${config.API_URL}`);
    console.log(`  NODE_ENV=${config.NODE_ENV}\n`);
    
    console.log('Build steps:');
    console.log('  1. Bundling React components');
    console.log('  2. Minifying JavaScript');
    console.log('  3. Optimizing assets');
    console.log('  4. Generating index.html\n');
    
    if (environment === 'production') {
      console.log('✓ Production optimizations applied');
      console.log('  - Code minified');
      console.log('  - Source maps removed');
      console.log('  - Tree-shaking applied\n');
    }
    
    console.log('✅ Build complete!');
    console.log(`   Output: dist/`);
    console.log(`   Ready to deploy to Vercel\n`);
  }
};

// Test
frontendApp.build('development');
frontendApp.build('production');