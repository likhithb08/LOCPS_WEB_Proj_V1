using LOCPS.Common;
using LOCPS.DTOs;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers.Api;

[ApiController]
[Route("api/loanapplications")]
public class LoanApplicationsApiController : ControllerBase
{
    private readonly ILoanApplicationService _applicationService;

    public LoanApplicationsApiController(ILoanApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    /// <summary>
    /// Create loan application
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LoanApplication application)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var created = await _applicationService.CreateAsync(application);
            return CreatedAtAction(nameof(GetById), new { id = created.ApplicationId }, ApiResult<LoanApplication>.Ok(created, "Loan application created successfully."));
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
    /// Get loan application by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var application = await _applicationService.GetByIdAsync(id);
            if (application == null)
                return NotFound(ApiResult.Fail("Application not found."));

            return Ok(ApiResult<LoanApplication>.Ok(application));
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
    /// Search loan applications with paging and filters
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] ApplicationStatus? status = null, [FromQuery] int? customerId = null)
    {
        try
        {
            var query = new PagedQuery { Page = pageIndex, PageSize = pageSize, Search = searchTerm };
            var result = await _applicationService.SearchAsync(query, status, customerId);
            return Ok(ApiResult<PagedResult<LoanApplication>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update loan application
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] LoanApplication application)
    {
        if (id != application.ApplicationId)
            return BadRequest(ApiResult.Fail("Application ID mismatch."));

        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var updated = await _applicationService.UpdateAsync(application);
            return Ok(ApiResult<LoanApplication>.Ok(updated, "Loan application updated successfully."));
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
    /// Update loan application status
    /// </summary>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req)
    {
        try
        {
            var updated = await _applicationService.UpdateStatusAsync(id, req.Status, req.ActorUserId);
            return Ok(ApiResult<LoanApplication>.Ok(updated, "Application status updated successfully."));
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
    /// Delete loan application
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _applicationService.DeleteAsync(id);
            return Ok(ApiResult<bool>.Ok(result, "Loan application deleted successfully."));
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
    /// Generate application number
    /// </summary>
    [HttpGet("generate-number")]
    public IActionResult GenerateNumber()
    {
        var appNumber = _applicationService.GenerateApplicationNumber();
        return Ok(ApiResult<string>.Ok(appNumber));
    }
}
