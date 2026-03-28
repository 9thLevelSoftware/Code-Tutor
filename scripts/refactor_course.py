from pathlib import Path
import json
import re
import sys
import shutil
import logging
from typing import Optional, Union, Any

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(levelname)s: %(message)s')
logger = logging.getLogger(__name__)

# Mapping of course language IDs to file extensions
LANGUAGE_EXTENSIONS: dict[str, str] = {
    "csharp": "cs",
    "python": "py",
    "java": "java",
    "javascript": "js",
    "typescript": "ts",
    "cpp": "cpp",
    "cplusplus": "cpp",
    "go": "go",
    "golang": "go",
    "rust": "rs",
    "swift": "swift",
    "kotlin": "kt",
    "dart": "dart",
    "flutter": "dart",
    "html": "html",
    "css": "css",
    "sql": "sql"
}

def slugify(name: str) -> str:
    """
    Converts text to a slug (e.g., "Getting Started" -> "getting-started").
    """
    if not name:
        return "unknown"
    text = name.lower()
    text = re.sub(r'[^a-z0-9\s-]', '', text)
    text = re.sub(r'[\s_-]+', '-', text)
    return text.strip('-')

def write_file(path: Path, content: str) -> None:
    """Writes content to a file, ensuring the directory exists."""
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content)

def write_json(path: Path, data: dict) -> None:
    """Writes data to a JSON file with indentation."""
    write_file(path, json.dumps(data, indent=2, ensure_ascii=False))

def get_frontmatter(metadata: dict) -> str:
    """Generates YAML frontmatter string from a dict."""
    yaml = "---\n"
    for key, value in metadata.items():
        if value:
            # Escape quotes if necessary, simplified for this use case
            clean_value = str(value).replace('"', '\\"')
            yaml += f'{key}: "{clean_value}"\n'
    yaml += "---\n\n"
    return yaml

def process_course(course_dir: Path) -> None:
    json_path = course_dir / 'course.json'

    if not json_path.exists():
        logger.error("course.json not found in %s", course_dir)
        return

    logger.info("Processing %s...", course_dir)

    # 1. Load original JSON
    with open(json_path, 'r', encoding='utf-8') as f:
        course_data: dict = json.load(f)

    # 2. Determine file extension
    lang_id = course_data.get('language', 'text').lower()
    # Handle edge cases or default to txt
    file_ext = LANGUAGE_EXTENSIONS.get(lang_id, 'txt')

    # 3. Create Backup
    shutil.copy(json_path, json_path.with_suffix('.json.bak'))
    logger.info("Created backup: course.json.bak")

    # 4. Process Modules
    modules = course_data.pop('modules', [])

    # Ensure modules directory exists
    modules_dir_base = course_dir / 'modules'
    if modules_dir_base.exists():
        # Optional: Warn or clean up existing modules dir if re-running
        logger.warning("'%s' already exists. Merging/Overwriting...", modules_dir_base)

    for mod_index, module in enumerate(modules, 1):
        mod_title = module.get('title', f'module-{mod_index}')
        mod_slug = f"{mod_index:02d}-{slugify(mod_title)}"
        mod_dir = modules_dir_base / mod_slug

        # Process Lessons
        lessons = module.pop('lessons', [])

        for lesson_index, lesson in enumerate(lessons, 1):
            # Use 'order' field if available, else iterator
            order = lesson.get('order', lesson_index)
            lesson_title = lesson.get('title', f'lesson-{order}')
            lesson_slug = f"{order:02d}-{slugify(lesson_title)}"
            lesson_dir = mod_dir / 'lessons' / lesson_slug

            # Process Content Sections
            sections = lesson.pop('contentSections', [])
            for sec_index, section in enumerate(sections, 1):
                sec_type = section.get('type', 'text').lower()
                sec_filename = f"{sec_index:02d}-{sec_type}.md"
                sec_path = lesson_dir / 'content' / sec_filename

                # Prepare Frontmatter
                frontmatter_data = {
                    "type": section.get('type'),
                    "title": section.get('title'),
                }

                # Prepare Content
                md_content = get_frontmatter(frontmatter_data)
                md_content += section.get('content', '') or ''

                # Append Code if present
                code_snippet = section.get('code')
                if code_snippet:
                    code_lang = section.get('language', lang_id)
                    md_content += f"\n\n```{code_lang}\n{code_snippet}\n```\n"

                write_file(sec_path, md_content)

            # Process Challenges
            challenges = lesson.pop('challenges', [])
            for chal_index, challenge in enumerate(challenges, 1):
                chal_title = challenge.get('title', f'challenge-{chal_index}')
                chal_slug = f"{chal_index:02d}-{slugify(chal_title)}"
                chal_dir = lesson_dir / 'challenges' / chal_slug

                # Extract code files
                starter_code = challenge.pop('starterCode', None)
                solution_code = challenge.pop('solution', None)

                # Write Metadata
                write_json(chal_dir / 'challenge.json', challenge)

                # Write Code Files
                if starter_code:
                    write_file(chal_dir / f'starter.{file_ext}', starter_code)
                if solution_code:
                    write_file(chal_dir / f'solution.{file_ext}', solution_code)

            # Write Lesson Metadata
            write_json(lesson_dir / 'lesson.json', lesson)

        # Write Module Metadata
        write_json(mod_dir / 'module.json', module)

    # 5. Overwrite root course.json with stripped metadata
    write_json(json_path, course_data)
    logger.info("Refactoring complete.")

if __name__ == "__main__":
    # Check for --verbose flag
    verbose = "--verbose" in sys.argv
    if verbose:
        sys.argv.remove("--verbose")
        logging.getLogger().setLevel(logging.DEBUG)

    # Use argument if provided, otherwise current directory
    target_dir = Path(sys.argv[1]) if len(sys.argv) > 1 else Path.cwd()

    if target_dir.is_dir():
        process_course(target_dir)
    else:
        logger.error("%s is not a directory.", target_dir)
