 # Architecture
 
 ## Repository Structure
 
 Code Tutor is a native C#/WPF desktop application for interactive programming education.
 
 ### Key Directories
 - `native-app-wpf/` - Main C#/.NET 8.0 WPF application (MVVM pattern)
 - `native-app.Tests/` - xUnit test project (E2E tests)
 - `content/courses/` - Course content (JSON + Markdown), 6 languages
 - `.planning/phases/` - Audit and planning artifacts
 
 ### Content Structure
 ```
 content/courses/{language}/
 ├── course.json              # Course metadata
 └── modules/
     └── {NN}-{module-slug}/
         ├── module.json       # Module metadata (if present)
         └── lessons/
             └── {NN}-{lesson-slug}/
                 ├── lesson.json       # Lesson metadata
                 ├── content/          # Markdown content files
                 │   ├── 01-theory.md
                 │   ├── 02-example.md
                 │   ├── 03-warning.md
                 │   ├── 04-analogy.md
                 │   └── 05-key_point.md
                 └── challenges/       # Optional challenge exercises
                     └── {challenge-slug}/
                         ├── challenge.json
                         ├── starter.{ext}
                         └── solution.{ext}
 ```
 
 ### Content File Types
 - `theory` - Core instructional content
 - `example` - Code examples with explanation
 - `warning` - Common pitfalls and mistakes
 - `analogy` - Real-world analogies for concepts
 - `key_point` - Summary takeaways
 - `legacy_comparison` - Comparison with older approaches
 - `experiment` - Hands-on exploration prompts
 
 ### Python Course Scope
 - 24 modules, ~166 lessons
 - Covers basics through capstone (FastAPI, Django, PostgreSQL)
 - Previously audited in phase 07 (broad pass, some gaps remain)
 
 ## Current Mission: Content Audit
 
 This mission produces a structured inventory of content issues.
 No code changes are made to the application or content files.
 Output goes to `.planning/phases/python-content-recheck/`.
