 # User Testing
 
 ## Validation Surface
 
 This mission produces **report files** (JSON + Markdown), not a running application.
 There is no browser, CLI, or API surface to test interactively.
 
 ### Testing Approach
 The validation surface is the set of report files in `.planning/phases/python-content-recheck/`.
 Validation checks:
 1. **File existence:** Per-module JSON and Markdown files exist for all 24 modules
 2. **JSON schema compliance:** Each JSON file has required fields with correct types
 3. **Coverage completeness:** Total lessons in reports matches actual lesson directory count
 4. **OUTDATED issue quality:** Every OUTDATED-category issue has a webResearchNote
 5. **Consolidated summary:** Final summary file exists with aggregate statistics
 
 ### Tools
 - Shell commands (PowerShell on Windows) for file counting and JSON parsing
 - Python one-liners for JSON validation
 
 ## Validation Concurrency
 
 **Surface: File system checks**
 - Max concurrent validators: 5
 - Rationale: File reads are lightweight, no services to start, no memory-intensive operations
 - Each validator reads a subset of report files and checks assertions
