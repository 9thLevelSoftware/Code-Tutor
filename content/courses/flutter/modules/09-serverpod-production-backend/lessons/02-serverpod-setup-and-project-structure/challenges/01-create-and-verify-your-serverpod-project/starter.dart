// SETUP VERIFICATION CHALLENGE
//
// Complete these steps and document your progress:
//
// Step 1: Install Serverpod CLI
// Command: dart pub global activate serverpod_cli
// Your output: _______________________
//
// Step 2: Verify installation
// Command: serverpod version
// Your output: _______________________
//
// Step 3: Create a new project called 'my_tasks'
// Command: serverpod create my_tasks
// Your output: _______________________
//
// Step 4: Start Docker services
// Commands:
//   cd my_tasks/my_tasks_server
//   docker compose up -d
// Verify: docker compose ps
// Your output: _______________________
//
// Step 5: Apply migrations and start server
// Command: dart run bin/main.dart --apply-migrations
// Your output: _______________________
//
// Step 6: Create this endpoint in my_tasks_server/lib/src/endpoints/
// Then run: serverpod generate
// Then restart the server

import 'package:serverpod/serverpod.dart';

class MathEndpoint extends Endpoint {
  // Calculate the sum of a list of numbers
  Future<int> sum(Session session, List<int> numbers) async {
    // TODO: Implement this method
    // Return the sum of all numbers in the list
    return 0;
  }
  
  // Calculate the average of a list of numbers
  Future<double> average(Session session, List<int> numbers) async {
    // TODO: Implement this method
    // Return the average (mean) of all numbers
    // Handle the case of an empty list
    return 0.0;
  }
  
  // Check if a number is prime
  Future<bool> isPrime(Session session, int number) async {
    // TODO: Implement this method
    // Return true if the number is prime, false otherwise
    return false;
  }
}

// After creating this endpoint:
// 1. Run: serverpod generate
// 2. Restart the server
// 3. Test with curl:
//    curl 'http://localhost:8080/math/sum?numbers=[1,2,3,4,5]'
//    Expected: {"data":15}
//
// Document any errors you encountered: