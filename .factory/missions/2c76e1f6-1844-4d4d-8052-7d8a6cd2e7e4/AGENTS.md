# Mission: Content Audit - All 6 Courses

## Mission Boundaries (NEVER VIOLATE)

**Content Scope:**
- ONLY modify files in `content/courses/[course]/` directories
- DO NOT touch native-app-wpf/ code
- DO NOT touch native-app.Tests/ tests
- DO NOT modify build infrastructure (build scripts, CI config)

**Output Directories:**
- Python audit output: `.planning/phases/python-content-audit/`
- C# audit output: `.planning/phases/csharp-content-audit/`
- JavaScript audit output: `.planning/phases/javascript-content-audit/`
- Flutter audit output: `.planning/phases/flutter-content-audit/`
- Kotlin audit output: `.planning/phases/kotlin-content-audit/`
- Java audit output: `.planning/phases/java-content-audit/`

**Version Targets (MUST VERIFY FIXES AGAINST THESE):**
- Python: 3.12 (not necessarily latest 3.13+)
- C#: .NET 9 / C# 13
- JavaScript: Node.js 22, React 19, Bun 1.3.x
- Flutter: Flutter 3.38 / Dart 3.10
- Kotlin: Kotlin 2.3
- Java: Java 25 / Spring Boot 4.0

See `content/version-manifest.json` for complete version matrix.

**Off-Limits:**
- `/data` directory - do not read or modify
- `.factory/` infrastructure files (except creating output in `.planning/phases/`)
- Repository root configuration files (.editorconfig, .gitignore, etc.)

## Worker Guidance

### AUDIT Mode

When auditing modules:
1. Read EVERY lesson in assigned modules
2. Check lesson.json for required fields
3. Read ALL content/*.md files
4. Check for: stubs (<50 words), TODOs, version references, deprecated patterns
5. Use WebSearch ONLY for clearly versioned or deprecated items
6. Create findings JSON + Markdown summary for each module

**Issue Categories (use exactly):**
- `STUB` - Placeholder content (<50 words)
- `OUTDATED` - Deprecated APIs or old versions
- `INACCURATE` - Factually wrong information
- `INCOMPLETE` - Missing expected sections
- `METADATA` - Wrong difficulty, missing fields
- `PEDAGOGY` - Title/content mismatch

**Severity Levels:**
- `critical` - Lesson unusable
- `major` - Significant learning impairment
- `minor` - Quality issue but functional

### FIX Mode

When fixing issues:
1. Read findings JSON for assigned modules
2. Use WebSearch to verify current/correct information
3. Apply minimal, pedagogically appropriate fixes
4. Match course target versions (not necessarily latest)
5. Record all fixes in `fixesApplied` array
6. Verify modified files remain valid

### File Formats

**Findings JSON Schema:**
```json
{
  "course": "python",
  "module": "01-the-absolute-basics",
  "moduleTitle": "The Absolute Basics",
  "reviewDate": "2026-03-28",
  "totalLessons": 5,
  "lessonsReviewed": 5,
  "findings": [
    {
      "lessonPath": "content/courses/python/modules/...",
      "lessonTitle": "...",
      "qualityRating": "good|acceptable|needs-work|critical",
      "issues": [
        {
          "category": "OUTDATED",
          "severity": "major",
          "file": "content/01-theory.md",
          "line": 23,
          "originalText": "...",
          "description": "...",
          "webResearchNote": "..."
        }
      ],
      "fixesApplied": []
    }
  ],
  "summary": {
    "totalIssues": 1,
    "bySeverity": { "critical": 0, "major": 1, "minor": 0 },
    "byCategory": { "STUB": 0, "OUTDATED": 1, "INACCURATE": 0, "INCOMPLETE": 0, "METADATA": 0, "PEDAGOGY": 0 }
  }
}
```

**fixesApplied Schema:**
```json
{
  "fixesApplied": [
    {
      "issueCategory": "OUTDATED",
      "file": "content/01-theory.md",
      "line": 23,
      "change": "old → new",
      "verification": "Validated against Python 3.12 docs"
    }
  ]
}
```

## Testing & Validation

**No application testing required** - this is a content audit mission.

Validation is via:
1. JSON file existence and validity
2. Lesson count matches between findings and actual directories
3. OUTDATED issues have webResearchNote
4. fixesApplied entries are complete
5. Modified content files remain valid

**Validation Commands:**
```bash
# Count lessons per module
ls content/courses/python/modules/01-the-absolute-basics/lessons/ | wc -l

# Validate JSON syntax
python3 -c "import json; json.load(open('M01-findings.json'))"

# List all findings files
ls .planning/phases/python-content-audit/*-findings.json
```

## Milestone Sequence

1. **python-audit**: Audit all 24 Python modules (then fix)
2. **csharp-audit**: Audit all 24 C# modules
3. **javascript-audit**: Audit all 21 JavaScript modules
4. **flutter-audit**: Audit all 18 Flutter modules
5. **kotlin-audit**: Audit all 15 Kotlin modules
6. **java-audit**: Audit all 16 Java modules

Each milestone has:
- AUDIT features (module batches)
- FIX features (after audit complete)
- Auto-injected scrutiny-validator
- Auto-injected user-testing-validator

## Communication

**Return to orchestrator when:**
- Module directory structure is unexpected
- Web research reveals systemic issue affecting many lessons
- >50% of lessons in a batch have critical issues
- Fix requires pedagogical restructuring judgment

**In handoff, include:**
- Specific commands run and their output
- Count of lessons audited vs expected
- Count of issues by category/severity
- Any discovered systemic issues
