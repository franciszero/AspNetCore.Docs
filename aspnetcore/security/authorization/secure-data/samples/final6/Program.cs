// final6

#define DEFAULT // ALT DEFAULT
#if NEVER
#elif DEFAULT
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using Microsoft.AspNetCore.Authorization;
using ContactManager.Authorization;

// snippet3 used in next define
#region snippet4  
#region snippet2
#region snippet
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    // Add Role services to Identity
    // Append AddRoles to add Role services:
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
#endregion

builder.Services.AddRazorPages();

// Require authenticated users
// Set the fallback authorization policy to require users to be authenticated
// Setting the fallback policy is the preferred way to require all users be authenticated.
builder.Services.AddAuthorization(options =>
{
    /**
     * The fallback authorization policy:
     *      Is applied to all requests that don't explicitly specify an authorization policy. 
     *      
     *      For requests served by endpoint routing, this includes any endpoint that doesn't specify an authorization attribute. 
     *      For requests served by other middleware after the authorization middleware, such as static files, this applies the policy to all requests.
     */
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        // RequireAuthenticatedUser adds DenyAnonymousAuthorizationRequirement to the current instance, which enforces that the current user is authenticated.
        .RequireAuthenticatedUser()
        .Build();
});
#endregion

/**
 * Services using Entity Framework Core must be registered for dependency injection using AddScoped. 
 * The ContactIsOwnerAuthorizationHandler uses ASP.NET Core Identity, which is built on Entity Framework Core. 
 * Register the handlers with the service collection so they're available to the ContactsController through dependency injection. 
 * Add the following code to the end of ConfigureServices:
 * 
 * ContactAdministratorsAuthorizationHandler and ContactManagerAuthorizationHandler are added as singletons. 
 * They're singletons because they don't use EF and all the information needed is in the Context parameter of the HandleRequirementAsync method.
 */
// Authorization handlers.
builder.Services.AddScoped<IAuthorizationHandler,
                      ContactIsOwnerAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler,
                      ContactAdministratorsAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler,
                      ContactManagerAuthorizationHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    // option 1:
    //  requires using Microsoft.Extensions.Configuration;
    //  Set password with the Secret Manager tool.
    //  dotnet user-secrets set SeedUserPW <pw>
    // option 2:
    //  appsettings.json: "SeedUserPW": "your_seed_user_password_here"
    var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW");
    await SeedData.Initialize(services, testUserPw);
}
#endregion

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
#elif ALT
#region snippet3
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();
#endregion

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
#endif