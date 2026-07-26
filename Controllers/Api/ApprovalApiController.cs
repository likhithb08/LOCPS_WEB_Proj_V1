using LOCPS.Common;
using LOCPS.DTOs;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers.Api;

[ApiController]
[Route("api/approval")]
public class ApprovalApiController : ControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalApiController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    /// <summary>
    /// Approve loan application
    /// </summary>
    [HttpPost("approve")]
    public async Task<IActionResult> ApproveLoan([FromBody] ApproveLoanRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var approval = await _approvalService.ApproveLoanAsync(
                req.ApplicationId,
                req.ApproverUserId,
                req.ApprovedAmount,
                req.TenureMonths,
                req.InterestRate,
                req.Comments);

            return Ok(ApiResult<Approval>.Ok(approval, "Loan approved successfully."));
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
    /// Reject loan application
    /// </summary>
    [HttpPost("reject")]
    public async Task<IActionResult> RejectLoan([FromBody] RejectLoanRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var approval = await _approvalService.RejectLoanAsync(
                req.ApplicationId,
                req.ApproverUserId,
                req.Reason,
                req.Comments);

            return Ok(ApiResult<Approval>.Ok(approval, "Loan rejected successfully."));
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
    /// Get approval details by application ID
    /// </summary>
    [HttpGet("application/{applicationId:int}")]
    public async Task<IActionResult> GetByApplicationId(int applicationId)
    {
        try
        {
            var approval = await _approvalService.GetByApplicationIdAsync(applicationId);
            if (approval == null)
                return NotFound(ApiResult.Fail("Approval record not found."));

            return Ok(ApiResult<Approval>.Ok(approval));
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
    /// Get approval history log
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        try
        {
            var history = await _approvalService.GetHistoryAsync();
            return Ok(ApiResult<IEnumerable<Approval>>.Ok(history));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Send application back to Loan Officer
    /// </summary>
    [HttpPost("sendback")]
    public async Task<IActionResult> SendBack([FromBody] SendBackLoanRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var approval = await _approvalService.SendBackToLoanOfficerAsync(
                req.ApplicationId,
                req.ApproverUserId,
                req.Remarks);

            return Ok(ApiResult<Approval>.Ok(approval, "Application sent back to Loan Officer successfully."));
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
