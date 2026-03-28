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

### Current Mission: Content Audit (All 6 Courses)

**Scope:** Systematically review all ~800 lessons across 6 courses to identify and fix incomplete, inaccurate, outdated, or stub content.

**Courses:**
| Course | Modules | Est. Lessons | Target Versions |
|--------|---------|--------------|-----------------|
| Python | 24 | ~132 | Python 3.12, FastAPI 0.115, Django 5.1 |
| C# | 24 | ~120 | .NET 9, C# 13, EF Core 9 |
| JavaScript | 21 | ~126 | Node.js 22, React 19, Bun 1.3 |
| Flutter | 18 | ~126 | Flutter 3.38, Dart 3.10 |
| Kotlin | 15 | ~150 | Kotlin 2.3, Ktor 3.4, Compose MP 1.10 |
| Java | 16 | ~104 | Java 25, Spring Boot 4.0 |

**Milestone Sequence:**
1. python-audit: Audit all Python modules, then fix
2. csharp-audit: Audit all C# modules, then fix
3. javascript-audit: Audit all JavaScript modules, then fix
4. flutter-audit: Audit all Flutter modules, then fix
5. kotlin-audit: Audit all Kotlin modules, then fix
6. java-audit: Audit all Java modules, then fix

**Output:** Structured findings in `.planning/phases/{course}-content-audit/`

**Issue Categories:**
- STUB - Placeholder content (<50 words)
- OUTDATED - Deprecated APIs or old versions
- INACCURATE - Factually wrong information
- INCOMPLETE - Missing expected sections
- METADATA - Wrong difficulty, missing fields
- PEDAGOGY - Title/content mismatch

**Version Manifest:** See `content/version-manifest.json` for course target versions. Fixes must align with these targets (not necessarily absolute latest versions).

---

### Previous Mission: Python Content Recheck (Phase 07)

**Status:** Completed. Some gaps remain that will be addressed in current mission.

- 24 modules, ~166 lessons (Python)
- Covers basics through capstone (FastAPI, Django, PostgreSQL)
- Prior audit available in `.planning/phases/python-content-recheck/`
