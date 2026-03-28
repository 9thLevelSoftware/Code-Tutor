---
type: "EXAMPLE"
title: "Best Practices"
---


### ✅ DO:
1. **Use StreamBuilder** for real-time data
2. **Index frequently queried fields** (Firebase Console → Indexes)
3. **Denormalize data** when needed (duplicate for read performance)
4. **Use batch writes** for multiple updates
5. **Paginate large datasets** (use `.limit()` and `.startAfter()`)
6. **Handle offline mode** (Firestore caches automatically)
7. **Use Timestamps** for dates (not Strings)

### ❌ DON'T:
1. **Don't fetch entire collections** (use queries with filters)
2. **Don't nest data too deeply** (max 3-4 levels)
3. **Don't use client-side filtering** (use Firestore queries)
4. **Don't store large files** in documents (use Cloud Storage)
5. **Don't forget security rules** (covered in next lesson)

