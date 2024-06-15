# <center id="title">W. Industries Core Architecture - <b>`W.Ind.Core`</b></center>

<center><b><em>Solutions Built From the Ground Up</em></b></center>
<br />


<table border="0" width="100%">
    <thead>
        <tr>
            <th scope="col">
                <center>
                    <details>
                        <summary>
                            <a href="#quick-start">1. Quick Start</a>
                        </summary>
                        <br>
                        <a href="#define-context"><div>2. Define</div></a>
                        <br>
                        <a href="#install-packages"><div>3. Install</div></a>
                        <br>
                        <a href="#configure-services"><div>4. Configure</div></a>
                        <br>
                        <a href="#seed-user"><div>5. Audit/Seed</div></a>
                        <br>
                        <a href="#create-migration"><div>6. Migrate</div></a>
                        <br>
                        <a href="#somedbcontextcs"><div>7. `DbContext.cs`</div></a>
                        <br>
                        <a href="#programcs"><div>8. `Program.cs`</div></a>
                        <br>
                    </details>
                </center>
            </th>
            <th scope="col">
                <center>
                    <details>
                        <summary>
                            <a href="#authcontroller">9. AuthController</a>
                        </summary>
                        <br>
                        <a href="#implement-register"><div>10. '/register'</div></a>
                        <br>
                        <a href="#implement-login"><div>11. '/login'</div></a>
                        <br>
                    </details>
                </center>
            </th>
            <th scope="col">
                <center>
                    <details>
                        <summary>
                            <a href="#handling-tokens">12. Handle Tokens</a>
                        </summary>
                        <br>
                        <a href="#define-middleware"><div>13. Middleware</div></a>
                        <br>
                        <a href="#logout"><div>14. '/logout'</div></a>
                        <br>
                    </details>
                </center>
            </th>
            <th scope="col">
                <details>
                    <summary>
                        <center>
                            <a href="#extend"><div>15. Extensions</div</a>
                        <center>
                    </summary>
                </details>
            </th>
            <th scope="col">
                <details>
                    <summary>
                        <center>
                            <a href="#roadmap"><div>Road Map</div></a>
                        <center>
                    </summary>
                    <br>
                </details>
            </th>
        </tr>
    </thead>
</table>

## About

Hit the ground running in <b>.NET 8</b> when you create an <b>EF Core</b> database, backed by <b>`Microsoft.AspNetCore.Identity`</b>. With extentible (pre-defined) core entity classes, you can immediately define your `IdentityDbContext` class after installation. Extensive intellisense/xml documentation. As well as simplified services, static helper classes, & base middleware defined for handling common use-cases. Such as:

- Temporal Audit Logging
- Easy Soft Delete implementation
- JWT Generation/Validation/Invalidation
- Generic UTC DateTime parsing
- Auth handling


## Quick Start

### <b id="define-context">Define IdentityDbContext</b>

Once installed, you have the ability to <em>immediately</em> define a Context class that inherits from `IdentityDbContext`


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
### <b id="install-packages">Install Packages</b>

Before configuring services to use this `DbContext`. There are a couple packages you should install in your API/Controller layer:
- <b><em>(latest)</em></b> `Microsoft.AspNetCore.Authentication.JwtBearer`
- <b><em>(latest)</em></b> `Microsoft.EntityFrameworkCore.Design`
- If your `DbContext` is located in a separate project, add a reference to the project containing it

---
### <b id="configure-services">Configure Services</b>

Once packages are installed. Go to your `Program.cs` file and configure the following services:

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
<center><a href="#title"><sub>[Top]</sub></a></center>

<br>

### <b id="seed-user">Seed User</b> (Required for Audit Logging)

A seeded System User is <b>required</b> to perform audit logs which reference the User who created a record. This is the <b>default</b> behavior for most concrete `W.Core.Ind.Entity` classes. Meaning you need a migration that seeds a System User.

<b>Note:</b> <em>You can change this default behavior and choose not to use Audit Logging by re-implementing those core entity's `abstract` base parent classes and omitting the `IAuditable` implementation <sub>(see [Extensions](#extend))</sub></em>

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
<center>
<b>Note:</b> The Password is "password"
</center>

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
<center><a href="#title"><sub>[Top]</sub></a></center>

<br>

### <b id="create-migration">Create Migration/Update Database</b>

<b>Create Initial Migration</b>

Using CLI (assuming you have `dotnet ef` CLI tools installed) navigate to the project folder containing your `DbContext` class and add your initial migration:
```ps
dotnet ef migrations add "Initial"

# Run when DbContext is located outside your startup project
dotnet ef migrations add "Initial" -s "../YourStartupProject"
```

<b>Update Database</b>

Once you've verified the migration was added successfully. In the same directory, run the CLI command `dotnet ef database update`:
```ps
dotnet ef database update

# Run when DbContext is located outside your startup project
dotnet ef database update -s "../StartupProject"
```

<center><a href="#title"><sub>[Top]</sub></a></center>

<br>

### Database Created

