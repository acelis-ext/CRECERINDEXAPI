using CrecerIndex.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Abstraction.Interfaces.IRepository
{
    public interface ICoverageRepository
    {
        Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity _filter);
        Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity _filter);
        Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity _filter);
        Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPNContra(FilterCoverageEntity filter);
        Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJContra(FilterCoverageEntity filter);

    }
}
