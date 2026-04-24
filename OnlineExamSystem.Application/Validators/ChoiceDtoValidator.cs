using FluentValidation;
using OnlineExamSystem.Application.DTOs.Question;

namespace OnlineExamSystem.Application.Validators
{
    public class ChoiceDtoValidator : AbstractValidator<ChoiceDto>
    {
        public ChoiceDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Choice text is required");
        }
    }
}