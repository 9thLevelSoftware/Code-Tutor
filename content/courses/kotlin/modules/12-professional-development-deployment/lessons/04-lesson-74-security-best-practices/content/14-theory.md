---
type: "THEORY"
title: "Exercise 3: Secure File Upload"
---

**Estimated Time**: 50 minutes
**Difficulty**: Intermediate

## Objective

Create a secure file upload endpoint that validates file types, limits sizes, scans for malware, and stores files safely—protecting your server from malicious uploads.

---

## Learning Goals

By completing this exercise, you will:
- Validate file extensions and magic numbers (not just filenames)
- Implement size limits to prevent storage exhaustion
- Scan uploads using ClamAV or cloud scanning services
- Store files outside the web root with randomized names

## Background

File uploads are a common attack vector. Malicious users may attempt to upload:
- Executable scripts disguised as images
- Oversized files to exhaust disk space
- Files with path traversal sequences (`../../etc/passwd`)
- Double extensions (`file.jpg.php`)

## Implementation Requirements

Your endpoint should:
1. Accept multipart file uploads via POST /api/uploads
2. Validate file type by inspecting magic numbers (first bytes)
3. Enforce maximum file size (e.g., 10MB)
4. Generate random filenames to prevent collisions
5. Store files outside web-accessible directories
6. Return secure download URLs (not direct file paths)

## Real-World Context

Cloudinary, AWS S3, and Imgur all implement strict upload validation. They check file signatures, scan for viruses, and serve files through CDNs rather than exposing storage directly. The techniques you'll implement mirror production practices at these services.

## Security Checklist

- [ ] Whitelist allowed file types (not blacklist)
- [ ] Verify MIME type matches file extension
- [ ] Limit total upload size per user/day
- [ ] Store metadata separately from file content
- [ ] Log all upload attempts for audit purposes
