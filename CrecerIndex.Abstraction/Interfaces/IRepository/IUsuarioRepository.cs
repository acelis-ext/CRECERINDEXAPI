
using CrecerIndex.Abstraction.Dtos;
using CrecerIndex.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Abstraction.Interfaces.IRepository
{
    public interface IUsuarioRepository
    {
        Usuario GetByCredentials(string usuario, string password);

    }
}
