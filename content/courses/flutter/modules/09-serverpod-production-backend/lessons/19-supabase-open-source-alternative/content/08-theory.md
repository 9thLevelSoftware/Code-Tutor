---
type: "THEORY"
title: "Firebase vs Supabase: Quick Reference"
---


| Operation | Firebase | Supabase |
|-----------|----------|----------|
| **Init** | `Firebase.initializeApp()` | `Supabase.initialize(url, key)` |
| **Auth Sign Up** | `createUserWithEmailAndPassword()` | `auth.signUp(email, password)` |
| **Auth Sign In** | `signInWithEmailAndPassword()` | `auth.signInWithPassword()` |
| **Insert** | `collection('x').add(data)` | `from('x').insert(data)` |
| **Query** | `where('field', '==', val)` | `.eq('field', val)` |
| **Real-time** | `snapshots()` | `channel().onPostgresChanges()` |
| **Storage Upload** | `ref(path).putFile(file)` | `storage.from(bucket).upload()` |

### Migration Path

If you need to migrate from Firebase to Supabase:
1. Export Firestore data as JSON
2. Transform to relational format
3. Import to Supabase using `psql` or Dashboard
4. Update Flutter code (similar APIs make this straightforward)

