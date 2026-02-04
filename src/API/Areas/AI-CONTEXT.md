# API/Areas - AI Context

## Purpose

This folder contains all API area modules for the MarbleLife Web API. Areas provide logical grouping of related controllers, organizing the API by business domains.

## Structure

The Areas folder contains 11 business domain areas:
- **Application**: Core application utilities and base functionality
- **Dashboard**: Business metrics and KPI endpoints
- **Geo**: Geographic and location services
- **MarketingLead**: Lead management and tracking
- **Organizations**: Franchisee and organization management
- **Payment**: Payment processing endpoints
- **Reports**: Business intelligence and reporting
- **Review**: Customer review management
- **Sales**: Sales pipeline and customer management
- **Scheduler**: Job scheduling and calendar operations
- **Users**: User authentication and management

## For AI Agents

Each area follows the ASP.NET Web API Area pattern:
- Controllers in `Controllers/` or `Controller/` subfolder
- ViewModels in `ViewModel/` subfolder (if exists)
- Enums in `Enum/` subfolder (if exists)
- Implementation helpers in `Impl/` subfolder (if exists)

When working with areas:
1. Register area routes in `AreaRegistration.cs` or `WebApiConfig.cs`
2. Controllers inherit from `ApiController` or custom base controller
3. Use attribute routing with `[RoutePrefix]` and `[Route]` attributes
4. Implement proper authorization with custom attributes
5. Return appropriate HTTP status codes (200, 201, 400, 404, 500)

## For Human Developers

Areas organize the API into logical business modules. Each area is self-contained with its own controllers and supporting files.

### Common Patterns:
```csharp
[RoutePrefix("api/organizations")]
[Authorize]
public class OrganizationController : ApiController
{
    [HttpGet]
    [Route("{id:int}")]
    public IHttpActionResult Get(int id)
    {
        // Implementation
    }
}
```

### Best Practices:
- Keep controllers focused on HTTP concerns
- Delegate business logic to Core services
- Use consistent naming conventions
- Implement proper error handling
- Add XML comments for API documentation
- Use ActionFilters for cross-cutting concerns
