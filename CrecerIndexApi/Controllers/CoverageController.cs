using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Entities.Models;
using CrecerIndex.Repository.Extensions;
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

        [HttpPost("getIndexData")]
        public async Task<ActionResult> getListCoveragePaginated([FromBody] FilterCoverageEntity filter)
        {
            /*CATALOGO DOCUMENTOS SUNAT
             '1' => 'DNI'
             '6' => 'RUC'
             '4' => 'CE'
             '7' => 'PS'             
             */

            //Lista No Core
            var listNoCore = await coverageRepo.GetCoverage(filter);

            filter.sdocumenttype = Functions.GetDocumentTypeCrecer(Convert.ToInt32(filter.sdocumenttype), filter.sdocumentnumber);
            //Lista Core - PersonaNatular
            var listCorePN = await coverageRepo.GetCoverageCrecerPN(filter);
            //Lista Core - PersonaNatular
            var listCorePJ = await coverageRepo.GetCoverageCrecerPJ(filter);

            var listaTotal = ((from noCore in listNoCore
                               select noCore)
                              .Union(from corePN in listCorePN
                                     select corePN)
                              .Union(from corePJ in listCorePJ
                                     select corePJ)).ToList().OrderByDescending(x => Convert.ToDateTime(x.SFECHA_PROCESO));

            //var listaTotal = (from noCore in listNoCore
            //                   select noCore).ToList();




            filter.pagination.TotalItems = listaTotal.Count();
            decimal _pages = Convert.ToDecimal(listaTotal.Count()) / Convert.ToDecimal(filter.pagination.ItemsPerPage);
            filter.pagination.TotalPages = Convert.ToInt32(Math.Ceiling(_pages));

            var index = 0;
            foreach (var item in listaTotal)
            {
                item.NID = index;
                index++;
            }

            var paginationList = Functions.Pagination<CoverageEntitySigma>(listaTotal, filter.pagination.CurrentPage, filter.pagination.ItemsPerPage);

            return Ok(new
            {
                Data = paginationList,
                Pagination = filter.pagination
            });
        }
    }
}
