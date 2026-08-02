using LOCPS.Common;
using LOCPS.DTOs;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers.Api;

[ApiController]
[Route("api/creditevaluation")]
public class CreditEvaluationApiController : ControllerBase
{
    private readonly ICreditEvaluationService _creditService;

    public CreditEvaluationApiController(ICreditEvaluationService creditService)
    {
        _creditService = creditService;
    }

    /// <summary>
    /// Calculate and save credit evaluation for loan application
    /// </summary>
    [HttpPost("calculate/{applicationId:int}")]
    public async Task<IActionResult> CalculateAndSave(int applicationId, [FromBody] CalculateCreditRequest req)
    {
        try
        {
            var evaluation = await _creditService.CalculateAndSaveAsync(applicationId, req.EvaluatedByUserId);
            return Ok(ApiResult<CreditEvaluation>.Ok(evaluation, "Credit evaluation calculated and saved successfully."));
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
    /// Get credit evaluation by application ID
    /// </summary>
    [HttpGet("application/{applicationId:int}")]
    public async Task<IActionResult> GetByApplicationId(int applicationId)
    {
        try
        {
            var evaluation = await _creditService.GetByApplicationIdAsync(applicationId);
            if (evaluation == null)
                return NotFound(ApiResult.Fail("Credit evaluation not found."));

            return Ok(ApiResult<CreditEvaluation>.Ok(evaluation));
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
    /// Approve credit evaluation
    /// </summary>
    [HttpPost("approve/{applicationId:int}")]
    public async Task<IActionResult> Approve(int applicationId, [FromBody] CreditDecisionRequest req)
    {
        try
        {
            var evaluation = await _creditService.ApproveAsync(applicationId, req.UserId, req.Comments);
            return Ok(ApiResult<CreditEvaluation>.Ok(evaluation, "Credit evaluation approved successfully."));
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
    /// Reject credit evaluation
    /// </summary>
    [HttpPost("reject/{applicationId:int}")]
    public async Task<IActionResult> Reject(int applicationId, [FromBody] CreditDecisionRequest req)
    {
        try
        {
            var evaluation = await _creditService.RejectAsync(applicationId, req.UserId, req.Comments);
            return Ok(ApiResult<CreditEvaluation>.Ok(evaluation, "Credit evaluation rejected successfully."));
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
