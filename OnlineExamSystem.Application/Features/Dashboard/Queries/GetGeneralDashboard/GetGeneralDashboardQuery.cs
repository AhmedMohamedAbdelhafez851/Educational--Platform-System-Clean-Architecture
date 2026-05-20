using MediatR;
using OnlineExamSystem.Application.DTOs.Dashboard;

namespace OnlineExamSystem.Application.Features.Dashboard.Queries.GetGeneralDashboard
{
    public class GetGeneralDashboardQuery : IRequest<GeneralDashboardDto>
    {
    }
}