// Solution: Troubleshooting and Debugging
// This challenge teaches you to use diagnostic commands and read errors.
//
// Terminal commands covered:
//
// 1. Detailed doctor output:
//    flutter doctor -v
//
// 2. Version check:
//    flutter --version
//
// 3. List devices:
//    flutter devices
//
// 4. Clean rebuild (useful for fixing issues):
//    flutter clean && flutter pub get
//
// Example of code WITH an error (missing semicolon):

// BROKEN CODE (will cause error):
// void main() {
//   print('Hello')  // <- Missing semicolon!
// }
//
// Error message you'll see:
// Error: Expected ';' after this.
//   print('Hello')
//                 ^

// FIXED CODE:
void main() {
  print('Hello');  // <- Semicolon added!
  
  // Common syntax errors and fixes:
  
  // 1. Missing semicolon
  // Error: Expected ';' after this
  // Fix: Add ; at end of statement
  
  // 2. Mismatched brackets
  // Error: Expected ')' or 'identifier'
  // Fix: Check all opening brackets have closing ones
  
  // 3. Undefined variable
  // Error: Undefined name 'variableName'
  // Fix: Declare the variable before using it
  
  // 4. Type mismatch
  // Error: A value of type 'X' can't be assigned to 'Y'
  // Fix: Use correct types or add type conversion
  
  print('Troubleshooting complete!');
  print('Remember: Error messages tell you WHAT is wrong and WHERE!');
}