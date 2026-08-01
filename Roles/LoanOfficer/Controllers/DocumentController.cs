using LOCPS.Enums;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOCPS.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly ILoanApplicationService _loanApplicationService;

        public DocumentController(IDocumentService documentService, ILoanApplicationService loanApplicationService)
        {
            _documentService = documentService;
            _loanApplicationService = loanApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Validate(int id)
        {
            var application = await _loanApplicationService.GetByIdAsync(id);
            if (application == null)
            {
                TempData["Error"] = "Application not found.";
                return RedirectToAction("Index", "Loan");
            }

            var documents = await _documentService.GetByApplicationIdAsync(id);
            ViewBag.Documents = documents;

            return View(application);
        }

        [HttpGet]
        public IActionResult Upload(int id) => View();

        [HttpPost]
        public async Task<IActionResult> Verify(int documentId, int applicationId, DocumentStatus status, string remarks)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var officerId = int.TryParse(userId, out var id) ? id : 0;

                if (status == DocumentStatus.Verified)
                {
                    await _documentService.ApproveAsync(documentId, officerId, remarks ?? string.Empty);
                    TempData["Success"] = "Document verified successfully.";
                }
                else if (status == DocumentStatus.Rejected)
                {
                    await _documentService.RejectAsync(documentId, officerId, remarks ?? string.Empty);
                    TempData["Success"] = "Document rejected. Customer notified.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Loan", new { id = applicationId });
        }
    }
}
