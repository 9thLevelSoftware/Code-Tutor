---
type: "THEORY"
title: "Installing the Serverpod CLI"
---


The Serverpod CLI is your main tool for creating projects, generating code, and running development servers.

**Installation Command:**

> **IMPORTANT:** `dart pub global activate serverpod_cli` installs the latest CLI version (currently 3.x). This course teaches Serverpod 2.x patterns. To follow along exactly, pin the version: `dart pub global activate serverpod_cli ^2.0.0`. Serverpod 3.0 introduced breaking changes in authentication (Relic replaces serverpod_auth) and web server components.

```bash
# Pin to 2.x to match this course:
dart pub global activate serverpod_cli ^2.0.0
```

This downloads and installs the Serverpod 2.x CLI globally on your system.

**Verify Installation:**

```bash
serverpod version
```

You should see output like:
```
Serverpod CLI version: 2.x.x
```

**If the command is not found**, your Dart global packages are not in your PATH. Add this to your shell configuration:

**macOS/Linux** (add to ~/.zshrc or ~/.bashrc):
```bash
export PATH="$PATH":"$HOME/.pub-cache/bin"
```

**Windows** (add to PATH environment variable):
```
%LOCALAPPDATA%\Pub\Cache\bin
```

Restart your terminal after making PATH changes.

