---
type: "KEY_POINT"
title: "✏️ Quiz Answer Key"
---


**Question 1**: **B) To separate data access logic from business logic**

Explanation: The Repository Pattern creates a clean separation between how data is stored/retrieved and how it's used in business logic. This makes code more maintainable and testable.

---

**Question 2**: **B) Service layer (business logic)**

Explanation: Validation is business logic. Services validate data, enforce rules, and coordinate operations. Routes just handle HTTP, repositories just access data.

---

**Question 3**: **C) They enable testing with mock implementations and allow swapping implementations**

Explanation: Interfaces let you create mock repositories for testing (no database needed) and easily swap implementations (e.g., SQL to NoSQL, add caching) without changing dependent code.

---

**Congratulations!** You now understand professional backend architecture! 🎉

