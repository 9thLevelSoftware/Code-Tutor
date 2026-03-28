---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 35 minutes
**Difficulty**: Beginner
**Prerequisites**: Lesson 5.1 (HTTP Fundamentals), Kotlin basics

## Introduction

Welcome to your first hands-on experience with Ktor! In this lesson, you'll set up a working Ktor project from scratch and understand the fundamental structure of a Ktor application.

Ktor 3.4 provides multiple ways to get started: the Ktor Plugin for IntelliJ IDEA, the online project generator, or manual Gradle configuration. We'll explore the IntelliJ plugin approach as it provides the smoothest development experience with built-in run configurations and debugging support.

You'll learn:

1. **Project Structure**: Understanding the `build.gradle.kts` configuration, application entry point, and plugin system
2. **Application Module**: How the `Application.module()` function serves as the configuration hub
3. **Embedded Server**: How Ktor embeds Netty/Jetty for zero-deployment development
4. **Development Workflow**: Running, testing, and debugging your Ktor application

## Why It Matters

Setting up a project correctly is the foundation of productive backend development. Understanding Ktor's modular architecture—where features are added as plugins—prepares you for building scalable APIs. The skills you learn here (Gradle configuration, dependency injection setup, logging) transfer directly to production environments.

## Real-World Context

Most production Ktor applications start exactly as you'll create today. Whether building a microservice at a fintech startup or an API for a mobile app, the project structure and configuration patterns remain consistent. Companies like Netflix, JetBrains, and Cash App use Ktor in production for its lightweight nature and coroutine-based performance.

Let's build your first Ktor server!

