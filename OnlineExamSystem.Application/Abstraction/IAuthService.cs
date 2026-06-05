using Microsoft.AspNetCore.Identity;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Web.ViewModels.UserDTO;
using System.Security.Claims;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IAuthService
    {
        Task<ApplicationUser?> FindUserByEmailAsync(string email);
        Task<bool> IsUserLockedOutAsync(ApplicationUser user);
        Task<int?> GetLockoutRemainingSecondsAsync(ApplicationUser user);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockoutOnFailure);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task SignInAsync(ApplicationUser user, bool isPersistent);
        Task SignOutAsync();
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    }
}