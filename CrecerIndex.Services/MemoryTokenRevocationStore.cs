using CrecerIndex.Abstraction.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;


namespace CrecerIndex.Services
{
    public sealed class MemoryTokenRevocationStore : ITokenRevocationStore
    {
        private readonly IMemoryCache _cache;
        public MemoryTokenRevocationStore(IMemoryCache cache) => _cache = cache;

        public Task RevokeAsync(string jti, DateTime expiresAtUtc)
        {
            // usamos el JTI como clave y lo dejamos vivir hasta la expiración original del token
            var ttl = expiresAtUtc - DateTime.UtcNow;
            if (ttl <= TimeSpan.Zero) ttl = TimeSpan.FromMinutes(1);
            _cache.Set($"revoked:{jti}", true, ttl);
            return Task.CompletedTask;
        }

        public Task<bool> IsRevokedAsync(string jti) =>
            Task.FromResult(_cache.TryGetValue($"revoked:{jti}", out _));
    }
}
