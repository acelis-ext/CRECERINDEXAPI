
//using CrecerIndex.Abstraction.Interfaces.IRepository;
//using CrecerIndex.Entities.Models;
//using CrecerIndex.Repository.Extensions;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace CrecerIndexApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CoverageController : ControllerBase
//    {
//        private readonly ICoverageRepository coverageRepo;

//        public CoverageController(ICoverageRepository coverageRepo)
//        {
//            this.coverageRepo = coverageRepo;
//        }

//        [HttpPost("getIndexData")]
//        [Authorize]
//        public async Task<ActionResult> getListCoveragePaginated([FromBody] FilterCoverageEntity filter)
//        {
//            filter.sdocumenttype = Functions.GetDocumentTypeCrecer(
//                string.IsNullOrWhiteSpace(filter.sdocumenttype) ? 0 : Convert.ToInt32(filter.sdocumenttype),
//                filter.sdocumentnumber ?? string.Empty);

//            var warnings = new List<string>();

//            // timeout total “defensivo” del endpoint (para no colgar y que FrontDoor no te mate)
//            // Si FrontDoor lo pondrás en 120s, aquí pon 110s.
//            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(110));
//            var ct = cts.Token;

//            // Ejecuta todo (como tú quieres)
//            var tSigma = coverageRepo.GetCoverage(filter, ct).ContinueWith(t => CatchList(t, "SIGMA", warnings), ct);
//            var tPN = coverageRepo.GetCoverageCrecerPN(filter, ct).ContinueWith(t => CatchList(t, "CORE_PN", warnings), ct);
//            var tPJ = coverageRepo.GetCoverageCrecerPJ(filter, ct).ContinueWith(t => CatchList(t, "CORE_PJ", warnings), ct);
//            var tPNC = coverageRepo.GetCoverageCrecerPNContra(filter, ct).ContinueWith(t => CatchList(t, "CORE_PN_CONTRA", warnings), ct);
//            var tPJC = coverageRepo.GetCoverageCrecerPJContra(filter, ct).ContinueWith(t => CatchList(t, "CORE_PJ_CONTRA", warnings), ct);

//            await Task.WhenAll(tSigma, tPN, tPJ, tPNC, tPJC);

//            var listaTotal = tSigma.Result
//                .Concat(tPN.Result)
//                .Concat(tPJ.Result)
//                .Concat(tPNC.Result)
//                .Concat(tPJC.Result)
//                .OrderByDescending(x => ParseDateOrMin(x.SFECHA_PROCESO))
//                .ToList();

//            filter.pagination ??= new PaginationEntity();
//            if (filter.pagination.ItemsPerPage <= 0) filter.pagination.ItemsPerPage = 10;

//            filter.pagination.TotalItems = listaTotal.Count;
//            filter.pagination.TotalPages = (int)Math.Ceiling((double)listaTotal.Count / filter.pagination.ItemsPerPage);

//            for (int i = 0; i < listaTotal.Count; i++) listaTotal[i].NID = i;

//            var page = Functions.Pagination<CoverageEntitySigma>(
//                listaTotal, filter.pagination.CurrentPage, filter.pagination.ItemsPerPage);

//            return Ok(new
//            {
//                Data = page,
//                Pagination = filter.pagination,
//                Warnings = warnings
//            });

//            //static List<CoverageEntitySigma> CatchList(Task<IEnumerable<CoverageEntitySigma>> t, string tag, List<string> warns)
//            //{
//            //    if (t.Status == TaskStatus.RanToCompletion) return t.Result?.ToList() ?? new();
//            //    warns.Add($"{tag} no disponible temporalmente");
//            //    return new();
//            //}

//            static List<CoverageEntitySigma> CatchList(
//            Task<IEnumerable<CoverageEntitySigma>> t,
//            string tag,
//            List<string> warns)
//                    {
//                        if (t.Status == TaskStatus.RanToCompletion)
//                            return t.Result?.ToList() ?? new();

//                        // 🔥 acá está la clave: sacar el mensaje real
//                        var ex = t.Exception?.GetBaseException();
//                        var msg = ex?.Message ?? "sin detalle";

//                        // opcional: recortar para que no sea gigante
//                        if (msg.Length > 180) msg = msg.Substring(0, 180);

