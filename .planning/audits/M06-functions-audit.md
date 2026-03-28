# M06 Functions — Content Audit Report

**Module:** 06-functions  
**Lessons audited:** 7 of 7  
**Auditor:** Automated content audit  
**Date:** 2026-03-27  

---

## Module-Level Summary

| # | Lesson | Quality | Issues |
|---|--------|---------|--------|
| 01 | Function Basics - Reusable Code Blocks | **B** | 2 |
| 02 | Parameters and Return Values | **B** | 2 |
| 03 | Variable Scope and Lifetime | **C** | 3 |
| 04 | Default and Keyword Arguments | **C** | 2 |
| 05 | Lambda Functions and Built-in Functions | **B** | 2 |
| 06 | Mini-Project: Personal Utility Library | **B** | 2 |
| 07 | Type Hints: Self-Documenting Code (Python 3.10+) | **B** | 2 |

**Overall module rating: B-** (good content quality, but missing challenges in 6 of 7 lessons and a few output-accuracy bugs)

---

## Lesson 1: Function Basics - Reusable Code Blocks

**Path:** `06-functions/lessons/01-function-basics-reusable-code-blocks/`  
**Title:** Function Basics - Reusable Code Blocks  
**Quality Rating: B**

### Metadata Check
- `id`: lesson-06-01 ✅
- `title`: "Function Basics - Reusable Code Blocks" ✅
- `moduleId`: module-06 ✅
- `order`: 1 ✅
- `estimatedMinutes`: 30 ✅
- `difficulty`: beginner ✅

### Content Files (5)
- `01-theory.md` — THEORY: "Understanding the Concept" — Chef/recipe analogy, clear and well-written ✅
- `02-theory.md` — THEORY: "Anatomy of a Function" — Good breakdown of def syntax ✅
- `03-example.md` — EXAMPLE: Code with expected output — Verified accurate ✅
- `04-warning.md` — WARNING: 5 common pitfalls — Comprehensive and accurate ✅
- `05-key_point.md` — KEY_POINT: Key takeaways — Good summary ✅

### Challenges
- **No challenges directory** ⚠️

### Issues

| # | Category | Severity | Description | Suggested Fix |
|---|----------|----------|-------------|---------------|
| 1 | INCOMPLETE | major | No challenges directory. All other early modules (M01–M05) have challenges in every lesson. Students have no hands-on practice for function basics. | Add a challenge (e.g., "Create a function that prints a formatted receipt") with challenge.json, starter.py, and solution.py. |
| 2 | PEDAGOGY | minor | No analogy section (type: ANALOGY). The chef/recipe analogy in 01-theory is good, but a dedicated analogy block would match the pattern used in other modules. | Consider adding a standalone analogy card if the content framework supports it, or leave as-is since the analogy is embedded in theory. |

---

## Lesson 2: Parameters and Return Values

**Path:** `06-functions/lessons/02-parameters-and-return-values/`  
**Title:** Parameters and Return Values  
**Quality Rating: B**

### Metadata Check
- `id`: lesson-06-02 ✅
- `title`: "Parameters and Return Values" ✅
- `moduleId`: module-06 ✅
- `order`: 2 ✅
- `estimatedMinutes`: 30 ✅
- `difficulty`: beginner ✅

### Content Files (5)
- `01-theory.md` — THEORY: continues Caesar salad analogy — Good continuity ✅
- `02-theory.md` — THEORY: parameters vs return values — Clear, accurate ✅
- `03-example.md` — EXAMPLE: Code with expected output — Verified accurate ✅
- `04-warning.md` — WARNING: 5 common pitfalls — Accurate ✅
- `05-key_point.md` — KEY_POINT: Key takeaways ✅

### Challenges
- **No challenges directory** ⚠️

### Issues

| # | Category | Severity | Description | Suggested Fix |
|---|----------|----------|-------------|---------------|
| 1 | INCOMPLETE | major | No challenges directory. Parameters and return values are fundamental — students need practice exercises. | Add a challenge (e.g., "Write functions that calculate area, convert temperatures, and format names") with starter.py/solution.py. |
| 2 | PEDAGOGY | minor | Warning #5 comment says "Return should be last" which is misleading. Guard clauses and early returns are a common, preferred pattern in Python. | Change the comment to "Make sure all necessary logic runs before you return" or add a note acknowledging that early returns are valid. |

---

## Lesson 3: Variable Scope and Lifetime

