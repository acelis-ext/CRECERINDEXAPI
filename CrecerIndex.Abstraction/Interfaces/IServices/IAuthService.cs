using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Abstraction.Interfaces.IServices
{
    public interface IAuthService
    {
        public string Login(string usuario, string password);

    }
}
