# Complete Ruff configuration for a FastAPI project

ruff_config = '''[tool.ruff]
# Match Black's formatting standards
line-length = 88

# Target Python 3.12 for modern syntax rules
target-version = "py312"

# Exclude common directories
exclude = [
    ".venv",
    "__pycache__",
    ".git",
    "migrations",
]

[tool.ruff.lint]
# Enable comprehensive rule set
select = [
    "E",     # pycodestyle errors
    "W",     # pycodestyle warnings  
    "F",     # pyflakes
    "I",     # isort
    "UP",    # pyupgrade
    "B",     # flake8-bugbear
    "SIM",   # flake8-simplify
    "C4",    # flake8-comprehensions
    "PTH",   # flake8-use-pathlib
]

# Rules to ignore
ignore = [
    "E501",  # Line too long (formatter handles this)
]

# Allow all auto-fixes
fixable = ["ALL"]
unfixable = []

# Per-file rule ignores
[tool.ruff.lint.per-file-ignores]
"scripts/*" = ["T201"]  # Allow print() in scripts
"tests/*" = ["S101"]    # Allow assert in tests
"__init__.py" = ["F401"]  # Allow unused imports in __init__

# Configure import sorting
[tool.ruff.lint.isort]
known-first-party = ["my_api"]
combine-as-imports = true
force-single-line = false

[tool.ruff.format]
# Use double quotes like Black
quote-style = "double"

# Use spaces for indentation
indent-style = "space"

# Unix-style line endings
line-ending = "lf"

# Keep magic trailing comma
skip-magic-trailing-comma = false
'''

print("Ruff Configuration for FastAPI Project:")
print("=" * 50)
print(ruff_config)

# Validate the configuration
print("\n" + "=" * 50)
print("Configuration Validation:")
print("=" * 50)

checks = [
    ("py312" in ruff_config, "Targets Python 3.12"),
    ("88" in ruff_config, "Line length is 88"),
    ('"E"' in ruff_config, "pycodestyle errors enabled"),
    ('"F"' in ruff_config, "pyflakes enabled"),
    ('"I"' in ruff_config, "isort enabled"),
    ('"UP"' in ruff_config, "pyupgrade enabled"),
    ('"B"' in ruff_config, "bugbear enabled"),
    ('"SIM"' in ruff_config, "simplify enabled"),
    ("per-file-ignores" in ruff_config, "Has per-file ignores"),
    ("scripts/*" in ruff_config, "Scripts folder configured"),
    ("known-first-party" in ruff_config, "isort knows first-party packages"),
    ("quote-style" in ruff_config, "Quote style configured"),
]

for passed, description in checks:
    status = "PASS" if passed else "FAIL"
    print(f"[{status}] {description}")

print("\n" + "=" * 50)
print("Usage:")
print("=" * 50)
print("""
# Format code:
uv run ruff format .

# Lint and fix:
uv run ruff check --fix .

# Check without fixing:
uv run ruff check .
""")