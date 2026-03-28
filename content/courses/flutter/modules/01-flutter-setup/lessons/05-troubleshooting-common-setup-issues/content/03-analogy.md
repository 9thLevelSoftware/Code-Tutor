---
type: "ANALOGY"
title: "Problem 1: \"flutter: command not found\""
---


### What it means:
Your computer doesn't know where Flutter is installed.

### Solution (Windows):


### Solution (Mac/Linux):




```bash
# Find where you installed Flutter
ls ~/flutter/bin/flutter

# Add to PATH (Mac with zsh)
echo 'export PATH="$PATH:$HOME/flutter/bin"' >> ~/.zshrc
source ~/.zshrc

# Or for bash
echo 'export PATH="$PATH:$HOME/flutter/bin"' >> ~/.bashrc
source ~/.bashrc

# Test
flutter --version
```
