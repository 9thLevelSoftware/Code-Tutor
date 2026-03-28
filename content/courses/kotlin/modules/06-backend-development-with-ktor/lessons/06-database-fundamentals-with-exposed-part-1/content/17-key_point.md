---
type: "KEY_POINT"
title: "✏️ Quiz Answer Key"
---


**Question 1**: **B) It's completely lost**

Explanation: In-memory databases (jdbc:h2:mem:) store everything in RAM. When the process ends, all data is lost. For persistence, use file-based storage (jdbc:h2:file:./data/mydb).

---

**Question 2**: **B) Automatically generates unique sequential IDs for new rows**

Explanation: autoIncrement() tells the database to automatically assign incrementing IDs (1, 2, 3, ...) when you insert new rows, removing the need to manually specify IDs.

---

**Question 3**: **B) To ensure all-or-nothing execution and maintain data consistency**

Explanation: Transactions provide ACID guarantees. If any operation fails, all changes are rolled back, preventing partial updates that could corrupt your data.

---

**Congratulations!** You've connected your API to a real database! Your apps can now remember things! 🎉

