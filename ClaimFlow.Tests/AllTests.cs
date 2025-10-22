using System.Text;
using ClaimFlow.Tests.Fakes;
using ClaimFlow.Tests.TestHelpers;
using ClaimFlow.Web.Controllers;
using ClaimFlow.Web.Models;
using ClaimFlow.Web.Models.ViewModels;
using ClaimFlow.Web.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        #endregion
        private (CoordinatorController ctrl, InMemoryClaimRepository repo) SubjectCoordinator()
        {
            var repo = new InMemoryClaimRepository(); // In-memory repo for testing

            // Mock the ILogger<CoordinatorController> dependency
            var loggerMock = new Mock<ILogger<CoordinatorController>>();

            // Create the CoordinatorController with the mocked logger and repo
            var ctrl = new CoordinatorController(repo, loggerMock.Object); // Pass the mocked logger
            return (ctrl, repo); // Return the controller and repository as a tuple
        }



        [TestMethod]
        public async Task CoordinatorController_Index_Returns_View()
        {
            // Arrange: Initialize the repository and the controller with a mocked logger
            var (coord, repo) = SubjectCoordinator();

            // Act: Call the Index method on the controller
            var result = await coord.Index() as ViewResult;

            // Assert: Verify that the result is a ViewResult and the view is correct
            result.Should().NotBeNull();  // Ensure the result is not null
            result!.ViewName.Should().BeNull();  // Ensure the default view is returned
        }

        [TestMethod]
        public async Task CoordinatorController_Reject_Invalid_ClaimId_Returns_Error_Message()
        {
            // Arrange: Initialize the repository and the controller with a mocked logger
            var (coord, repo) = SubjectCoordinator();

            // Initialize TempData
            coord.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            // Act: Call the Reject method with an invalid claim ID (Guid.Empty)
            var result = await coord.Reject(Guid.Empty, "Rejection reason") as RedirectToActionResult;

            // Assert: Verify the redirect action and that an error message is set
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(CoordinatorController.Index)); // Ensure the action redirects to 'Index'

            // Verify that the error message is displayed in TempData
            coord.TempData.Should().ContainKey("Err");
            coord.TempData["Err"].Should().Be("Invalid claim id.");
        }




    }
}
