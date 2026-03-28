---
type: "KEY_POINT"
title: "✏️ Quiz Answer Key"
---


**Question 1**: **B) Updates only books published before 1950, incrementing their year by 1**

Explanation: The WHERE clause `{ Books.year less 1950 }` filters to only books before 1950, then `year + 1` increments each one.

---

**Question 2**: **B) innerJoin only returns rows where both tables have matches; leftJoin returns all rows from the left table**

Explanation: INNER JOIN requires matches in both tables. LEFT JOIN returns all left table rows, with NULL for unmatched right table columns.

---

**Question 3**: **C) They're much faster and use fewer database connections**

Explanation: Batch operations send multiple records in a single database round-trip, dramatically improving performance compared to individual operations in a loop.

---

**Congratulations!** You now have complete CRUD mastery with Exposed! 🎉

