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

        //public async Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity _filter)
        //{
        //    IEnumerable<CoverageEntitySigma> entityAlertParams = null;
        //    List<OracleParameter> parameter = new List<OracleParameter>
        //    {
        //        new OracleParameter("STIPO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input),
        //        new OracleParameter("SNRO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input),
        //        new OracleParameter("SNOMBRE_COMPLETO", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input),
        //        new OracleParameter("NINDICADOR_PAGINADO", OracleDbType.Varchar2, 0, ParameterDirection.Input),
        //        new OracleParameter("PNPAGESIZE", OracleDbType.Varchar2, 0, ParameterDirection.Input),
        //        new OracleParameter("PNPAGENUM", OracleDbType.Varchar2, 0, ParameterDirection.Input),
        //        new OracleParameter("C_COLUMNS", OracleDbType.RefCursor, ParameterDirection.Output)
        //    };

        //    using (OracleConnection conn = new OracleConnection(_oracleDbConnection))
        //    using (OracleCommand cmd = new OracleCommand("PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddRange(parameter.ToArray());
        //        await conn.OpenAsync();

        //        using (OracleDataReader dr = await cmd.ExecuteReaderAsync())
        //        {
        //            try
        //            {
        //                entityAlertParams = dr.ReadRows<CoverageEntitySigma>();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Error al mapear resultados", ex);
        //            }
        //        }
        //    }

        //    return entityAlertParams;
        //}


        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity _filter)
        {
            var results = new List<CoverageEntitySigma>();

            try
            {
                using (var conn = new OracleConnection(_oracleDbConnection))
                using (var cmd = new OracleCommand("PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("STIPO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
                    cmd.Parameters.Add("SNRO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
                    cmd.Parameters.Add("SNOMBRE_COMPLETO", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);
                    cmd.Parameters.Add("NINDICADOR_PAGINADO", OracleDbType.Int32, 0, ParameterDirection.Input); // No paginado
                    cmd.Parameters.Add("PNPAGESIZE", OracleDbType.Int32, 0, ParameterDirection.Input);
                    cmd.Parameters.Add("PNPAGENUM", OracleDbType.Int32, 0, ParameterDirection.Input);
                    cmd.Parameters.Add("C_COLUMNS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    await conn.OpenAsync();

                    using (var adapter = new OracleDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            results.Add(new CoverageEntitySigma
                            {
                                NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
                                SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
                                SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
                                SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),
                                STIPO_DOCUMENTO_ASEGURADO = row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString(),
                                SNRO_DOCUMENTO_ASEGURADO = row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString(),
                                SNOMBRE_COMPLETO = row["SNOMBRE_COMPLETO"]?.ToString(),
                                SNOMBRES_RAZONSOCIAL_ASEGURADO = row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString(),
                                SAPELLIDO_PATERNO_ASEGURADO = row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString(),
                                SAPELLIDO_MATERNO_ASEGURADO = row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString(),
                                SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
                                SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),
                                NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
                                NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
                                SMONEDA = row["SMONEDA"]?.ToString(),
                                SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
                                SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
                                SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),
                                OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,
                                INI_POLIZA = row["INI_POLIZA"]?.ToString(),
                                FIN_POLIZA = row["FIN_POLIZA"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetCoverage: {ex.Message}\n{ex.StackTrace}");
                throw;
            }

            return results;
        }


        //public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity _filter)
        //{
        //    IEnumerable<CoverageEntitySigma> entityAlertParams = null;
        //    try
        //    {
        //        List<OracleParameter> parameter = new List<OracleParameter>
        //        {
        //            new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output),
        //            new OracleParameter("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input),
        //            new OracleParameter("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input),
        //            new OracleParameter("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input)
        //        };

        //        using (OracleConnection conn = new OracleConnection(_oracleCoreDbConnection))
        //        using (OracleCommand cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddRange(parameter.ToArray());
        //            await conn.OpenAsync();

        //            using (OracleDataReader dr = await cmd.ExecuteReaderAsync())
        //            {
        //                try
        //                {
        //                    entityAlertParams = dr.ReadRows<CoverageEntitySigma>();
        //                }
        //                catch (Exception ex)
        //                {
        //                    var logText = $"Error GetCoverageCrecerPN => message => {ex.Message}, stacktrace => {ex.StackTrace}";
        //                    File.WriteAllText("E:\\LogAsegurabilidad\\Log_1.txt", logText);
        //                    throw;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var _ = ex;
        //    }

        //    return entityAlertParams;
        //}


        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity _filter)
        {
            var results = new List<CoverageEntitySigma>();

            try
            {
                using (var conn = new OracleConnection(_oracleCoreDbConnection))
                using (var cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
                    cmd.Parameters.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
                    cmd.Parameters.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);

                    await conn.OpenAsync();

                    using (var adapter = new OracleDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            results.Add(new CoverageEntitySigma
                            {
                                NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
                                SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
                                SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
                                SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),
                                STIPO_DOCUMENTO_ASEGURADO = row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString(),
                                SNRO_DOCUMENTO_ASEGURADO = row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString(),
                                SNOMBRE_COMPLETO = row["SNOMBRE_COMPLETO"]?.ToString(),
                                SNOMBRES_RAZONSOCIAL_ASEGURADO = row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString(),
                                SAPELLIDO_PATERNO_ASEGURADO = row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString(),
                                SAPELLIDO_MATERNO_ASEGURADO = row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString(),
                                SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
                                SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),
                                NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
                                NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
                                SMONEDA = row["SMONEDA"]?.ToString(),
                                SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
                                SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
                                SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),
                                OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,
                                INI_POLIZA = row["INI_POLIZA"]?.ToString(),
                                FIN_POLIZA = row["FIN_POLIZA"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetCoverageCrecerPN: {ex.Message}\n{ex.StackTrace}");
                throw; // Re-lanza para manejo externo
            }

            return results;
        }

        //public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity _filter)
        //{
        //    IEnumerable<CoverageEntitySigma> entityAlertParams = null;
        //    try
        //    {
        //        List<OracleParameter> parameter = new List<OracleParameter>
        //        {
        //            new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output),
        //            new OracleParameter("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input),
        //            new OracleParameter("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input),
        //            new OracleParameter("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input)
        //        };

        //        using (OracleConnection conn = new OracleConnection(_oracleCoreDbConnection))
        //        using (OracleCommand cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddRange(parameter.ToArray());
        //            await conn.OpenAsync();

        //            using (OracleDataReader dr = await cmd.ExecuteReaderAsync())
        //            {
        //                try
        //                {
        //                    entityAlertParams = dr.ReadRows<CoverageEntitySigma>();
        //                }
        //                catch (Exception ex)
        //                {
        //                    var logText = $"Error GetCoverageCrecerPJ => message => {ex.Message}, stacktrace => {ex.StackTrace}";
        //                    File.WriteAllText("E:\\LogAsegurabilidad\\Log_2.txt", logText);
        //                    throw;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var _ = ex;
        //    }

        //    return entityAlertParams;
        //}

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity _filter)
        {
            var results = new List<CoverageEntitySigma>();

            try
            {
                using (var conn = new OracleConnection(_oracleCoreDbConnection))
                using (var cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
                    cmd.Parameters.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
                    cmd.Parameters.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);

                    await conn.OpenAsync();

                    using (var adapter = new OracleDataAdapter(cmd))
                    {
                        var dt = new DataTable();

                        // Puedes medir tiempos si deseas
                        // var sw = Stopwatch.StartNew();
                        adapter.Fill(dt);
                        // sw.Stop();
                        // Console.WriteLine($"Tiempo de carga DataTable PJ: {sw.ElapsedMilliseconds} ms");

                        foreach (DataRow row in dt.Rows)
                        {
                            results.Add(new CoverageEntitySigma
                            {
                                NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
                                SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
                                SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
                                SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),
                                STIPO_DOCUMENTO_ASEGURADO = row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString(),
                                SNRO_DOCUMENTO_ASEGURADO = row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString(),
                                SNOMBRE_COMPLETO = row["SNOMBRE_COMPLETO"]?.ToString(),
                                SNOMBRES_RAZONSOCIAL_ASEGURADO = row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString(),
                                SAPELLIDO_PATERNO_ASEGURADO = row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString(),
                                SAPELLIDO_MATERNO_ASEGURADO = row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString(),
                                SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
                                SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),
                                NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
                                NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
                                SMONEDA = row["SMONEDA"]?.ToString(),
                                SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
                                SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
                                SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),
                                OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,
                                INI_POLIZA = row["INI_POLIZA"]?.ToString(),
                                FIN_POLIZA = row["FIN_POLIZA"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetCoverageCrecerPJ: {ex.Message}\n{ex.StackTrace}");
                throw;
            }

            return results;
        }

    }
}
