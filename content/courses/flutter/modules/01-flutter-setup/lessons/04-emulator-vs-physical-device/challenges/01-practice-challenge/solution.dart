// Solution: Running on Different Devices
// This challenge is about testing your app on multiple platforms.
//
// Terminal commands to run your app:
//
// 1. List available devices:
//    flutter devices
//
// 2. Run on Chrome (web):
//    flutter run -d chrome
//
// 3. Run on Android emulator:
//    flutter run -d emulator-5554
//
// 4. Run on connected physical device:
//    flutter run -d <device-id>
//
// Here's a sample app that works on all platforms:

import 'package:flutter/material.dart';

void main() {
  runApp(const MultiPlatformApp());
}

class MultiPlatformApp extends StatelessWidget {
  const MultiPlatformApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Multi-Platform Demo',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.blue),
        useMaterial3: true,
      ),
      home: const HomePage(),
    );
  }
}

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Hello Flutter!'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.devices, size: 64),
            const SizedBox(height: 16),
            Text(
              'Running on Flutter!',
              style: Theme.of(context).textTheme.headlineMedium,
            ),
            const SizedBox(height: 8),
            const Text('This app works on Web, Android, iOS, and Desktop'),
          ],
        ),
      ),
    );
  }
}

// Key observations:
// - Chrome: Fastest to start, good for quick testing
// - Emulator: More accurate representation of mobile
// - Physical device: Best for real performance testing