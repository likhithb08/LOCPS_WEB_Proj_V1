using Microsoft.AspNetCore.Mvc;
using LOCPS.Services.Interfaces;
using LOCPS.Models;
using LOCPS.DTOs;
using System.Text.Json;
using System.Text;

namespace LOCPS.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILoanProductService _loanProductService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProductController> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ProductController(
            ILoanProductService loanProductService,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ProductController> logger)
        {
            _loanProductService = loanProductService;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Helper method to create an HttpClient targeting the local Web API (LoanProductsApiController)
        /// </summary>
        private HttpClient GetApiClient()
        {
            return _httpClientFactory.CreateClient("LoanProductApi");
        }

        // GET: /Product
        // Consumes GET /api/loanproducts from LoanProductsApiController
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = GetApiClient();
                if (client.BaseAddress != null)
                {
                    var response = await client.GetAsync("/api/loanproducts?activeOnly=true");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var apiResult = JsonSerializer.Deserialize<ApiResult<IEnumerable<LoanProduct>>>(content, _jsonOptions);
                        if (apiResult != null && apiResult.Success && apiResult.Data != null)
                        {
                            // ViewBag.Source = "Consumed via Web API (/api/loanproducts)";
                            return View(apiResult.Data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "HTTP request to LoanProductsApiController failed. Falling back to direct service.");
            }

            var products = await _loanProductService.GetAllAsync(true);
            ViewBag.Source = "Direct Service";
            return View(products);
        }

        public IActionResult Create() => View();

        // POST: /Product/Create
        // Consumes POST /api/loanproducts from LoanProductsApiController
        [HttpPost]
        public async Task<IActionResult> Create(LoanProduct product)
        {
            ModelState.Remove("User");

            if (!ModelState.IsValid)
                return View(product);

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var createdByUserId = int.TryParse(userId, out var id) ? id : 1;

                try
                {
                    var client = GetApiClient();
                    if (client.BaseAddress != null)
                    {
                        var createReq = new CreateProductRequest
                        {
                            ProductName = product.ProductName,
                            ProductDescription = product.ProductDescription,
                            MinAmount = product.MinAmount,
                            MaxAmount = product.MaxAmount,
                            InterestRate = product.InterestRate,
                            MaxTenureMonths = product.MaxTenureMonths,
                            ProcessingFee = product.ProcessingFee,
                            CreatedByUserId = createdByUserId
                        };

                        var jsonContent = new StringContent(JsonSerializer.Serialize(createReq), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync("/api/loanproducts", jsonContent);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "HTTP call to POST /api/loanproducts failed. Falling back to service.");
                }

                await _loanProductService.CreateAsync(product, createdByUserId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to create loan product.");
                return View(product);
            }
        }

        // GET: /Product/Details/5
        // Consumes GET /api/loanproducts/{id} from LoanProductsApiController
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = GetApiClient();
                if (client.BaseAddress != null)
                {
                    var response = await client.GetAsync($"/api/loanproducts/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var apiResult = JsonSerializer.Deserialize<ApiResult<LoanProduct>>(content, _jsonOptions);
                        if (apiResult != null && apiResult.Success && apiResult.Data != null)
                        {
                            return View(apiResult.Data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "HTTP call to GET /api/loanproducts/{Id} failed. Falling back to service.", id);
            }

            var product = await _loanProductService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _loanProductService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Product/Edit
        // Consumes PUT /api/loanproducts/{id} from LoanProductsApiController
        [HttpPost]
        public async Task<IActionResult> Edit(LoanProduct product)
        {
            ModelState.Remove("User");

            if (!ModelState.IsValid)
                return View(product);

            try
            {
                try
                {
                    var client = GetApiClient();
                    if (client.BaseAddress != null)
                    {
                        var jsonContent = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
                        var response = await client.PutAsync($"/api/loanproducts/{product.ProductId}", jsonContent);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "HTTP call to PUT /api/loanproducts failed. Falling back to service.");
                }

                await _loanProductService.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to update: {ex.InnerException?.Message ?? ex.Message}");
                return View(product);
            }
        }

        // GET: /Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _loanProductService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: /Product/Delete
        // Consumes DELETE /api/loanproducts/{id} from LoanProductsApiController
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string dummyParameter = "")
        {
            if (id == 0)
            {
                int.TryParse(Request.Form["id"], out id);
            }

            if (id > 0)
            {
                try
                {
                    var client = GetApiClient();
                    if (client.BaseAddress != null)
                    {
                        var response = await client.DeleteAsync($"/api/loanproducts/{id}");
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "HTTP call to DELETE /api/loanproducts/{Id} failed. Falling back to service.", id);
                }

                await _loanProductService.DeleteAsync(id);
            }

            return RedirectToAction("Index");
        }
    }
}