---
type: "THEORY"
title: "Setting Up Supabase"
---


### Step 1: Create a Supabase Project

1. Go to https://supabase.com
2. Sign up (free tier is generous)
3. Click "New Project"
4. Choose organization, name, password, region
5. Wait 2 minutes for database provisioning

### Step 2: Get Your Credentials

In your Supabase dashboard:
1. Go to **Settings** > **API**
2. Copy:
   - **Project URL**: `https://xxxxx.supabase.co`
   - **anon/public key**: `eyJhbGciOi...`

### Step 3: Add to Flutter Project

```yaml
# pubspec.yaml
dependencies:
  supabase_flutter: ^2.3.0
```

Run: `flutter pub get`

### Step 4: Initialize Supabase

```dart
// lib/main.dart
import 'package:flutter/material.dart';
import 'package:supabase_flutter/supabase_flutter.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  await Supabase.initialize(
    url: 'https://YOUR_PROJECT.supabase.co',
    anonKey: 'YOUR_ANON_KEY',
  );
  
  runApp(const MyApp());
}

// Access client anywhere
final supabase = Supabase.instance.client;
```

