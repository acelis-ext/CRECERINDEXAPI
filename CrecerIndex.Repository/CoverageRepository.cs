using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Entities.Models;
using CrecerIndex.Repository.Context;
using CrecerIndex.Repository.Extensions;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Repository
{
    public class CoverageRepository : ICoverageRepository
    {
        private readonly string _oracleDbConnection;
        private readonly string _oracleCoreDbConnection;

        public CoverageRepository(IConfiguration configuration)
        {
            _oracleDbConnection = configuration.GetConnectionString("OracleDB");
            _oracleCoreDbConnection = configuration.GetConnectionString("OracleCoreDB");
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity _filter)
        {
            IEnumerable<CoverageEntitySigma> entityAlertParams = null;
            List<OracleParameter> parameter = new List<OracleParameter>
            {
                new OracleParameter("STIPO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input),
                new OracleParameter("SNRO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input),
                new OracleParameter("SNOMBRE_COMPLETO", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input),
                new OracleParameter("NINDICADOR_PAGINADO", OracleDbType.Varchar2, 0, ParameterDirection.Input),
                new OracleParameter("PNPAGESIZE", OracleDbType.Varchar2, 0, ParameterDirection.Input),
                new OracleParameter("PNPAGENUM", OracleDbType.Varchar2, 0, ParameterDirection.Input),
                new OracleParameter("C_COLUMNS", OracleDbType.RefCursor, ParameterDirection.Output)
            };

            using (OracleConnection conn = new OracleConnection(_oracleDbConnection))
            using (OracleCommand cmd = new OracleCommand("PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameter.ToArray());
                await conn.OpenAsync();

                using (OracleDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    try
                    {
                        entityAlertParams = dr.ReadRows<CoverageEntitySigma>();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error al mapear resultados", ex);
                    }
                }
            }

            return entityAlertParams;
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity _filter)
        {
            IEnumerable<CoverageEntitySigma> entityAlertParams = null;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output),
                    new OracleParameter("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input),
                    new OracleParameter("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input),
                    new OracleParameter("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input)
                };

                using (OracleConnection conn = new OracleConnection(_oracleCoreDbConnection))
                using (OracleCommand cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD.open_ListaPN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameter.ToArray());
                    await conn.OpenAsync();

                    using (OracleDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        try
                        {
                            entityAlertParams = dr.ReadRows<CoverageEntitySigma>();
                        }
                        catch (Exception ex)
                        {
                            var logText = $"Error GetCoverageCrecerPN => message => {ex.Message}, stacktrace => {ex.StackTrace}";
                            File.WriteAllText("E:\\LogAsegurabilidad\\Log_1.txt", logText);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var _ = ex;
            }

            return entityAlertParams;
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity _filter)
        {
            IEnumerable<CoverageEntitySigma> entityAlertParams = null;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output),
                    new OracleParameter("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input),
                    new OracleParameter("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input),
                    new OracleParameter("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input)
                };

                using (OracleConnection conn = new OracleConnection(_oracleCoreDbConnection))
                using (OracleCommand cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD.open_ListaPJ", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameter.ToArray());
                    await conn.OpenAsync();

                    using (OracleDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        try
                        {
                            entityAlertParams = dr.ReadRows<CoverageEntitySigma>();
                        }
                        catch (Exception ex)
                        {
                            var logText = $"Error GetCoverageCrecerPJ => message => {ex.Message}, stacktrace => {ex.StackTrace}";
                            File.WriteAllText("E:\\LogAsegurabilidad\\Log_2.txt", logText);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var _ = ex;
            }

            return entityAlertParams;
        }
    }
}
