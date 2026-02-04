# API/App_Start - AI Context

## Purpose

The **App_Start** folder contains application startup configuration files that execute when the ASP.NET Web API application initializes. These files configure routing, filters, serialization, and other global application settings.

## Files in This Folder

### WebApiConfig.cs
The primary Web API configuration file that sets up routing, serialization, and dependency injection.

#### Key Configurations

**JSON Serialization**
```csharp
var settings = config.Formatters.JsonFormatter.SerializerSettings;
settings.Formatting = Newtonsoft.Json.Formatting.Indented;
settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
```
- **Indented Formatting**: Pretty-printed JSON for readability
- **CamelCase Properties**: Converts C# PascalCase to JSON camelCase (e.g., `UserId` → `userId`)

**Route Configuration**
The application uses convention-based routing with multiple route templates to support various HTTP methods and patterns:

1. **DefaultApiWithId**: `/{area}/{controller}/{id}` - For operations with numeric IDs
2. **DefaultApiWithAction**: `/{area}/{controller}/{action}` - For custom action names
3. **DefParamApiGet**: GET requests with optional ID parameter
4. **DefaultApiGet**: GET requests without parameters
5. **DefaultApiPost**: POST requests for creation
6. **DefaultApiPut**: PUT requests for updates with ID
7. **DefaultApiDelete**: DELETE requests with ID

**Route Evaluation Order**
Routes are evaluated in the order they're registered. More specific routes (with constraints) are registered first.

**HTTP Method Constraints**
Routes use `HttpMethodConstraint` to map URLs to appropriate HTTP verbs:
- GET: Retrieve data
- POST: Create resources or complex queries
- PUT: Update resources
- DELETE: Remove resources

**Attribute Routing**
```csharp
config.MapHttpAttributeRoutes();
```
Enables `[Route]` attributes on controller actions for custom routing.

**Custom Dependency Resolver**
```csharp
config.DependencyResolver = new DependencyResolver();
```
Integrates the application's IoC container with Web API's dependency injection system.

The custom `DependencyResolver` class:
- Implements `IDependencyResolver` interface
- Resolves controllers and services from `ApplicationManager.DependencyInjection`
- Only resolves types that inherit from `BaseController` or `ApiController`

### FilterConfig.cs
Registers global action filters that apply to all API requests.

#### Typical Filters
- **BasicAuthenticationAttribute**: Token-based authentication
- **BasicExceptionFilterAttribute**: Global exception handling
- **DbTransactionFilterAttribute**: Automatic transaction management
- **CORS Filters**: Cross-origin request handling

These filters execute in the order registered and apply to every controller action unless overridden.

## Routing Examples

### GET Requests
```
GET /Organizations/Franchisee           → FranchiseeController.Get()
GET /Organizations/Franchisee/123       → FranchiseeController.Get(123)
GET /Sales/Customer/Search?name=John    → CustomerController.Search(name)
```

### POST Requests
```
POST /Organizations/Franchisee          → FranchiseeController.Post(model)
     Body: { "name": "New Franchisee", ... }

POST /Sales/Invoice/Generate            → InvoiceController.Generate(model)
```

### PUT Requests
```
PUT /Organizations/Franchisee/123       → FranchiseeController.Put(123, model)
    Body: { "name": "Updated Name", ... }
```

### DELETE Requests
```
DELETE /Organizations/Franchisee/123    → FranchiseeController.Delete(123)
```

### Custom Actions
```
GET /Organizations/Franchisee/GetFeeProfile?id=123
    → FranchiseeController.GetFeeProfile(123)
```

## Configuration Flow

1. **Application_Start** in Global.asax.cs calls:
   ```csharp
   GlobalConfiguration.Configure(WebApiConfig.Register);
   FilterConfig.RegisterGlobalFilters(GlobalConfiguration.Configuration.Filters);
   ```

2. **WebApiConfig.Register** executes:
   - Configures JSON serialization
   - Registers routes (attribute routing first, then conventions)
   - Sets up dependency resolver

3. **FilterConfig.RegisterGlobalFilters** executes:
   - Registers authentication filters
   - Registers exception handling filters
   - Registers transaction filters
   - Registers CORS filters

4. Application ready to accept requests

## For AI Agents

### Adding New Global Routes

1. **Open WebApiConfig.cs**
2. **Add route registration** in `Register` method:
   ```csharp
   config.Routes.MapHttpRoute(
       name: "CustomRoute",
       routeTemplate: "{area}/{controller}/custom/{id}",
       defaults: new { action = "CustomAction", id = RouteParameter.Optional },
       constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
   );
   ```
