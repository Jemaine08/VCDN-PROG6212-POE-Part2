using ClaimFlow.Web.Models;
using ClaimFlow.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClaimFlow.Web.Controllers
{
    // Guarantees /Manager and /Manager/* routes resolve
    [Route("Manager")]
    public class ManagerController : Controller
    {
        private readonly IClaimRepository _repo;
        public ManagerController(IClaimRepository repo) => _repo = repo;

        // GET /Manager
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var all = await _repo.GetAllAsync();
            var pending = all.Where(c => c.Status == ClaimStatus.Verified)
                             .OrderBy(c => c.CreatedAt)
                             .ToList();
            return View(pending);
        }

        // POST /Manager/Approve
        [HttpPost("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, string managerName)
        {
            if (id == Guid.Empty) { TempData["Err"] = "Invalid claim id."; return RedirectToAction(nameof(Index)); }
            var claim = await _repo.GetAsync(id);
            if (claim == null) { TempData["Err"] = "Claim not found."; return RedirectToAction(nameof(Index)); }
            if (claim.Status != ClaimStatus.Verified)
            {
                TempData["Err"] = "Only verified claims can be approved.";
                return RedirectToAction(nameof(Index));
            }

            claim.Status = ClaimStatus.Approved;
            claim.ManagerName = string.IsNullOrWhiteSpace(managerName) ? "Manager" : managerName.Trim();
            claim.ManagerId = Guid.NewGuid();
            claim.ApprovedOrRejectedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(claim);

            TempData["Ok"] = "Approved.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Manager/Reject
        [HttpPost("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            if (id == Guid.Empty) { TempData["Err"] = "Invalid claim id."; return RedirectToAction(nameof(Index)); }
            var claim = await _repo.GetAsync(id);
            if (claim == null) { TempData["Err"] = "Claim not found."; return RedirectToAction(nameof(Index)); }

            if (claim.Status != ClaimStatus.Verified && claim.Status != ClaimStatus.Submitted)
            {
                TempData["Err"] = "Only submitted or verified claims can be rejected.";
                return RedirectToAction(nameof(Index));
            }

            claim.Status = ClaimStatus.Rejected;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                claim.Notes = string.IsNullOrEmpty(claim.Notes)
                    ? $"[Manager Rejection] {reason}"
                    : $"{claim.Notes}\n[Manager Rejection] {reason}";
            }
            claim.ApprovedOrRejectedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(claim);

            TempData["Ok"] = "Rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}
