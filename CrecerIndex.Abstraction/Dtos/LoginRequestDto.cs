using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CrecerIndex.Abstraction.Dtos
{
    public class LoginRequestDto
    {
        //[JsonPropertyName("usuario")]
        public string Usuario { get; set; }
        //[JsonPropertyName("password")]
        public string Password { get; set; }
    }

}
