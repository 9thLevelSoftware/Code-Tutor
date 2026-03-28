import json
import os

# List of all challenge files to update with their lessonIds
challenges = [
    # M13
    ("content/courses/csharp/modules/13-building-interactive-uis-with-blazor/lessons/01-what-is-blazor-c-in-the-browser/challenges/01-practice-challenge/challenge.json", "lesson-13-01"),
    ("content/courses/csharp/modules/13-building-interactive-uis-with-blazor/lessons/02-blazor-rendering-modes-net-8/challenges/01-practice-challenge/challenge.json", "lesson-13-02"),
    ("content/courses/csharp/modules/13-building-interactive-uis-with-blazor/lessons/03-creating-razor-components-building-blocks/challenges/01-practice-challenge/challenge.json", "lesson-13-03"),
    ("content/courses/csharp/modules/13-building-interactive-uis-with-blazor/lessons/04-component-parameters-customization/challenges/01-practice-challenge/challenge.json", "lesson-13-04"),
    ("content/courses/csharp/modules/13-building-interactive-uis-with-blazor/lessons/05-event-handling-onclick-onchange/challenges/01-practice-challenge/challenge.json", "lesson-13-05"),
    ("content/courses/csharp/modules/13-building-interactive-uis-with-blazor/lessons/06-data-binding-bind-directive/challenges/01-practice-challenge/challenge.json", "lesson-13-06"),
    ("content/courses/csharp/modules/13-building-interactive-uis-with-blazor/lessons/07-quickgrid-component-net-8-feature/challenges/01-practice-challenge/challenge.json", "lesson-13-07"),
    # M14
    ("content/courses/csharp/modules/14-blazor-net-aspire-deployment/lessons/01-connecting-blazor-to-api-frontend-backend/challenges/01-practice-challenge/challenge.json", "lesson-14-01"),
    ("content/courses/csharp/modules/14-blazor-net-aspire-deployment/lessons/02-full-crud-operations-complete-data-management/challenges/01-practice-challenge/challenge.json", "lesson-14-02"),
    ("content/courses/csharp/modules/14-blazor-net-aspire-deployment/lessons/03-net-aspire-modern-distributed-apps/challenges/01-practice-challenge/challenge.json", "lesson-14-03"),
    ("content/courses/csharp/modules/14-blazor-net-aspire-deployment/lessons/04-version-control-with-git-save-your-work/challenges/01-practice-challenge/challenge.json", "lesson-14-04"),
    ("content/courses/csharp/modules/14-blazor-net-aspire-deployment/lessons/05-deploying-to-azure-go-live/challenges/01-practice-challenge/challenge.json", "lesson-14-05"),
    ("content/courses/csharp/modules/14-blazor-net-aspire-deployment/lessons/06-next-steps-your-journey-continues/challenges/01-practice-challenge/challenge.json", "lesson-14-06"),
    # M15
    ("content/courses/csharp/modules/15-unit-testing-with-xunit/lessons/01-xunit-testing-fundamentals-the-quality-check/challenges/01-practice-challenge/challenge.json", "lesson-15-01"),
    ("content/courses/csharp/modules/15-unit-testing-with-xunit/lessons/02-mocking-dependencies-fake-collaborators/challenges/01-practice-challenge/challenge.json", "lesson-15-02"),
    ("content/courses/csharp/modules/15-unit-testing-with-xunit/lessons/03-integration-testing-test-organization/challenges/01-practice-challenge/challenge.json", "lesson-15-03"),
    ("content/courses/csharp/modules/15-unit-testing-with-xunit/lessons/04-test-driven-development-red-green-refactor/challenges/01-tdd-practice-password-validator/challenge.json", "lesson-15-04"),
]

counts = {"M13": 0, "M14": 0, "M15": 0}
fixed_files = []

for file_path, lesson_id in challenges:
    try:
        with open(file_path, 'r') as f:
            data = json.load(f)
        
        # Check if lessonId already exists
        if 'lessonId' not in data:
            data['lessonId'] = lesson_id
            data['order'] = 1
            
            with open(file_path, 'w') as f:
                json.dump(data, f, indent=2)
            
            # Count by module
            if lesson_id.startswith("lesson-13"):
                counts["M13"] += 1
            elif lesson_id.startswith("lesson-14"):
                counts["M14"] += 1
            elif lesson_id.startswith("lesson-15"):
                counts["M15"] += 1
                
            fixed_files.append(file_path)
            print(f"Fixed: {file_path}")
        else:
            print(f"Skipped (already has lessonId): {file_path}")
    except Exception as e:
        print(f"Error processing {file_path}: {e}")

print(f"\n=== SUMMARY ===")
print(f"M13 files fixed: {counts['M13']}")
print(f"M14 files fixed: {counts['M14']}")
print(f"M15 files fixed: {counts['M15']}")

# Write fixed files list for reference
with open('/tmp/fixed_challenges.txt', 'w') as f:
    for fp in fixed_files:
        f.write(fp + '\n')
