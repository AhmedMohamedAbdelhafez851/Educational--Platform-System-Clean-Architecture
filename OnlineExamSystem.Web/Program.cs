using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.DependencyInjection;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Infrastructure.DependencyInjection;
using OnlineExamSystem.Infrastructure.Persistence;
using OnlineExamSystem.Web.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
});

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply DB
await ApplyDatabaseAsync(app);

// Middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<LoggingEnrichmentMiddleware>();
app.UseSerilogRequestLogging();

// Routes
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "exam", pattern: "{controller=Exam}/{action=Index}/{id?}");

app.Run();

async Task ApplyDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Seed Roles
    string[] roles = { "Admin", "Teacher", "Student" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    // Seed Admin
    var email = "admin@site.com";
    var admin = await userManager.FindByEmailAsync(email);
    if (admin == null)
    {
        admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        await userManager.CreateAsync(admin, "Admin@123");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}