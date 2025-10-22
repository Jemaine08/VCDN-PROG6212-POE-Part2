using Microsoft.Extensions.Options;

namespace ClaimFlow.Tests.TestHelpers
{
    public class OptionsMonitorStub<T> : IOptionsMonitor<T> where T : class, new()
    {
        private readonly T _value;
        public OptionsMonitorStub(T value) { _value = value; }
        public T CurrentValue => _value;
        public T Get(string? name) => _value;
        public IDisposable OnChange(Action<T, string?> listener) => new Noop();
        private class Noop : IDisposable { public void Dispose() { } }
    }
}
