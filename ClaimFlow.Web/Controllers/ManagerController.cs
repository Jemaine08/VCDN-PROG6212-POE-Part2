using ClaimFlow.Web.Models;
using ClaimFlow.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClaimFlow.Web.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IClaimRepository _repo;
        public ManagerController(IClaimRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var all = await _repo.GetAllAsync();
            var pending = all.Where(c => c.Status == ClaimStatus.Verified).OrderBy(c => c.CreatedAt).ToList();
            return View(pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, string managerName)
        {
            var claim = await _repo.GetAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Approved;
            claim.ManagerName = string.IsNullOrWhiteSpace(managerName) ? "Manager" : managerName;
            claim.ManagerId = Guid.NewGuid(); // demo ID capture
            claim.ApprovedOrRejectedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(claim);

            TempData["Ok"] = "Approved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            var claim = await _repo.GetAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Rejected;
            claim.ManagerName = "Manager";
            claim.ManagerId = Guid.NewGuid();
            claim.ApprovedOrRejectedAt = DateTime.UtcNow;
            claim.Notes = string.IsNullOrWhiteSpace(reason) ? claim.Notes : $"{claim.Notes}\n[Manager Rejection] {reason}";
            await _repo.UpdateAsync(claim);

            TempData["Ok"] = "Rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}
