def new_project_workflow(project_name: str, python_version: str = "3.12") -> list[str]:
    """Generate commands to create a new uv project.
    
    Args:
        project_name: Name of the new project
        python_version: Python version to use (default: 3.12)
    
    Returns:
        List of terminal commands to execute
    """
    # TODO: Return list of uv commands
    pass

def clone_project_workflow(repo_url: str) -> list[str]:
    """Generate commands to clone and set up an existing project.
    
    Args:
        repo_url: Git repository URL
    
    Returns:
        List of terminal commands to execute
    """
    # TODO: Return list of commands (git clone + uv sync)
    pass

def add_dependencies_workflow(packages: list[str], dev: bool = False) -> list[str]:
    """Generate commands to add dependencies.
    
    Args:
        packages: List of package names to add
        dev: If True, add as dev dependencies
    
    Returns:
        List of uv commands
    """
    # TODO: Return uv add commands
    pass

def update_dependencies_workflow(package: str | None = None) -> list[str]:
    """Generate commands to update dependencies.
    
    Args:
        package: Specific package to update, or None for all
    
    Returns:
        List of uv commands
    """
    # TODO: Return uv lock --upgrade commands
    pass

# Test the functions
print("=== New Project Workflow ===")
for cmd in new_project_workflow("my-app", "3.12") or []:
    print(f"  $ {cmd}")

print("\n=== Clone Project Workflow ===")
for cmd in clone_project_workflow("https://github.com/user/repo.git") or []:
    print(f"  $ {cmd}")