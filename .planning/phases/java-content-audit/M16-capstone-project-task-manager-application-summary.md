# Module 16: Capstone Project - Task Manager Application - Audit Summary

**Course:** Java Full-Stack Development  
**Target Versions:** Java 25, Spring Boot 4.0.x  
**Review Date:** 2026-03-28  
**Total Lessons:** 9

## Lessons Overview

| Lesson | Title | Rating | Issues |
|--------|-------|--------|--------|
| 16.1 | Project Requirements and Architecture | Good | 0 |
| 16.2 | Backend Setup - Data Layer | Good | 0 |
| 16.3 | REST API Development | Acceptable | 1 minor |
| 16.4 | Authentication Implementation | Acceptable | 1 minor |
| 16.5 | Business Logic and Validation | Good | 0 |
| 16.6 | React Frontend Development | Acceptable | 1 minor |
| 16.7 | Frontend-Backend Integration | Acceptable | 1 minor |
| 16.8 | Testing the Application | Acceptable | 1 minor |
| 16.9 | Deployment to Production | Acceptable | 2 (1 major, 1 minor) |

## Summary Statistics

- **Total Issues:** 7
- **Critical:** 0
- **Major:** 1
- **Minor:** 6

## Special Focus: Capstone Completeness

### ✅ Strengths

1. **Well-Structured Learning Path:** Clear progression from requirements → backend → frontend → integration → deployment
2. **Dual Frontend Paths:** Both Thymeleaf (server-side) and React (client-side) paths are fully documented
3. **Production-Ready Scope:** Covers authentication, validation, testing, and deployment
4. **Comprehensive Feature Set:** Tasks, categories, priorities, due dates, status tracking

### ⚠️ Areas Needing Attention

#### 1. Railway Deployment Issue (Lesson 16.9) - MAJOR
**Same issue as M14.5:** Railway's free tier has been discontinued.

**Impact:** Students completing the capstone will be unable to deploy without providing payment information.

**Fix Required:**
- Update deployment instructions with current platform status
- Add alternatives:
  - **Render** (free tier available)
  - **Fly.io** (free allowance)
  - **Railway** (paid, with credit card requirement noted)
  - **Oracle Cloud Free Tier** (always free resources)

#### 2. Missing CORS Cross-Reference (Lesson 16.6) - MINOR
React path students may forget to configure CORS on the backend before attempting frontend integration.

**Fix:** Add prominent note in 16.6:
> "⚠️ Before starting the React frontend, ensure you've configured CORS in your Spring Boot backend. See M13.6 for CORS configuration details."

#### 3. Missing Troubleshooting Guide (Lesson 16.7) - MINOR
Integration between frontend and backend is where students typically get stuck.

**Recommended Additions:**
```markdown
## Common Integration Issues

1. **CORS Errors in Browser Console**
   - Symptom: "Access-Control-Allow-Origin" error
   - Fix: Add CORS configuration to Spring Boot

2. **Cannot Connect to Backend**
   - Check: Is Spring Boot running on port 8080?
   - Check: Is React dev server proxy configured?

3. **401 Unauthorized on API Calls**
   - Check: Is JWT token being sent in Authorization header?
   - Check: Is token still valid (not expired)?

4. **Port Already in Use**
   - Spring Boot: Change in application.properties: `server.port=8081`
   - React: `PORT=3001 npm start`
```

## Architecture Completeness

The capstone covers all essential layers:

| Layer | Coverage | Lesson |
|-------|----------|--------|
| Database Schema | ✅ Complete | 16.2 |
| JPA Entities | ✅ Complete | 16.2 |
| Repositories | ✅ Complete | 16.2 |
| Services | ✅ Complete | 16.5 |
| REST Controllers | ✅ Complete | 16.3 |
| DTOs/Validation | ✅ Complete | 16.5 |
| Authentication | ✅ Complete | 16.4 |
| Frontend (Both Paths) | ✅ Complete | 16.6 |
| Integration | ✅ Complete | 16.7 |
| Testing | ✅ Basic | 16.8 |
| Deployment | ⚠️ Needs Update | 16.9 |

## Security Implementation

**Well Covered:**
- ✅ JWT token authentication
- ✅ Password hashing with BCrypt
- ✅ Spring Security filter chain
- ✅ Protected route handling (React)
- ✅ Input validation (@NotBlank, @Email, etc.)

**Could Strengthen:**
- ⚠️ Pre-deployment security checklist (16.9)
- ⚠️ HTTPS configuration guidance
- ⚠️ Environment variable management for secrets

## Testing Strategy (Lesson 16.8)

**Current Coverage:**
- Unit tests for services
- Integration tests for controllers
- Basic test structure

**Enhancement Opportunity:**
Consider adding Testcontainers for database integration tests:
```java
@Testcontainers
@SpringBootTest
class TaskRepositoryTest {
    @Container
    static PostgreSQLContainer<?> postgres = 
        new PostgreSQLContainer<>("postgres:16-alpine");
    
    // Tests run against real PostgreSQL, not H2
}
```

This provides more realistic testing since H2 has different SQL dialect and behavior than PostgreSQL.

## Version Compatibility

| Technology | Content Version | Status |
|------------|-----------------|--------|
| Spring Boot | 4.0.x | ✅ Target version |
| Java | 25 | ✅ Target version |
| PostgreSQL | 16 | ✅ Current LTS |
| React | 19.x (implied) | ✅ Latest |
| JWT | Current | ✅ Standard |
| Docker | Multi-stage | ✅ Current |

## Prerequisites Verification

The capstone assumes knowledge from previous modules. All prerequisites are satisfied:
- ✅ M7 (Virtual Threads) - Foundation for 15.7
- ✅ M8 (Testing) - Foundation for 16.8
- ✅ M9 (Databases) - Foundation for 16.2
- ✅ M11 (Spring Boot) - Foundation for 16.3-16.5
- ✅ M12 (Security/JWT) - Foundation for 16.4
- ✅ M13 (React) - Foundation for 16.6 (React path)
- ✅ M14 (DevOps) - Foundation for 16.9
- ✅ M15 (Full-Stack) - Direct preparation for capstone

## Recommendations

### Immediate Actions Required
1. **Fix Railway deployment** in Lesson 16.9 (free tier discontinued)
2. **Add CORS cross-reference** in Lesson 16.6

### High Priority
3. **Add troubleshooting section** to Lesson 16.7
4. **Add security checklist** to Lesson 16.9

### Optional Enhancements
5. **Add HTTP status code reference** to Lesson 16.3
6. **Add complete SecurityConfig example** to Lesson 16.4
7. **Add Testcontainers mention** to Lesson 16.8

## Pedagogical Assessment

**Excellent:**
- Progressive complexity builds confidence
- Two-path approach accommodates different learning goals
- Real-world project scope (not a toy example)
- Portfolio-worthy outcome

**Could Improve:**
- More troubleshooting guidance for common issues
- More explicit cross-references to prerequisite content
- Deployment options need updating for current platform landscape

## Final Assessment

The capstone is **comprehensive and well-structured** with one major issue (Railway deployment) that requires immediate attention. Once the deployment platform is updated and minor enhancements are added, this will be an excellent culminating project for the Java full-stack course.

The dual-path approach (Thymeleaf vs React) is a strength, allowing students to choose based on their interests and career goals. Both paths lead to a production-ready application demonstrating all core Java full-stack competencies.
