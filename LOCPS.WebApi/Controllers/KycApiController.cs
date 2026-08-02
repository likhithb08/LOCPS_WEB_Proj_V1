using LOCPS.Common;
using LOCPS.DTOs;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers.Api;

[ApiController]
[Route("api/kyc")]
public class KycApiController : ControllerBase
{
    private readonly IKycService _kycService;

    public KycApiController(IKycService kycService)
    {
        _kycService = kycService;
    }

    /// <summary>
    /// Submit KYC details
    /// </summary>
    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] Kyc kyc)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var created = await _kycService.SubmitAsync(kyc);
            return Ok(ApiResult<Kyc>.Ok(created, "KYC submitted successfully."));
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
    /// Get KYC record by application ID
    /// </summary>
    [HttpGet("application/{applicationId:int}")]
    public async Task<IActionResult> GetByApplicationId(int applicationId)
    {
        try
        {
            var kyc = await _kycService.GetByApplicationIdAsync(applicationId);
            if (kyc == null)
                return NotFound(ApiResult.Fail("KYC record not found."));

            return Ok(ApiResult<Kyc>.Ok(kyc));
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
    /// Verify KYC record
    /// </summary>
    [HttpPost("{kycId:int}/verify")]
    public async Task<IActionResult> Verify(int kycId, [FromBody] KycDecisionRequest req)
    {
        try
        {
            var kyc = await _kycService.VerifyAsync(kycId, req.VerifiedByUserId, req.Remarks);
            return Ok(ApiResult<Kyc>.Ok(kyc, "KYC verified successfully."));
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
    /// Reject KYC record
    /// </summary>
    [HttpPost("{kycId:int}/reject")]
    public async Task<IActionResult> Reject(int kycId, [FromBody] KycDecisionRequest req)
    {
        try
        {
            var kyc = await _kycService.RejectAsync(kycId, req.VerifiedByUserId, req.Remarks);
            return Ok(ApiResult<Kyc>.Ok(kyc, "KYC rejected successfully."));
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
    /// Initiate KYC process for loan application
    /// </summary>
    [HttpPost("initiate/{applicationId:int}")]
    public async Task<IActionResult> InitiateKyc(int applicationId, [FromQuery] int loanOfficerId)
    {
        try
        {
            await _kycService.InitiateKycAsync(applicationId, loanOfficerId);
            return Ok(ApiResult.Ok("KYC initiated successfully."));
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
