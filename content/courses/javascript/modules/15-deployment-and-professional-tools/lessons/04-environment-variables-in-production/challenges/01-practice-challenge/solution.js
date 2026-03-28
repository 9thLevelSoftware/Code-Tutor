// Complete environment configuration system

class Config {
  constructor() {
    // Define required variables
    this.required = [
      'NODE_ENV',
      'DATABASE_URL',
      'JWT_SECRET'
    ];
    
    // Define optional variables with defaults
    this.defaults = {
      PORT: '3000',
      LOG_LEVEL: 'info',
      CORS_ORIGIN: 'http://localhost:5173',
      JWT_EXPIRES: '7d'
    };
    
    this.env = process.env;
  }
  
  validate() {
    console.log('🔍 Validating environment configuration...\n');
    
    const missing = [];
    const present = [];
    
    this.required.forEach(key => {
      if (!this.env[key]) {
        missing.push(key);
      } else {
        present.push(key);
      }
    });
    
    if (present.length > 0) {
      console.log('✅ Found required variables:');
      present.forEach(v => console.log(`  ✓ ${v}`));
      console.log('');
    }
    
    if (missing.length > 0) {
      console.log('❌ Missing required variables:');
      missing.forEach(v => console.log(`  ✗ ${v}`));
      console.log('\n⚠ Application cannot start without these variables!\n');
      return false;
    }
    
    console.log('✅ All required environment variables present!\n');
    return true;
  }
  
  get(key, fallback = null) {
    let value = this.env[key];
    
    // Use default if available
    if (!value && this.defaults[key]) {
      value = this.defaults[key];
    }
    
    // Use provided fallback
    if (!value && fallback !== null) {
      value = fallback;
    }
    
    return value;
  }
  
  getInt(key, fallback = 0) {
    const value = this.get(key, fallback.toString());
    return parseInt(value, 10);
  }
  
  getBool(key, fallback = false) {
    const value = this.get(key, fallback.toString());
    return value === 'true' || value === '1';
  }
  
  isProduction() {
    return this.get('NODE_ENV') === 'production';
  }
  
  isDevelopment() {
    return this.get('NODE_ENV') === 'development';
  }
  
  maskSecret(value) {
    if (!value || value === 'not set') return value;
    if (value.length < 8) return '***';
    return value.substring(0, 8) + '...[hidden]';
  }
  
  show() {
    console.log('┌─────────────────────────────────────┐');
    console.log('│     Environment Configuration        │');
    console.log('└─────────────────────────────────────┘\n');
    
    console.log('General:');
    console.log(`  NODE_ENV:     ${this.get('NODE_ENV')}`);
    console.log(`  PORT:         ${this.get('PORT')}`);
    console.log(`  LOG_LEVEL:    ${this.get('LOG_LEVEL')}\n`);
    
    console.log('Database:');
    console.log(`  DATABASE_URL: ${this.maskSecret(this.get('DATABASE_URL', 'not set'))}\n`);
    
    console.log('Security:');
    console.log(`  JWT_SECRET:   ${this.maskSecret(this.get('JWT_SECRET', 'not set'))}`);
    console.log(`  JWT_EXPIRES:  ${this.get('JWT_EXPIRES')}\n`);
    
    console.log('CORS:');
    console.log(`  CORS_ORIGIN:  ${this.get('CORS_ORIGIN')}\n`);
    
    if (this.isProduction()) {
      console.log('🚀 Running in PRODUCTION mode');
    } else {
      console.log('🔧 Running in DEVELOPMENT mode');
    }
    
    console.log('\n' + '═'.repeat(39) + '\n');
  }
  
  toObject() {
    return {
      env: this.get('NODE_ENV'),
      port: this.getInt('PORT'),
      logLevel: this.get('LOG_LEVEL'),
      
      database: {
        url: this.get('DATABASE_URL')
      },
      
      jwt: {
        secret: this.get('JWT_SECRET'),
        expiresIn: this.get('JWT_EXPIRES')
      },
      
      cors: {
        origin: this.get('CORS_ORIGIN')
      }
    };
  }
}

// Simulate different environments
console.log('=== Testing Environment Configurations ===\n');

// Test 1: Development
console.log('Test 1: Development Environment\n');
process.env.NODE_ENV = 'development';
process.env.PORT = '3000';
process.env.DATABASE_URL = 'postgres://localhost/myapp_dev';
process.env.JWT_SECRET = 'dev-secret-simple';

let config = new Config();
config.validate();
config.show();

// Test 2: Production
console.log('\nTest 2: Production Environment\n');
process.env.NODE_ENV = 'production';
process.env.PORT = '8080';
process.env.DATABASE_URL = 'postgres://prod.aws.com/myapp';
process.env.JWT_SECRET = 'super-complex-prod-secret-xyz789';
process.env.LOG_LEVEL = 'error';
process.env.CORS_ORIGIN = 'https://myapp.com';

config = new Config();
config.validate();
config.show();

// Test 3: Missing required variable
console.log('\nTest 3: Missing Required Variable\n');
delete process.env.JWT_SECRET;

config = new Config();
const isValid = config.validate();

if (!isValid) {
  console.log('Cannot start application. Please set missing variables.\n');
}

// Best practices guide
console.log('\n=== Environment Variable Best Practices ===\n');

const guide = [
  {
    do: '✓ Use .env for local development',
    dont: '✗ Commit .env to Git'
  },
  {
    do: '✓ Provide .env.example template',
    dont: '✗ Put real secrets in .env.example'
  },
  {
    do: '✓ Validate required vars on startup',
    dont: "✗ Let app crash with 'undefined' errors"
  },
  {
    do: '✓ Use descriptive variable names',
    dont: '✗ Use vague names like SECRET or KEY'
  },
  {
    do: '✓ Different secrets per environment',
    dont: '✗ Reuse dev secrets in production'
  },
  {
    do: '✓ Document all variables in README',
    dont: '✗ Leave developers guessing'
  }
];

guide.forEach(({ do: good, dont: bad }) => {
  console.log(good);
  console.log(bad);
  console.log('');
});