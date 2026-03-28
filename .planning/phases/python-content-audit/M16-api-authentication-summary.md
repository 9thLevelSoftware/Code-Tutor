# Module 16: API Authentication - Audit Summary

**Course:** python | **Reviewed:** 6/6 lessons | **Issues:** 13 (0 critical, 10 major, 3 minor)

## Findings Overview

| Category | Count |
|----------|-------|
| OUTDATED | 10 |
| INACCURATE | 1 |
| INCOMPLETE | 1 |
| METADATA | 1 |

## Lesson-by-Lesson Breakdown

### Lesson 01: Password Hashing
**Rating:** good | **Issues:** 0

Content is well-structured with clear theory, examples, and security warnings. Uses passlib with bcrypt correctly. No issues found.

---

### Lesson 02: Authentication and API Security
**Rating:** needs-work | **Issues:** 3

**Issue 1: [OUTDATED/major]** content/02-example.md:135 - Uses `python-jose` library instead of PyJWT
- **Problem:** python-jose has a known security vulnerability (CVE-2024-33663 - algorithm confusion attack)
- **Fix:** Replace `from jose import jwt` with `import jwt` (PyJWT)

**Issue 2: [OUTDATED/major]** content/02-example.md:155 - Uses deprecated `datetime.utcnow()`
- **Problem:** `datetime.utcnow()` is deprecated in Python 3.12
- **Fix:** Replace with `datetime.now(timezone.utc)`

**Issue 3: [OUTDATED/minor]** content/02-example.md:45 - Uses raw `hashlib.pbkdf2_hmac` instead of passlib
- **Note:** While functional, the lesson should be consistent with Lesson 01's recommendation of passlib

---

### Lesson 03: JWT Authentication
**Rating:** good | **Issues:** 5

**Issue 1: [OUTDATED/major]** content/02-example.md:34 - `expire = datetime.utcnow() + (expires_delta or timedelta(minutes=15))`
**Issue 2: [OUTDATED/major]** content/03-example.md:58 - `expire = datetime.utcnow() + timedelta(minutes=30)`
**Issue 3: [OUTDATED/major]** content/05-key_point.md:25 - Quick reference uses `datetime.utcnow()`
**Issue 4: [OUTDATED/major]** content/06-warning.md:27 - Warning example uses `datetime.utcnow()`

All instances need replacement with `datetime.now(timezone.utc)`.

**Issue 5: [INACCURATE/minor]** content/01-theory.md:42 - Notes python-jose is deprecated, but creates inconsistency
- The theory correctly states PyJWT should be used and python-jose is deprecated
- However, other lessons still use python-jose, creating inconsistency across the module

---

### Lesson 04: OAuth2 with FastAPI
**Rating:** needs-work | **Issues:** 4

**Issue 1: [OUTDATED/major]** content/02-example.md:94 - Token creation uses `datetime.utcnow()`
**Issue 2: [OUTDATED/major]** content/03-example.md:28 - Scope token example uses `datetime.utcnow()`
**Issue 3: [OUTDATED/major]** content/04-example.md:23 - Access token uses `datetime.utcnow()`
**Issue 4: [OUTDATED/major]** content/04-example.md:31 - Refresh token uses `datetime.utcnow()`

All instances need replacement with `datetime.now(timezone.utc)`.

---

### Lesson 05: Mini-Project: Blog API with Authentication
**Rating:** needs-work | **Issues:** 3

**Issue 1: [OUTDATED/major]** content/02-example.md:110 - Uses `from jose import jwt` (python-jose)
- **Problem:** CVE-2024-33663 vulnerability
- **Fix:** Replace with `import jwt` (PyJWT)

**Issue 2: [OUTDATED/major]** content/02-example.md:118 - `expire = datetime.utcnow() + timedelta(...)`
- **Fix:** Replace with `datetime.now(timezone.utc)`

**Issue 3: [OUTDATED/major]** challenges/01-extend-the-blog-api/solution.py:4 - Solution file uses `from jose import jwt`
- **Fix:** Update challenge solution to use PyJWT

