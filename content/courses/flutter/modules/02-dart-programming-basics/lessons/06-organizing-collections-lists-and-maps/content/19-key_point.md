---
type: KEY_POINT
---

- `List<T>` stores ordered, indexed items accessed by position starting at index 0
- `Map<K, V>` stores key-value pairs for fast lookup by key -- use it when you need to find items by a unique identifier
- Use `add()`, `remove()`, and `contains()` for list manipulation; use `map[key]` for map access and `map[key] = value` for insertion
- Iterate with `for-in` loops or `forEach()` when you need to process every item in a collection
- Choose the right collection: List for ordered sequences, Map for named lookups, Set for unique-only items
