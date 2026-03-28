---
type: "THEORY"
title: "Setting Up User Registration"
---


### Step 1: Add bcrypt Dependency

Add bcrypt to your `build.gradle.kts`:


Sync your Gradle project to download the dependency.

### Step 2: Create User Model and Table


**Key Security Principle**: The `User` model exposed to clients NEVER includes the password hash. That stays in the database layer only.

### Step 3: Create Password Hashing Utility


### Step 4: Create Password Validator

Strong passwords are essential. Let's enforce requirements:


### Step 5: Create User Validator


### Step 6: Create User Repository


### Step 7: Create User Service with Registration Logic


### Step 8: Create Registration Route


### Step 9: Update Database Factory

Add the Users table to schema creation:


### Step 10: Wire Everything Together


---



```kotlin
// src/main/kotlin/com/example/Application.kt
package com.example

import com.example.database.DatabaseFactory
import com.example.plugins.configureErrorHandling
import com.example.repositories.UserRepositoryImpl
import com.example.routes.authRoutes
import com.example.services.UserService
import io.ktor.serialization.kotlinx.json.*
import io.ktor.server.application.*
import io.ktor.server.cio.*
import io.ktor.server.engine.*
import io.ktor.server.plugins.contentnegotiation.*
import io.ktor.server.routing.*
import kotlinx.serialization.json.Json

fun main() {
    embeddedServer(CIO, port = 8080, module = Application::module).start(wait = true)
}

fun Application.module() {
    // Install plugins
    install(ContentNegotiation) {
        json(Json {
            prettyPrint = true
            ignoreUnknownKeys = true
        })
    }

    // Install error handling
    configureErrorHandling()

    // Initialize database
    DatabaseFactory.init()

    // Create dependencies
    val userRepository = UserRepositoryImpl()
    val userService = UserService(userRepository)

    // Configure routes
    routing {
        authRoutes(userService)
    }
}
```
