using LOCPS.Common;
using LOCPS.Constants;
using LOCPS.DTOs;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using LOCPS.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers.Api;

[ApiController]
[Route("api/users")]
public class UsersApiController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersApiController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResult<IEnumerable<User>>.Ok(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(ApiResult.Fail("User not found."));

            return Ok(ApiResult<User>.Ok(user));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Get users by role
    /// </summary>
    [HttpGet("role/{role}")]
    public async Task<IActionResult> GetByRole(Roles role)
    {
        try
        {
            var users = await _userService.GetUsersByRoleAsync(role);
            return Ok(ApiResult<IEnumerable<User>>.Ok(users));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Register a new user (with custom roleId)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

        try
        {
            var created = await _userService.RegisterUserAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.UserId }, ApiResult<User>.Ok(created, "User registered successfully."));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Register a new Admin user
    /// </summary>
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

        try
        {
            var model = new UserCreateViewModel
            {
                UserName = req.UserName,
                Email = req.Email,
                FullName = req.FullName,
                PhoneNumber = req.PhoneNumber,
                Password = req.Password,
                RoleId = RoleConstants.AdminRoleId // 2
            };

            var created = await _userService.RegisterUserAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.UserId }, ApiResult<User>.Ok(created, "Admin registered successfully."));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Register a new Customer user
    /// </summary>
    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

        try
        {
            var model = new UserCreateViewModel
            {
                UserName = req.UserName,
                Email = req.Email,
                FullName = req.FullName,
                PhoneNumber = req.PhoneNumber,
                Password = req.Password,
                RoleId = RoleConstants.CustomerRoleId // 1
            };

            var created = await _userService.RegisterUserAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.UserId }, ApiResult<User>.Ok(created, "Customer registered successfully."));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var user = await _userService.LoginAsync(model.Email, model.Password);
            if (user == null)
                return Unauthorized(ApiResult.Fail("Invalid credentials."));

            return Ok(ApiResult<User>.Ok(user, "Login successful."));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateViewModel model)
    {
        if (id != model.UserId)
            return BadRequest(ApiResult.Fail("User ID mismatch."));

        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var updated = await _userService.UpdateUserAsync(model);
            return Ok(ApiResult<User>.Ok(updated, "User updated successfully."));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Delete user by ID
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(ApiResult<bool>.Ok(result, "User deleted successfully."));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("{id:int}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest req)
    {
        try
        {
            var result = await _userService.ChangePasswordAsync(id, req.OldPassword, req.NewPassword);
            return Ok(ApiResult<bool>.Ok(result, "Password changed successfully."));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Assign role to user
    /// </summary>
    [HttpPost("{id:int}/assign-role")]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleRequest req)
    {
        try
        {
            var result = await _userService.AssignRoleAsync(id, req.RoleId);
            return Ok(ApiResult<bool>.Ok(result, "Role assigned successfully."));
        }
        catch (ServiceException ex)
        {
            return StatusCode(ex.StatusCode, ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }
}
