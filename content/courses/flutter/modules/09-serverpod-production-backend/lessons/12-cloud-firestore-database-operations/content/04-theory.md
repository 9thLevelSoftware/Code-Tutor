---
type: "THEORY"
title: "Firestore Structure"
---


### Collections and Documents


**Key Concepts**:
- **Collection**: Container for documents (like a folder)
- **Document**: Individual record with key-value pairs (like a file)
- **Documents must be inside collections** (alternating structure)
- **Documents can contain subcollections**



```dart
firestore_database/
├── users/ (Collection)
│   ├── user123/ (Document)
│   │   ├── name: "Alice"
│   │   ├── email: "alice@example.com"
│   │   └── posts/ (Subcollection)
│   │       ├── post1/ (Document)
│   │       │   ├── title: "My First Post"
│   │       │   └── content: "Hello world!"
│   │       └── post2/ (Document)
│   │           ├── title: "Second Post"
│   │           └── content: "Still learning!"
│   │
│   └── user456/ (Document)
│       ├── name: "Bob"
│       └── email: "bob@example.com"
│
└── posts/ (Collection)
    ├── post123/ (Document)
    │   ├── title: "Flutter is Amazing"
    │   ├── authorId: "user123"
    │   └── likes: 42
    └── post456/ (Document)
        ├── title: "Learning Firestore"
        ├── authorId: "user456"
        └── likes: 15
```
