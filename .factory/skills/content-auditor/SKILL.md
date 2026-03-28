---
name: content-auditor
model: openai/gpt-5
location: personal
description: Audits course lesson content for completeness, accuracy, and currentness, then fixes identified issues through web research.
---

# Content Auditor

NOTE: Startup and cleanup are handled by `worker-base`. This skill defines the WORK PROCEDURE.

## When to Use This Skill

Use for features that:
1. **AUDIT mode**: Audit batches of course modules, reading every lesson and producing structured findings in JSON and Markdown format.
2. **FIX mode**: Fix specific content issues identified in prior audits using web research.

## Required Skills

- `WebSearch` - Use to verify outdated technology versions, deprecated APIs, and current best practices.

## Work Procedure

### READ THE FEATURE DESCRIPTION CAREFULLY

The feature description will specify:
- **Course**: Which course to work on (python, javascript, csharp, java, kotlin, flutter)
- **Mode**: AUDIT or FIX
- **Scope**: Which modules or specific issues to address

---

## AUDIT MODE PROCEDURE

### Step 1: Identify Assigned Modules

Read the feature description to determine which modules to audit. List all lesson directories for those modules under `content/courses/{course}/modules/`.

### Step 2: Audit Each Lesson Systematically

For EVERY lesson in the assigned modules, perform these checks:

**a) Metadata Check (lesson.json)**
- Read `lesson.json`
- Verify fields exist: `id`, `title`, `moduleId`, `order`, `estimatedMinutes`, `difficulty`
- Check difficulty makes sense for content
- Flag missing fields as `METADATA` issues

