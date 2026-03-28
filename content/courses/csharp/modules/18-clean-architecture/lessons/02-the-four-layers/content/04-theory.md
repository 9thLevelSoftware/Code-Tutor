---
type: "THEORY"
title: "Project References in .NET"
---

## Configuring Project References for Clean Architecture

In .NET, project references define compile-time dependencies. Clean Architecture requires careful configuration of these references to enforce the dependency rule.

```xml
// ===== DOMAIN PROJECT (.csproj) =====
// File: src/ShopFlow.Domain/ShopFlow.Domain.csproj

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <!-- NOTICE: NO ProjectReferences! Domain is completely independent -->
  <!-- Only basic packages for things like data annotations if needed -->
  <ItemGroup>
    <!-- Minimal dependencies - avoid external packages when possible -->
  </ItemGroup>

</Project>


// ===== APPLICATION PROJECT (.csproj) =====
// File: src/ShopFlow.Application/ShopFlow.Application.csproj

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <!-- References ONLY Domain -->
  <ItemGroup>
    <ProjectReference Include="..\ShopFlow.Domain\ShopFlow.Domain.csproj" />
  </ItemGroup>

  <!-- Application-level packages (no infrastructure dependencies!) -->
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.2.0" />
    <PackageReference Include="FluentValidation" Version="11.9.0" />
    <PackageReference Include="AutoMapper" Version="12.0.1" />
  </ItemGroup>

</Project>


// ===== INFRASTRUCTURE PROJECT (.csproj) =====
// File: src/ShopFlow.Infrastructure/ShopFlow.Infrastructure.csproj

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <!-- References Application (which transitively includes Domain) -->
  <ItemGroup>
    <ProjectReference Include="..\ShopFlow.Application\ShopFlow.Application.csproj" />
  </ItemGroup>

  <!-- THIS is where all the "heavy" packages live -->
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0" />
    <PackageReference Include="SendGrid" Version="9.29.2" />
    <PackageReference Include="Stripe.net" Version="43.0.0" />
  </ItemGroup>

</Project>


// ===== API/PRESENTATION PROJECT (.csproj) =====
// File: src/ShopFlow.API/ShopFlow.API.csproj

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <!-- References Infrastructure (which transitively includes Application and Domain) -->
  <ItemGroup>
    <ProjectReference Include="..\ShopFlow.Infrastructure\ShopFlow.Infrastructure.csproj" />
  </ItemGroup>

  <!-- Only presentation-layer packages -->
  <ItemGroup>
    <PackageReference Include="Scalar.AspNetCore" Version="1.2.0" />
  </ItemGroup>

</Project>


// ===== WHY THIS STRUCTURE WORKS =====
/*
Compile-time dependency chain:
  API → Infrastructure → Application → Domain

At compile time:
- Domain knows about: nothing external
- Application knows about: Domain entities, value objects, events
- Infrastructure knows about: Application interfaces + Domain types
- API knows about: Everything (but should only use Application services)

At runtime:
- DI container resolves interfaces to implementations
- API calls Application services
- Application services use interfaces (IProductRepository)
- Infrastructure implementations get injected

BENEFIT: If you accidentally try to use EF Core in Application layer,
you'll get a compile error because Application doesn't reference
Microsoft.EntityFrameworkCore!
*/
```
