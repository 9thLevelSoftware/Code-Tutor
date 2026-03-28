---
type: "EXAMPLE"
title: "Best Practices"
---


### ✅ DO:
1. **Use environment variables** for different Firebase projects (dev, staging, prod)
2. **Enable App Check** (prevents abuse from unauthorized apps)
3. **Set up security rules** before going to production
4. **Monitor usage** to avoid surprise bills
5. **Use emulators** for local testing (covered in later lessons)

### ❌ DON'T:
1. **Don't share API keys publicly** (though they're not super sensitive, still avoid it)
2. **Don't commit `.env` files** with secrets
3. **Don't skip security rules** (anyone can read/write by default!)
4. **Don't use production Firebase** for testing

