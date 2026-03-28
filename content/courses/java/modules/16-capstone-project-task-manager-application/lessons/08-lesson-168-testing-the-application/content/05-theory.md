---
type: "THEORY"
title: "Shared: End-to-End Testing Overview"
---

BOTH PATHS

End-to-end (E2E) tests verify the entire application works together, from browser to database. They simulate real user scenarios. Whether you chose Thymeleaf or React, Playwright works the same way -- it drives a real browser, so it does not care how the HTML was generated.

Popular E2E Testing Tools:

1. Playwright (Recommended)
- Fast, reliable, modern architecture
- Cross-browser (Chrome, Firefox, Safari)
- Excellent debugging with trace viewer
- Auto-wait for elements

2. Cypress
- Great developer experience
- Real-time test preview
- Time-travel debugging
- Chrome-based (WebKit support limited)

Playwright Setup:
```bash
npm init playwright@latest
```

Example E2E Test (Playwright):

```typescript
// e2e/tasks.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Task Management', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login');
    await page.fill('[name="email"]', 'test@example.com');
    await page.fill('[name="password"]', 'password123');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard');
  });

  test('user can create a new task', async ({ page }) => {
    await page.goto('/tasks/new');
    
    await page.fill('[name="title"]', 'E2E Test Task');
    await page.fill('[name="description"]', 'Created by Playwright');
    await page.selectOption('[name="priority"]', 'HIGH');
    await page.click('button[type="submit"]');
    
    await page.waitForURL('/tasks');
    await expect(page.locator('text=E2E Test Task')).toBeVisible();
  });

  test('user can mark task as complete', async ({ page }) => {
    await page.goto('/tasks');
    
    // Find a task and toggle its status
    const taskCard = page.locator('.task-card').first();
    await taskCard.locator('button:has-text("Complete")').click();
    
    await expect(taskCard.locator('.status-badge')).toContainText('COMPLETED');
  });

  test('user can filter tasks by status', async ({ page }) => {
    await page.goto('/tasks');
    
    await page.selectOption('[name="status-filter"]', 'COMPLETED');
    
    const tasks = page.locator('.task-card');
    await expect(tasks).toHaveCount(await tasks.count());
    
    for (const task of await tasks.all()) {
      await expect(task.locator('.status-badge')).toContainText('COMPLETED');
    }
  });

  test('user can delete a task', async ({ page }) => {
    await page.goto('/tasks');
    
    const taskCard = page.locator('.task-card').first();
    const taskTitle = await taskCard.locator('h3').textContent();
    
    page.on('dialog', dialog => dialog.accept());
    await taskCard.locator('button:has-text("Delete")').click();
    
    await expect(page.locator(`text=${taskTitle}`)).not.toBeVisible();
  });
});
```

Run E2E tests:
```bash
npx playwright test
npx playwright test --ui  # Interactive mode
npx playwright show-report  # View results
```

E2E tests are slower but catch integration issues that unit tests miss. Run them in CI/CD before deployment.

> **Production Tip:** For more realistic integration tests, consider [Testcontainers](https://testcontainers.com/) to test with an actual PostgreSQL database instead of H2. This catches SQL dialect differences early.