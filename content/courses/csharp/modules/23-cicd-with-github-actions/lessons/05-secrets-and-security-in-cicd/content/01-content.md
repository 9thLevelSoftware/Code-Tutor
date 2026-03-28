---
type: "THEORY"
title: "Secure Secret Management in CI/CD"
---

Secrets are sensitive pieces of information that your application needs to function: database passwords, API keys, service credentials, and encryption keys. Exposing these secrets can lead to data breaches, unauthorized access, and significant security incidents. Proper secret management in CI/CD pipelines is essential for maintaining the security posture of your applications.

**The Principle of Least Exposure**

Never hardcode secrets directly in your source code or configuration files. Even private repositories can be compromised, and history contains all previous versions forever. Instead, use dedicated secret management systems designed specifically for secure credential storage and access control.

**GitHub Secrets**

GitHub provides built-in secret management for repositories and organizations. Secrets are encrypted and only exposed to workflows that explicitly need them. Set up repository secrets through Settings > Secrets and variables > Actions. These secrets are available to all workflows in that repository.

For environment-specific secrets, use GitHub Environments. Create environments like "staging" and "production", then assign secrets to specific environments. Workflows can then gate access based on branch protection rules and required reviews. This ensures production secrets are never available during development builds.

**Accessing Secrets in Workflows**

Reference secrets in your workflow files using the secrets context:

```yaml
- name: Deploy to Production
  env:
    DATABASE_PASSWORD: ${{ secrets.DATABASE_PASSWORD }}
    API_KEY: ${{ secrets.API_KEY }}
  run: dotnet ef database update --connection "Server=...;Password=${DATABASE_PASSWORD}"
```

The secret values are automatically masked in logs, preventing accidental exposure through console output.

**Secret Rotation and Lifecycle**

Production secrets should rotate regularly. Implement automated rotation where possible, or establish calendar reminders for manual rotation. Document which secrets need rotation and the procedures for updating them. Test your rotation procedures regularly to ensure they work when needed.

When rotating secrets, update the value in your secret vault first, then verify the new value works in a staging environment before relying on it in production. This staged approach prevents service interruptions during rotation.