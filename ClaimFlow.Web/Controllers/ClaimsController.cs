using ClaimFlow.Web.Models;
using ClaimFlow.Web.Models.ViewModels;
using ClaimFlow.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClaimFlow.Web.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimRepository _repo;
        private readonly IFileEncryptor _crypto;
        private readonly IFileValidationService _files;

        public ClaimsController(IClaimRepository repo, IFileEncryptor crypto, IFileValidationService files)
        {
            _repo = repo; _crypto = crypto; _files = files;
        }

        [HttpGet]
        public IActionResult Create() => View(new ClaimCreateVm());

        [HttpPost]
        public async Task<IActionResult> Create(ClaimCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Err"] = "Please correct the highlighted errors.";
                return View(vm);
            }

            var claim = new Claim
            {
                LecturerName = vm.LecturerName,
                HoursWorked = vm.HoursWorked,
                HourlyRate = vm.HourlyRate,
                Notes = vm.Notes
            };

            try
            {
                if (vm.Upload != null)
                {
                    _files.Validate(vm.Upload);
                    var encName = await _crypto.EncryptToFileAsync(vm.Upload.OpenReadStream(), Guid.NewGuid().ToString("N"));
                    claim.Documents.Add(new DocumentRef
                    {
                        OriginalFileName = vm.Upload.FileName,
                        EncryptedFileName = encName,
                        SizeBytes = vm.Upload.Length
                    });
                }

                await _repo.AddAsync(claim);
                TempData["Ok"] = "Claim submitted!";
                return RedirectToAction(nameof(My));
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"Upload failed: {ex.Message}";
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var all = await _repo.GetAllAsync();
            // naive filter: pretend current lecturer is the last used name; in demo we show all
            return View(all.OrderByDescending(c => c.CreatedAt).ToList());
        }

        [HttpGet]
        public async Task<FileResult> Download(Guid id, string file)
        {
            var claim = await _repo.GetAsync(id);
            var doc = claim?.Documents.FirstOrDefault(d => d.EncryptedFileName == file);
            if (claim == null || doc == null) throw new FileNotFoundException();
            var bytes = await _crypto.DecryptAsync(doc.EncryptedFileName);
            return File(bytes, "application/octet-stream", doc.OriginalFileName);
        }
    }
}