**Path:** `06-functions/lessons/03-variable-scope-and-lifetime/`  
**Title:** Variable Scope and Lifetime  
**Quality Rating: C**

### Metadata Check
- `id`: lesson-06-03 ✅
- `title`: "Variable Scope and Lifetime" ✅
- `moduleId`: module-06 ✅
- `order`: 3 ✅
- `estimatedMinutes`: 30 ✅
- `difficulty`: beginner ✅

### Content Files (5)
- `01-theory.md` — THEORY: Office desk/break-room analogy — Excellent ✅
- `02-theory.md` — THEORY: LEGB rule — Clear and accurate ✅
- `03-example.md` — EXAMPLE: Code with expected output — **Has output mismatch** ❌
- `04-warning.md` — WARNING: 5 pitfalls — Good, includes mutable default warning ✅
- `05-key_point.md` — KEY_POINT: Key takeaways ✅

### Challenges
- **No challenges directory** ⚠️

### Issues

| # | Category | Severity | Description | Suggested Fix |
|---|----------|----------|-------------|---------------|
| 1 | INACCURATE | major | In `03-example.md`, the "Variable Lifetime" section's expected output shows `Call 1: counter = 1`, `Call 2: counter = 1`, `Call 3: counter = 1`. But the actual code always prints `Call 1: counter = 1` three times because `counter` is always 1 (reset each call). The call number in the output (`Call 2`, `Call 3`) would never appear — the code uses `counter` for both the call label and the value, and counter is always 1. This directly contradicts the lesson's key teaching point about variable lifetime. | Fix the expected output to show `Call 1: counter = 1` three times, OR change the function to accept a call number parameter: `def count_calls(call_num):` and print `f"Call {call_num}: counter = {counter}"`. |
| 2 | INCOMPLETE | major | No challenges directory. Scope is a notoriously confusing topic — students need hands-on practice to solidify understanding. | Add a challenge with scope-related exercises (e.g., "Predict and fix scope bugs", "Refactor global variables to use return values"). |
| 3 | PEDAGOGY | minor | Warning #5 about mutable default arguments is duplicated nearly verbatim in Lesson 04's Warning #1. While not wrong, it may feel repetitive. | Consider keeping a brief mention in L03 and the full explanation in L04 where it's more topically relevant (default arguments lesson). |

---

## Lesson 4: Default and Keyword Arguments

**Path:** `06-functions/lessons/04-default-and-keyword-arguments/`  
**Title:** Default and Keyword Arguments  
**Quality Rating: C**

### Metadata Check
- `id`: lesson-06-04 ✅
- `title`: "Default and Keyword Arguments" ✅
- `moduleId`: module-06 ✅
- `order`: 4 ✅
- `estimatedMinutes`: 30 ✅
- `difficulty`: beginner ✅

### Content Files (5)
- `01-theory.md` — THEORY: Coffee order analogy — Excellent ✅
- `02-theory.md` — THEORY: Syntax and rules — Clear and accurate ✅
- `03-example.md` — EXAMPLE: Code with expected output — **Has output mismatch** ❌
- `04-warning.md` — WARNING: 5 pitfalls including mutable defaults, `time.time()` — Good ✅
- `05-key_point.md` — KEY_POINT: Key takeaways ✅

### Challenges
- **No challenges directory** ⚠️

### Issues

| # | Category | Severity | Description | Suggested Fix |
|---|----------|----------|-------------|---------------|
| 1 | INACCURATE | major | In `03-example.md`, the "Keyword Arguments" section's expected output shows `Order #1: burger with fries, Drink: cola` but the actual code would produce `Order #1: Order: burger with fries, Drink: cola`. The `create_order()` function's print statement includes the prefix `"Order: "` which is duplicated with the `print("Order #1: ", end="")` call before it. The expected output omits the inner `"Order: "` prefix. | Either (a) remove `"Order: "` from the function's f-string so it prints just `"{main_dish} with {side}, Drink: {drink}"`, or (b) remove the `print("Order #N: ", end="")` lines and let the function handle all formatting, or (c) fix the expected output to include `"Order: "`. |
| 2 | INCOMPLETE | major | No challenges directory. Default/keyword arguments are tricky for beginners and need hands-on practice. | Add a challenge (e.g., "Build a function with defaults to format user profiles or generate reports"). |

---

## Lesson 5: Lambda Functions and Built-in Functions

**Path:** `06-functions/lessons/05-lambda-functions-and-built-in-functions/`  
**Title:** Lambda Functions and Built-in Functions  
**Quality Rating: B**

