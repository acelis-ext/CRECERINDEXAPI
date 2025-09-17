using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Entities.Models
{
    public class RecaptchaEntity
    {
        public bool success { get; set; }
        public DateTime? challenge_ts { get; set; }
        public string hostname { get; set; }
        public string[] error_codes { get; set; }
        // Campos de v3 (por si migras luego)
        public float? score { get; set; }
        public string action { get; set; }
    }
}
