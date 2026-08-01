using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class CreditController : Controller
    {
        private readonly ICreditEvaluationService _creditService;
        private readonly ILoanApplicationService _loanApplicationService;

        public CreditController(ICreditEvaluationService creditService, ILoanApplicationService loanApplicationService)
        {
            _creditService = creditService;
            _loanApplicationService = loanApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Evaluate(int id)
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
            {
                TempData["Error"] = "Application not found.";
                return RedirectToAction("Index", "Loan");
            }

            var credit = await _creditService.GetByApplicationIdAsync(id);
            ViewBag.CreditEvaluation = credit;

            return View(application);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
            {
                TempData["Error"] = "Application not found.";
                return RedirectToAction("Index", "Loan");
            }

            var credit = await _creditService.GetByApplicationIdAsync(id);
            ViewBag.CreditEvaluation = credit;

            return View(application);
        }

        [HttpPost]
        [ActionName("Evaluate")]
        public async Task<IActionResult> EvaluatePost(int applicationId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var officerId = int.TryParse(userId, out var id) ? id : 0;

                await _creditService.CalculateAndSaveAsync(applicationId, officerId);
                TempData["Success"] = "Credit evaluation completed successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Loan", new { id = applicationId });
        }
    }
}
