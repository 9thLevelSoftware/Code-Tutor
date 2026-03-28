---
type: "THEORY"
title: "Multiple Paths - else if and else"
---

**Estimated Time**: 50 minutes
**Difficulty**: Beginner

## Introduction

Life rarely offers just two choices, and neither do programs. The `else if` and `else` constructs let your code handle multiple scenarios gracefully—like a flowchart that adapts to any situation it encounters.

In this lesson, you'll learn:

1. **Chaining Conditions**: Using `else if` to check multiple possibilities
2. **Default Actions**: The `else` clause as a catch-all
3. **Execution Flow**: Understanding how conditions are evaluated in sequence
4. **Nesting Decisions**: When to nest vs. when to flatten

## Building Decision Chains

Start with the most specific case and work toward the general:

```javascript
function getTicketPrice(age) {
    if (age < 5) {
        return "Free";
    } else if (age < 13) {
        return "$10 (Child)";
    } else if (age < 65) {
        return "$20 (Adult)";
    } else {
        return "$15 (Senior)";
    }
}
```

**Key insight**: Once a condition is `true`, its block executes and the rest are skipped. Order matters—putting `age < 65` before `age < 13` would prevent the child discount from ever applying.

## Else as Safety Net

The `else` clause handles everything not caught by previous conditions:

```javascript
function processInput(value) {
    if (typeof value === "string" && value.trim() !== "") {
        return value.toUpperCase();
    } else if (typeof value === "number" && value > 0) {
        return value * 2;
    } else {
        return "Invalid input";
    }
}
```

## Nested vs. Flattened

**Nested (harder to read):**
```javascript
if (user) {
    if (user.isActive) {
        if (user.hasPermission) {
            showDashboard();
        }
    }
}
```

**Flattened (clearer):**
```javascript
if (!user) return;
if (!user.isActive) return;
if (!user.hasPermission) return;
showDashboard();
```

## Real-World Context

Multi-path decisions appear constantly:
- **User roles**: Admin, editor, viewer, guest—each gets different UI
- **Payment processing**: Credit card, PayPal, crypto, invoice
- **Game AI**: Aggressive, defensive, or neutral based on health/resources
- **Recommendation engines**: New user, returning user, premium subscriber

Understanding `else if` chains prevents the "pyramid of doom"—deeply nested code that's impossible to follow. Well-structured conditionals make complex business logic readable.

## Best Practices

1. **Order conditions by specificity** or by frequency (check common cases first)
2. **Keep nesting shallow**—extract complex conditions to functions
3. **Consider switch** for comparing one variable against many values
4. **Early returns** often flatten nested logic beautifully

Multiple paths give your code the flexibility to handle the real world's complexity.