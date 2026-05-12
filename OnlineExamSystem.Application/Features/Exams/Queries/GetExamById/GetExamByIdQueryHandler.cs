using MediatR;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Queries.GetExamById
{
    public class GetExamByIdQueryHandler : IRequestHandler<GetExamByIdQuery, CreateExamDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetExamByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateExamDto?> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.Id);
            if (exam == null) return null;

            return new CreateExamDto
            {
                Title = exam.Title,
                DurationInMinutes = exam.DurationInMinutes,
                TotalDegree = exam.TotalDegree
            };
        }
    }
}