3. **Position matters**: Add specific routes before general routes
4. **Test routing**: Use RouteDebugger or test with actual requests

### Adding Global Filters

1. **Open FilterConfig.cs**
2. **Register filter**:
   ```csharp
   public static void RegisterGlobalFilters(HttpFilterCollection filters)
   {
       filters.Add(new BasicAuthenticationAttribute());
       filters.Add(new BasicExceptionFilterAttribute());
       filters.Add(new CustomFilterAttribute()); // Your new filter
   }
   ```
3. **Order matters**: Filters execute in registration order
4. **Test behavior**: Verify filter applies to all controllers

### Modifying JSON Serialization

To change serialization settings:
```csharp
var settings = config.Formatters.JsonFormatter.SerializerSettings;
settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
settings.NullValueHandling = NullValueHandling.Ignore;
settings.DateFormatString = "yyyy-MM-dd";
```

### Enabling CORS (if not in Global.asax)

```csharp
// In WebApiConfig.Register
var cors = new EnableCorsAttribute("*", "*", "*");
config.EnableCors(cors);
```
Currently CORS is handled in Global.asax.cs BeginRequest.

## For Human Developers

### Understanding Route Resolution

**Route Matching Process:**
1. Web API examines incoming URL and HTTP method
2. Iterates through routes in registration order
3. First matching route is selected
4. Route parameters extracted and passed to controller action
5. If no route matches, returns 404

**Route Constraints:**
- `@"\d+"`: ID must be numeric
- `HttpMethodConstraint`: Must match specific HTTP verb
- Custom constraints can be implemented via `IHttpRouteConstraint`

### Debugging Routes

**Route Not Found (404):**
1. Check route template matches URL pattern
2. Verify HTTP method constraint matches request
3. Ensure route registered before more general routes
4. Check controller and action names match conventions

**Wrong Action Invoked:**
1. More general route may be matching first
2. Reorder route registration (specific before general)
3. Add constraints to narrow matching

**Tools:**
- Enable route debugging (third-party packages)
- Log route matching in custom message handlers
- Use Fiddler/Postman to test routes

### Best Practices

#### Route Design
- Use RESTful conventions (GET/POST/PUT/DELETE)
- Keep URLs readable and hierarchical
- Use nouns for resources, not verbs
- Version APIs when making breaking changes

#### Configuration
- Keep configurations in App_Start, not scattered
- Document custom routes with XML comments
- Use attribute routing for complex/special cases
- Avoid route conflicts (specific routes first)

#### Serialization
- Stick to JSON for consistency
- Use camelCase for JavaScript compatibility
- Handle null values appropriately
- Consider performance for large objects

#### Filters
- Order filters carefully (auth before others)
- Keep filters focused and single-purpose
- Test filter combinations
- Document filter behaviors

### Common Configurations

#### Custom Date Format
```csharp
settings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ"; // ISO 8601
```

#### Ignore Null Values
```csharp
settings.NullValueHandling = NullValueHandling.Ignore;
```

#### Handle Circular References
```csharp
settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
```

#### Pretty Print JSON (Development Only)
```csharp
#if DEBUG
settings.Formatting = Formatting.Indented;
#else
settings.Formatting = Formatting.None; // Minimize response size
#endif
```

### Security Considerations

- **Authentication Filter**: Must be first to prevent unauthorized access
- **CORS Configuration**: Restrict origins in production (not "*")
- **HTTPS**: Enforce HTTPS in production
- **Rate Limiting**: Consider adding rate limiting filters
- **Input Validation**: Use model validation filters

### Performance Considerations

- **Minimize Filters**: Each filter adds overhead to every request
- **Efficient Serialization**: Avoid including unnecessary data in responses
- **Compression**: Enable response compression in IIS/web.config
- **Caching**: Add caching headers for GET requests

### Maintenance Tips

- Document route changes in version control commits
- Test route changes with existing clients
- Keep filter logic simple (complex logic in services)
- Review and clean up unused routes periodically

## Related Files

- **Global.asax.cs**: Calls configuration methods during startup
- **Web.config**: Additional HTTP settings, compression, handlers
- **DependencyInjection/ApiDependencyRegistrar.cs**: IoC setup
- **Attribute/*.cs**: Custom filter implementations

## Impact of Changes

Changes in App_Start require **application restart** to take effect:
- IIS reset
- Recycle application pool
- Redeploy application

Always test configuration changes thoroughly in development before production deployment.
