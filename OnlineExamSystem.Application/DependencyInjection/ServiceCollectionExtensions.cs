using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.Behaviors;
using OnlineExamSystem.Application.Caching;
using OnlineExamSystem.Application.Services;
using OnlineExamSystem.Application.Validators;
using OnlineExamSystem.Infrastructure.Services;

namespace OnlineExamSystem.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // ✅ MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(
                    typeof(ServiceCollectionExtensions).Assembly);
            });

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));


            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(AuditBehavior<,>));

            // ✅ NEW: Account Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRedirectionService, RedirectionService>();
            services.AddScoped<ILanguageService, LanguageService>();

            // ✅ Cache
            services.AddMemoryCache();

            services.AddSingleton<ICacheService, CacheService>();

            // ✅ Services
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IExamSubmissionService, ExamSubmissionService>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuditService, AuditService>();

            // ✅ Validators
            services.AddValidatorsFromAssemblyContaining<CreateQuestionDtoValidator>();

            services.AddHttpContextAccessor();

            return services;
        }
    }
}