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
