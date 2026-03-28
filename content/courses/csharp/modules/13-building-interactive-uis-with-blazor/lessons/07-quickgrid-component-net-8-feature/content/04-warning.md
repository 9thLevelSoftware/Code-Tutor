---
type: "WARNING"
title: "Common Pitfalls"
---

## Watch Out For These Issues

**IQueryable vs IEnumerable performance**: Use IQueryable with EF Core! Sorting/paging executes in database. IEnumerable loads ALL data first, then filters in memory - slow for large datasets!

**Missing NuGet package**: QuickGrid requires `Microsoft.AspNetCore.Components.QuickGrid`. Included in .NET 9+ templates, but older projects need manual install.

**Render mode required**: QuickGrid needs interactive render mode for sorting/paging! Static SSR won't work. Add `@rendermode InteractiveServer` or similar.

**Context variable in templates**: Inside TemplateColumn, use 'context' (not 'item') to access current row! `@context.Name` works, `@item.Name` doesn't exist.

**Same PaginationState instance**: Both QuickGrid AND Paginator must use the SAME PaginationState instance! Different instances = pagination breaks.

**.NET 9 change**: Empty rows now have empty `<td></td>` cells for stable row height across pages. CSS may need adjustment if you styled empty rows differently.

**Virtualization caveat**: For very large datasets (10000+ rows), consider virtualization: `<QuickGrid Items="@data" Virtualize="true" />`. Much better scrolling performance!