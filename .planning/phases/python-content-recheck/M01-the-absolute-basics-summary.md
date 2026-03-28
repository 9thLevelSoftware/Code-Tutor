# M01: The Absolute Basics — Audit Summary

**Module:** 01-the-absolute-basics  
**Lessons Reviewed:** 5 / 5  
**Total Issues Found:** 12  
**Overall Quality:** B (Good — some issues but mostly functional)

## Issue Breakdown

| Severity | Count |
|----------|-------|
| Critical | 1 |
| Major    | 2 |
| Minor    | 9 |

| Category   | Count |
|------------|-------|
| PEDAGOGY   | 5 |
| INACCURATE | 4 |
| INCOMPLETE | 2 |
| OUTDATED   | 1 |
| STUB       | 0 |
| METADATA   | 0 |

## Per-Lesson Ratings

| # | Lesson | Rating | Issues |
|---|--------|--------|--------|
| 1 | What is Programming, Really? | A | 1 minor |
| 2 | Your First Python Playground | B | 3 minor |
| 3 | Making the Computer Talk | B | 1 major, 2 minor |
| 4 | Listening to the User | C | 1 critical, 1 major, 1 minor |
| 5 | Mini-Project: A Conversation Program | A | 2 minor |

## Critical & Major Issues

### 🔴 Critical
1. **Lesson 04 — Incorrect f-string output example**: `02-example.md` shows literal `{city}` in expected output instead of the substituted value `Seattle`. Misleads learners about how f-strings work.

### 🟠 Major
2. **Lesson 03 — Title/content mismatch**: "Making the Computer Talk" implies output, but teaches input(). Lesson 04 "Listening to the User" implies input, but teaches f-strings (output). Titles appear swapped.
3. **Lesson 04 — Correct code marked as wrong**: `challenge.json` labels `print(f"Hello {name}")` as incorrect (❌), but this is perfectly valid Python.

## Key Observations

- **Metadata is complete and well-structured** across all 5 lessons. All have title, estimatedMinutes, difficulty, moduleId, and order fields.
- **Content quality is generally good** — rich analogies, clear code examples, progressive difficulty.
- **Challenge structure is consistent** — each lesson has one practice exercise with challenge.json, starter.py, and solution.py.
- **Main concern is Lessons 03-04** where titles don't match content and there's a factual error in the f-string example. These are the most important fixes for this module.
- **Minor HTML rendering concerns** — some lessons use inline HTML/CSS that may not render consistently across renderers.