You've now completed setting up your identity backed database with core auth entities configured to handle both soft deletes, and audit logging. In case you weren't keeping track, that whole process required less than 100 lines of C# code over the span of 2 files `Program.cs` and `SomeDbContext`. The rest of the changes were related to either:
1. [API layer dependencies](#install-packages)
2. [Your Configuration/appSettings file](#configure-services) <em>(see comment)</em>
3. [Copying/Inserting Seed Data file(s)](#seed-user)
4. [Adding your migration via `dotnet ef` CLI](#initial-migration)

Here's an expanded look at all the C# file changes made up until this point:

#### `SomeDbContext.cs`
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
#### `Program.cs`

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

<center><a href="#title"><sub>[Top]</sub></a></center>

<br>

## `AuthController`

After the database is created you can begin implementing your `AuthController`. The two simplest examples would be with your `/register` and `/login` routes. Here, we'll go over how to implement these routes with the injected `UserService`.

1. <b>Define the `AuthController`</b>

```cs
using W.Ind.Core.Entity;
using W.Ind.Core.Service;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // Inject UserService<TUser, TKey>
    protected readonly IUserService<User, long> _userService;

    public AuthController(IUserService<User, long> userService) 
    {
        _userService = userService;
    }
}
```
---
2. <b id="implement-register">Implement `/register`</b>
```cs
// Add this method to your Auth Controller
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

*NOTE:* <em>You can extend the `UserRegistraton` DTO to include more custom data, and override the `UserService.Register` method to use that class. <sub>(see [Extensions](#extend))</sub></em>

---
3. <b id="implement-login">Implement `/login`</b>
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

*NOTE:* <em>You can extend the `LoginRequest` & `LoginResponse` DTOs to include more custom data, and override the `UserService.ValidateLoginAsync` method to use those fields. <sub>(see [Extensions](#extend))</sub></em>

<center><a href="#title"><sub>[Top]</sub></a></center>

<br>

## Handling Tokens

#### Define Middleware

To access routes where JWT validation is required. You will need to implement a custom middleware class that processes your token, and generates a new token to return to the client for each HTTP request. You can simplify this process by implementing the base class, <b>W.Ind.Core.Middleware.</b>`JwtAccessBase`:
```cs
using Microsoft.AspNetCore.Authorization;
using W.Ind.Core.Config;
using W.Ind.Core.Service;
using W.Ind.Core.Middleware;


// Generic type param "JwtConfig" can also be any type that inherits from JwtConfig & is injected into the app
public class JwtAccessMiddleware : JwtAccessBase<JwtConfig>
{
    public JwtAccessMiddleware(RequestDelegate next, JwtConfig config, IJwtInvalidator invalidator) 
        : base(next, config, invalidator) 
    { }

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
<b>Note:</b> <em>You can `override` any methods defined in `JwtAccessBase` for extensible implementation</em>
### <b>Use Middleware</b>

Go back to your `Program.cs` file and configure your app to use `JwtAccessMiddleware`
```cs
app.MapControllers();

// Add this line
app.UseMiddleware<JwtAccessMiddleware>();

app.Run();
```

#### <b id="logout">Testing - `/logout`</b>
To test out this custom middleware, you can add a new route to your `AuthController` for logging out. This will handle invalidating the current token, as well as ensuring no new token is sent to the client.

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

<center><a href="#title"><sub>[Top]</sub></a></center>

<br>

## Extend 
With this package, you're free to extend/override any and all non-static classes and interfaces it comes with. Generic type parameters are included throughout the project that allow you to use derived custom child classes as with base services/helpers. Package includes very thorough XML documentation & symbols loaded a robust intellisense experience. Inspect the package source code for a full understanding on what's happening under the hood. 
<br>
<br>
<center><b>Examples coming soon...</b></center>
<br>
<br>

---

<center><a href="#title"><sub>[Top]</sub></a></center>
<br>

## Roadmap

<center><b>V1</b></center>
<table border="1" width="100%">
    <thead>
        <tr>
            <th><center>Description</center</th>
            <th><center>Tasks</center></th>
            <th><center>Status</center></th>
            <th><center>Date Completed</center></th>
            <th><center>Version</center></th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><center><em><b>Expand User Service</b></em></center></td>
            <td><center><em><br>
                <div>- <b>Introduce:</b> Email Verification (SMTP Service)</div>
                <br>
                <div>- <b>Introduce:</b> TwoFactor Authentication</div>
                <br>
                <div>- <b>Introduce:</b> Role Service</div>
                <br>
                <div>- <b>Expand:</b> Claims Provider</div>
            <br></em></center></td>
            <td><center><input type="checkbox" disabled /></center></td>
            <td><center>TBD</center></td>
            <td><center>TBD</center></td>
        </tr>
        <tr>
            <td><center><em><b>Expand JWT Support</b></em></center></td>
            <td><center><em><br>
                <div>- <b>Support:</b> Refresh Tokens</div>
                <br>
                <div>- <b>Support:</b> TwoFactor Tokens</div>
                <br>
                <div>- <b>Support:</b> External Login Tokens</div>
                <br>
                <div>- <b>Related:</b> Base Middleware</div>
                <br />
                <div>- <b>Refactor:</b> Existing JWT Services</div>
            <br></em></center></td>
            <td><center><input type="checkbox" disabled /></center></td>
            <td><center>TBD</center></td>
            <td><center>TBD</center></td>
        </tr>
        <tr>
            <td><center><em><b>Update README #Extensions Section</b></div></em></center></td>
            <td><center><em><br>
                <div>- <b>Walkthrough:</b> Extension Classes</div>
                <br>
                <div>- <b>Walkthrough:</b> Extensions & Generics</div>
                <br>
                <div>- <b>Walkthrough:</b> Interface Implementation</div>
                <br>
                <div>- <b>Walkthrough:</b> Exploring Source Code</div>
            <br></em></center></td>
            <td><center><input type="checkbox" disabled /></center></td>
            <td><center>TBD</center></td>
            <td><center>TBD</center></td>
        </tr>
        <tr>
            <td><center><em><b>Publish Official Docs Site</b></em></center></td>
            <td><center><em><br>
                <div>- Explore XML to MD options</div>
                <br>
                <div>- Determine complexity</div>
            <br></em></center></td>
            <td><center><input type="checkbox" disabled /></center></td>
            <td><center>TBD</center></td>
            <td><center>TBD</center></td>
        </tr>
        <tr></tr>
    </tbody>
</table>
<center><a href="#title"><sub>[Top]</sub></a></center>
<br>
