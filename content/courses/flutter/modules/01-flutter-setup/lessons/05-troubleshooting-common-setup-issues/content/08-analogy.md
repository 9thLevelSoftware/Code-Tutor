---
type: "ANALOGY"
title: "Problem 6: Hot Reload Doesn't Work"
---


### Symptoms:
- You save changes but nothing updates
- App needs full restart every time

### Solutions:

**1. Make sure you're editing the right file**
- Are you editing `lib/main.dart`?
- Not a file in `android/` or `ios/`?

**2. Try Hot Restart instead**
- Press `Ctrl/Cmd + Shift + F5`
- Or click the circular arrow icon

**3. Check for errors**
- Look at the terminal for error messages
- Fix any syntax errors

**4. Full restart**



```bash
# Stop the app
q (in terminal)

# Clean
flutter clean

# Run again
flutter run
```
