// SETUP VERIFICATION CHALLENGE - SOLUTION
//
// Complete these steps and document your progress:
//
// Step 1: Install Serverpod CLI
// Command: dart pub global activate serverpod_cli
// Output: Activated serverpod_cli 2.x.x
//
// Step 2: Verify installation
// Command: serverpod version
// Output: Serverpod CLI version: 2.x.x
//
// Step 3: Create a new project called 'my_tasks'
// Command: serverpod create my_tasks
// Output: Creating project my_tasks... (takes ~1 minute)
//
// Step 4: Start Docker services
// Commands:
//   cd my_tasks/my_tasks_server
//   docker compose up -d
// Verify: docker compose ps
// Output: postgres and redis containers running
//
// Step 5: Apply migrations and start server
// Command: dart run bin/main.dart --apply-migrations
// Output: Serverpod is running.

import 'package:serverpod/serverpod.dart';

class MathEndpoint extends Endpoint {
  // Calculate the sum of a list of numbers
  Future<int> sum(Session session, List<int> numbers) async {
    if (numbers.isEmpty) return 0;
    
    int total = 0;
    for (final number in numbers) {
      total += number;
    }
    return total;
    
    // Alternative using fold:
    // return numbers.fold(0, (sum, n) => sum + n);
  }
  
  // Calculate the average of a list of numbers
  Future<double> average(Session session, List<int> numbers) async {
    if (numbers.isEmpty) return 0.0;
    
    int total = 0;
    for (final number in numbers) {
      total += number;
    }
    return total / numbers.length;
    
    // Alternative:
    // return numbers.fold(0, (sum, n) => sum + n) / numbers.length;
  }
  
  // Check if a number is prime
  Future<bool> isPrime(Session session, int number) async {
    // Numbers less than 2 are not prime
    if (number < 2) return false;
    
    // 2 is the only even prime
    if (number == 2) return true;
    
    // Even numbers > 2 are not prime
    if (number % 2 == 0) return false;
    
    // Check odd divisors up to square root
    for (int i = 3; i * i <= number; i += 2) {
      if (number % i == 0) return false;
    }
    
    return true;
  }
}

// Testing commands:
// curl 'http://localhost:8080/math/sum?numbers=[1,2,3,4,5]'
// Expected: {"data":15}
//
// curl 'http://localhost:8080/math/average?numbers=[10,20,30]'
// Expected: {"data":20.0}
//
// curl 'http://localhost:8080/math/isPrime?number=17'
// Expected: {"data":true}
//
// curl 'http://localhost:8080/math/isPrime?number=18'
// Expected: {"data":false}