---
type: "THEORY"
title: "Two Approaches to Web Frontends"
---

You have built the complete Spring Boot backend: entities, repositories, services, REST API, authentication, and validation. Now it is time to add a frontend so users can actually interact with your application.

> **Remember:** Before testing your React frontend against the Spring Boot backend, ensure CORS is configured! 
> See [M13.L6: Connecting to Spring Boot](/courses/java/modules/13-react-frontend-integration/lessons/06-lesson-r6-connecting-to-spring-boot) for CORS setup instructions.

You have two choices, and this lesson covers both. Pick the one that fits your goals:

Server-Side Rendering with Thymeleaf:
Thymeleaf is a Java template engine that integrates directly with Spring MVC. The server generates complete HTML pages and sends them to the browser. Forms submit data via standard HTTP POST requests. There is no separate frontend project, no JavaScript framework, and no API calls -- everything lives inside your Spring Boot application.

Choose Thymeleaf if you want to:
- Stay in the Java ecosystem (no JavaScript/Node.js required)
- Deploy a single JAR file with everything included
- Build something quickly without learning a new framework
- Create traditional web applications with server-rendered pages

Client-Side Rendering with React:
React is a JavaScript library that runs in the browser. It communicates with your Spring Boot backend through the REST API you already built. The frontend is a separate project with its own build process, and it runs independently from the backend.

Choose React if you want to:
- Build a single-page application with dynamic, instant UI updates
- Gain experience with the most popular JavaScript framework
- Practice full-stack development with separate frontend and backend
- Deploy a multi-service architecture

Both approaches produce a fully functional Task Manager. The backend you built in Lessons 01-05 works with either path -- that is the power of a well-designed service layer.

Read the sections below for your chosen path. If you are unsure, start with Thymeleaf -- it is faster to set up and lets you focus on the Java skills you have been building throughout this course.
