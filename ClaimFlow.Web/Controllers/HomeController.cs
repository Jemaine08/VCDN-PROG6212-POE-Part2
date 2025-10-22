// Reference: Microsoft Learn (2024) ASP.NET Core Controllers and Actions
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions
// Used to guide the structure of controller actions like Verify and Reject.

using ClaimFlow.Web.Models;
using ClaimFlow.Web.Models.ViewModels;
using ClaimFlow.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClaimFlow.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClaimRepository _repo;
        public HomeController(IClaimRepository repo) => _repo = repo;

        [HttpGet("/")]
        [HttpGet("/Home")]
        [HttpGet("/Home/Index")]
        public async Task<IActionResult> Index()
        {
            var all = await _repo.GetAllAsync();

            var vm = new HomeVm
            {
                Total = all.Count,
                Submitted = all.Count(c => c.Status == ClaimStatus.Submitted),
                Verified = all.Count(c => c.Status == ClaimStatus.Verified),
                Approved = all.Count(c => c.Status == ClaimStatus.Approved),
                Rejected = all.Count(c => c.Status == ClaimStatus.Rejected),
                Recent = all
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToList()
            };

            return View(vm);
        }
    }
}
