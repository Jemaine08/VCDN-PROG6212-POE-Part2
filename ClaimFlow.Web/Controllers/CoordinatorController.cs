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
            var pending = all.Where(c => c.Status == ClaimStatus.Submitted).OrderBy(c => c.CreatedAt).ToList();
            return View(pending);
        }

        [HttpPost]
        public async Task<IActionResult> Verify(Guid id, string coordinatorName)
        {
            var claim = await _repo.GetAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Verified;
            claim.CoordinatorName = string.IsNullOrWhiteSpace(coordinatorName) ? "Coordinator" : coordinatorName;
            claim.VerifiedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(claim);
            TempData["Ok"] = "Claim verified.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            var claim = await _repo.GetAsync(id);
            if (claim == null) return NotFound();
            claim.Status = ClaimStatus.Rejected;
            claim.Notes = string.IsNullOrWhiteSpace(reason) ? claim.Notes : $"{claim.Notes}\n[Coordinator Rejection] {reason}";
            claim.VerifiedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(claim);
            TempData["Ok"] = "Claim rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}
