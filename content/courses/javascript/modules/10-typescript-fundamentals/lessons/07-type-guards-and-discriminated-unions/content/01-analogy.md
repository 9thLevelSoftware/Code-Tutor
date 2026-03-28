---
type: "ANALOGY"
title: "The Badge and the Scanner"
---

Imagine you're a security guard at a high-tech building with three different doors: one for Staff, one for Maintenance, and one for Visitors.

Everyone entering the building is wearing a generic "Authorized Personnel" jacket (this is a **Union Type**). To get them through the correct door, you need to check their badge.

1.  **The Scanner (Type Guard):** You use a machine to check the jacket. If the machine says "This is a Staff Member," you let them through the Staff door. Once they are through that door, you **know** they have a staff login and a desk number. 
2.  **The Badge Label (Discriminant):** To make the scanner's job easier, every jacket has a specific tag: `type: "staff"`, `type: "guest"`, or `type: "admin"`. By looking at this one specific label, you can immediately tell everything else about the person.

In TypeScript, **Type Guards** are the scanners. They allow you to "narrow" a general type (like "User") into a specific type (like "Admin") so you can safely use properties that only exist on that specific type.
