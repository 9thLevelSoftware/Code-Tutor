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
  // Use switch on field.type
  // text: check minLength/maxLength if specified
  // email: check valid email format
  // number: check min/max if specified
}

// 4. Validate entire form
function validateForm(fields: FormField[]): { isValid: boolean; errors: string[] } {
  // Process each field and collect errors
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
console.log('Errors:', result.errors);