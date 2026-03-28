---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 35 minutes
**Difficulty**: Beginner-Intermediate
**Prerequisites**: Lessons 5.1-5.4 (HTTP, Ktor setup, routing, parameters)

## Introduction

JSON is the universal language of modern APIs. Ktor integrates seamlessly with kotlinx.serialization, Kotlin's official serialization library, to automatically convert between Kotlin objects and JSON payloads.

In this lesson, you'll learn:

1. **Content Negotiation**: Installing and configuring the serialization plugin
2. **@Serializable**: Annotating data classes for automatic conversion
3. **Receive and Respond**: Using `call.receive<T>()` and `call.respond()`
4. **Custom Serializers**: Handling dates, enums, and complex nested types

## Why It Matters

Manual JSON parsing is error-prone and tedious. kotlinx.serialization generates serialization code at compile time, ensuring type safety and excellent performance. Content negotiation allows your API to support multiple formats (JSON, XML, CBOR) without changing your endpoint code.

## Real-World Context

Every modern API exchanges JSON: payment processors transmit transaction data, weather services return forecast objects, and social platforms send user profiles. kotlinx.serialization powers serialization at JetBrains, Gradle, and in Android's official architecture samples. Mastering this integration is essential for professional Kotlin backend development.

Let's serialize some JSON!

