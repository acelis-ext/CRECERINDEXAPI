using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CrecerIndex.Abstraction.Interfaces.IServices;
using CrecerIndex.Entities.Models;
using Newtonsoft.Json;



namespace CrecerIndex.Services
{
    public class RecaptchaService : IRecaptchaService
    {
        private readonly HttpClient _http;
        private readonly RecaptchaSettings _settings;

        public RecaptchaService(HttpClient http, IOptions<RecaptchaSettings> settings)
        {
            _http = http;
            _settings = settings.Value;
        }


        public async Task<RecaptchaEntity> VerifyAsync(string token, string? remoteIp = null)
        {
            var result = new RecaptchaEntity { success = false };
            if (string.IsNullOrWhiteSpace(token))
            {
                result.hostname = "Recaptcha token vacío";
                return result;
            }

            try
            {
                // 🔸 USAR la clave desde `Recaptcha:scaptchatoken` (como el otro back)
                // Si tu RecaptchaSettings ya tenía SecretKey, no pasa nada; aquí priorizamos scaptchatoken.
                var secret = string.IsNullOrWhiteSpace(_settings.scaptchatoken)
                             ? _settings.SecretKey   // fallback si existe
                             : _settings.scaptchatoken;

                var parameters = new List<KeyValuePair<string, string>>
                {
                    new("secret",   secret),
                    new("response", token)
                };
                if (!string.IsNullOrWhiteSpace(remoteIp))
                    parameters.Add(new("remoteip", remoteIp));

                using var content = new FormUrlEncodedContent(parameters);
                using var resp = await _http.PostAsync("siteverify", content);
                var json = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RecaptchaEntity>(json) ?? result;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.hostname = ex.Message;
                return result;
            }
        }
    }
}


