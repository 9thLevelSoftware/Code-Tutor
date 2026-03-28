---
type: "WARNING"
title: "Common Pitfalls"
---

1. **Don't double-escape**
```javascript
// WRONG: Calling escape twice
const bad = RegExp.escape(RegExp.escape('test.'));
// 'test\\\\.'' - Oops, too many backslashes!

// RIGHT: Escape once
const good = RegExp.escape('test.');
// 'test\\.'
```

2. **Escape before combining, not after**
```javascript
// WRONG: Escaping the full pattern
const search = 'file';
const bad = RegExp.escape(`${search}.*`);
// 'file\\.\\*' - Escaped the intentional .* too!

// RIGHT: Escape only the user input
const good = `${RegExp.escape(search)}.*`;
// 'file.*' - .* still works as wildcard
```

3. **Runtime support check**
```javascript
// For older runtimes, check availability
if (typeof RegExp.escape !== 'function') {
  // Polyfill or fallback
  RegExp.escape = (str) => str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}
```

4. **ES2025 Status**

`RegExp.escape` is an ES2025 feature. Check runtime support before using:

| Runtime | Version | Support |
|---------|---------|---------|
| Bun | 1.1+ | ✅ Yes |
| Node.js | 23+ (24+ stable) | ✅ Yes |
| Chrome | 134+ | ✅ Yes |
| Firefox | - | ❌ Not yet |
| Safari | - | ❌ Not yet |

For production use, include the polyfill (section 3) or use a library like `lodash.escaperegexp` for broader compatibility.