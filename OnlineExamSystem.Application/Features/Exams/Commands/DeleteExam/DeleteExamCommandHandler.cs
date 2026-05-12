using MediatR;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Commands.DeleteExam
{
    public class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public DeleteExamCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Unit> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.Id);
            if (exam == null) throw new Exception("Exam not found");

            await _unitOfWork.Repository<Exam>().DeleteAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            _cacheService.Remove("AllExams");

            return Unit.Value;
        }
    }
}