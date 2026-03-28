---
type: "THEORY"
title: "💻 Complete Example: Virtual Thread HTTP Client"
---

```java
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.net.URI;
import java.util.List;
import java.util.concurrent.*;

public class VirtualThreadDemo {
    
    public static void main(String[] args) throws Exception {
        HttpClient client = HttpClient.newHttpClient();
        List<String> urls = List.of(
            "https://api.github.com",
            "https://api.example.com/users",
            "https://api.example.com/orders"
            // ... imagine 1000 more URLs
        );
        
        // Fetch ALL URLs concurrently with virtual threads
        try (var executor = Executors.newVirtualThreadPerTaskExecutor()) {
            
            List<Future<String>> futures = urls.stream()
                .map(url -> executor.submit(() -> fetchUrl(client, url)))
                .toList();
            
            // Collect results
            for (Future<String> future : futures) {
                try {
                    IO.println(future.get());
                } catch (ExecutionException e) {
                    IO.println("Failed: " + e.getCause());
                }
            }
        }
    }
    
    private static String fetchUrl(HttpClient client, String url) 
            throws Exception {
        HttpRequest request = HttpRequest.newBuilder()
            .uri(URI.create(url))
            .build();
        
        HttpResponse<String> response = client.send(
            request, 
            HttpResponse.BodyHandlers.ofString()
        );
        
        return url + " -> Status: " + response.statusCode();
    }
}

// This code creates a virtual thread per URL
// 1000 URLs = 1000 virtual threads (uses ~1MB total, not 1GB!)
// All requests happen concurrently
// Simple, readable, blocking code - JVM handles the magic
```