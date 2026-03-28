---
type: "THEORY"
title: "Exercise 2: Implement API Rate Limiting"
---

**Estimated Time**: 45 minutes
**Difficulty**: Intermediate

## Objective

Build rate limiting middleware for a Ktor API to prevent abuse and ensure fair resource usage. Rate limiting protects your backend from denial-of-service attacks, brute-force attempts, and runaway clients.

---

## Learning Goals

By completing this exercise, you will:
- Implement token bucket or sliding window rate limiting algorithms
- Store rate limit counters using Redis or in-memory caches
- Return proper 429 (Too Many Requests) responses with Retry-After headers
- Apply different limits per endpoint or user tier

## Background

Rate limiting is essential for production APIs. Without it, a single misconfigured client or malicious actor can overwhelm your servers. Common strategies include:

- **Fixed Window**: Allow N requests per time window (simple but can burst)
- **Sliding Window**: Smooth distribution over time (more fair)
- **Token Bucket**: Allow bursts up to capacity, then rate-limited refill (flexible)

## Implementation Requirements

Your middleware should:
1. Track requests per client (by IP or API key)
2. Enforce configurable limits (e.g., 100 requests/minute for standard users)
3. Return 429 status with remaining limit headers
4. Support different tiers (free: 100/hr, pro: 10,000/hr)

## Real-World Context

GitHub's API uses rate limiting with clear headers:
```
X-RateLimit-Limit: 5000
X-RateLimit-Remaining: 4999
X-RateLimit-Reset: 1372700873
```

Stripe, Twilio, and AWS all implement sophisticated rate limiting to protect their infrastructure while providing fair service to all customers.

## Success Criteria

- [ ] Rate limiting applies to all API endpoints
- [ ] Clients receive clear headers about their limits
- [ ] Different limits for authenticated vs. anonymous users
- [ ] Graceful handling of Redis/cache failures
