using System.Text.Json;
using ClaimFlow.Web.Models;
using Microsoft.Extensions.Options;

namespace ClaimFlow.Web.Services
{
    public class JsonClaimRepository : IClaimRepository
    {
        private readonly string _file;
        private static readonly SemaphoreSlim Gate = new(1, 1);

        public class StorageOptions { public string ClaimsFile { get; set; } = "App_Data/claims.json"; }
        public JsonClaimRepository(IOptionsMonitor<StorageOptions> opts)
        {
            _file = opts.CurrentValue.ClaimsFile;
            var dir = Path.GetDirectoryName(_file);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (!File.Exists(_file)) File.WriteAllText(_file, "[]");
        }

        public async Task<List<Claim>> GetAllAsync()
        {
            await Gate.WaitAsync();
            try
            {
                var json = await File.ReadAllTextAsync(_file);
                return JsonSerializer.Deserialize<List<Claim>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            finally { Gate.Release(); }
        }

        public async Task<Claim?> GetAsync(Guid id)
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault(c => c.Id == id);
        }

        public async Task AddAsync(Claim claim)
        {
            await Gate.WaitAsync();
            try
            {
                var all = await GetAllAsync();
                all.Add(claim);
                var json = JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_file, json);
            }
            finally { Gate.Release(); }
        }

        public async Task UpdateAsync(Claim claim)
        {
            await Gate.WaitAsync();
            try
            {
                var all = await GetAllAsync();
                var idx = all.FindIndex(c => c.Id == claim.Id);
                if (idx >= 0) all[idx] = claim;
                var json = JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_file, json);
            }
            finally { Gate.Release(); }
        }
    }
}
