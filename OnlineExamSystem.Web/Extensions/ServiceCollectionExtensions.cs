using System.Globalization;
using Microsoft.AspNetCore.Localization;
using OnlineExamSystem.Application.DependencyInjection;
using OnlineExamSystem.Infrastructure.DependencyInjection;
using OnlineExamSystem.Web.Services;
using Serilog;

namespace OnlineExamSystem.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
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

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();
            return services;
        }

        public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(configuration).Enrich.FromLogContext();
            });
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplication();
            services.AddInfrastructure(configuration);
            return services;
        }

        // ✅ NEW: Register Web-specific services
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddScoped<DatabaseInitializationService>();
            return services;
        }
    }
}