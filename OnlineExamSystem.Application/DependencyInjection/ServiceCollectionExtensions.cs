using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.Services;
using OnlineExamSystem.Application.Validators;
using OnlineExamSystem.Infrastructure.Services;
//using OnlineExamSystem.Application.Validators;
namespace OnlineExamSystem.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IExamSubmissionService, ExamSubmissionService>();
            services.AddValidatorsFromAssemblyContaining<CreateQuestionDtoValidator>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuditService, AuditService>();

            services.AddHttpContextAccessor();


            return services;
        }
    }
}

