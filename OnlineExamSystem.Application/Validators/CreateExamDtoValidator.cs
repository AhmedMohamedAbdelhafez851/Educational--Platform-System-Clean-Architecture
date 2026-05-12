//using FluentValidation;
//using OnlineExamSystem.Application.DTOs.Exam;

//namespace OnlineExamSystem.Application.Validators
//{
//    public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
//    {
//        public CreateExamDtoValidator()
//        {
//            RuleFor(x => x.Title)
//                .NotEmpty()
//                .MinimumLength(3)
//                .MaximumLength(100);
//        }
//    }
//}