### Metadata Check
- `id`: lesson-06-05 ✅
- `title`: "Lambda Functions and Built-in Functions" ✅
- `moduleId`: module-06 ✅
- `order`: 5 ✅
- `estimatedMinutes`: 30 ✅
- `difficulty`: beginner — **Arguably should be intermediate** ⚠️

### Content Files (5)
- `01-theory.md` — THEORY: Lambda concept — Clear ✅
- `02-theory.md` — THEORY: Built-in functions reference — Comprehensive ✅
- `03-example.md` — EXAMPLE: Code with expected output — Verified accurate ✅
- `04-warning.md` — WARNING: 5 pitfalls including late binding, iterator exhaustion — Excellent, covers advanced topics ✅
- `05-key_point.md` — KEY_POINT: Key takeaways ✅

### Challenges
- **No challenges directory** ⚠️

### Issues

| # | Category | Severity | Description | Suggested Fix |
|---|----------|----------|-------------|---------------|
| 1 | METADATA | minor | Difficulty is "beginner" but content covers lambda closures, late binding, map/filter/reduce patterns, and iterator exhaustion. These are intermediate topics. Warning #1 (late binding in loop lambdas) is particularly advanced. | Change difficulty to "intermediate" to set proper expectations. |
| 2 | INCOMPLETE | major | No challenges directory. Lambda functions and map/filter are best learned through practice. | Add a challenge (e.g., "Use lambdas and built-in functions to process a student grade list: sort, filter, transform"). |

---

## Lesson 6: Mini-Project: Personal Utility Library

**Path:** `06-functions/lessons/06-mini-project-personal-utility-library/`  
**Title:** Mini-Project: Personal Utility Library  
**Quality Rating: B**

### Metadata Check
- `id`: lesson-06-06 ✅
- `title`: "Mini-Project: Personal Utility Library" ✅
- `moduleId`: module-06 ✅
- `order`: 6 ✅
- `estimatedMinutes`: 30 ✅
- `difficulty`: beginner ✅

### Content Files (5)
- `01-theory.md` — THEORY: What makes a good utility function — Good framing ✅
- `02-theory.md` — THEORY: Function design process — Clear methodology ✅
- `03-example.md` — EXAMPLE: Full utility library with 4 categories — Comprehensive and well-structured ✅
- `04-warning.md` — WARNING: 5 pitfalls — Excellent coverage of function design anti-patterns ✅
- `05-key_point.md` — KEY_POINT: Key takeaways ✅

### Challenges
- **No challenges directory** ⚠️

### Issues

| # | Category | Severity | Description | Suggested Fix |
|---|----------|----------|-------------|---------------|
| 1 | INCOMPLETE | major | No challenges directory. This is a **mini-project lesson** — having no challenge for students to build their own utility functions is a critical gap. Mini-project lessons in other modules (e.g., M01-L05, M04-L05, M05-L06) all have challenges. | Add a challenge asking students to build 3-4 utility functions of their own (e.g., "Create a date_utils module with functions to calculate age, format dates, and check if a year is a leap year"). |
| 2 | PEDAGOGY | minor | The `is_valid_email()` function is extremely simplistic (only checks for `@` and `.` after `@`). While the docstring says "simplified", beginners may adopt this pattern. `"user@.com"` or `"@example.com"` would pass validation. | Add a comment explicitly stating this is for demonstration only and real email validation should use a library like `email-validator`. Or improve the validation slightly to also check for non-empty parts. |

---

## Lesson 7: Type Hints: Self-Documenting Code (Python 3.10+)

**Path:** `06-functions/lessons/07-type-hints-self-documenting-code-python-310/`  
**Title:** Type Hints: Self-Documenting Code (Python 3.10+)  
**Quality Rating: B**

### Metadata Check
- `id`: lesson-06-07 ✅
- `title`: "Type Hints: Self-Documenting Code (Python 3.10+)" ✅
- `moduleId`: module-06 ✅
- `order`: 7 ✅
- `estimatedMinutes`: 35 ✅ (slightly higher than other lessons, appropriate for the topic)
- `difficulty`: intermediate ✅ (correctly marked higher than the rest)

### Content Files (5)
- `01-theory.md` — THEORY: Car-lending analogy, why type hints matter — **Has inaccuracy** ❌
- `02-example.md` — EXAMPLE: Full code with Python 3.10+ syntax — Verified accurate ✅
- `03-theory.md` — THEORY: Syntax reference table — Excellent reference ✅
- `04-warning.md` — WARNING: 5 pitfalls — Accurate and well-targeted ✅
- `05-key_point.md` — KEY_POINT: Comprehensive with "when to use" / "when overkill" — Excellent ✅

