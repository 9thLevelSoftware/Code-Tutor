---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// Deploying React Frontend - Concepts

console.log('=== React Frontend Deployment ===\n');

// CONCEPT: Build Process
const buildProcess = {
  name: 'Vite Build',
  
  steps: [
    'Read all your React components',
    'Bundle JavaScript into optimized files',
    'Minify code (remove whitespace, shorten names)',
    'Optimize images and assets',
    'Generate index.html',
    'Output to dist/ folder'
  ],
  
  before: {
    files: ['src/App.jsx', 'src/components/*.jsx', 'src/main.jsx'],
    size: '2.5 MB (readable code)'
  },
  
  after: {
    files: ['dist/index.html', 'dist/assets/index-a3b4c5d6.js', 'dist/assets/index-e7f8g9h0.css'],
    size: '150 KB (minified and optimized!)'
  },
  
  run() {
    console.log('🔨 Building React app...\n');
    this.steps.forEach((step, i) => {
      console.log(`  ${i + 1}. ${step}`);
    });
    console.log('\n📦 Before build:');
    console.log(`  Files: ${this.before.files.join(', ')}`);
    console.log(`  Size: ${this.before.size}`);
    console.log('\n✅ After build:');
    console.log(`  Files: ${this.after.files.join(', ')}`);
    console.log(`  Size: ${this.after.size}`);
    console.log('\n✓ Build complete! Ready to deploy.');
  }
};

buildProcess.run();

// CONCEPT: Environment Variables
console.log('\n\n=== Environment Variables ===\n');

const environmentConfig = {
  development: {
    API_URL: 'http://localhost:3000',
    DEBUG: 'true'
  },
  
  production: {
    API_URL: 'https://my-api.onrender.com',
    DEBUG: 'false'
  },
  
  showConfig(env) {
    console.log(`${env.toUpperCase()} Environment:`);
    const config = this[env];
    for (let [key, value] of Object.entries(config)) {
      console.log(`  VITE_${key} = "${value}"`);
    }
  }
};

console.log('Local development:');
environmentConfig.showConfig('development');

console.log('\nProduction deployment:');
environmentConfig.showConfig('production');

console.log('\nIn your React code:');
console.log("const API_URL = import.meta.env.VITE_API_URL;");
console.log("fetch(`${API_URL}/api/users`);\n");

// DEPLOYMENT STEPS SIMULATION
console.log('\n=== Deployment Process (Vercel) ===\n');

const deploymentSteps = [
  {
    step: 1,
    title: 'Prepare Your React App',
    tasks: [
      'Update API URL to use environment variable',
      'Test build locally: npm run build',
      'Add .env to .gitignore',
      'Commit and push to GitHub'
    ]
  },
  {
    step: 2,
    title: 'Create Vercel Account',
    tasks: [
      'Go to vercel.com',
      'Sign up with GitHub',
      'Click "Add New" → "Project"'
    ]
  },
  {
    step: 3,
    title: 'Import Repository',
    tasks: [
      'Select your GitHub repo',
      'Framework Preset: Vite (auto-detected)',
      'Root Directory: ./ (or your frontend folder)',
      'Build Command: npm run build',
      'Output Directory: dist'
    ]
  },
  {
    step: 4,
    title: 'Configure Environment Variables',
    tasks: [
      'Click "Environment Variables"',
      'Add: VITE_API_URL = https://my-api.onrender.com',
      'Add any other VITE_ prefixed variables'
    ]
  },
  {
    step: 5,
    title: 'Deploy!',
    tasks: [
      'Click "Deploy"',
      'Wait for build (~1 minute)',
      'Get your URL: https://my-app.vercel.app',
      'Test: Open URL and check if it connects to API'
    ]
  },
  {
    step: 6,
    title: 'Update Backend CORS',
    tasks: [
      'Add your Vercel URL to backend CORS',
      "In Express: allowedOrigins.push('https://my-app.vercel.app')",
      'Redeploy backend',
      'Test frontend → backend connection'
    ]
  }
];

deploymentSteps.forEach(({ step, title, tasks }) => {
  console.log(`Step ${step}: ${title}`);
  tasks.forEach(task => console.log(`  - ${task}`));
  console.log('');
});

// SIMULATING DEPLOYMENT
console.log('=== Simulating Frontend Deployment ===\n');

const deployment = {
  platform: 'Vercel',
  project: 'my-react-app',
  url: 'https://my-react-app.vercel.app',
  status: 'Building',
  
  logs: [
    '[1/6] Cloning repository from GitHub...',
    '[2/6] Installing dependencies (npm install)...',
    '[3/6] Building project (npm run build)...',
    '  ✓ 1247 modules transformed',
    '  ✓ Built in 8.3s',
    '[4/6] Optimizing assets...',
    '  ✓ Images optimized: 12 files',
    '  ✓ JavaScript minified: 145 KB → 48 KB',
    '[5/6] Deploying to global CDN...',
    '[6/6] Assigning domains...',
    '',
    '✅ Deployment successful!',
    '🌐 https://my-react-app.vercel.app',
    '⚡ Served from 100+ edge locations worldwide'
  ],
  
  deploy() {
    console.log(`Deploying ${this.project} to ${this.platform}...\n`);
    this.logs.forEach(log => {
      console.log(log);
    });
    console.log(`\n✓ Live at: ${this.url}`);
    this.status = 'Live';
  },
  
  stats() {
    console.log('\n=== Deployment Stats ===\n');
    console.log('Status:', this.status);
    console.log('Platform:', this.platform);
    console.log('Build time: 8.3 seconds');
    console.log('Deploy time: 12 seconds');
    console.log('Total size: 48 KB (gzipped)');
    console.log('CDN locations: 100+');
    console.log('SSL: Enabled (HTTPS)');
    console.log('Auto-deploy: Enabled (push to main → auto deploy)');
  }
};

deployment.deploy();
deployment.stats();

// AUTO-DEPLOYMENT
console.log('\n\n=== Auto-Deployment Workflow ===\n');

const autoDeploySteps = [
  '1. You push code to GitHub (git push)',
  '2. Vercel detects the push',
  '3. Automatically runs build',
  '4. Deploys new version',
  '5. Updates live site',
  '',
  '⚡ Total time: ~1 minute from push to live!'
];

autoDeploySteps.forEach(step => console.log(step));
```
