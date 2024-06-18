> # W. Industries: Core Architecture
> ***Solutions Built From the Ground Up***  
  
> ### Table of Contents
> |1. [Quick Start](#quick-start)|9. [`AuthController`](#authcontroller)|13. [Token Middleware](#handling-tokens)|17. [Extend](#extend)|[Roadmap](#roadmap-v1) |
> |   :----:  |    :----:    |    :----:   | :----: |  :----: |
> |2. [Define](#define-identitydbcontext)|10. [Define](#1-define-the-authcontroller)|14. [Define](#define-middleware)|    |     |
> |3. [API Dependencies](#api-dependencies)|11. [`/register`](#2-implement-register)|15. [Testing](#testing-middleware)|    |    |
> |4. [Configure](#configure-services)|12. [`/login`](#3-implement-login)|16. [`\logout`](#logout)|    |    |
> |5. [Audit/Seed](#seed-user-required-for-audit-logging)|   |   |   |   |
> |6. [Migrate](#createrun-migration)|   |   |   |   |
> |7. [`DbContext.cs`](#somedbcontextcs)|   |   |   |   |
> |8. [`Program.cs`](#programcs)|   |   |   |   |
>  

## **About**

Hit the ground running in **.NET 8** when you create an **EF Core** database, backed by **`Microsoft.AspNetCore.Identity`**. Using it's pre-defined core entity classes, you can immediately create your `IdentityDbContext` class after installation. Highly extendible with extensive intellisense/xml documentation. Simplified services, static helper classes, & base middleware defined for handling common use-cases. Such as:

> *Generic Temporal Audit Logging*  
  
> *Generic Soft Deletes*  
  
> *Generic UTC DateTime Parsing*  
  
> *JWT Bearer Token Generation/Validation/Invalidation*    
  
> *Authentication/Authorization Handling*  


## **Quick Start**

### **Define IdentityDbContext**

Once installed, you *immediately* have the ability to define an `IdentityDbContext` class out of the box  
  
*Example:*
```cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using W.Ind.Core.Entity;
using W.Ind.Core.Service;

// Pre-Defined entity class types as generic type params
public class SomeDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    // Manually injects IUserService during CLI operations
    protected readonly IServiceProvider _serviceProvider;
    protected IUserService<User, long> _userService { get { return _serviceProvider.GetRequiredService<IUserService<User, long>>(); } }

    public SomeDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options)
    {
        _serviceProvider = serviceProvider;
    }
}
```
---
### **API Dependencies**

Before configuring services to use this `DbContext`. There are a couple packages you should install in your API/Controller layer:
- ***(latest)*** `Microsoft.AspNetCore.Authentication.JwtBearer`
- ***(latest)*** `Microsoft.EntityFrameworkCore.Design`
- **Note:** If your `DbContext` is located in a separate project, add a reference to the project containing it

---
### **Configure Services**

Once packages are installed. Go to that project's `Program.cs` file (or `Startup.cs` if necessary) and configure the following services:

```cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using W.Ind.Core.Config;
using W.Ind.Core.Entity;
using W.Ind.Core.Helper;
using MyApp.db;

// NOTE: Ensure your Configuration/appSettings file has defined "Jwt" and "ConnectionStrings" sections including a "DefaultConnection"

// Map JWT options directly from Configuration/appSettings file
// Properties: "Issuer", "Audience", & "SecretKey" are required for service injection
JwtConfig jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>()!;

// Configure W.Ind.Core Services
builder.Services.ConfigureWicServices<JwtConfig, User, long>(jwtConfig);

// Configure Auth
builder.Services.AddAuthentication(options => {...})
    // JWT Access Token
    .AddJwtBearer(options => {...});

// Configure EF Identity
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<SomeDbContext>()
    .AddDefaultTokenProviders();

// Add DbContext (Default Connection)
builder.Services.AddDbContext<SomeDbContext>(o => 
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```  
  
[*\[Top\]*](#w-industries-core-architecture)  
  
---

### **Seed User** (Required for Audit Logging)

A seeded System User is **required** to perform audit logs which reference the User who created a record. This is the **default** behavior for most concrete `W.Core.Ind.Entity` classes. Meaning you need a migration that seeds a System User.

**Note:** *You can change this default behavior and choose not to use Audit Logging by re-implementing those core entity's `abstract` base parent classes and omitting the `IAuditable` implementation (see [Extensions](#extend))*

Data can be seeded via JSON file using the `ContextHelper.GetFromJsonFile<TEntity>("filePath")` method

At the same time, we can finish setting up audit logs using the same static `ContextHelper` class

1. Create/copy the User seed data into a JSON file & place inside a subdirectory of your startup project folder
```json
[
  {
    "Id": 1,
    "Email": "system@prototype.com",
    "FirstName": "System",
    "LastName": "Prototype",
    "IsDeleted": false,
    "Timestamp": "AAAAAAAAB9c=",
    "CreatedById": 1,
    "CreatedOn": "2024-06-07T00:00:00",
    "SysEndTime": "9999-12-31T23:59:59.9999999",
    "SysStartTime": "2024-06-08T03:41:52.3303911",
    "UserName": "SYSTEM",
    "NormalizedUserName": "SYSTEM",
    "NormalizedEmail": "SYSTEM@PROTOTYPE.COM",
    "EmailConfirmed": true,
    "PasswordHash": "AQAAAAIAAYagAAAAECU9IiWDcY1BNXnTWViz78D4cHVYm3oMqvQ7RX2T+tkxRibZVGjx3xXQYAuOhEJqXQ==",
    "PhoneNumberConfirmed": false,
    "TwoFactorEnabled": false,
    "LockoutEnabled": false,
    "AccessFailedCount": 0,
    "ConcurrencyStamp": "b530f6a0-64cf-4eb4-922d-e5026a2f3c10",
    "SecurityStamp": "98441edc-a147-4144-b722-d533e3f0183e"
  }
]
```  

**Note:** The Password is "password"  

---
2. Override your `DbContext.OnModelCreating` method
```cs
using W.Ind.Core.Helper;

// Override your DbContext's OnModelCreating method
protected override void OnModelCreating(ModelBuilder builder) 
{
    base.OnModelCreating(builder);

    // Configure temporal history table for audit logs
    // [REQUIRED] Do this for all entities that implement `IAuditable`
    builder.Entity<Role>(b => b.ToTable(ContextHelper.BuildTemporal));
    //Etc..
    builder.Entity<...>(b => b.ToTable(ContextHelper.BuildTemporal));

    // Example with JSON file seeding
    builder.Entity<User>(b => b.ToTable(ContextHelper.BuildTemporal)
        // Seeds the User table 
        // NOTE: File path is relative to the startup project directory
        .HasData(ContextHelper.GetFromJsonFile<List<User>>("Seed/Users.json")));
}
```
---  

3. Complete Audit Log Setup
```cs
// Override your DbContext's SaveChanges/SaveChangesAsync methods
public override async Task<int> SaveChangesAsync(CancelationToken cancelationToken = default) {
    // Bonus: Implement soft delete
    ChangeTracker.Entries<ISoftDelete>().HandleSoftDelete();

    // Get Current Context User ID (defaults to System User)
    long currentUserId = await _userService.GetCurrent();
    ChangeTracker.Entries<IAuditable>().HandleAudit(currentUserId);

    return await base.SaveChangesAsync(cancelationToken);
}
```
  
[*\[Top\]*](#w-industries-core-architecture)  
  
---
### **Create/Run Migration**

**Create Initial**

Using CLI (assuming you have `dotnet ef` CLI tools installed) navigate to the project folder containing your `DbContext` class and add your initial migration:
```ps
dotnet ef migrations add "Initial"

# Run when DbContext is located outside your startup project
dotnet ef migrations add "Initial" -s "../YourStartupProject"
```

**Update Database**

Once you've verified the migration was added successfully. In the same directory, run the CLI command `dotnet ef database update`:
```ps
dotnet ef database update

# Run when DbContext is located outside your startup project
dotnet ef database update -s "../StartupProject"
```  
  
[*\[Top\]*](#w-industries-core-architecture)  
  

---
## Database Created

You've now completed setting up your identity backed database with core auth entities configured to handle both soft deletes, and audit logging. In case you weren't keeping track, that whole process required less than 100 lines of C# code over the span of 2 files (`Program.cs` and your `DbContext` class file). The rest of the changes were related to either:
1. [Installing API layer dependencies](#install-packages)
2. [Your Configuration/appSettings file](#configure-services) *(see 'NOTE:' comment)*
3. [Copying/Inserting Seed Data file(s)](#seed-user-required-for-audit-logging)
4. [Adding your migration via `dotnet ef` CLI](#createrun-migration)

Here's an expanded look at all the C# file changes made up until this point:

### `SomeDbContext.cs`
```cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using W.Ind.Core.Entity;
using W.Ind.Core.Service;
using W.Ind.Core.Helper;

public class SomeDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    protected readonly IServiceProvider _serviceProvider;
    protected IUserService<User, long> _userService { get { return _serviceProvider.GetRequiredService<IUserService<User, long>>(); } }

    public SomeDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override OnModelCreating(ModelBuilder builder) 
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable(ContextHelper.BuildTemporal)
            .HasData(ContextHelper.GetFromJsonFile<List<User>>("Seed/Users.json"));

        builder.Entity<Role>().ToTable(ContextHelper.BuildTemporal);

        builder.Entity<UserRole>().ToTable(ContextHelper.BuildTemporal);

        builder.Entity<UserClaim>().ToTable(ContextHelper.BuildTemporal);

        builder.Entity<RoleClaim>().ToTable(ContextHelper.BuildTemporal);
    }

    public override int SaveChanges()
    {
        ChangeTracker.Entries<ISoftDelete>().HandleSoftDelete();
        ChangeTracker.Entries<IAuditable>().HandleAudit(_userService.GetCurrent().Result);

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancelationToken)
    {
        ChangeTracker.Entries<ISoftDelete>().HandleSoftDelete();
        ChangeTracker.Entries<IAuditable>().HandleAudit(await _userService.GetCurrent());

        return await base.SaveChangesAsync(cancelationToken);
    }
}
```
---
### `Program.cs`

```cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.db;
using W.Ind.Core.Config;
using W.Ind.Core.Entity;
using W.Ind.Core.Helper;

JwtConfig jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>()!;
builder.Services.ConfigureWicServices<JwtConfig, User, long>(jwtConfig);

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
    };
});

builder.Services.AddIdentity<User, Role>(options => options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<SomeDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<SomeDbContext>(o => 
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```
In the next section, we'll cover setting up your `AuthController` to both register Users to your database and validating User logins.

  
  
[*\[Top\]*](#w-industries-core-architecture)  
  
---
## `AuthController`

After the database is created you can begin implementing your `AuthController`. The two simplest examples would be with your `/register` and `/login` routes. Here, we'll go over how to implement these routes with the injected `UserService`.

### 1. **Define the `AuthController`**

```cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using W.Ind.Core.Dto;
using W.Ind.Core.Entity;
using W.Ind.Core.Service;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // Injected UserService<TUser, TKey>
    protected readonly IUserService<User, long> _userService;

    public AuthController(IUserService<User, long> userService) 
    {
        _userService = userService;
    }
}
```
---
### 2. **Implement `/register`**
```cs
// Add this method/Route to your Auth Controller
[AllowAnonymous]
[HttpPost("register")]
public async Task<ActionResult<IdentityResult>> RegisterAsync(UserRegistration request)
{
    try
    {
        IdentityResult result = await _userService.RegisterAsync(request);
        return Ok(result);
    }
    catch (Exception)
    {
        throw;
    }
}
```
Once created, you can register a new user to your system through this endpoint (`/api/auth/register`). Simply run the app and send the HTTP request, or integrate it into an existing HTTP Testing module.

*NOTE:* *You can extend the `UserRegistraton` DTO to include more custom data, and override the `UserService.Register` method to use that class (see [Extensions](#extend))*

---
### 3. **Implement `/login`**
```cs
[AllowAnonymous]
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request) 
{
    try
    {
        ILoginResponse result = await _userService.ValidateLoginAsync<LoginRequest, LoginResponse>(request);
        return Ok(result);
    }
    catch (Exception)
    {
        throw;
    }
}
```
Once created, you can login as an existing user through this endpoint (`/api/auth/login`). Simply run the app and send the HTTP request, or integrate it into an existing HTTP Testing module.

**NOTE:** *You can extend the `LoginRequest` & `LoginResponse` DTOs to include more custom data, and override the `UserService.ValidateLoginAsync` method to use those fields. (see [Extensions](#extend))*
  
[*\[Top\]*](#w-industries-core-architecture)  

---
  

## Handling Tokens

To handle Authenticated HTTP requests, you'll need to setup a middleware class that validates & re-issues JWT `'Bearer'` tokens upon request. Middleware is essential for handling these types of tasks before the controller method is even invoked. Which is why this package comes with a pre-defined abstract class, '**W.Ind.Core.Middleware.**`JwtAccessBase`' to greatly simplify this process.

### Define Middleware

```cs
using Microsoft.AspNetCore.Authorization;
using W.Ind.Core.Config;
using W.Ind.Core.Service;
using W.Ind.Core.Middleware;


// Generic type param "JwtConfig" can also be any type that inherits from JwtConfig & is injected into the app
public class JwtAccessMiddleware : JwtAccessBase<JwtConfig>
{
    public JwtAccessMiddleware(RequestDelegate next, JwtConfig config, IJwtInvalidator invalidator) 
        : base(next, config, invalidator) { }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip Middleware if endpoint uses [AllowAnonymous] 
        if (ShouldSkip(context))
        {
            await _next(context);
            return;
        }

        // Inheritted method
        // Validates token from request header
        // Adds a new token to the response header
        await ProcessTokenAsync(context);
    }

    // Only method in Base that's not implemented
    protected override bool ShouldSkip(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint != null && endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;
    }
}
```
**Note:** *You can `override` any methods defined in `JwtAccessBase` for extensible implementation*
### **Use Middleware**

Go back to your `Program.cs` file and configure your app to use `JwtAccessMiddleware`
```cs
app.MapControllers();

// Add this line
app.UseMiddleware<JwtAccessMiddleware>();

app.Run();
```

### **Testing Middleware**
To test out this custom middleware, you can add a new route to your `AuthController` for logging out. This will handle invalidating the current token, as well as ensuring no new token is sent to the client.

#### `/logout`

```cs
protected readonly IUserService<User, long> _userService;
// Add this member
protected readonly IJwtService<User, long> _jwtService;

// Update Constructor
public AuthController(IUserService<User, long> userService, IJwtService<User, long> jwtService) 
{
    _userService = userService;
    _jwtService = jwtService;
}

// No [AllowAnonymous] attribute
[HttpPost("logout")]
public IActionResult LogoutAsync() 
{
    // Remove response token
    // In the future, this will be handled within the base middleware class
    HttpContext.Response.Headers["Authorization"] = String.Empty;

    try
    {
        // Invalidate request token
        _jwtService.InvalidateToken(HttpContext.GetBearerToken());
        return Ok();
    }
    catch (Exception)
    {
        throw;
    }
}
```
You can now test `/logout` simply by running the app and sending the HTTP request, or integrating it into an existing HTTP Testing module.
  
[*\[Top\]*](#w-industries-core-architecture)  
  
---
## Extend 

With this package, you're free to extend/override any and all non-static classes and interfaces it comes with. Generic type parameters are included throughout the project that allow you to use derived custom child classes as with base services/helpers. This package includes very thorough XML documentation & symbols loaded for a robust intellisense experience. Inspect the package source code for a full understanding on what's happening under the hood.  

**Examples coming soon...**  

---
  
[*\[Top\]*](#w-industries-core-architecture)  
  
---
## Roadmap (v1)

|Category|Name|Details|Commits|Status|Date Completed|Version
|:----:|:----:|:----:|:----:|:----:|:----:|:----:|
|**EXPAND**|*User Service*| **Introduce:** Email Verification (SMTP Service), TwoFactor Authentication, & Role/Claim Services | N/A | Incomplete | TBD | TBD |
|**EXPAND**|*JWT Support*| **Introduce:** Refresh Tokens, TwoFactor Tokens, Ext. Login Tokens. **Refactor:** Existing JWT Service/Middleware| N/A | Incomplete | TBD | TBD |
|**README**|*Update #Extend Section*|**Walkthrough:** Extension Classes, Using Generics, Implementing Interfaces, Exploring Source | N/A | Incomplete | TBD | TBD |
|**README**|*Create #Debug Section*|**Walkthrough:** Debugging Source Code, Best Practices| N/A | Incomplete | TBD | TBD |
|**DOCS**|*Publish Official Site*| **Explore:** XML -> MD options (determine complexity) | N/A | Incomplete | TBD | TBD |

---  

[*\[Top\]*](#w-industries-core-architecture)  
  