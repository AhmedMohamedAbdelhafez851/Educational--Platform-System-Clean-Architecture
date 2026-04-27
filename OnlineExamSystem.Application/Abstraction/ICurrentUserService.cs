using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExamSystem.Infrastructure.Services
{
    public interface ICurrentUserService
    {
        string GetUserId();
    }
}
