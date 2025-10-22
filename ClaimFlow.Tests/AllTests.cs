using System.Text;
using ClaimFlow.Web.Models;
using ClaimFlow.Web.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClaimFlow.Tests.TestHelpers;
using ClaimFlow.Tests.Fakes;

namespace ClaimFlow.Tests
{
    [TestClass]
    public class AllTests
    {
        private static string TempDir()
            => Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))).FullName;

        #region Domain Tests

        [TestMethod]
        public void Claim_Total_Computes_Correctly()
        {
            var c = new Claim { HoursWorked = 7.5m, HourlyRate = 123.45m };
            c.Total.Should().Be(925.88m); // 7.5 * 123.45 = 925.875 -> rounds 925.88
        }

        [TestMethod]
        public void FileValidation_Blocks_Disallowed_Extension()
        {
            var rules = new FileValidationService(new OptionsMonitorStub<FileValidationService.FileRules>(
                new FileValidationService.FileRules { MaxFileSizeBytes = 1024 * 1024, AllowedExtensions = new[] { ".pdf" } }
            ));
            var bad = new FormFileStub("virus.exe", new byte[100]);

            Action act = () => rules.Validate(bad);
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*not allowed*");
        }

        [TestMethod]
        public async Task AesFileEncryptor_RoundTrip_Works()
        {
            var dir = TempDir();
            var key = Convert.ToBase64String(Enumerable.Repeat((byte)42, 32).ToArray());
            var enc = new AesFileEncryptor(new OptionsMonitorStub<AesFileEncryptor.StorageCryptoOptions>(
                new AesFileEncryptor.StorageCryptoOptions { UploadsFolder = dir, AesKeyBase64 = key }));

            var payload = Encoding.UTF8.GetBytes("hello-claimflow");
            using var ms = new MemoryStream(payload);

            var fileName = await enc.EncryptToFileAsync(ms, "u1");
            var plain = await enc.DecryptAsync(fileName);

            plain.Should().BeEquivalentTo(payload);
        }

        #endregion

        #region ClaimsController Tests

        [TestMethod]
        public async Task ClaimsController_Create_Post_Valid_Redirects_And_Persists()
        {
            var (ctrl, repo) = Subject();
            var vm = new ClaimCreateVm { LecturerName = "A Learner", HoursWorked = 2, HourlyRate = 100, Notes = "ok" };
            var result = await ctrl.Create(vm) as RedirectToActionResult;

            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(ClaimsController.My));

            var all = await repo.GetAllAsync();
            all.Should().HaveCount(1);
            all[0].Total.Should().Be(200m);
        }

        [TestMethod]
        public async Task ClaimsController_Create_Post_Invalid_Returns_View_With_Errors()
        {
            var (ctrl, _) = Subject();
            ctrl.ModelState.AddModelError("LecturerName", "Required");
            var vm = new ClaimCreateVm { HoursWorked = 1, HourlyRate = 100 };

            var result = await ctrl.Create(vm) as ViewResult;
            result.Should().NotBeNull();
            result!.ViewName.Should().BeNull(); // returns default view
            result.Model.Should().Be(vm);
        }

        #endregion

        #region CoordinatorController Tests

        [TestMethod]
        public async Task CoordinatorController_Verify_Moves_Submitted_To_Verified()
        {
            var repo = new InMemoryClaimRepository();
            var claim = new Claim { LecturerName = "L", HoursWorked = 1, HourlyRate = 100, Status = ClaimStatus.Submitted };
            await repo.AddAsync(claim);

            var coord = new CoordinatorController(repo);
            var result = await coord.Verify(claim.Id, "Co-ord") as RedirectToActionResult;
            result!.ActionName.Should().Be(nameof(CoordinatorController.Index));

            var updated = await repo.GetAsync(claim.Id);
            updated!.Status.Should().Be(ClaimStatus.Verified);
            updated.CoordinatorName.Should().Be("Co-ord");
            updated.VerifiedAt.Should().NotBeNull();
        }

        [TestMethod]
        public async Task CoordinatorController_Reject_Changes_Status_To_Rejected()
        {
            var repo = new InMemoryClaimRepository();
            var claim = new Claim { LecturerName = "L", HoursWorked = 1, HourlyRate = 100, Status = ClaimStatus.Submitted };
            await repo.AddAsync(claim);

            var coord = new CoordinatorController(repo);
            var result = await coord.Reject(claim.Id, "Not valid") as RedirectToActionResult;
            result!.ActionName.Should().Be(nameof(CoordinatorController.Index));

            var updated = await repo.GetAsync(claim.Id);
            updated!.Status.Should().Be(ClaimStatus.Rejected);
            updated.Notes.Should().Contain("Not valid");
        }

        #endregion

        #region ManagerController Tests

        [TestMethod]
        public async Task ManagerController_Approve_Sets_ManagerId_And_Status()
        {
            var repo = new InMemoryClaimRepository();
            var claim = new Claim { LecturerName = "L", HoursWorked = 1, HourlyRate = 100, Status = ClaimStatus.Verified };
            await repo.AddAsync(claim);

            var mgr = new ManagerController(repo);
            var result = await mgr.Approve(claim.Id, "Boss") as RedirectToActionResult;
            result!.ActionName.Should().Be(nameof(ManagerController.Index));

            var updated = await repo.GetAsync(claim.Id);
            updated!.Status.Should().Be(ClaimStatus.Approved);
            updated.ManagerName.Should().Be("Boss");
            updated.ManagerId.Should().NotBeNull();
            updated.ApprovedOrRejectedAt.Should().NotBeNull();
        }

        [TestMethod]
        public async Task ManagerController_Reject_Changes_Status_To_Rejected()
        {
            var repo = new InMemoryClaimRepository();
            var claim = new Claim { LecturerName = "L", HoursWorked = 1, HourlyRate = 100, Status = ClaimStatus.Verified };
            await repo.AddAsync(claim);

            var mgr = new ManagerController(repo);
            var result = await mgr.Reject(claim.Id, "Rejected for reason") as RedirectToActionResult;
            result!.ActionName.Should().Be(nameof(ManagerController.Index));

            var updated = await repo.GetAsync(claim.Id);
            updated!.Status.Should().Be(ClaimStatus.Rejected);
            updated.Notes.Should().Contain("Rejected for reason");
        }

        #endregion

        #region Helper Methods

        private (ClaimsController ctrl, InMemoryClaimRepository repo) Subject()
        {
            var repo = new InMemoryClaimRepository();
            var key = Convert.ToBase64String(Enumerable.Repeat((byte)7, 32).ToArray());
            var crypto = new AesFileEncryptor(new OptionsMonitorStub<AesFileEncryptor.StorageCryptoOptions>(
                new AesFileEncryptor.StorageCryptoOptions { UploadsFolder = Path.GetTempPath(), AesKeyBase64 = key }));
            var files = new FileValidationService(new OptionsMonitorStub<FileValidationService.FileRules>(
                new FileValidationService.FileRules { MaxFileSizeBytes = 5_000_000, AllowedExtensions = new[] { ".pdf", ".docx", ".xlsx" } }));
            var ctrl = new ClaimsController(repo, crypto, files);
            return (ctrl, repo);
        }

        #endregion
    }
}
