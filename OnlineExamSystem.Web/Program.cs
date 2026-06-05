using OnlineExamSystem.Web.Extensions;
using OnlineExamSystem.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddCustomLogging(builder.Configuration);
builder.Services.AddCustomLocalization();
builder.Services.AddCustomMvc();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddScoped<DatabaseInitializationService>();  

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
    await dbInitializer.InitializeAsync();
}

// Middleware Pipeline
app.UseCustomMiddleware();
app.UseCustomSecurity();
app.UseCustomLocalization();
app.UseCustomLogging();
app.MapCustomRoutes();

app.Run();