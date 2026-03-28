---
type: "THEORY"
title: "Spring Boot with Virtual Threads"
---

Virtual Threads in Spring Boot:

Spring Boot 4.0 enables virtual threads by default. Every @RestController
handler runs on a virtual thread -- no configuration needed.

This is automatic with Java 25 + Spring Boot 4.0.x.

BEFORE (Platform Threads - old default):
- Tomcat default: 200 threads
- 200 concurrent requests max
- Request 201 waits in queue

AFTER (Virtual Threads - Spring Boot 4.0 default):
- Each request gets its own virtual thread
- Handle 10,000+ concurrent requests
- No thread pool exhaustion!

Your existing code works unchanged:

@RestController
public class UserController {
    @GetMapping("/api/users/{id}")
    public User getUser(@PathVariable Long id) {
        // This blocks waiting for DB - that's FINE now!
        return userRepository.findById(id).orElseThrow();
    }
}

With virtual threads enabled:
- JVM automatically "parks" virtual thread during DB wait
- Platform thread freed to handle other requests
- Virtual thread "unparked" when DB returns
- You don't change ANY code!