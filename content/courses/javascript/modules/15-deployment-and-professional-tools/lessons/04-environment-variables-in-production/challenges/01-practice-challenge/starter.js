// Environment configuration manager

class Config {
  constructor() {
    this.required = ['DATABASE_URL', 'JWT_SECRET'];
  }
  
  validate() {
    const missing = this.required.filter(key => !process.env[key]);
    
    if (missing.length > 0) {
      console.log('❌ Missing required variables:');
      missing.forEach(v => console.log(`  - ${v}`));
      return false;
    }
    
    console.log('✅ All required variables present');
    return true;
  }
  
  get(key, fallback = null) {
    return process.env[key] || fallback;
  }
  
  maskSecret(value) {
    if (!value || value.length < 8) return '***';
    return value.substring(0, 8) + '...';
  }
  
  show() {
    console.log('\nConfiguration:');
    console.log(`  NODE_ENV: ${this.get('NODE_ENV', 'development')}`);
    console.log(`  PORT: ${this.get('PORT', '3000')}`);
    console.log(`  DATABASE: ${this.maskSecret(this.get('DATABASE_URL', 'not set'))}`);
    console.log(`  JWT_SECRET: ${this.maskSecret(this.get('JWT_SECRET', 'not set'))}`);
  }
}

// Test
process.env.NODE_ENV = 'production';
process.env.PORT = '8080';
process.env.DATABASE_URL = 'postgres://prod-db.com/myapp';
process.env.JWT_SECRET = 'super-secret-key-xyz';

const config = new Config();
config.validate();
config.show();