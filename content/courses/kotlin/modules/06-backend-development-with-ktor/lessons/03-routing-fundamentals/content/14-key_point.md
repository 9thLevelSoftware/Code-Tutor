---
type: "KEY_POINT"
title: "✏️ Quiz Answer Key"
---


**Question 1**: **B) `/books/{id}`**

Explanation: The `route("/books")` sets the base path, and `get("/{id}")` appends to it, resulting in `/books/{id}`.

---

**Question 2**: **C) 400 Bad Request**

Explanation: 400 indicates the client sent invalid data. The request format is correct (it's JSON), but the content violates business rules (empty title).

---

**Question 3**: **B) Converts the JSON request body into a CreateBookRequest object**

Explanation: `call.receive<T>()` uses kotlinx.serialization to automatically parse the JSON body into the specified Kotlin type. It's the "receive" counterpart to "respond".

---

**Congratulations!** You've built a complete REST API with full CRUD operations! You now have a real, testable backend that handles JSON data. 🎉

