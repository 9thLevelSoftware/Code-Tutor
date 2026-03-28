# M02: Variables — Audit Summary

**Module:** 02-variables  
**Lessons Reviewed:** 5 / 5  
**Total Issues Found:** 10  
**Overall Quality:** B (Good — some issues in later lessons)

## Issue Breakdown

| Severity | Count |
|----------|-------|
| Critical | 0 |
| Major    | 3 |
| Minor    | 7 |

| Category   | Count |
|------------|-------|
| PEDAGOGY   | 3 |
| INACCURATE | 2 |
| INCOMPLETE | 2 |
| OUTDATED   | 1 |
| STUB       | 0 |
| METADATA   | 0 |

## Per-Lesson Ratings

| # | Lesson | Rating | Issues |
|---|--------|--------|--------|
| 1 | The Labeled Box (Variables Explained) | A | 1 minor |
| 2 | Different Kinds of Boxes (Data Types) | A | 1 minor |
| 3 | Converting Between Data Types | A | 2 minor |
| 4 | Box Math: Basic Operators | B | 1 major, 1 minor |
| 5 | Mini-Project: Simple Calculator | B | 2 major, 1 minor |

## Major Issues

### 🟠 Major
1. **Lesson 04 — Syntactically invalid starter.py**: Bare assignments (`hours = `, `minutes = `) are Python SyntaxErrors. Students cannot run the file before filling in their code.
2. **Lesson 05 — Syntactically invalid starter.py**: Same pattern — bare assignments (`name = `, `choice = `, `num1 = `, etc.) that prevent the file from running.
3. **Lesson 05 — Uses untaught concepts**: The mini-project extensively uses if/elif/else statements, but conditionals aren't taught until M03. This creates a dependency on concepts students haven't learned yet.

## Key Observations

- **First 3 lessons are excellent** (all rated A) — clear analogies (labeled boxes), well-structured content, accurate code examples.
- **Metadata is complete and well-structured** across all 5 lessons with appropriate difficulty labels.
- **Starter.py quality degrades in lessons 4-5** — bare assignments create syntax errors that prevent students from running or testing the scaffolding.
- **Content files are high quality** — no stubs, good code examples with expected output, comprehensive type explanations.
- **One outdated reference**: `current_year = 2025` in lesson 03's example code.
- **Prior audit overlap**: The bare-assignment pattern in starter files is a systemic issue that was not specifically flagged in the prior phase 07 audit. This is a recurring pattern worth checking across all modules.
