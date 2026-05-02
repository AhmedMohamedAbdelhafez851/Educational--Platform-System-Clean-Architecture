using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.DependencyInjection;
using OnlineExamSystem.Infrastructure.DependencyInjection;
using OnlineExamSystem.Infrastructure.Persistence;
using OnlineExamSystem.Domains.Entities;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// ✅ Services
builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ✅ Apply Migrations + Seed Data
await ApplyDatabaseAsync(app);

// ✅ Middleware
app.UseMiddleware<ExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// 🔥 Method: Migration + Seed
async Task ApplyDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // ✅ Seed Admin User
    var email = "admin@site.com";
    var password = "Admin@123";

    var existingUser = await userManager.FindByEmailAsync(email);

    if (existingUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, password);
    }
}