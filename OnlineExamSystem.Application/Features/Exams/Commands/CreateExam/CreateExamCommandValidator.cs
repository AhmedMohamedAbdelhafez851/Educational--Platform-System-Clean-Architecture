using FluentValidation;

namespace OnlineExamSystem.Application.Features.Exams.Commands.CreateExam
{
    public class CreateExamCommandValidator : AbstractValidator<CreateExamCommand>
    {
        public CreateExamCommandValidator()
        {
            RuleFor(x => x.Dto.Title).NotEmpty().MinimumLength(3).MaximumLength(200);
            RuleFor(x => x.Dto.DurationInMinutes).GreaterThan(0);
            RuleFor(x => x.Dto.TotalDegree).GreaterThan(0);
        }
    }
}