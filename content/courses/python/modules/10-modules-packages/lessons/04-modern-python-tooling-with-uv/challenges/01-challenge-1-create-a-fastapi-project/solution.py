# Complete pyproject.toml for a FastAPI project
# This demonstrates modern Python project configuration

pyproject_content = '''[project]
name = "my-api"
version = "0.1.0"
description = "A modern FastAPI web API"
readme = "README.md"
requires-python = ">=3.12"
dependencies = [
    "fastapi>=0.115.0",
    "pydantic>=2.10.0",
    "uvicorn[standard]>=0.32.0",
]

[project.optional-dependencies]
dev = [
    "pytest>=8.0.0",
    "httpx>=0.28.0",
    "ruff>=0.8.0",
]

[build-system]
requires = ["hatchling"]
build-backend = "hatchling.build"

[tool.ruff]
line-length = 88
target-version = "py312"

[tool.ruff.lint]
select = ["E", "F", "I", "UP", "B"]

[tool.pytest.ini_options]
testpaths = ["tests"]
asyncio_mode = "auto"
'''

# Display the configuration
print("Generated pyproject.toml:")
print("=" * 50)
print(pyproject_content)

# Validate the structure
print("\n" + "=" * 50)
print("Validation:")
print("=" * 50)

checks = [
    ("[project]" in pyproject_content, "Has [project] section"),
    ("fastapi" in pyproject_content.lower(), "Has FastAPI dependency"),
    ("pydantic" in pyproject_content.lower(), "Has Pydantic dependency"),
    ("uvicorn" in pyproject_content.lower(), "Has Uvicorn dependency"),
    ("requires-python" in pyproject_content, "Has Python version requirement"),
    (">=3.12" in pyproject_content, "Requires Python 3.12+"),
    ("[project.optional-dependencies]" in pyproject_content, "Has dev dependencies"),
    ("pytest" in pyproject_content.lower(), "Has pytest for testing"),
    ("ruff" in pyproject_content.lower(), "Has ruff for linting"),
    ("[tool.ruff]" in pyproject_content, "Has Ruff configuration"),
]

for passed, description in checks:
    status = "PASS" if passed else "FAIL"
    print(f"[{status}] {description}")

print("\n" + "=" * 50)
print("To use this project:")
print("=" * 50)
print("""
1. Save as pyproject.toml
2. Run: uv sync
3. Run: uv run uvicorn main:app --reload
""")