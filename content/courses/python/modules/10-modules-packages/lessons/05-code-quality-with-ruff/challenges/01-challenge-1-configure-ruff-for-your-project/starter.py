# Create a Ruff configuration for a FastAPI project
# Project name: my_api

ruff_config = '''
[tool.ruff]
# TODO: Add line-length
# TODO: Add target-version

[tool.ruff.lint]
# TODO: Add rule selection
# TODO: Add ignores if needed

# TODO: Add per-file ignores for scripts/

[tool.ruff.format]
# TODO: Configure formatting options
'''

print("Ruff Configuration:")
print("=" * 50)
print(ruff_config)

# Validate the configuration
if "py312" in ruff_config or "py312" in ruff_config:
    print("\n[OK] Targets Python 3.12")
if "88" in ruff_config:
    print("[OK] Line length is 88")
if '"E"' in ruff_config and '"F"' in ruff_config:
    print("[OK] Has E and F rules enabled")