### Challenges (1)
- `01-practice-add-type-hints/`
  - `challenge.json` — FREE_CODING type, 5 functions to annotate, good test cases ✅
  - `starter.py` — Clean unannotated functions ✅
  - `solution.py` — Correctly annotated with Python 3.10+ syntax ✅

### Issues

| # | Category | Severity | Description | Suggested Fix |
|---|----------|----------|-------------|---------------|
| 1 | INACCURATE | major | In `01-theory.md`, the example claims `greet(42)` produces "No runtime error from type hint." However, the function uses string concatenation (`"Hello, " + name`), which WOULD raise `TypeError: can only concatenate str (not "int") to str` at runtime. The comment is technically correct (the error isn't from the type hint) but practically misleading — a student trying this example will get a crash. Note: the later example in `02-example.md` correctly uses f-strings which would work with any type. | Change the theory example to use an f-string (`f"Hello, {name}!"`) to match the claim of "no runtime error", OR change the example to show a function where the mismatch truly doesn't cause a runtime error (e.g., a function that only stores the value). |
| 2 | PEDAGOGY | minor | Content ordering is unusual: `01-theory`, `02-example`, `03-theory`, `04-warning`, `05-key_point`. Other lessons in M06 follow `01-theory`, `02-theory`, `03-example`, `04-warning`, `05-key_point`. This interleaving could be intentional (show an example before the syntax reference) but is inconsistent. | Consider renaming/reordering to match the module's pattern, or document the intentional deviation. Not blocking. |

---

## Cross-Cutting Findings

### 1. Missing Challenges (Module-Wide) — CRITICAL

**6 of 7 lessons have no challenges directory.** Only Lesson 07 has a challenge. Comparing with other modules:
- M01 (The Absolute Basics): 5/5 lessons have challenges
- M02 (Variables): 5/5 lessons have challenges
- M03 (Boolean Logic): 7/7 lessons have challenges
- M04 (Loops): 5/5 lessons have challenges
- M05 (Lists & Tuples): 6/6 lessons have challenges

**M06 is a severe outlier.** This represents the single biggest quality gap in the module.

### 2. Expected Output Mismatches — 2 Confirmed Bugs

| Lesson | Section | Issue |
|--------|---------|-------|
| L03 | Variable Lifetime | Expected shows `Call 1/2/3` but code always prints `Call 1` (counter resets each call) |
| L04 | Keyword Arguments | Expected omits `"Order: "` prefix that the function actually prints |

### 3. Difficulty Calibration

All lessons except L07 are marked "beginner". L05 (Lambda/map/filter with closure late-binding) would be better classified as "intermediate".

### 4. Python Version Currency Check (Lesson 07)

- Uses `X | None` syntax (Python 3.10+) ✅ Current
- Uses `list[int]`, `dict[str, int]` lowercase syntax (Python 3.9+) ✅ Current
- Correctly advises against `from typing import Optional/Union` old style ✅
- References `mypy` for static checking ✅ Current
- No references to deprecated APIs or removed features ✅
- **No outdated items found** — Lesson 07 is up-to-date as of Python 3.10+

### 5. Duplicate Content

Mutable default arguments warning appears in both L03 (Warning #5) and L04 (Warning #1) with nearly identical examples. Minor redundancy.

---

## Issue Summary by Severity

| Severity | Count | Description |
|----------|-------|-------------|
| Critical | 0 | — |
| Major | 9 | 6× missing challenges, 2× output mismatches, 1× misleading runtime error claim |
| Minor | 6 | 1× early-return comment, 1× difficulty miscalibration, 1× duplicate content, 1× missing analogy block, 1× content ordering, 1× simplified email validation |

---

## Recommended Priority Actions

1. **Add challenges to Lessons 01-06** (6 challenges needed) — This is the most impactful improvement.
2. **Fix expected output in L03 `03-example.md`** — Variable Lifetime section shows wrong call numbers.
3. **Fix expected output in L04 `03-example.md`** — Keyword Arguments section has "Order:" prefix mismatch.
4. **Fix L07 `01-theory.md` example** — Change `"Hello, " + name` to f-string to avoid misleading runtime error claim.
5. **Change L05 difficulty** from "beginner" to "intermediate".
