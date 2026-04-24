using FluentValidation;
using OnlineExamSystem.Application.DTOs.Question;

namespace OnlineExamSystem.Application.Validators
{
    public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
    {
        public CreateQuestionDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(3);

            RuleFor(x => x.ExamId)
                .NotEmpty();

            RuleFor(x => x.CorrectChoiceIndex)
                .InclusiveBetween(0, 3);

            RuleFor(x => x.Choices)
                .Must(x => x.Count == 4)
                .WithMessage("Exactly 4 choices required");

            RuleForEach(x => x.Choices)
                .SetValidator(new ChoiceDtoValidator());
        }
    }
}