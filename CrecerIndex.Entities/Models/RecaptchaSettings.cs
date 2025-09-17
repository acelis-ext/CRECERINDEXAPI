using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Entities.Models
{
    public class RecaptchaSettings
    {
        public string? scaptchatoken { get; set; }   // ← NUEVO: donde guardas la SECRET KEY (nombre “raro” a pedido)

        public string SecretKey { get; set; } = string.Empty;
    }
}
