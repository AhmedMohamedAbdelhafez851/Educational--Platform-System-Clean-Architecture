using MediatR;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Commands.UpdateExam
{
    public class UpdateExamCommandHandler : IRequestHandler<UpdateExamCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public UpdateExamCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Unit> Handle(UpdateExamCommand request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.Id);
            if (exam == null) throw new Exception("Exam not found");

            exam.Title = request.Dto.Title;
            exam.DurationInMinutes = request.Dto.DurationInMinutes;
            exam.TotalDegree = request.Dto.TotalDegree;

            await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            _cacheService.Remove("AllExams");

            return Unit.Value;
        }
    }
}