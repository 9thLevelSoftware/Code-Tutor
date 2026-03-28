---
type: "KEY_POINT"
title: "🛠️ Flutter DevTools - Your Advanced Debugging Suite"
---


Flutter comes with powerful debugging tools called **Flutter DevTools**. Think of it as X-ray vision for your app!

### What is DevTools?

DevTools is a browser-based suite of debugging and profiling tools built specifically for Flutter and Dart.

### Launching DevTools

**Option 1: From VS Code**
1. Run your app (`F5` or `flutter run`)
2. Open Command Palette (`Ctrl/Cmd + Shift + P`)
3. Type "Dart: Open DevTools"
4. Choose which tool to open

**Option 2: From Terminal**
```bash
# Run app and get observatory URL
flutter run

# In another terminal:
flutter pub global activate devtools
flutter pub global run devtools
```

**Option 3: From Browser**
When running `flutter run`, look for:
```
Flutter DevTools: http://127.0.0.1:9100?uri=...
```

### DevTools Tabs Explained

#### 1. 🔍 Widget Inspector (Most Important!)

See your entire widget tree visually:

```
🌳 Widget Tree View:

MaterialApp
 └─ Scaffold
     ├─ AppBar
     │   └─ Text: "My App"
     └─ Center
         └─ Column
             ├─ Text: "Hello"
             └─ ElevatedButton
```

**What you can do:**
- Click any widget to see its properties
- See padding, margins, and constraints
- Find layout issues (overflow, wrong sizes)
- Select widgets directly on the device/emulator

**Pro Tip**: Click the "Select Widget Mode" button, then tap any widget in your app to jump directly to it in the tree!

#### 2. 📊 Performance Overlay

Monitor your app's performance in real-time:

```
┌─────────────────────────────────┐
│ UI Thread:  ████░░░░  16ms    │ ← Should be under 16ms
│ GPU Thread: ██░░░░░░  8ms     │ ← Should be under 16ms
└─────────────────────────────────┘
```

**Enable from code:**
```dart
MaterialApp(
  showPerformanceOverlay: true,  // 👈 Add this!
  home: MyHomePage(),
)
```

**What to look for:**
- Green bars = Good (under 16ms = 60 FPS)
- Red bars = Bad (frame dropped, causes jank)
- Spikes = Potential performance issue

#### 3. 🧠 Memory Tab

Track memory usage and find leaks:

- See live memory graph
- Detect memory leaks
- Take heap snapshots
- Compare allocations

**Warning signs:**
- Memory constantly increasing = potential leak
- Spikes during specific actions = heavy operations

#### 4. 🌐 Network Tab

Monitor all HTTP requests:

```
Method | URL                    | Status | Time
-------+------------------------+--------+------
GET    | api.example.com/users  | 200    | 120ms
POST   | api.example.com/login  | 401    | 85ms ❌
GET    | api.example.com/posts  | 200    | 95ms
```

**What you can inspect:**
- Request headers and body
- Response data
- Timing (how long each request takes)
- Failed requests

#### 5. 🐛 Debugger

Set breakpoints and step through code:
- Pause at any line
- Inspect variables
- Step in/out/over functions
- Watch expressions

#### 6. 📝 Logging

View all logs from your app:

```dart
import 'dart:developer' as developer;

// These show in DevTools Logging tab
developer.log('User logged in', name: 'Auth');
developer.log('Fetched 10 items', name: 'API');
```

### Quick Debugging Shortcuts (VS Code)

| Shortcut | Action |
|----------|--------|
| `F5` | Start debugging |
| `F9` | Toggle breakpoint |
| `F10` | Step over |
| `F11` | Step into |
| `Shift+F11` | Step out |
| `Ctrl+Shift+D` | Open Debug panel |

### Debugging Layout Issues with Inspector

**Problem**: Your widget is in the wrong place or the wrong size.

**Solution**:
1. Open Widget Inspector
2. Click "Select Widget Mode" (crosshair icon)
3. Tap the problematic widget in your app
4. In DevTools, look at:
   - **Constraints**: What size was it told it can be?
   - **Size**: What size did it actually choose?
   - **Parent**: Who gave it those constraints?

```
Constraints: BoxConstraints(0.0<=w<=400.0, 0.0<=h<=600.0)
Actual Size: Size(200.0, 50.0)
Parent: Center → gives tight constraints from parent
```

### Performance Debugging Workflow

1. **Enable Performance Overlay**
2. **Use your app normally**
3. **Watch for red bars** (dropped frames)
4. **Open Timeline View** in DevTools
5. **Record** the problematic action
6. **Analyze** what's taking too long:
   - Build? → Too many widgets rebuilding
   - Layout? → Expensive layout calculations
   - Paint? → Complex graphics

**Common fixes:**
- Add `const` constructors
- Use `ListView.builder` instead of `ListView`
- Cache expensive calculations
- Use `RepaintBoundary` for complex animations

### DevTools Cheat Sheet

| I want to... | Use... |
|--------------|--------|
| Find why layout is wrong | Widget Inspector |
| Fix slow animations | Performance tab |
| Find memory leaks | Memory tab |
| Debug API calls | Network tab |
| Set breakpoints | Debugger tab |
| View logs | Logging tab |
| Profile CPU usage | CPU Profiler tab |

**Bookmark this**: https://docs.flutter.dev/tools/devtools

---

