# PaymentAPI — Agent Guidelines

## Architecture

Four-project solution targeting **net10.0**:

| Project | Role |
|---|---|
| `PaymentAPI` | ASP.NET Core web host — controllers, middleware, DI composition root |
| `PaymentAPI.Domain` | MediatR requests/handlers, domain interfaces, models, enums, exceptions, `IOrderApiClient` |
| `PaymentAPI.DAL` | EF Core `DbContext`, storage implementations, entity config, Specifications, migrations |
| `PaymentAPI.Tests` | xUnit tests for handlers, storage classes, and controllers |

Dependency direction: `Web → DAL → Domain`. Domain has **no** infrastructure references.

## Build & Test

```bash
dotnet build PaymentAPI.sln
dotnet test PaymentAPI.sln

# EF Core migrations (run from solution root)
dotnet ef migrations add <Name> --project PaymentAPI.DAL --startup-project PaymentAPI
dotnet ef database update --project PaymentAPI.DAL --startup-project PaymentAPI
```

## Key Conventions

### Use Case pattern (CQRS via MediatR 14)
Each use case lives in its own folder — split into two co-located files:
- `UseCases/{Action}Payment/{Action}PaymentRequest.cs` — implements `IRequest<TResponse>`
- `UseCases/{Action}Payment/{Action}PaymentRequest.Handler.cs` — implements `IRequestHandler<..>`

Handlers live in **`PaymentAPI.Domain`**, not the web project. Both assemblies are registered with MediatR in `Program.cs`.

### Storage pattern
- Domain defines the interface: `PaymentAPI.Domain/Storage/{Action}/I{Action}Storage.cs`
- DAL implements it: `PaymentAPI.DAL/Storage/{Action}/{Action}Storage.cs`
- Storage classes **project EF entities to domain models** before returning — a static `Expression<Func<Payment, PaymentModel>> ToModel` field is the standard projection idiom. Never expose `Payment` entity outside DAL.

### Specification pattern (DAL only)
`ISpecification<T>` wraps `Expression<Func<T, bool>>` for composable EF Core queries. Specifications live in `PaymentAPI.DAL/Specifications/Payments/`. Domain handlers never reference specifications.

### EF Core
- Use `DbContextPool` (not `AddDbContext`)
- Configurations via `ApplyConfigurationsFromAssembly`
- Enums stored as strings: `.HasConversion<string>()`
- `BaseDbEntity` (from `Homework.Ticketing.System.Shared`) provides `Id`, `CreatedAt`, `UpdatedAt`, `UpdatedBy`

### Auth
- `[AllowAnonymous]` on `CreatePayment` endpoint only; all others require JWT Bearer
- User identity extracted via `ClaimTypes.NameIdentifier` and passed as `UpdatedBy`

## Testing Conventions

| Test type | Location | Pattern |
|---|---|---|
| Handler unit tests | `PaymentAPI.Tests/UseCases/` | Mock all `IStorage` dependencies with Moq; test both happy path and exception paths |
| Controller tests | `PaymentAPI.Tests/Controllers/` | Mock `IMediator`; manually construct controller with `ControllerContext` + fake `ClaimsPrincipal` |
| DAL/storage tests | `PaymentAPI.Tests/DAL/` | Use EF Core InMemory provider — no mocking, real storage against in-memory DB |

Test method naming: `Handle_{Scenario}_When{Condition}` (handlers) or descriptive action-based names (controllers/DAL).

`GlobalUsings.cs` provides `global using Xunit;` — no need to add it per file.
