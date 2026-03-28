---
type: "THEORY"
title: "Authenticated API Calls"
---

Every API call to protected endpoints needs the JWT:

MANUAL APPROACH (tedious):

const token = localStorage.getItem('token');

fetch('/api/users', {
    headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    }
});

BETTER: API SERVICE WRAPPER:

// src/services/api.js
const API_BASE = 'http://localhost:8080/api';

async function fetchWithAuth(endpoint, options = {}) {
    const token = localStorage.getItem('token');
    
    const config = {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...options.headers,
        }
    };
    
    if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
    }
    
    const response = await fetch(`${API_BASE}${endpoint}`, config);
    
    // Handle token expiration
    if (response.status === 401) {
        localStorage.removeItem('token');
        window.location.href = '/login';
        throw new Error('Session expired');
    }
    
    if (!response.ok) {
        const error = await response.json().catch(() => ({}));
        throw new Error(error.message || `HTTP ${response.status}`);
    }
    
    return response.json();
}

export const api = {
    get: (endpoint) => fetchWithAuth(endpoint),
    post: (endpoint, data) => fetchWithAuth(endpoint, {
        method: 'POST',
        body: JSON.stringify(data)
    }),
    put: (endpoint, data) => fetchWithAuth(endpoint, {
        method: 'PUT',
        body: JSON.stringify(data)
    }),
    delete: (endpoint) => fetchWithAuth(endpoint, {
        method: 'DELETE'
    })
};

USAGE:
import { api } from './services/api';

const users = await api.get('/users');
await api.post('/users', { name: 'John' });

CORS CONFIGURATION (Spring Boot):

When connecting React (running on port 5173) to Spring Boot (port 8080), you must configure CORS:

```java
@Bean
public WebMvcConfigurer corsConfigurer() {
    return new WebMvcConfigurer() {
        @Override
        public void addCorsMappings(CorsRegistry registry) {
            registry.addMapping("/api/**")
                .allowedOrigins("http://localhost:5173")
                .allowedMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .allowedHeaders("*")
                .allowCredentials(true);
        }
    };
}
```

Or use the `@CrossOrigin` annotation on your controller:

```java
@CrossOrigin(origins = "http://localhost:5173", allowedHeaders = "*")
@RestController
@RequestMapping("/api")
public class MyController { }
```