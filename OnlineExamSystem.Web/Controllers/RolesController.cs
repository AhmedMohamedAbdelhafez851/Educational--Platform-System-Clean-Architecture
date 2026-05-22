using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Web.ViewModels.UserDTO;
using System.Text.Json;

namespace OnlineExamSystem.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: ManageRoles
        public async Task<IActionResult> ManageRoles(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID cannot be empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new ManageRolesViewModel
            {
                UserId = user.Id,
                UserRoles = userRoles.ToList(),
                Roles = roles.Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name!,
                    IsAssigned = userRoles.Contains(role.Name!)
                }).ToList()
            };

            return View(model);
        }

        // POST: ManageRoles
        [HttpPost]
        public async Task<IActionResult> ManageRoles(ManageRolesViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId) || model.Roles == null)
            {
                return BadRequest("User ID or Roles cannot be null.");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles.Where(r => r.IsAssigned).Select(r => r.Name).ToList();

            var addResult = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError("", $"Failed to add role '{error.Description}'.");
                }
                return View(model);
            }

            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError("", $"Failed to remove role '{error.Description}'.");
                }
                return View(model);
            }

            return RedirectToAction("Index", "Users");
        }

        // GET: CreateRole (View)
        public IActionResult CreateRole()
        {
            var model = new ManageRolesViewModel
            {
                Roles = _roleManager.Roles.Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name!
                }).ToList()
            };
            return View(model);
        }

        // POST: CreateRole - API endpoint for modal (FIXED)
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleApiModel request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.NewRoleName))
                {
                    return Json(new { success = false, message = "Role name is required" });
                }

                var roleName = request.NewRoleName.Trim();

                // Check if role exists
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (roleExists)
                {
                    return Json(new { success = false, message = $"Role '{roleName}' already exists" });
                }

                // Create the role
                var newRole = new IdentityRole(roleName);
                var result = await _roleManager.CreateAsync(newRole);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role '{roleName}' created successfully.");
                    return Json(new { success = true, message = $"Role '{roleName}' created successfully" });
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return Json(new { success = false, message = "An error occurred while creating the role" });
            }
        }

        // GET: EditRole (View)
        public async Task<IActionResult> EditRole(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Role ID cannot be empty.");
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var model = new RoleDto
            {
                Id = role.Id,
                Name = role.Name!
            };

            return View(model);
        }

        // POST: EditRole - API endpoint for modal (FIXED)
        [HttpPost]
        public async Task<IActionResult> EditRole([FromBody] EditRoleApiModel request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.Name))
                {
                    return Json(new { success = false, message = "Role ID and name are required" });
                }

                var role = await _roleManager.FindByIdAsync(request.Id);
                if (role == null)
                {
                    return Json(new { success = false, message = "Role not found" });
                }

                var newRoleName = request.Name.Trim();

                // Check if another role with the same name exists
                if (role.Name != newRoleName)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(newRoleName);
                    if (roleExists)
                    {
                        return Json(new { success = false, message = $"Role '{newRoleName}' already exists" });
                    }
                }

                // Update role name
                var oldName = role.Name;
                role.Name = newRoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role '{oldName}' updated to '{newRoleName}' successfully.");
                    return Json(new { success = true, message = $"Role updated successfully" });
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing role");
                return Json(new { success = false, message = "An error occurred while updating the role" });
            }
        }

        // POST: DeleteRole (FIXED - returns JSON)
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "Role ID cannot be empty." });
                }

                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return Json(new { success = false, message = "Role not found." });
                }

                var roleName = role.Name;
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role '{roleName}' deleted successfully.");
                    return Json(new { success = true, message = $"Role '{roleName}' deleted successfully." });
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning($"Failed to delete role '{roleName}': {errors}");
                return Json(new { success = false, message = $"Failed to delete role: {errors}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting role with ID {id}");
                return Json(new { success = false, message = "An unexpected error occurred while deleting the role." });
            }
        }

        // GET: GetAllRoles (for modal)
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _roleManager.Roles
                    .Select(r => new { id = r.Id, name = r.Name })
                    .ToListAsync();

                return Json(new { success = true, roles = roles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
                return Json(new { success = false, message = "Error loading roles" });
            }
        }
    }

    // API Models for JSON requests
    public class CreateRoleApiModel
    {
        public string NewRoleName { get; set; } = "";
    }

    public class EditRoleApiModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}