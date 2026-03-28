---
type: "KEY_POINT"
title: "Key Takeaways"
---

**Context parameters (formerly context receivers) pass implicit arguments** to functions without explicit parameters. Write `context(logger: Logger) fun process()` to automatically receive Logger from calling context.

**Context parameters are Beta in Kotlin 2.2 and experimental in Kotlin 2.3.4**—enable with `-Xcontext-parameters` compiler flag. They're experimental, so APIs may change before stabilization.

**Use context parameters for cross-cutting concerns** like logging, transaction scopes, or dependency injection—things you want available everywhere without explicit passing. This reduces boilerplate while maintaining type safety.
