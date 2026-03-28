// 1. Form field types (discriminated union)
interface TextField {
  type: 'text';
  name: string;
  value: string;
  minLength?: number;
  maxLength?: number;
}

interface EmailField {
  type: 'email';
  name: string;
  value: string;
}

interface NumberField {
  type: 'number';
  name: string;
  value: number;
  min?: number;
  max?: number;
}

type FormField = TextField | EmailField | NumberField;

// 2. Validation result (discriminated union)
interface ValidResult {
  valid: true;
  fieldName: string;
}

interface InvalidResult {
  valid: false;
  fieldName: string;
  error: string;
}

type ValidationResult = ValidResult | InvalidResult;

// Helper: Email validation
function isValidEmail(email: string): boolean {
  return email.includes('@') && email.includes('.');
}

// 3. Validate single field
function validateField(field: FormField): ValidationResult {
  switch (field.type) {
    case 'text': {
      // TypeScript knows: field is TextField
      if (field.minLength !== undefined && field.value.length < field.minLength) {
        return {
          valid: false,
          fieldName: field.name,
          error: `${field.name} must be at least ${field.minLength} characters`
        };
      }
      if (field.maxLength !== undefined && field.value.length > field.maxLength) {
        return {
          valid: false,
          fieldName: field.name,
          error: `${field.name} must be at most ${field.maxLength} characters`
        };
      }
      return { valid: true, fieldName: field.name };
    }
    
    case 'email': {
      // TypeScript knows: field is EmailField
      if (!isValidEmail(field.value)) {
        return {
          valid: false,
          fieldName: field.name,
          error: `${field.name} must be a valid email address`
        };
      }
      return { valid: true, fieldName: field.name };
    }
    
    case 'number': {
      // TypeScript knows: field is NumberField
      if (field.min !== undefined && field.value < field.min) {
        return {
          valid: false,
          fieldName: field.name,
          error: `${field.name} must be at least ${field.min}`
        };
      }
      if (field.max !== undefined && field.value > field.max) {
        return {
          valid: false,
          fieldName: field.name,
          error: `${field.name} must be at most ${field.max}`
        };
      }
      return { valid: true, fieldName: field.name };
    }
  }
}

// 4. Validate entire form
function validateForm(fields: FormField[]): { isValid: boolean; errors: string[] } {
  let errors: string[] = [];
  
  for (let field of fields) {
    let result = validateField(field);
    
    if (!result.valid) {
      // TypeScript knows: result is InvalidResult
      errors.push(result.error);
    }
  }
  
  return {
    isValid: errors.length === 0,
    errors
  };
}

// 5. Test the validation
let formFields: FormField[] = [
  { type: 'text', name: 'username', value: 'alice', minLength: 3 },
  { type: 'email', name: 'email', value: 'alice@test.com' },
  { type: 'number', name: 'age', value: 25, min: 18, max: 120 },
  { type: 'text', name: 'bio', value: 'Hi', minLength: 10 },  // Too short!
  { type: 'email', name: 'backup', value: 'invalid-email' },  // Invalid!
  { type: 'number', name: 'score', value: 150, max: 100 }     // Too high!
];

let result = validateForm(formFields);
console.log('Form valid:', result.isValid);
// Form valid: false

console.log('Errors:', result.errors);
// Errors: [
//   'bio must be at least 10 characters',
//   'backup must be a valid email address',
//   'score must be at most 100'
// ]