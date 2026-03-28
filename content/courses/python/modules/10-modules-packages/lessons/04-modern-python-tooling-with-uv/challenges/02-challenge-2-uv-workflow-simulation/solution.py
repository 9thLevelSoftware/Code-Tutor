def new_project_workflow(project_name: str, python_version: str = "3.12") -> list[str]:
    """Generate commands to create a new uv project.
    
    Args:
        project_name: Name of the new project
        python_version: Python version to use (default: 3.12)
    
    Returns:
        List of terminal commands to execute
    """
    return [
        f"uv python install {python_version}",
        f"uv init {project_name} --python {python_version}",
        f"cd {project_name}",
        "uv add --dev pytest ruff",
        "uv run python hello.py",
    ]

def clone_project_workflow(repo_url: str) -> list[str]:
    """Generate commands to clone and set up an existing project.
    
    Args:
        repo_url: Git repository URL
    
    Returns:
        List of terminal commands to execute
    """
    # Extract project name from URL
    project_name = repo_url.rstrip("/").split("/")[-1].replace(".git", "")
    
    return [
        f"git clone {repo_url}",
        f"cd {project_name}",
        "uv sync",
        "uv run python -m pytest",
    ]

def add_dependencies_workflow(packages: list[str], dev: bool = False) -> list[str]:
    """Generate commands to add dependencies.
    
    Args:
        packages: List of package names to add
        dev: If True, add as dev dependencies
    
    Returns:
        List of uv commands
    """
    if not packages:
        return ["# No packages specified"]
    
    packages_str = " ".join(packages)
    dev_flag = "--dev " if dev else ""
    
    return [
        f"uv add {dev_flag}{packages_str}",
        "# pyproject.toml and uv.lock are automatically updated",
        "git add pyproject.toml uv.lock",
        f"git commit -m 'Add {', '.join(packages)}'",
    ]

def update_dependencies_workflow(package: str | None = None) -> list[str]:
    """Generate commands to update dependencies.
    
    Args:
        package: Specific package to update, or None for all
    
    Returns:
        List of uv commands
    """
    if package:
        return [
            f"uv lock --upgrade-package {package}",
            "uv sync",
            "uv run python -m pytest",
            f"git commit -am 'Update {package}'",
        ]
    else:
        return [
            "uv lock --upgrade",
            "uv sync",
            "uv run python -m pytest",
            "git commit -am 'Update all dependencies'",
        ]

# Test all workflows
print("=" * 60)
print("uv WORKFLOW COMMAND GENERATOR")
print("=" * 60)

print("\n=== 1. New Project Workflow ===")
print("Creating a new Python 3.12 project named 'my-app':")
for cmd in new_project_workflow("my-app", "3.12"):
    print(f"  $ {cmd}")

print("\n=== 2. Clone Project Workflow ===")
print("Cloning and setting up an existing project:")
for cmd in clone_project_workflow("https://github.com/user/awesome-api.git"):
    print(f"  $ {cmd}")

print("\n=== 3. Add Dependencies Workflow ===")
print("Adding production dependencies:")
for cmd in add_dependencies_workflow(["requests", "pandas"]):
    print(f"  $ {cmd}")

print("\nAdding dev dependencies:")
for cmd in add_dependencies_workflow(["pytest", "black"], dev=True):
    print(f"  $ {cmd}")

print("\n=== 4. Update Dependencies Workflow ===")
print("Updating a specific package:")
for cmd in update_dependencies_workflow("requests"):
    print(f"  $ {cmd}")

print("\nUpdating all packages:")
for cmd in update_dependencies_workflow():
    print(f"  $ {cmd}")

print("\n" + "=" * 60)
print("Key uv Commands Summary:")
print("=" * 60)
print("""
  uv init <name>          Create new project
  uv add <package>        Add dependency
  uv add --dev <package>  Add dev dependency
  uv sync                 Install from lock file
  uv run <command>        Run with project deps
  uv lock --upgrade       Update all packages
  uv python install       Install Python version
""")