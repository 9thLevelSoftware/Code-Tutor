---
type: "THEORY"
title: "Finding Elements with querySelector"
---

**Estimated Time**: 45 minutes
**Difficulty**: Beginner

## Introduction

The DOM (Document Object Model) represents your HTML as a tree of objects. To interact with the page—changing text, adding event listeners, or modifying styles—you first need to find the elements you want to work with. The `querySelector` family of methods provides a powerful, CSS-based way to target elements.

**Learning Objectives**:
- Use `querySelector()` to find single elements
- Use `querySelectorAll()` to find multiple elements
- Write CSS selectors that target elements by ID, class, tag, and attributes
- Understand the difference between NodeLists and Arrays

## The querySelector Methods

```javascript
// Find first matching element
const header = document.querySelector('#main-header');
const button = document.querySelector('.btn-primary');
const link = document.querySelector('a[href="/about"]');

// Find ALL matching elements
const allButtons = document.querySelectorAll('button');
const navLinks = document.querySelectorAll('.nav-link');
```

## CSS Selector Power

Selector | Matches
---------|----------
`#id` | Element with specific ID
`.class` | Elements with specific class
`tag` | Elements with specific tag name
`[attr]` | Elements with specific attribute
`parent > child` | Direct children
`ancestor descendant` | Nested descendants
`:first-child` | First child of its parent
`:nth-child(n)` | Nth child of its parent

## Real-World Context

Every interactive website uses element selection. Form validation finds input fields. Image galleries target thumbnails. Dropdown menus select toggle buttons. Mastering `querySelector` is the gateway to DOM manipulation and dynamic web applications.

The jQuery library became famous largely for simplifying element selection—modern `querySelector` makes that power available natively in all browsers.

Find elements precisely, then make them dance!