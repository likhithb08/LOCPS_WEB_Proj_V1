using LOCPS.Common;
using LOCPS.DTOs;
using LOCPS.Models;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers.Api;

[ApiController]
[Route("api/loanproducts")]
public class LoanProductsApiController : ControllerBase
{
    private readonly ILoanProductService _productService;

    public LoanProductsApiController(ILoanProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all loan products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true)
    {
        try
        {
            var products = await _productService.GetAllAsync(activeOnly);
            return Ok(ApiResult<IEnumerable<LoanProduct>>.Ok(products));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Get loan product by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResult.Fail("Loan product not found."));

            return Ok(ApiResult<LoanProduct>.Ok(product));
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
    /// Create a new loan product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var product = new LoanProduct
            {
                ProductName = req.ProductName,
                ProductDescription = req.ProductDescription,
                MinAmount = req.MinAmount,
                MaxAmount = req.MaxAmount,
                InterestRate = req.InterestRate,
                MaxTenureMonths = req.MaxTenureMonths,
                ProcessingFee = req.ProcessingFee
            };

            var created = await _productService.CreateAsync(product, req.CreatedByUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, ApiResult<LoanProduct>.Ok(created, "Loan product created successfully."));
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
    /// Update existing loan product
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] LoanProduct product)
    {
        if (id != product.ProductId)
            return BadRequest(ApiResult.Fail("Product ID mismatch."));

        if (!ModelState.IsValid)
            return BadRequest(ApiResult.Fail("Invalid payload."));

        try
        {
            var updated = await _productService.UpdateAsync(product);
            return Ok(ApiResult<LoanProduct>.Ok(updated, "Loan product updated successfully."));
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
    /// Delete loan product by ID
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _productService.DeleteAsync(id);
            return Ok(ApiResult<bool>.Ok(result, "Loan product deleted successfully."));
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
