using CrecerIndex.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Abstraction.Interfaces.IServices
{
    public interface IRecaptchaService
    {
        Task<RecaptchaEntity> VerifyAsync(string token, string? remoteIp = null);

    }
}