**b) Content Files Check (content/*.md)**
- Read ALL markdown files in `content/`
- Check for stubs: fewer than 50 words, "TODO", "TBD", "placeholder", "coming soon"
- Check for missing expected sections: theory AND example minimum
- Check code blocks for issues: missing imports, syntax errors, undefined variables
- **Check version references**: Note ANY specific versions ("Python 3.10", "FastAPI 0.95", "Spring Boot 3.x") - these need verification
- **Check for deprecated patterns**: `datetime.utcnow()`, `asyncio.get_event_loop()`, old syntax patterns
- Check title/content alignment

**c) Challenge Check (challenges/)**
- Verify `challenge.json`, `starter.*`, `solution.*` all exist and are non-empty
- Flag missing/empty files as `INCOMPLETE`

**d) Web Research (FOR OUTDATED ITEMS)**
- When you find version references or deprecated patterns, use WebSearch to verify current status
- Record findings in `webResearchNote`
- Be CONSERVATIVE - only research items that are clearly versioned or known-deprecated patterns

### Step 3: Produce Per-Module Findings

Create two files in `.planning/phases/{course}-content-audit/`:

**JSON findings file** (`M{NN}-{module-slug}-findings.json`):
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
      "lessonPath": "content/courses/python/modules/01-the-absolute-basics/lessons/01-what-is-programming",
      "lessonTitle": "What Is Programming",
      "qualityRating": "good",
      "issues": [],
      "fixesApplied": []
    },
    {
      "lessonPath": "content/courses/python/modules/01-the-absolute-basics/lessons/02-first-playground",
      "lessonTitle": "First Playground",
      "qualityRating": "needs-work",
      "issues": [
        {
          "category": "OUTDATED",
          "severity": "major",
          "file": "content/01-theory.md",
          "line": 23,
          "originalText": "datetime.utcnow()",
          "description": "Uses deprecated datetime.utcnow() instead of datetime.now(timezone.utc)",
          "webResearchNote": "Python 3.12 deprecated datetime.utcnow(); use datetime.now(timezone.utc) per PEP 696"
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

**Markdown summary file** (`M{NN}-{module-slug}-summary.md`):
```markdown
# Module 01: The Absolute Basics - Audit Summary

**Course:** python | **Reviewed:** 5/5 lessons | **Issues:** 1 (0 critical, 1 major, 0 minor)

## Findings

### Lesson 01: What Is Programming
**Rating:** good | **Issues:** 0

### Lesson 02: First Playground  
**Rating:** needs-work | **Issues:** 1
- [OUTDATED/major] content/01-theory.md:23 - Uses deprecated datetime.utcnow() instead of datetime.now(timezone.utc)
```

### Step 4: Quality Ratings

- `good` - No issues or only trivial ones
- `acceptable` - Minor issues that don't impair learning
- `needs-work` - Has issues that should be fixed
- `critical` - Lesson unusable or significantly misleading

### Step 5: Issue Categories

| Category | Description |
|----------|-------------|
| `STUB` | Placeholder or minimal content (<50 words) |
| `OUTDATED` | Version references or deprecated APIs - MUST have webResearchNote |
| `INACCURATE` | Factually wrong code or explanations |
| `INCOMPLETE` | Missing expected sections or partial content |
| `METADATA` | Wrong difficulty, missing fields |
| `PEDAGOGY` | Title/content mismatch, missing analogies |

---

## FIX MODE PROCEDURE

### Step 1: Read Findings and Identify Fixes

Read the findings JSON file(s) specified in the feature. Identify which issues have:
- Clear fix path (version bump, syntax update, import fix)
- Web research already done (webResearchNote present)
- File location and line number specified

### Step 2: Research Current State

For each OUTDATED issue:
- Use WebSearch to find CURRENT version/API information
- Verify the fix approach is correct for the course's target versions (see `content/version-manifest.json`)
- Note: Course targets specific versions (e.g., Python 3.12, not necessarily latest)

### Step 3: Apply Fixes

**Direct file edits** - Use Edit tool to:
1. Update version references to correct current versions
2. Replace deprecated code patterns with modern equivalents
3. Add missing imports or fix syntax errors
4. Expand stub content with appropriate material (use WebSearch for content)

**Fix must be**:
- Accurate for the course's target version
- Pedagogically appropriate (don't over-complicate for beginners)
- Minimal change (preserve style and structure)

### Step 4: Record Fixes

Update the findings JSON with `fixesApplied` array:
```json
{
  "fixesApplied": [
    {
      "issueCategory": "OUTDATED",
      "file": "content/01-theory.md",
      "line": 23,
      "change": "datetime.utcnow() → datetime.now(timezone.utc)",
      "verification": "Pattern validated against Python 3.12 docs"
    }
  ]
}
```

### Step 5: Verify Fixes

- Re-read modified files to confirm changes applied correctly
- Ensure JSON remains valid after edits
- Count fixes applied per module

---

## Example Handoff (AUDIT Mode)

```json
{
  "salientSummary": "Audited 22 lessons across M01-M04 of python course. Found 8 issues (0 critical, 3 major, 5 minor). Key findings: M01 L02 has Python version mismatch, M03 has 2 lessons with missing imports, M04 L03 has outdated loop pattern. Produced 4 JSON findings files and 4 Markdown summaries to .planning/phases/python-content-audit/.",
  "whatWasImplemented": "Created audit reports for modules 01-the-absolute-basics (5 lessons, 1 issue), 02-variables (5 lessons, 2 issues), 03-boolean-logic (7 lessons, 3 issues), 04-loops (5 lessons, 2 issues). Each module has findings.json and summary.md.",
  "whatWasLeftUndone": "Issues identified require FIX mode features to resolve. No content files were modified in audit mode.",
  "verification": {
    "commandsRun": [
      {
        "command": "ls .planning/phases/python-content-audit/M0*",
        "exitCode": 0,
        "observation": "8 files present: 4 JSON + 4 MD for M01-M04"
      },
      {
        "command": "python3 -c \"import json, glob; [json.load(open(f)) for f in glob.glob('.planning/phases/python-content-audit/*.json')]\"",
        "exitCode": 0,
        "observation": "All JSON files parse successfully, 8 total issues recorded"
      }
    ],
    "interactiveChecks": []
  },
  "tests": {
    "added": []
  },
  "discoveredIssues": [
    {
      "severity": "info",
      "description": "M03 L05 challenge solution.py uses pattern differing from lesson - noted but not flagged as it's pedagogical choice"
    }
  ]
}
```

## Example Handoff (FIX Mode)

```json
{
  "salientSummary": "Fixed 5 OUTDATED issues in python M01-M02. Updated datetime.utcnow() to datetime.now(timezone.utc) in 3 files, bumped FastAPI reference from 0.95 to 0.115 in 2 files. All changes verified against version-manifest.json targets.",
  "whatWasImplemented": "Applied fixes to 5 issues: M01 L02 content/01-theory.md line 23, M01 L03 content/02-example.md line 15, M02 L01 content/01-theory.md lines 8-12, M02 L04 content/03-warning.md line 5, M02 L05 content/01-theory.md line 31. Updated findings JSONs with fixesApplied records.",
  "whatWasLeftUndone": "3 INACCURATE issues require more complex research and remain for next fix batch. 2 STUB issues need content creation - deferred to content-writing phase.",
  "verification": {
    "commandsRun": [
      {
        "command": "git diff --stat",
        "exitCode": 0,
        "observation": "5 files modified, 12 insertions(+), 8 deletions(-)"
      },
      {
        "command": "python3 -c \"import json; f=json.load(open('.planning/phases/python-content-audit/M01-findings.json')); print(len([i for l in f['findings'] for i in l.get('fixesApplied',[])]))\"",
        "exitCode": 0,
        "observation": "2 fixes recorded in M01 findings"
      }
    ],
    "interactiveChecks": []
  },
  "tests": {
    "added": []
  },
  "discoveredIssues": []
}
```

## When to Return to Orchestrator

- A module directory is missing or has unexpected structure
- Web research reveals a systemic issue affecting many lessons (e.g., entire framework version wrong)
- More than 50% of lessons in a batch have critical issues
- Fix requires pedagogical judgment (e.g., restructuring a lesson)
- Content schema has changed and findings format needs updating
