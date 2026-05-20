using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.DependencyInjection;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Infrastructure.DependencyInjection;
using OnlineExamSystem.Infrastructure.Persistence;
using OnlineExamSystem.Web.Middleware;
using Serilog;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// ✅ Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
});

// ✅ Add Localization Services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("ar-EG")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

// ✅ Add MVC with View Localization
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// ✅ Application Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ✅ Apply DB and Seed Users
await ApplyDatabaseAsync(app);

// ✅ Use Request Localization
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

// ✅ Middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<LoggingEnrichmentMiddleware>();
app.UseSerilogRequestLogging();

// ✅ Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "exam",
    pattern: "{controller=Exam}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "userExam",
    pattern: "UserExam/{action=Index}/{id?}",
    defaults: new { controller = "UserExam" });

app.Run();

// ✅ Database Initialization and Seeding
async Task ApplyDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // ✅ Check if database exists using a simple SQL query
    var databaseExists = await CheckDatabaseExistsAsync(db);

    if (!databaseExists)
    {
        Console.WriteLine("Creating database...");
        await db.Database.MigrateAsync();
        Console.WriteLine("✅ Database created successfully.");
    }
    else
    {
        Console.WriteLine("Database already exists. Checking for pending migrations...");

        try
        {
            // Only apply pending migrations, don't try to create database
            var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
                await db.Database.MigrateAsync();
                Console.WriteLine("✅ Migrations applied successfully.");
            }
            else
            {
                Console.WriteLine("✅ Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Migration warning: {ex.Message}");
        }
    }

    // ✅ Seed users
    await SeedProductionUsersAsync(scope);
}

// ✅ Helper method to check if database exists without trying to create it
async Task<bool> CheckDatabaseExistsAsync(ApplicationDbContext db)
{
    try
    {
        // Try to execute a simple query
        await db.Database.ExecuteSqlRawAsync("SELECT 1");
        return true;
    }
    catch (SqlException ex)
    {
        // If database doesn't exist, error number 4060 is returned
        if (ex.Number == 4060) // Database not found
        {
            return false;
        }
        throw;
    }
}

async Task SeedProductionUsersAsync(IServiceScope scope)
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    try
    {
        // =====================================================
        // CREATE ROLES (Only if they don't exist)
        // =====================================================

        string[] roles = { "SuperAdmin", "Admin", "Teacher", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"✅ Role '{role}' created.");
            }
            else
            {
                Console.WriteLine($"Role '{role}' already exists.");
            }
        }

        // =====================================================
        // CREATE SUPER ADMIN
        // =====================================================

        var superAdminEmail = "superadmin@examify.com";
        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
        if (superAdmin == null)
        {
            superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true,
                FullName = "Super Administrator"
            };
            var result = await userManager.CreateAsync(superAdmin, "SuperAdmin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                await userManager.AddToRoleAsync(superAdmin, "Admin");
                await userManager.AddToRoleAsync(superAdmin, "Teacher");
                Console.WriteLine("✅ Super Admin created: superadmin@examify.com");
            }
            else
            {
                Console.WriteLine("❌ Failed to create Super Admin");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"   Error: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("Super Admin already exists.");
        }

        // =====================================================
        // CREATE DEMO ADMIN
        // =====================================================

        var demoAdminEmail = "demo@examify.com";
        var demoAdmin = await userManager.FindByEmailAsync(demoAdminEmail);
        if (demoAdmin == null)
        {
            demoAdmin = new ApplicationUser
            {
                UserName = demoAdminEmail,
                Email = demoAdminEmail,
                EmailConfirmed = true,
                FullName = "Demo Administrator"
            };
            var result = await userManager.CreateAsync(demoAdmin, "Demo@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoAdmin, "Admin");
                await userManager.AddToRoleAsync(demoAdmin, "Teacher");
                Console.WriteLine("✅ Demo Admin created: demo@examify.com");
            }
        }
        else
        {
            Console.WriteLine("Demo Admin already exists.");
        }

        // =====================================================
        // CREATE DEMO TEACHER
        // =====================================================

        var demoTeacherEmail = "teacher@examify.com";
        var demoTeacher = await userManager.FindByEmailAsync(demoTeacherEmail);
        if (demoTeacher == null)
        {
            demoTeacher = new ApplicationUser
            {
                UserName = demoTeacherEmail,
                Email = demoTeacherEmail,
                EmailConfirmed = true,
                FullName = "Demo Teacher"
            };
            var result = await userManager.CreateAsync(demoTeacher, "Teacher@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoTeacher, "Teacher");
                Console.WriteLine("✅ Demo Teacher created: teacher@examify.com");
            }
        }
        else
        {
            Console.WriteLine("Demo Teacher already exists.");
        }

        // =====================================================
        // CREATE DEMO STUDENT
        // =====================================================

        var demoStudentEmail = "student@examify.com";
        var demoStudent = await userManager.FindByEmailAsync(demoStudentEmail);
        if (demoStudent == null)
        {
            demoStudent = new ApplicationUser
            {
                UserName = demoStudentEmail,
                Email = demoStudentEmail,
                EmailConfirmed = true,
                FullName = "Demo Student"
            };
            var result = await userManager.CreateAsync(demoStudent, "Student@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoStudent, "Student");
                Console.WriteLine("✅ Demo Student created: student@examify.com");
            }
        }
        else
        {
            Console.WriteLine("Demo Student already exists.");
        }

        // =====================================================
        // CREATE ORIGINAL ADMIN
        // =====================================================

        var originalAdminEmail = "admin@site.com";
        var originalAdmin = await userManager.FindByEmailAsync(originalAdminEmail);
        if (originalAdmin == null)
        {
            originalAdmin = new ApplicationUser
            {
                UserName = originalAdminEmail,
                Email = originalAdminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator"
            };
            var result = await userManager.CreateAsync(originalAdmin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(originalAdmin, "SuperAdmin");
                await userManager.AddToRoleAsync(originalAdmin, "Admin");
                await userManager.AddToRoleAsync(originalAdmin, "Teacher");
                Console.WriteLine("✅ Original Admin created: admin@site.com");
            }
        }
        else
        {
            Console.WriteLine("Original Admin already exists.");
        }

        Console.WriteLine("✅ User seeding completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Seeding warning: {ex.Message}");
    }
}