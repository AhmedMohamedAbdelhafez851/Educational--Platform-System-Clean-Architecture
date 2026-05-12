using MediatR;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Commands.CreateExam
{
    public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public CreateExamCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<int> Handle(CreateExamCommand request, CancellationToken cancellationToken)
        {
            var exam = new Exam
            {
                Title = request.Dto.Title,
                DurationInMinutes = request.Dto.DurationInMinutes,
                TotalDegree = request.Dto.TotalDegree,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Exam>().AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            _cacheService.Remove("AllExams");

            return exam.ExamId;
        }
    }
}