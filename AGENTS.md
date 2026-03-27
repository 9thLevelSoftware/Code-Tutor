# Repository Guidelines

This repository is a monorepo for the Code Tutor platform (native desktop app).

## Project Structure & Modules

- Native desktop app lives under `native-app-wpf/` (C#/.NET 8.0 WPF).
- Tests are in `native-app.Tests/` (xUnit E2E tests).
- Shared content and curricula are in `content/`.
- Documentation lives in `docs/`; helper scripts in `scripts/`.

## Build, Test, and Development

### Setup (First Time)
```bash
# Restore NuGet packages
dotnet restore native-app-wpf/CodeTutor.Wpf.csproj

# Restore dotnet tools (CSharpier formatter)
dotnet tool restore

# Optional: Install pre-commit hooks (requires Python)
pip install pre-commit
pre-commit install
```

### Daily Development
```bash
# Build the application
dotnet build native-app-wpf/CodeTutor.Wpf.csproj

# Run the application
dotnet run --project native-app-wpf/CodeTutor.Wpf.csproj

# Run tests
dotnet test native-app.Tests/native-app.Tests.csproj

# Format code with CSharpier
dotnet csharpier format native-app-wpf native-app.Tests
```

## Coding Style & Naming

- **C# Conventions**: Follow standard C# naming conventions (PascalCase for public members, camelCase with `_` prefix for private fields).
- **Indentation**: 4 spaces (configured in `.editorconfig`).
- **Linting**: StyleCop.Analyzers is configured with project-specific rules in `native-app-wpf/CodeTutor.ruleset`.
- **Formatting**: CSharpier is configured in `.csharpierrc` - run `dotnet csharpier format` before committing.
- Keep files small and focused; group features by domain.

## Testing Guidelines

- Tests are in `native-app.Tests/E2E/` with subdirectories for ContentValidation, CodeExecution, and UserJourneys.
- Name tests after the unit under test (e.g., `LearningJourneyTests.cs`).
- Ensure new features include tests and maintain coverage.
- Run `dotnet test --list-tests` to verify tests are discoverable.

## Commit & Pull Request Practices

- Write clear, imperative commit messages (e.g., `Add course import validation`).
- Run `dotnet build` and `dotnet test` before committing.
- Format code with `dotnet csharpier format` before committing.
- For PRs, include a short summary, testing notes, and screenshots for UI changes.
- Link related issues and call out breaking changes explicitly.

