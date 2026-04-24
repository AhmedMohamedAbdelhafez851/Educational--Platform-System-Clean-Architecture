//using FluentValidation;
//using OnlineExamSystem.Web.ViewModels;

//namespace OnlineExamSystem.Application.Validators
//{
//    public class QuestionViewModelValidator : AbstractValidator<QuestionViewModel>
//    {
//        public QuestionViewModelValidator()
//        {
//            RuleFor(x => x.Title)
//                .NotEmpty()
//                .MinimumLength(3);

//            RuleFor(x => x.ExamId)
//                .NotEmpty();

//            RuleFor(x => x.CorrectChoiceIndex)
//                .InclusiveBetween(0, 3);

//            RuleFor(x => x.Choices)
//                .NotNull()
//                .Must(x => x.Count == 4)
//                .WithMessage("Exactly 4 choices are required.");

//            //RuleForEach(x => x.Choices)
//            //    .SetValidator(new ChoiceViewModelValidator());
//        }
//    }
//}