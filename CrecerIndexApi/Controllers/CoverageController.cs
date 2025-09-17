using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Entities.Models;
using CrecerIndex.Repository.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrecerIndexApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoverageController : ControllerBase
    {
        private readonly ICoverageRepository coverageRepo;

        public CoverageController(ICoverageRepository coverageRepo)
        {
            this.coverageRepo = coverageRepo;
        }


        //[HttpPost("getIndexData")]
        //[Authorize]
        //public async Task<ActionResult> getListCoveragePaginated([FromBody] FilterCoverageEntity filter)
        //{
        //    // normaliza doc type para Core
        //    filter.sdocumenttype = Functions.GetDocumentTypeCrecer(
        //        string.IsNullOrWhiteSpace(filter.sdocumenttype) ? 0 : Convert.ToInt32(filter.sdocumenttype),
        //        filter.sdocumentnumber ?? string.Empty);

        //    var warnings = new List<string>();

        //    // Ejecuta en paralelo
        //    var tSigma = coverageRepo.GetCoverage(filter).ContinueWith(t => CatchList(t, "SIGMA", warnings));
        //    var tPN = coverageRepo.GetCoverageCrecerPN(filter).ContinueWith(t => CatchList(t, "CORE_PN", warnings));
        //    var tPJ = coverageRepo.GetCoverageCrecerPJ(filter).ContinueWith(t => CatchList(t, "CORE_PJ", warnings));

        //    await Task.WhenAll(tSigma, tPN, tPJ);

        //    var listaTotal = tSigma.Result.Concat(tPN.Result).Concat(tPJ.Result)
        //        .OrderByDescending(x => ParseDateOrMin(x.SFECHA_PROCESO))
        //        .ToList();

        //    // paginación defensiva
        //    filter.pagination ??= new PaginationEntity();
        //    if (filter.pagination.ItemsPerPage <= 0) filter.pagination.ItemsPerPage = 10;

        //    filter.pagination.TotalItems = listaTotal.Count;
        //    filter.pagination.TotalPages = (int)Math.Ceiling(
        //        (double)listaTotal.Count / filter.pagination.ItemsPerPage);

        //    for (int i = 0; i < listaTotal.Count; i++) listaTotal[i].NID = i;

        //    var page = Functions.Pagination<CoverageEntitySigma>(
        //        listaTotal, filter.pagination.CurrentPage, filter.pagination.ItemsPerPage);

        //    return Ok(new
        //    {
        //        Data = page,
        //        Pagination = filter.pagination,
        //        Warnings = warnings // <- el front puede mostrar un toast si vino algo
        //    });

        //    // helpers locales
        //    static List<CoverageEntitySigma> CatchList(Task<IEnumerable<CoverageEntitySigma>> t, string tag, List<string> warns)
        //    {
        //        if (t.Status == TaskStatus.RanToCompletion) return t.Result?.ToList() ?? new();
        //        warns.Add($"{tag} no disponible temporalmente");
        //        return new();
        //    }
        //    static DateTime ParseDateOrMin(string s) =>
        //        DateTime.TryParseExact(s, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture,
        //            System.Globalization.DateTimeStyles.None, out var d) ? d : DateTime.MinValue;
        //}

        [HttpPost("getIndexData")]
        [Authorize]
        public async Task<ActionResult> getListCoveragePaginated([FromBody] FilterCoverageEntity filter)
        {
            // normaliza doc type para Core
            filter.sdocumenttype = Functions.GetDocumentTypeCrecer(
                string.IsNullOrWhiteSpace(filter.sdocumenttype) ? 0 : Convert.ToInt32(filter.sdocumenttype),
                filter.sdocumentnumber ?? string.Empty);

            var warnings = new List<string>();

            // Ejecuta en paralelo (igual que tenías) + agrego las 2 Contra
            var tSigma = coverageRepo.GetCoverage(filter).ContinueWith(t => CatchList(t, "SIGMA", warnings));
            var tPN = coverageRepo.GetCoverageCrecerPN(filter).ContinueWith(t => CatchList(t, "CORE_PN", warnings));
            var tPJ = coverageRepo.GetCoverageCrecerPJ(filter).ContinueWith(t => CatchList(t, "CORE_PJ", warnings));
            var tPNC = coverageRepo.GetCoverageCrecerPNContra(filter).ContinueWith(t => CatchList(t, "CORE_PN_CONTRA", warnings));
            var tPJC = coverageRepo.GetCoverageCrecerPJContra(filter).ContinueWith(t => CatchList(t, "CORE_PJ_CONTRA", warnings));

            await Task.WhenAll(tSigma, tPN, tPJ, tPNC, tPJC);

            // Concat SIN eliminar duplicados, y ordenar por SFECHA_PROCESO desc
            var listaTotal = tSigma.Result
                .Concat(tPN.Result)
                .Concat(tPJ.Result)
                .Concat(tPNC.Result)
                .Concat(tPJC.Result)
                .OrderByDescending(x => ParseDateOrMin(x.SFECHA_PROCESO))
                .ToList();

            // paginación defensiva
            filter.pagination ??= new PaginationEntity();
            if (filter.pagination.ItemsPerPage <= 0) filter.pagination.ItemsPerPage = 10;

            filter.pagination.TotalItems = listaTotal.Count;
            filter.pagination.TotalPages = (int)Math.Ceiling(
                (double)listaTotal.Count / filter.pagination.ItemsPerPage);

            for (int i = 0; i < listaTotal.Count; i++) listaTotal[i].NID = i;

            var page = Functions.Pagination<CoverageEntitySigma>(
                listaTotal, filter.pagination.CurrentPage, filter.pagination.ItemsPerPage);

            return Ok(new
            {
                Data = page,
                Pagination = filter.pagination,
                Warnings = warnings // el front puede mostrar un toast si alguna fuente falló
            });

            // helpers locales (idénticos al patrón que estabas usando)
            static List<CoverageEntitySigma> CatchList(Task<IEnumerable<CoverageEntitySigma>> t, string tag, List<string> warns)
            {
                if (t.Status == TaskStatus.RanToCompletion) return t.Result?.ToList() ?? new();
                warns.Add($"{tag} no disponible temporalmente");
                return new();
            }
            static DateTime ParseDateOrMin(string s) =>
                DateTime.TryParseExact(s, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var d) ? d : DateTime.MinValue;
        }



    }
}
