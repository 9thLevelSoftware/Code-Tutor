---
type: "WARNING"
title: "Spring Security 7.x Pattern"
---

Spring Security 7.x uses a modern bean-based configuration approach:

```java
@Configuration
@EnableWebSecurity
public class SecurityConfig {
    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        return http
            .authorizeHttpRequests(auth -> auth
                .requestMatchers("/admin/**").hasRole("ADMIN")
            )
            .build();
    }
}
```

KEY POINTS:
- Use `SecurityFilterChain` bean instead of deprecated class inheritance
- `authorizeHttpRequests()` with lambda DSL style
- `requestMatchers()` for URL patterns
- Method returns `SecurityFilterChain` bean