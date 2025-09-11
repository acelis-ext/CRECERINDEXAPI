using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Abstraction.Interfaces.IServices
{
    public interface ITokenRevocationStore
    {
        Task RevokeAsync(string jti, DateTime expiresAtUtc);     // guardar hasta que expire
        Task<bool> IsRevokedAsync(string jti);
    }
}
