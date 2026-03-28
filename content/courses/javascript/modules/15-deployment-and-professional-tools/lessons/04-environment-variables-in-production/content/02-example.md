---
type: "EXAMPLE"
title: "Code Example"
---

See the code example above demonstrating Code Example.

```javascript
// Environment Variables - Concepts

console.log('=== Environment Variables ===\n');

// CONCEPT: Separating Code from Configuration

// BAD: Hardcoded secrets (NEVER DO THIS!)
const badExample = {
  database: 'postgres://admin:super_secret_password@db.com/prod',
  jwtSecret: 'my-secret-key-12345',
  stripeKey: 'sk_live_abc123xyz',
  apiKey: 'secret-api-key-do-not-share'
};

console.log('❌ WRONG - Hardcoded Secrets:\n');
Object.entries(badExample).forEach(([key, value]) => {
  console.log(`const ${key} = '${value}';`);
});

console.log('\nProblems:');
const problems = [
  '✗ Secrets visible in Git history forever',
  '✗ Everyone with code access sees secrets',
  '✗ Different environments need different values',
  '✗ Changing secrets requires code changes',
  '✗ Accidental public repository = leaked secrets!'
];
problems.forEach(p => console.log(`  ${p}`));

// GOOD: Environment variables
console.log('\n\n✅ CORRECT - Environment Variables:\n');

const goodExample = {
  database: process.env.DATABASE_URL,
  jwtSecret: process.env.JWT_SECRET,
  stripeKey: process.env.STRIPE_SECRET_KEY,
  apiKey: process.env.API_KEY
};

Object.entries(goodExample).forEach(([key, value]) => {
  console.log(`const ${key} = process.env.${value.replace('process.env.', '')};`);
});

console.log('\nBenefits:');
const benefits = [
  '✓ Secrets never in code or Git',
  '✓ Different values per environment',
  '✓ Change secrets without code changes',
  '✓ Secure storage on hosting platforms',
  '✓ Easy to rotate credentials'
];
benefits.forEach(b => console.log(`  ${b}`));

// CONCEPT: Different Environments
console.log('\n\n=== Multi-Environment Setup ===\n');

const environments = {
  development: {
    NODE_ENV: 'development',
    DATABASE_URL: 'postgres://localhost/myapp_dev',
    API_URL: 'http://localhost:3000',
    JWT_SECRET: 'dev-secret-ok-to-be-simple',
    DEBUG: 'true',
    LOG_LEVEL: 'debug'
  },
  
  staging: {
    NODE_ENV: 'staging',
    DATABASE_URL: 'postgres://staging-db.internal/myapp_staging',
    API_URL: 'https://staging-api.myapp.com',
    JWT_SECRET: 'complex-staging-secret-xyz789',
    DEBUG: 'false',
    LOG_LEVEL: 'info'
  },
  
  production: {
    NODE_ENV: 'production',
    DATABASE_URL: 'postgres://prod-db.aws.com/myapp_prod',
    API_URL: 'https://api.myapp.com',
    JWT_SECRET: 'super-complex-production-secret-abc123xyz',
    DEBUG: 'false',
    LOG_LEVEL: 'error'
  }
};

Object.entries(environments).forEach(([env, vars]) => {
  console.log(`${env.toUpperCase()} Environment:`);
  Object.entries(vars).forEach(([key, value]) => {
    // Mask secrets
    let displayValue = value;
    if (key.includes('SECRET') || key.includes('PASSWORD')) {
      displayValue = value.substring(0, 8) + '...[hidden]';
    }
    console.log(`  ${key}=${displayValue}`);
  });
  console.log('');
});

// SIMULATING ENVIRONMENT VARIABLE USAGE
console.log('=== Using Environment Variables ===\n');

class AppConfig {
  constructor() {
    // Simulate process.env
    this.env = process.env;
    
    // Required variables
    this.requiredVars = [
      'DATABASE_URL',
      'JWT_SECRET',
      'NODE_ENV'
    ];
  }
  
  validate() {
    console.log('Validating environment variables...\n');
    
    const missing = [];
    
    this.requiredVars.forEach(varName => {
      if (!this.env[varName]) {
        missing.push(varName);
      }
    });
    
    if (missing.length > 0) {
      console.log('❌ Missing required environment variables:');
      missing.forEach(v => console.log(`  - ${v}`));
      console.log('\nApp cannot start without these variables!\n');
      return false;
    } else {
      console.log('✅ All required environment variables present\n');
      return true;
    }
  }
  
  get(key, fallback = null) {
    const value = this.env[key];
    if (!value && fallback !== null) {
      console.log(`⚠ ${key} not set, using fallback: ${fallback}`);
      return fallback;
    }
    return value;
  }
  
  showConfig() {
    console.log('Current Configuration:');
    console.log(`  Environment: ${this.get('NODE_ENV', 'development')}`);
    console.log(`  Database: ${this.maskSecret(this.get('DATABASE_URL', 'Not set'))}`);
    console.log(`  JWT Secret: ${this.maskSecret(this.get('JWT_SECRET', 'Not set'))}`);
    console.log(`  Port: ${this.get('PORT', '3000')}`);
    console.log(`  Debug: ${this.get('DEBUG', 'false')}\n`);
  }
  
  maskSecret(value) {
    if (!value || value === 'Not set') return value;
    if (value.length < 10) return '***';
    return value.substring(0, 10) + '...[hidden]';
  }
}

// Test with development environment
process.env.NODE_ENV = 'development';
process.env.DATABASE_URL = 'postgres://localhost/myapp_dev';
process.env.JWT_SECRET = 'dev-secret';
process.env.PORT = '3000';

const config = new AppConfig();
config.validate();
config.showConfig();

// BEST PRACTICES
console.log('=== Environment Variable Best Practices ===\n');

const bestPractices = [
  {
    rule: '1. Never commit secrets to Git',
    example: 'Add .env to .gitignore immediately!'
  },
  {
    rule: '2. Use descriptive names',
    example: 'DATABASE_URL (good) vs DB (bad)'
  },
  {
    rule: '3. Provide .env.example',
    example: 'Template showing what vars are needed (no real values!)'
  },
  {
    rule: '4. Validate on startup',
    example: 'Crash early if required vars missing'
  },
  {
    rule: '5. Use different values per environment',
    example: 'dev-simple-secret vs prod-complex-secret-xyz789'
  },
  {
    rule: '6. Rotate secrets regularly',
    example: 'Change JWT_SECRET every few months'
  },
  {
    rule: '7. Document all variables',
    example: 'README lists all env vars and what they do'
  }
];

bestPractices.forEach(({ rule, example }) => {
  console.log(rule);
  console.log(`   → ${example}\n`);
});

// COMMON VARIABLES
console.log('=== Common Environment Variables ===\n');

const commonVars = {
  'NODE_ENV': 'development | production | test',
  'PORT': 'Server port (3000, 8080, etc.)',
  'DATABASE_URL': 'Full database connection string',
  'JWT_SECRET': 'Secret key for signing tokens',
  'API_KEY': 'Third-party API keys',
  'STRIPE_SECRET_KEY': 'Payment processing key',
  'AWS_ACCESS_KEY_ID': 'AWS credentials',
  'SMTP_HOST': 'Email server settings',
  'CORS_ORIGIN': 'Allowed frontend URLs',
  'LOG_LEVEL': 'debug | info | warn | error'
};

Object.entries(commonVars).forEach(([name, description]) => {
  console.log(`${name.padEnd(20)} - ${description}`);
});
```
