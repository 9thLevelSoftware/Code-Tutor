 # Environment
 
 Environment variables, external dependencies, and setup notes.
 
 **What belongs here:** Required env vars, external API keys/services, dependency quirks, platform-specific notes.
 **What does NOT belong here:** Service ports/commands (use `.factory/services.yaml`).
 
 ---
 
 ## Platform
 - Windows 10 (win32 10.0.26200)
 - Python 3.14.2 available
 - .NET 8.0 SDK installed
 - Git 2.52.0, gh CLI 2.81.0 (authenticated)
 
 ## External Dependencies
 - Web search access needed for verifying outdated content
 - No API keys or credentials required for this mission
 
 ## Notes
 - Content files use LF line endings (configured in .editorconfig)
 - Shell commands should use PowerShell syntax (Windows environment)
 - `head`, `tail`, `grep` not available natively; use PowerShell equivalents or Python