//                        warns.Add($"{tag} no disponible temporalmente: {msg}");
//                        return new();
//            }


//            static DateTime ParseDateOrMin(string s) =>
//                DateTime.TryParseExact(s, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture,
//                    System.Globalization.DateTimeStyles.None, out var d) ? d : DateTime.MinValue;
//        }
//    }
//}

using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Entities.Models;
using CrecerIndex.Repository.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrecerIndexApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoverageController : ControllerBase
    {
        private readonly ICoverageRepository _coverageRepo;
        private readonly ILogger<CoverageController> _logger;

        public CoverageController(ICoverageRepository coverageRepo, ILogger<CoverageController> logger)
        {
            _coverageRepo = coverageRepo;
            _logger = logger;
        }

        [HttpPost("getIndexData")]
        [Authorize]
        public async Task<ActionResult> getListCoveragePaginated([FromBody] FilterCoverageEntity filter)
        {
            _logger.LogInformation("=== INICIO getIndexData ===");
            _logger.LogInformation("Filter - DocType:{DocType} DocNum:{DocNum} Name:{Name}",
                filter?.sdocumenttype, filter?.sdocumentnumber, filter?.sname);

            try
            {
                if (filter == null)
                {
                    _logger.LogWarning("❌ Filter es NULL");
                    return BadRequest("Filter es requerido");
                }

                filter.sdocumenttype = Functions.GetDocumentTypeCrecer(
                    string.IsNullOrWhiteSpace(filter.sdocumenttype) ? 0 : Convert.ToInt32(filter.sdocumenttype),
                    filter.sdocumentnumber ?? string.Empty);

                _logger.LogInformation("DocType normalizado: {DocType}", filter.sdocumenttype);

                var warnings = new List<string>();

                // timeout total "defensivo" del endpoint
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(110));
                var ct = cts.Token;

                _logger.LogInformation("Ejecutando consultas en paralelo...");
                var sw = System.Diagnostics.Stopwatch.StartNew();

                // Ejecuta todo en paralelo
                var tSigma = _coverageRepo.GetCoverage(filter, ct).ContinueWith(t => CatchList(t, "SIGMA", warnings, _logger), ct);
                var tPN = _coverageRepo.GetCoverageCrecerPN(filter, ct).ContinueWith(t => CatchList(t, "CORE_PN", warnings, _logger), ct);
                var tPJ = _coverageRepo.GetCoverageCrecerPJ(filter, ct).ContinueWith(t => CatchList(t, "CORE_PJ", warnings, _logger), ct);
                var tPNC = _coverageRepo.GetCoverageCrecerPNContra(filter, ct).ContinueWith(t => CatchList(t, "CORE_PN_CONTRA", warnings, _logger), ct);
                var tPJC = _coverageRepo.GetCoverageCrecerPJContra(filter, ct).ContinueWith(t => CatchList(t, "CORE_PJ_CONTRA", warnings, _logger), ct);

                await Task.WhenAll(tSigma, tPN, tPJ, tPNC, tPJC);

                var elapsed = sw.ElapsedMilliseconds;
                _logger.LogInformation("✅ Consultas completadas en {Time}ms", elapsed);
                _logger.LogInformation("Resultados - SIGMA:{S} PN:{PN} PJ:{PJ} PNC:{PNC} PJC:{PJC}",
                    tSigma.Result.Count, tPN.Result.Count, tPJ.Result.Count, tPNC.Result.Count, tPJC.Result.Count);

                var listaTotal = tSigma.Result
                    .Concat(tPN.Result)
                    .Concat(tPJ.Result)
                    .Concat(tPNC.Result)
                    .Concat(tPJC.Result)
                    .OrderByDescending(x => ParseDateOrMin(x.SFECHA_PROCESO))
                    .ToList();

                _logger.LogInformation("Total registros combinados: {Total}", listaTotal.Count);

                filter.pagination ??= new PaginationEntity();
                if (filter.pagination.ItemsPerPage <= 0) filter.pagination.ItemsPerPage = 10;

                filter.pagination.TotalItems = listaTotal.Count;
                filter.pagination.TotalPages = (int)Math.Ceiling((double)listaTotal.Count / filter.pagination.ItemsPerPage);

                for (int i = 0; i < listaTotal.Count; i++) listaTotal[i].NID = i;

                var page = Functions.Pagination<CoverageEntitySigma>(
                    listaTotal, filter.pagination.CurrentPage, filter.pagination.ItemsPerPage);

                if (warnings.Count > 0)
                {
                    _logger.LogWarning("⚠️ Warnings: {Warnings}", string.Join(" | ", warnings));
                }

                _logger.LogInformation("✅ getIndexData completado exitosamente");

                return Ok(new
                {
                    Data = page,
                    Pagination = filter.pagination,
                    Warnings = warnings
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("❌ Timeout: La operación excedió el tiempo límite de 110 segundos");
                return StatusCode(504, new { error = "Timeout", message = "La consulta excedió el tiempo límite" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en getIndexData: {Message}", ex.Message);
                _logger.LogError("StackTrace: {Stack}", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {Inner}", ex.InnerException.Message);
                }

                return StatusCode(500, new
                {
                    error = "Error interno",
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // Endpoint de prueba para verificar conexión a Oracle
        [HttpGet("test-oracle")]
        [AllowAnonymous]
        public async Task<ActionResult> TestOracle()
        {
            _logger.LogInformation("=== TEST ORACLE ===");
            var results = new Dictionary<string, object>();

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var ct = cts.Token;

                var filter = new FilterCoverageEntity
                {
                    sdocumenttype = "1",
                    sdocumentnumber = "00000000", // DNI inexistente para prueba rápida
                    sname = ""
                };

                // Probar SIGMA
                try
                {
                    _logger.LogInformation("Probando conexión SIGMA...");
                    var sigmaSw = System.Diagnostics.Stopwatch.StartNew();
                    var sigma = await _coverageRepo.GetCoverage(filter, ct);
                    results["SIGMA"] = new { status = "OK", time = sigmaSw.ElapsedMilliseconds + "ms", rows = sigma.Count() };
                    _logger.LogInformation("✅ SIGMA OK en {Time}ms", sigmaSw.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    results["SIGMA"] = new { status = "ERROR", message = ex.Message, inner = ex.InnerException?.Message };
                    _logger.LogError(ex, "❌ SIGMA Error: {Message}", ex.Message);
                }

                // Probar CORE PN
                try
                {
                    _logger.LogInformation("Probando conexión CORE (PN)...");
                    var coreSw = System.Diagnostics.Stopwatch.StartNew();
                    var core = await _coverageRepo.GetCoverageCrecerPN(filter, ct);
                    results["CORE_PN"] = new { status = "OK", time = coreSw.ElapsedMilliseconds + "ms", rows = core.Count() };
                    _logger.LogInformation("✅ CORE_PN OK en {Time}ms", coreSw.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    results["CORE_PN"] = new { status = "ERROR", message = ex.Message, inner = ex.InnerException?.Message };
                    _logger.LogError(ex, "❌ CORE_PN Error: {Message}", ex.Message);
                }

                return Ok(new { status = "Test completado", results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general en test-oracle: {Message}", ex.Message);
                return StatusCode(500, new { status = "ERROR", message = ex.Message });
            }
        }

        // Helper para capturar errores de cada fuente
        private static List<CoverageEntitySigma> CatchList(
            Task<IEnumerable<CoverageEntitySigma>> t,
            string tag,
            List<string> warns,
            ILogger logger)
        {
            if (t.Status == TaskStatus.RanToCompletion)
            {
                var result = t.Result?.ToList() ?? new();
                logger.LogInformation("✅ {Tag} completado con {Count} registros", tag, result.Count);
                return result;
            }

            // Sacar el mensaje real del error
            var ex = t.Exception?.GetBaseException();
            var msg = ex?.Message ?? "sin detalle";

            // Recortar si es muy largo
            if (msg.Length > 200) msg = msg.Substring(0, 200) + "...";

            logger.LogError("❌ {Tag} falló: {Message}", tag, msg);
            warns.Add($"{tag}: {msg}");
            return new();
        }

        private static DateTime ParseDateOrMin(string s) =>
            DateTime.TryParseExact(s, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var d) ? d : DateTime.MinValue;
    }
}