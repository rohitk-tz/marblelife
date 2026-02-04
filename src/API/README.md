<!-- AUTO-GENERATED: Header -->
# API Layer
> RESTful Web API Entry Point
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The API layer exposes the application's functionality over HTTP. It is built using **ASP.NET Web API 2**.

Key Responsibilities:
-   **Endpoint Exposure**: Serving JSON data to clients.
-   **Authentication**: Validating tokens and setting up user sessions.
-   **Input Validation**: Ensuring request data is valid before hitting business logic.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Running Locally
This project is typically hosted in IIS or IIS Express.
-   **Start**: Set `API` as the Startup Project in Visual Studio.
-   **URL**: Usually `http://localhost:port/api/...`

### Adding an Endpoint
1.  Go to the relevant Area (e.g., `Areas/Users`).
2.  Add/Edit a Controller (e.g., `AccountController.cs`).
3.  Inject the Service interface.
4.  Add Action method.

```csharp
[Route("login")]
[HttpPost]
public IHttpActionResult Login(LoginModel model) 
{
    if (!ModelState.IsValid) return BadRequest(ModelState);
    
    var result = _service.Login(model);
    return Ok(result);
}
```

### Headers
Every request (except public ones like Login) requires:
-   `token`: The specific auth token for the user.
-   `Content-Type`: `application/json`.
-   `timezoneoffset`: (Optional) For client-side time adjustment.

<!-- END AUTO-GENERATED -->