---

### Lesson 06: Bridge: From FastAPI to Django
**Rating:** acceptable | **Issues:** 2

**Issue 1: [METADATA/minor]** lesson.json:7 - Difficulty is "intermediate" while module is "advanced"
- All other lessons in this module are marked "advanced"
- Consider aligning difficulty with the rest of the module

**Issue 2: [INCOMPLETE/minor]** No challenges directory
- All other lessons have at least one challenge
- Consider adding a bridge-related challenge or mini-project

---

## Key Issues Requiring Attention

### 1. datetime.utcnow() Deprecation (Python 3.12)
**Affected files:** 10 locations across lessons 02-05

Python 3.12 deprecated `datetime.utcnow()` and `datetime.utcfromtimestamp()`. All instances should be updated to use `datetime.now(timezone.utc)`.

**Pattern to find:**
```python
expire = datetime.utcnow() + timedelta(...)
```

**Pattern to replace:**
```python
from datetime import datetime, timedelta, timezone
expire = datetime.now(timezone.utc) + timedelta(...)
```

### 2. python-jose Security Vulnerability
**Affected files:** 2 locations (lesson 02, lesson 05 content + challenge)

python-jose has CVE-2024-33663 (algorithm confusion vulnerability). Should be replaced with PyJWT which is actively maintained.

**Pattern to find:**
```python
from jose import jwt
```

**Pattern to replace:**
```python
import jwt  # PyJWT
```

Note: Lesson 03 already correctly uses PyJWT and even notes python-jose is deprecated.

---

## Web Research Notes

1. **CVE-2024-33663**: Algorithm confusion vulnerability in python-jose affecting all versions. See https://www.sentinelone.com/vulnerability-database/cve-2024-33663/

2. **Python 3.12 Deprecations**: `datetime.utcnow()` and `datetime.utcfromtimestamp()` deprecated per PEP 696 and Python 3.12 release notes. See https://blog.miguelgrinberg.com/post/it-s-time-for-a-change-datetime-utcnow-is-now-deprecated

3. **FastAPI Community Recommendation**: FastAPI discussions and Reddit r/FastAPI community recommend PyJWT over python-jose for new projects in 2025.

---

## Recommended Fix Priority

1. **HIGH**: Replace python-jose with PyJWT (security vulnerability)
2. **HIGH**: Replace datetime.utcnow() with datetime.now(timezone.utc) (Python 3.12 deprecation)
3. **MEDIUM**: Align lesson 06 difficulty with module (change "intermediate" to "advanced")
4. **LOW**: Consider adding a challenge to lesson 06

---

## Files Affected

### Content files requiring updates:
- `content/courses/python/modules/16-api-authentication/lessons/02-authentication-and-api-security/content/02-example.md`
- `content/courses/python/modules/16-api-authentication/lessons/03-jwt-authentication/content/02-example.md`
- `content/courses/python/modules/16-api-authentication/lessons/03-jwt-authentication/content/03-example.md`
- `content/courses/python/modules/16-api-authentication/lessons/03-jwt-authentication/content/05-key_point.md`
- `content/courses/python/modules/16-api-authentication/lessons/03-jwt-authentication/content/06-warning.md`
- `content/courses/python/modules/16-api-authentication/lessons/04-oauth2-with-fastapi/content/02-example.md`
- `content/courses/python/modules/16-api-authentication/lessons/04-oauth2-with-fastapi/content/03-example.md`
- `content/courses/python/modules/16-api-authentication/lessons/04-oauth2-with-fastapi/content/04-example.md`
- `content/courses/python/modules/16-api-authentication/lessons/05-mini-project-blog-api-with-authentication/content/02-example.md`

### Challenge files requiring updates:
- `content/courses/python/modules/16-api-authentication/lessons/05-mini-project-blog-api-with-authentication/challenges/01-extend-the-blog-api/solution.py`

### Metadata files requiring updates:
- `content/courses/python/modules/16-api-authentication/lessons/06-bridge-from-fastapi-to-django/lesson.json`
