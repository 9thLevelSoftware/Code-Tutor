# Create a pyproject.toml for a FastAPI project
# Requirements:
# - Project name: my-api
# - Version: 0.1.0
# - Python >= 3.12
# - Dependencies: fastapi, pydantic, uvicorn
# - Dev dependencies: pytest, httpx, ruff

pyproject_content = '''
# TODO: Write your pyproject.toml content here
'''

# Display the configuration
print("Generated pyproject.toml:")
print("=" * 50)
print(pyproject_content)

# Validate structure
if "[project]" in pyproject_content:
    print("\n[OK] Has [project] section")
if "fastapi" in pyproject_content.lower():
    print("[OK] Has FastAPI dependency")
if "requires-python" in pyproject_content:
    print("[OK] Has Python version requirement")