 ---
 name: content-auditor
 description: Audits course lesson content for completeness, accuracy, and currentness, producing structured findings reports.
 ---
 
 # Content Auditor
 
 NOTE: Startup and cleanup are handled by `worker-base`. This skill defines the WORK PROCEDURE.
 
 ## When to Use This Skill
 
 Use for features that audit batches of course modules, reading every lesson and producing structured findings in JSON and Markdown format.
 
 ## Required Skills
 
 None. This worker uses file reading tools and WebSearch for research.
 
 ## Work Procedure
 
 ### Step 1: Identify Assigned Modules
 
 Read the feature description to determine which modules to audit. List all lesson directories for those modules under `content/courses/python/modules/`.
 
 ### Step 2: Audit Each Lesson Systematically
 
 For EVERY lesson in the assigned modules, perform the following checks:
 
 **a) Metadata Check (lesson.json)**
 - Read `lesson.json`
 - Verify fields exist: `id`, `title`, `moduleId`, `order`, `estimatedMinutes`, `difficulty`
 - Check `difficulty` makes sense for the content (a lesson teaching JWT auth should not be "beginner")
 - Flag missing fields as `METADATA` issues
 
 **b) Content Files Check (content/*.md)**
 - Read ALL markdown files in `content/`
 - Check for stubs/placeholders: files with fewer than 50 words, "TODO", "TBD", "placeholder" text
 - Check for missing expected sections: a lesson should have at minimum theory AND example content
 - Check code blocks for obvious issues: missing imports, syntax errors, references to undefined variables
 - Check version references: note any specific version numbers (e.g., "Python 3.10", "FastAPI 0.95", "Pydantic v1") and flag if they look potentially outdated
 - Check for deprecated patterns: `datetime.utcnow()`, old-style string formatting in modern lessons, etc.
 - Check title/content alignment: does the lesson content match what the title promises?
 
 **c) Challenge Check (challenges/)**
 - If `challenges/` directory exists, verify each challenge has:
   - `challenge.json` with required fields
   - `starter.*` file (non-empty)
   - `solution.*` file (non-empty)
 - Flag missing or empty challenge files as `INCOMPLETE`
 
 **d) Web Research (ONLY for flagged items)**
 - When you encounter a version reference or API pattern that might be outdated, use WebSearch to verify
 - Record the research result in `webResearchNote`
 - Do NOT research every lesson - only items that look suspicious
 
 ### Step 3: Produce Per-Module Findings
 
 For each module in the batch, create two files in `.planning/phases/python-content-recheck/`:
 
 **JSON findings file** (`M{NN}-{module-slug}-findings.json`):
 ```json
 {
   "module": "01-the-absolute-basics",
   "moduleTitle": "The Absolute Basics",
   "reviewDate": "2026-03-28",
   "totalLessons": 5,
   "lessonsReviewed": 5,
   "findings": [
     {
       "lessonPath": "content/courses/python/modules/01-the-absolute-basics/lessons/01-what-is-programming-really",
       "lessonTitle": "What Is Programming Really",
       "qualityRating": "good",
       "issues": []
     },
     {
       "lessonPath": "content/courses/python/modules/01-the-absolute-basics/lessons/02-your-first-python-playground",
       "lessonTitle": "Your First Python Playground",
       "qualityRating": "needs-work",
       "issues": [
         {
           "category": "METADATA",
           "severity": "minor",
           "file": "content/01-theory.md",
           "description": "References Python 3.13+ but course metadata says 3.12 minimum",
           "webResearchNote": ""
         }
       ]
     }
   ],
   "summary": {
     "totalIssues": 1,
     "bySeverity": { "critical": 0, "major": 0, "minor": 1 },
     "byCategory": { "METADATA": 1 }
   }
 }
 ```
 
 **Markdown summary file** (`M{NN}-{module-slug}-summary.md`):
 ```markdown
 # Module 01: The Absolute Basics - Audit Summary
 
 **Reviewed:** 5/5 lessons | **Issues:** 1 (0 critical, 0 major, 1 minor)
 
 ## Findings
 
 ### Lesson 01: What Is Programming Really
 **Rating:** good | **Issues:** 0
 
 ### Lesson 02: Your First Python Playground
 **Rating:** needs-work | **Issues:** 1
 - [METADATA/minor] content/01-theory.md: References Python 3.13+ but course metadata says 3.12 minimum
 ```
 
 ### Step 4: Quality Ratings
 
 Assign each lesson one of these ratings:
 - `good` - No issues or only trivial ones
 - `acceptable` - Minor issues that don't impair learning
 - `needs-work` - Has issues that should be fixed (major severity)
 - `critical` - Lesson is unusable or significantly misleading
 
 ### Step 5: Issue Categories and Severities
 
 **Categories:**
 - `STUB` - Placeholder or minimal content (fewer than 50 words of substance)
 - `OUTDATED` - Version references or deprecated APIs (MUST include webResearchNote)
 - `INACCURATE` - Factually wrong code or explanations
 - `INCOMPLETE` - Missing expected sections or partial content
 - `METADATA` - Wrong difficulty, missing fields, inconsistent data
 - `PEDAGOGY` - Title/content mismatch, missing analogies/warnings where needed
 
 **Severities:**
 - `critical` - Lesson unusable (broken examples, completely wrong info)
 - `major` - Significant gap affecting learning (outdated auth patterns, missing imports)
 - `minor` - Quality issue but lesson still functional (metadata mismatch, mild title drift)
 
 ### Step 6: Verify Output
 
 - Count lessons reviewed across all module JSON files and confirm it matches the number of lesson directories on disk for the assigned modules
 - Ensure every JSON file is valid (parseable)
 - Ensure every OUTDATED issue has a non-empty webResearchNote
 
 ## Example Handoff
 
 ```json
 {
   "salientSummary": "Audited 22 lessons across M01-M04. Found 8 issues (0 critical, 3 major, 5 minor). Key findings: M01 L02 has Python version mismatch (3.13 vs 3.12), M03 has 2 lessons with missing imports in examples, M04 L03 has an outdated loop pattern reference. Produced 4 JSON findings files and 4 Markdown summaries.",
   "whatWasImplemented": "Created findings reports for modules 01-the-absolute-basics (5 lessons, 1 issue), 02-variables (5 lessons, 2 issues), 03-boolean-logic (7 lessons, 3 issues), 04-loops (5 lessons, 2 issues). All files written to .planning/phases/python-content-recheck/.",
   "whatWasLeftUndone": "",
   "verification": {
     "commandsRun": [
       {
         "command": "ls .planning/phases/python-content-recheck/M0*",
         "exitCode": 0,
         "observation": "8 files present: 4 JSON + 4 MD for M01-M04"
       },
       {
         "command": "python3 -c \"import json; [json.load(open(f)) for f in ['M01-findings.json', ...]]\"",
         "exitCode": 0,
         "observation": "All JSON files parse successfully"
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
       "description": "M03 L05 challenge solution.py uses a pattern that differs from what the lesson teaches"
     }
   ]
 }
 ```
 
 ## When to Return to Orchestrator
 
 - A module directory is missing or has unexpected structure
 - Content files are in an unexpected format (not Markdown with YAML frontmatter)
 - Web research reveals a systemic issue affecting many lessons (e.g., an entire framework version is wrong across the course)
 - More than 50% of lessons in a batch have critical issues, suggesting a deeper problem
