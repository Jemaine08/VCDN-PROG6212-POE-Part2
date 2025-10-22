using ClaimFlow.Web.Models;
using ClaimFlow.Web.Services;

namespace ClaimFlow.Tests.Fakes
{
    /// <summary>In-memory IClaimRepository for controller tests (no file I/O).</summary>
    public class InMemoryClaimRepository : IClaimRepository
    {
        private readonly List<Claim> _items = new();
        public Task<List<Claim>> GetAllAsync() => Task.FromResult(_items.ToList());
        public Task<Claim?> GetAsync(Guid id) => Task.FromResult(_items.FirstOrDefault(c => c.Id == id));
        public Task AddAsync(Claim claim) { _items.Add(claim); return Task.CompletedTask; }
        public Task UpdateAsync(Claim claim)
        {
            var i = _items.FindIndex(x => x.Id == claim.Id);
            if (i >= 0) _items[i] = claim;
            return Task.CompletedTask;
        }
    }
}
