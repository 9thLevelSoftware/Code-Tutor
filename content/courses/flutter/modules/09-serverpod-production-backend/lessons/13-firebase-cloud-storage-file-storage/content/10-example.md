---
type: "EXAMPLE"
title: "Best Practices"
---


### ✅ DO:
1. **Compress images before upload** (use image_picker maxWidth/quality)
2. **Use unique filenames** (timestamp + random string)
3. **Organize by user ID** (`users/{userId}/...`)
4. **Validate file types and sizes** in security rules
5. **Delete old files** when uploading new ones (avoid storage bloat)
6. **Show upload progress** for better UX
7. **Handle errors gracefully** (network issues, quota exceeded)
8. **Use CDN URLs** (Firebase provides these automatically)

### ❌ DON'T:
1. **Don't upload full-resolution images** (compress first!)
2. **Don't store sensitive data** in filenames
3. **Don't allow public write access** (use authentication)
4. **Don't forget to delete old files** (costs add up)
5. **Don't upload without size limits** (prevent abuse)
6. **Don't use HTTP URLs** (always HTTPS)

