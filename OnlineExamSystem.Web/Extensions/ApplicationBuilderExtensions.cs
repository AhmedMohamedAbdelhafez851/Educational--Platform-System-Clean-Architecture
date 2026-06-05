using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using OnlineExamSystem.Web.Middleware;
using Serilog;

namespace OnlineExamSystem.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseCustomLocalization(this IApplicationBuilder app)
        {
            var localizationOptions = app.ApplicationServices
                .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            return app;
        }

        public static IApplicationBuilder UseCustomLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggingEnrichmentMiddleware>();
            app.UseSerilogRequestLogging();
            return app;
        }

        public static IApplicationBuilder UseCustomSecurity(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        public static IApplicationBuilder MapCustomRoutes(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");

                endpoints.MapControllerRoute(
                    name: "exam",
                    pattern: "{controller=Exam}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "userExam",
                    pattern: "UserExam/{action=Index}/{id?}",
                    defaults: new { controller = "UserExam" });
            });
            return app;
        }
    }
}