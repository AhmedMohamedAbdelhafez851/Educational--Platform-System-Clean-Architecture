using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Web.ViewModels.UserDTO;

namespace OnlineExamSystem.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // =====================================================
        // INDEX
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userManager.Users
                    .OrderBy(x => x.FullName)
                    .ToListAsync();

                var userRoles = new Dictionary<string, List<string>>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    userRoles[user.Id] = roles.ToList();
                }

                ViewBag.UserRoles = userRoles;

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");

                return View(new List<ApplicationUser>());
            }
        }

        // =====================================================
        // CREATE USER
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        success = false,
                        message = string.Join(", ", errors)
                    });
                }

                var emailExists =
                    await _userManager.FindByEmailAsync(model.Email);

                if (emailExists != null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Email already exists"
                    });
                }

                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    UserName = model.Email,
                    EmailConfirmed = true
                };

                var result =
                    await _userManager.CreateAsync(
                        user,
                        model.Password);

                if (!result.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = string.Join(
                            ", ",
                            result.Errors.Select(x => x.Description))
                    });
                }

                // DEFAULT ROLE
                if (await _roleManager.RoleExistsAsync("Student"))
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                }

                return Json(new
                {
                    success = true,
                    message = "User created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while creating user"
                });
            }
        }

        // =====================================================
        // EDIT USER
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Edit(
            [FromBody] EditUserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid data"
                    });
                }

                var user =
                    await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }

                var emailExists =
                    await _userManager.Users.AnyAsync(x =>
                        x.Email == model.Email &&
                        x.Id != model.Id);

                if (emailExists)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Email already exists"
                    });
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result =
                    await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = string.Join(
                            ", ",
                            result.Errors.Select(x => x.Description))
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "User updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing user");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating user"
                });
            }
        }

        // =====================================================
        // DELETE USER
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var user =
                    await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }

                var result =
                    await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = string.Join(
                            ", ",
                            result.Errors.Select(x => x.Description))
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "User deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while deleting user"
                });
            }
        }

        // =====================================================
        // GET USER ROLES
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                var user =
                    await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }

                var allRoles =
                    await _roleManager.Roles.ToListAsync();

                var userRoles =
                    await _userManager.GetRolesAsync(user);

                var roles = allRoles.Select(x => new RoleDto
                {
                    Id = x.Id,
                    Name = x.Name ?? "",
                    IsAssigned = userRoles.Contains(x.Name!)
                }).ToList();

                return Json(new
                {
                    success = true,
                    roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading roles");

                return Json(new
                {
                    success = false,
                    message = "Error loading roles"
                });
            }
        }

        // =====================================================
        // UPDATE USER ROLES
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(
            [FromBody] ManageRolesViewModel model)
        {
            try
            {
                var user =
                    await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }

                var selectedRoles =
                    model.UserRoles ?? new List<string>();

                var currentRoles =
                    await _userManager.GetRolesAsync(user);

                var rolesToRemove =
                    currentRoles.Except(selectedRoles);

                var removeResult =
                    await _userManager.RemoveFromRolesAsync(
                        user,
                        rolesToRemove);

                if (!removeResult.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed removing roles"
                    });
                }

                var rolesToAdd =
                    selectedRoles.Except(currentRoles);

                var addResult =
                    await _userManager.AddToRolesAsync(
                        user,
                        rolesToAdd);

                if (!addResult.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed adding roles"
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Roles updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating roles");

                return Json(new
                {
                    success = false,
                    message = "Error updating roles"
                });
            }
        }
    }
}