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

        //        [HttpPost("getIndexData")]
        //        public async Task<ActionResult> getListCoveragePaginated([FromBody] FilterCoverageEntity filter)
        //        {
        //            try
        //            {
        ///*CATALOGO DOCUMENTOS SUNAT
        //             '1' => 'DNI'
        //             '6' => 'RUC'
        //             '4' => 'CE'
        //             '7' => 'PS'             
        //             */

        //            //Lista No Core
        //            var listNoCore = await coverageRepo.GetCoverage(filter);

        //            filter.sdocumenttype = Functions.GetDocumentTypeCrecer(Convert.ToInt32(filter.sdocumenttype), filter.sdocumentnumber);
        //            //Lista Core - PersonaNatular
        //            var listCorePN = await coverageRepo.GetCoverageCrecerPN(filter);
        //            //Lista Core - PersonaNatular
        //            var listCorePJ = await coverageRepo.GetCoverageCrecerPJ(filter);

        //                var listaTotal = ((from noCore in listNoCore
        //                                   select noCore)
        //                                  .Union(from corePN in listCorePN
        //                                         select corePN)
        //                                  .Union(from corePJ in listCorePJ
        //                                         select corePJ)).ToList().OrderByDescending(x =>
        //                                         {
        //                                             if (DateTime.TryParseExact(
        //                                                     x.SFECHA_PROCESO,
        //                                                     "dd/MM/yyyy",
        //                                                     System.Globalization.CultureInfo.InvariantCulture,
        //                                                     System.Globalization.DateTimeStyles.None,
        //                                                     out var parsedDate))
        //                                             {
        //                                                 return parsedDate;
        //                                             }
        //                                             else
        //                                             {
        //                                                 return DateTime.MinValue; // o la fecha que tú consideres
        //                                             }
        //                                         })    ;


        //                //var listaTotal = (from noCore in listNoCore
        //                //                   select noCore).ToList();

        //                // 👇 VALIDACIÓN DEFENSIVA agregada
        //                if (filter.pagination == null)
        //                {
        //                    filter.pagination = new PaginationEntity();
        //                }

        //                if (filter.pagination.ItemsPerPage <= 0)
        //                {
        //                    filter.pagination.ItemsPerPage = 10; // valor por defecto seguro
        //                }


        //                filter.pagination.TotalItems = listaTotal.Count();
        //            decimal _pages = Convert.ToDecimal(listaTotal.Count()) / Convert.ToDecimal(filter.pagination.ItemsPerPage);
        //            filter.pagination.TotalPages = Convert.ToInt32(Math.Ceiling(_pages));

        //            var index = 0;
        //            foreach (var item in listaTotal)
        //            {
        //                item.NID = index;
        //                index++;
        //            }

        //            var paginationList = Functions.Pagination<CoverageEntitySigma>(listaTotal, filter.pagination.CurrentPage, filter.pagination.ItemsPerPage);

        //            return Ok(new
        //            {
        //                Data = paginationList,
        //                Pagination = filter.pagination
        //            });
        //            }
        //            catch(Exception ex)
        //            {
        //                Console.WriteLine($"[ERROR] Controller getListCoveragePaginated: {ex.Message}\n{ex.StackTrace}");
        //                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
        //            }

        //        }

        [HttpPost("getIndexData")]
        //[Authorize]

        public async Task<ActionResult> getListCoveragePaginated([FromBody] FilterCoverageEntity filter)
        {
            try
            {
                /*CATALOGO DOCUMENTOS SUNAT
                     '1' => 'DNI'
                     '6' => 'RUC'
                     '4' => 'CE'
                     '7' => 'PS'             
                     */

                // Lista No Core (SIGMA)
                var listNoCore = await coverageRepo.GetCoverage(filter);

                // Mapea el tipo de doc al formato Core (Crecer)
                filter.sdocumenttype = Functions.GetDocumentTypeCrecer(string.IsNullOrWhiteSpace(filter.sdocumenttype) ? 0 : Convert.ToInt32(filter.sdocumenttype),filter.sdocumentnumber);
                //filter.sdocumenttype = Functions.GetDocumentTypeCrecer(Convert.ToInt32(filter.sdocumenttype), filter.sdocumentnumber);REVISARR ESTOOO
                // Listas Core
                var listCorePN = await coverageRepo.GetCoverageCrecerPN(filter);
                var listCorePJ = await coverageRepo.GetCoverageCrecerPJ(filter);

                // NUEVO: Listas Core por Contratante
                //var listCorePNContra = await coverageRepo.GetCoverageCrecerPNContra(filter);
                //var listCorePJContra = await coverageRepo.GetCoverageCrecerPJContra(filter);

                var listaTotal =
                    ((from noCore in listNoCore select noCore)
                    .Union(from corePN in listCorePN select corePN)
                    .Union(from corePJ in listCorePJ select corePJ)
                    //.Union(from corePNC in listCorePNContra select corePNC)
                    //.Union(from corePJC in listCorePJContra select corePJC)
                    )
                    .ToList()
                    .OrderByDescending(x =>
                    {
                        if (DateTime.TryParseExact(
                                x.SFECHA_PROCESO,
                                "dd/MM/yyyy",
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None,
                                out var parsedDate))
                        {
                            return parsedDate;
                        }
                        else
                        {
                            return DateTime.MinValue; // fecha mínima si no parsea
                        }
                    });

                // 👇 VALIDACIÓN DEFENSIVA
                if (filter.pagination == null)
                    filter.pagination = new PaginationEntity();

                if (filter.pagination.ItemsPerPage <= 0)
                    filter.pagination.ItemsPerPage = 10;

                filter.pagination.TotalItems = listaTotal.Count();
                decimal _pages = Convert.ToDecimal(listaTotal.Count()) / Convert.ToDecimal(filter.pagination.ItemsPerPage);
                filter.pagination.TotalPages = Convert.ToInt32(Math.Ceiling(_pages));

                var index = 0;
                foreach (var item in listaTotal)
                {
                    item.NID = index;
                    index++;
                }

                var paginationList = Functions.Pagination<CoverageEntitySigma>(
                    listaTotal,
                    filter.pagination.CurrentPage,
                    filter.pagination.ItemsPerPage
                );

                return Ok(new
                {
                    Data = paginationList,
                    Pagination = filter.pagination
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Controller getListCoveragePaginated: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

    }
}
