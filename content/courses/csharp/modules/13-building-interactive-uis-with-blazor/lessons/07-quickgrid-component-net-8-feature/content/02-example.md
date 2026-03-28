---
type: "EXAMPLE"
title: "Code Example"
---

This example demonstrates the concepts in action.

```csharp
// INSTALL: Microsoft.AspNetCore.Components.QuickGrid
// (Included in .NET 9 templates!)

using Microsoft.AspNetCore.Components.QuickGrid;

// BASIC QUICKGRID
<QuickGrid Items="@products">
    <PropertyColumn Property="@(p => p.Name)" Sortable="true" />
    <PropertyColumn Property="@(p => p.Price)" Format="C2" Sortable="true" />
    <PropertyColumn Property="@(p => p.Stock)" />
</QuickGrid>

@code {
    private List<Product> products = new() {
        new Product { Name = "Laptop", Price = 999.99m, Stock = 5 },
        new Product { Name = "Mouse", Price = 29.99m, Stock = 50 }
    };
}

// ADVANCED WITH CUSTOM COLUMNS
<QuickGrid Items="@products" Class="table table-striped">
    <PropertyColumn Property="@(p => p.Name)" Title="Product" />
    
    <TemplateColumn Title="Price">
        <span class="@(context.Price > 500 ? "text-danger" : "text-success")">
            $@context.Price
        </span>
    </TemplateColumn>
    
    <TemplateColumn Title="Actions">
        <button @onclick="() => Edit(context)">Edit</button>
        <button @onclick="() => Delete(context)">Delete</button>
    </TemplateColumn>
</QuickGrid>

// WITH PAGINATION
<QuickGrid Items="@products" Pagination="@pagination">
    <PropertyColumn Property="@(p => p.Name)" />
</QuickGrid>
<Paginator State="@pagination" />

@code {
    private PaginationState pagination = new PaginationState { ItemsPerPage = 10 };
}

// WITH IASYNCQUERYABLE (DATABASE)
<QuickGrid Items="@context.Products.AsQueryable()">
    // Queries database efficiently!
</QuickGrid>

// FILTERING
<input @bind="searchTerm" @bind:event="oninput" placeholder="Search..." />
<QuickGrid Items="@FilteredProducts">
    <PropertyColumn Property="@(p => p.Name)" />
</QuickGrid>

@code {
    private string searchTerm = "";
    private IQueryable<Product> FilteredProducts => 
        products.AsQueryable()
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
}
```
