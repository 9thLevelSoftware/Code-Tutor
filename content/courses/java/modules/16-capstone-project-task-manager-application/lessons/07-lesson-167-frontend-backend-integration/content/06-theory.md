---
type: "THEORY"
title: "Shared: Loading States and User Experience"
---

BOTH PATHS

Professional applications handle loading states gracefully so users always know what is happening.

Thymeleaf Approach:
Server-side rendering naturally shows complete pages. There are no loading spinners needed because the browser waits for the full response before rendering. However, you can add subtle UX improvements:

```html
<!-- Show a "no results" message rather than a blank page -->
<div th:if="${#lists.isEmpty(tasks)}" class="text-center py-12 text-gray-500">
    <p>No tasks found matching your criteria.</p>
    <a th:href="@{/tasks/new}" class="text-blue-600 hover:underline">Create your first task</a>
</div>

<!-- Confirmation dialogs for destructive actions -->
<form th:action="@{/tasks/{id}/delete(id=${task.id})}" method="post">
    <button type="submit" onclick="return confirm('Are you sure you want to delete this task?')"
            class="text-red-600 hover:underline">Delete</button>
</form>
```

React Approach:
Client-side rendering requires explicit loading and error states:

```jsx
// Skeleton loading pattern
function TaskSkeleton() {
  return (
    <div className="border rounded-lg p-4 animate-pulse">
      <div className="h-4 bg-gray-200 rounded w-3/4 mb-2" />
      <div className="h-3 bg-gray-200 rounded w-1/2" />
    </div>
  );
}

// Usage
{loading ? (
  <div className="grid gap-4">
    {[1, 2, 3].map(i => <TaskSkeleton key={i} />)}
  </div>
) : (
  <TaskList tasks={tasks} />
)}
```

Both approaches aim for the same goal: users should never face a blank screen or wonder if something is broken. The Thymeleaf path achieves this naturally through full page loads, while the React path requires explicit loading state management.

## Troubleshooting Checklist

If your frontend can't connect to the backend:

1. **Is Spring Boot running?** Check `http://localhost:8080/actuator/health`
2. **Is CORS configured?** Look for CORS errors in browser console
3. **Are ports correct?** Frontend (5173) → Backend (8080)
4. **Is the JWT token included?** Check Authorization header in network tab
5. **Are environment variables set?** Check `.env` for VITE_API_URL
