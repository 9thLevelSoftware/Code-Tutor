---
type: "THEORY"
title: "Understanding Git Basics"
---

**Estimated Time**: 45 minutes

**Learning Objectives**: By the end of this lesson, you will understand version control fundamentals, Git's core concepts, and how to initialize and manage repositories effectively.

---

## Introduction to Version Control

Version control is a system that records changes to files over time, allowing you to recall specific versions later. Think of it as a "save history" for your entire project, not just individual files. Git is the most widely-used distributed version control system in software development today.

**Real-World Relevance**: Whether you're working solo or in a team of hundreds, Git enables you to track changes, collaborate seamlessly, and recover from mistakes. Companies like Google, Microsoft, and Netflix rely on Git to manage millions of lines of code.

## Core Git Concepts

### Repositories
A Git repository is a directory that contains all project files and the complete history of changes. To initialize a new repository:

```bash
# Create a new directory for your project
mkdir my-project
cd my-project

# Initialize a Git repository
git init
```

### The Three States
Git manages file states in three areas:

1. **Working Directory**: Where you actively edit files
2. **Staging Area (Index)**: A preparation area for your next commit
3. **Repository (.git folder)**: Where committed snapshots are permanently stored

### Basic Workflow Commands

```bash
# Check the status of your repository
git status

# Add files to the staging area
git add filename.txt
git add .  # Add all changes

# Commit changes with a descriptive message
git commit -m "Add user authentication feature"

# View commit history
git log --oneline
```

## Best Practices

- **Commit often**: Small, focused commits are easier to understand and debug
- **Write meaningful commit messages**: Explain *why* you made changes, not just *what*
- **Use .gitignore**: Exclude build artifacts, dependencies, and sensitive files
- **Never commit secrets**: Passwords, API keys, and credentials should never be tracked

Understanding Git basics is foundational for any developer. These skills transfer across all programming languages and development environments.