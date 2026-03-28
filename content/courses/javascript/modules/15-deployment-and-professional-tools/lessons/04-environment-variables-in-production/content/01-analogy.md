---
type: "ANALOGY"
title: "Understanding the Concept"
---

Imagine you own a restaurant chain:

Bad approach (hardcoded secrets):
- Write secret sauce recipe directly in the cookbook
- Every employee gets a copy
- Recipe is in every branch location
- Employee leaves → they have your secrets!
- Want to change recipe → reprint all cookbooks!

Good approach (environment variables):
- Cookbook says: "Use the secret sauce (see manager)"
- Each location stores recipe in a safe
- Only managers have the combination
- Employee leaves → recipe stays safe
- Change recipe → just update the safe

Environment variables work the same way:
- Code says: process.env.DATABASE_PASSWORD
- Secret stored separately (not in code!)
- Different value per environment (dev vs prod)
- Change secrets without changing code
- Never committed to Git (safe!)

Think of env vars as a safe for your app's secrets!