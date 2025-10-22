// Reference: Microsoft Learn (2024) AES Class — System.Security.Cryptography.Aes
// Available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes
// Used to implement AES encryption and decryption for file uploads in the ClaimsController.

using ClaimFlow.Web.Models;

namespace ClaimFlow.Web.Services
{
    public interface IClaimRepository
    {
        Task<List<Claim>> GetAllAsync();
        Task<Claim?> GetAsync(Guid id);
        Task AddAsync(Claim claim);
        Task UpdateAsync(Claim claim);
    }
}
