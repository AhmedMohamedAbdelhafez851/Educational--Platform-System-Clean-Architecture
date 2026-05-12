using FluentValidation;

namespace OnlineExamSystem.Application.Features.Exams.Commands.UpdateExam
{
    public class UpdateExamCommandValidator
        : AbstractValidator<UpdateExamCommand>
    {
        public UpdateExamCommandValidator()
        {
            RuleFor(x => x.Dto.Title)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);
        }
    }
}