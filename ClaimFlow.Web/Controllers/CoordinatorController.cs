using ClaimFlow.Web.Models;
using ClaimFlow.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClaimFlow.Web.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly IClaimRepository _repo;
        public CoordinatorController(IClaimRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var all = await _repo.GetAllAsync();
            var pending = all.Where(c => c.Status == ClaimStatus.Submitted)
                             .OrderBy(c => c.CreatedAt)
                             .ToList();
            return View(pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(Guid id, string coordinatorName)
        {
            if (id == Guid.Empty)
            {
                TempData["Err"] = "Invalid claim id.";
                return RedirectToAction(nameof(Index));
            }

            var claim = await _repo.GetAsync(id);
            if (claim == null)
            {
                TempData["Err"] = "Claim not found.";
                return RedirectToAction(nameof(Index));
            }

            if (claim.Status != ClaimStatus.Submitted)
            {
                TempData["Err"] = "Only newly submitted claims can be verified.";
                return RedirectToAction(nameof(Index));
            }

            claim.Status = ClaimStatus.Verified;
            claim.CoordinatorName = string.IsNullOrWhiteSpace(coordinatorName) ? "Coordinator" : coordinatorName.Trim();
            claim.VerifiedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(claim);

            TempData["Ok"] = "Claim verified.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            if (id == Guid.Empty)
            {
                TempData["Err"] = "Invalid claim id.";
                return RedirectToAction(nameof(Index));
            }

            var claim = await _repo.GetAsync(id);
            if (claim == null)
            {
                TempData["Err"] = "Claim not found.";
                return RedirectToAction(nameof(Index));
            }

            if (claim.Status != ClaimStatus.Submitted && claim.Status != ClaimStatus.Verified)
            {
                TempData["Err"] = "Only submitted or verified claims can be rejected.";
                return RedirectToAction(nameof(Index));
            }

            claim.Status = ClaimStatus.Rejected;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                claim.Notes = string.IsNullOrEmpty(claim.Notes)
                    ? $"[Coordinator Rejection] {reason}"
                    : $"{claim.Notes}\n[Coordinator Rejection] {reason}";
            }
            claim.VerifiedAt ??= DateTime.UtcNow; // record review time even if rejecting immediately
            await _repo.UpdateAsync(claim);

            TempData["Ok"] = "Claim rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}
