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
        //    var results = new List<CoverageEntitySigma>();

        //    try
        //    {
        //        using (var conn = new OracleConnection(_oracleDbConnection))
        //        using (var cmd = new OracleCommand("PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.Add("STIPO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
        //            cmd.Parameters.Add("SNRO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
        //            cmd.Parameters.Add("SNOMBRE_COMPLETO", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);
        //            cmd.Parameters.Add("NINDICADOR_PAGINADO", OracleDbType.Int32, 0, ParameterDirection.Input); // No paginado
        //            cmd.Parameters.Add("PNPAGESIZE", OracleDbType.Int32, 0, ParameterDirection.Input);
        //            cmd.Parameters.Add("PNPAGENUM", OracleDbType.Int32, 0, ParameterDirection.Input);
        //            cmd.Parameters.Add("C_COLUMNS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //            await conn.OpenAsync();

        //            using (var adapter = new OracleDataAdapter(cmd))
        //            {
        //                var dt = new DataTable();
        //                adapter.Fill(dt);

        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    results.Add(new CoverageEntitySigma
        //                    {
        //                        NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
        //                        SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
        //                        SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
        //                        SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),
        //                        STIPO_DOCUMENTO_ASEGURADO = row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString(),
        //                        SNRO_DOCUMENTO_ASEGURADO = row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString(),
        //                        SNOMBRE_COMPLETO = row["SNOMBRE_COMPLETO"]?.ToString(),
        //                        SNOMBRES_RAZONSOCIAL_ASEGURADO = row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString(),
        //                        SAPELLIDO_PATERNO_ASEGURADO = row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString(),
        //                        SAPELLIDO_MATERNO_ASEGURADO = row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString(),
        //                        SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
        //                        SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),
        //                        NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
        //                        NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
        //                        SMONEDA = row["SMONEDA"]?.ToString(),
        //                        SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
        //                        SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
        //                        SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),
        //                        OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,
        //                        INI_POLIZA = row["INI_POLIZA"]?.ToString(),
        //                        FIN_POLIZA = row["FIN_POLIZA"]?.ToString()
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[ERROR] GetCoverage: {ex.Message}\n{ex.StackTrace}");

        //        // devuélvelo con más contexto
        //        throw new Exception($"Error en GetCoverage: {ex.Message}", ex);
        //    }

        //    return results;
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
                    cmd.Parameters.Add("NINDICADOR_PAGINADO", OracleDbType.Int32, 0, ParameterDirection.Input);
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
                                FIN_POLIZA = row["FIN_POLIZA"]?.ToString(),

                                // Campos no retornados en SP Sigma
                                CONTRATANTE = string.Empty,
                                CANAL = string.Empty,
                                ES_CANAL_VINCULADO = string.Empty,
                                ESTADO_POLIZA = string.Empty,
                                EVENTO_POLIZA = string.Empty,
                                ESTADO_UR = string.Empty,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetCoverage: {ex.Message}\n{ex.StackTrace}");
                throw new Exception($"Error en GetCoverage: {ex.Message}", ex);
            }

            return results;
        }



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

                                CONTRATANTE = row["CONTRATRANTE"]?.ToString(),
                                CANAL = row["CANAL"]?.ToString(),
                                ES_CANAL_VINCULADO = row["ES_CANAL_VINCULADO"]?.ToString(),
                                ESTADO_POLIZA = row["ESTADO_POLIZA"]?.ToString(),
                                EVENTO_POLIZA = row["EVENTO_POLIZA"]?.ToString(),
                                ESTADO_UR = row["ESTADO_UR"]?.ToString(),


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

                // devuélvelo con más contexto
                throw new Exception($"Error en GetCoverageCrecerPN: {ex.Message}", ex);
            }

            return results;
        }




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

                                CONTRATANTE = row["CONTRATRANTE"]?.ToString(),
                                CANAL = row["CANAL"]?.ToString(),
                                ES_CANAL_VINCULADO = row["ES_CANAL_VINCULADO"]?.ToString(),
                                ESTADO_POLIZA = row["ESTADO_POLIZA"]?.ToString(),
                                EVENTO_POLIZA = row["EVENTO_POLIZA"]?.ToString(),
                                ESTADO_UR = row["ESTADO_UR"]?.ToString(),


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

                // devuélvelo con más contexto
                throw new Exception($"Error en GetCoverageCrecerPJ: {ex.Message}", ex);
            }

            return results;
        }


        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPNContra(FilterCoverageEntity _filter)
        {
            var results = new List<CoverageEntitySigma>();

            try
            {
                using (var conn = new OracleConnection(_oracleCoreDbConnection))
                using (var cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Contra", conn))
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

                                STIPO_DOCUMENTO_ASEGURADO = row.Table.Columns.Contains("STIPO_DOCUMENTO_ASEGURADO") ? row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString() : null,
                                SNRO_DOCUMENTO_ASEGURADO = row.Table.Columns.Contains("SNRO_DOCUMENTO_ASEGURADO") ? row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString() : null,
                                SNOMBRE_COMPLETO = row.Table.Columns.Contains("SNOMBRE_COMPLETO") ? row["SNOMBRE_COMPLETO"]?.ToString() : null,
                                SNOMBRES_RAZONSOCIAL_ASEGURADO = row.Table.Columns.Contains("SNOMBRES_RAZONSOCIAL_ASEGURADO") ? row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString() : null,
                                SAPELLIDO_PATERNO_ASEGURADO = row.Table.Columns.Contains("SAPELLIDO_PATERNO_ASEGURADO") ? row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString() : null,
                                SAPELLIDO_MATERNO_ASEGURADO = row.Table.Columns.Contains("SAPELLIDO_MATERNO_ASEGURADO") ? row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString() : null,

                                SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
                                SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),

                                NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
                                NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
                                SMONEDA = row["SMONEDA"]?.ToString(),

                                SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
                                SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
                                SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),

                                OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,

                                CONTRATANTE = row.Table.Columns.Contains("CONTRATANTE") ? row["CONTRATANTE"]?.ToString(): (row.Table.Columns.Contains("CONTRATRANTE") ? row["CONTRATRANTE"]?.ToString() : null),
                                CANAL = row.Table.Columns.Contains("CANAL") ? row["CANAL"]?.ToString() : null,
                                ES_CANAL_VINCULADO = row.Table.Columns.Contains("ES_CANAL_VINCULADO") ? row["ES_CANAL_VINCULADO"]?.ToString() : null,
                                ESTADO_POLIZA = row.Table.Columns.Contains("ESTADO_POLIZA") ? row["ESTADO_POLIZA"]?.ToString() : null,
                                EVENTO_POLIZA = row.Table.Columns.Contains("EVENTO_POLIZA") ? row["EVENTO_POLIZA"]?.ToString() : null,
                                ESTADO_UR = row.Table.Columns.Contains("ESTADO_UR") ? row["ESTADO_UR"]?.ToString() : null,

                                INI_POLIZA = row.Table.Columns.Contains("INI_POLIZA") ? row["INI_POLIZA"]?.ToString() : null,
                                FIN_POLIZA = row.Table.Columns.Contains("FIN_POLIZA") ? row["FIN_POLIZA"]?.ToString() : null
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetCoverageCrecerPNContra: {ex.Message}\n{ex.StackTrace}");
                throw new Exception($"Error en GetCoverageCrecerPNContra: {ex.Message}", ex);
            }

            return results;
        }


        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJContra(FilterCoverageEntity _filter)
        {
            var results = new List<CoverageEntitySigma>();

            try
            {
                using (var conn = new OracleConnection(_oracleCoreDbConnection))
                using (var cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index_Contra", conn))
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

                                STIPO_DOCUMENTO_ASEGURADO = row.Table.Columns.Contains("STIPO_DOCUMENTO_ASEGURADO") ? row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString() : null,
                                SNRO_DOCUMENTO_ASEGURADO = row.Table.Columns.Contains("SNRO_DOCUMENTO_ASEGURADO") ? row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString() : null,
                                SNOMBRE_COMPLETO = row.Table.Columns.Contains("SNOMBRE_COMPLETO") ? row["SNOMBRE_COMPLETO"]?.ToString() : null,
                                SNOMBRES_RAZONSOCIAL_ASEGURADO = row.Table.Columns.Contains("SNOMBRES_RAZONSOCIAL_ASEGURADO") ? row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString() : null,
                                SAPELLIDO_PATERNO_ASEGURADO = row.Table.Columns.Contains("SAPELLIDO_PATERNO_ASEGURADO") ? row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString() : null,
                                SAPELLIDO_MATERNO_ASEGURADO = row.Table.Columns.Contains("SAPELLIDO_MATERNO_ASEGURADO") ? row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString() : null,

                                SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
                                SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),

                                NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
                                NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
                                SMONEDA = row["SMONEDA"]?.ToString(),

                                SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
                                SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
                                SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),

                                OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,

                                CONTRATANTE = row.Table.Columns.Contains("CONTRATANTE") ? row["CONTRATANTE"]?.ToString(): (row.Table.Columns.Contains("CONTRATRANTE") ? row["CONTRATRANTE"]?.ToString() : null),
                                CANAL = row.Table.Columns.Contains("CANAL") ? row["CANAL"]?.ToString() : null,
                                ES_CANAL_VINCULADO = row.Table.Columns.Contains("ES_CANAL_VINCULADO") ? row["ES_CANAL_VINCULADO"]?.ToString() : null,
                                ESTADO_POLIZA = row.Table.Columns.Contains("ESTADO_POLIZA") ? row["ESTADO_POLIZA"]?.ToString() : null,
                                EVENTO_POLIZA = row.Table.Columns.Contains("EVENTO_POLIZA") ? row["EVENTO_POLIZA"]?.ToString() : null,
                                ESTADO_UR = row.Table.Columns.Contains("ESTADO_UR") ? row["ESTADO_UR"]?.ToString() : null,

                                INI_POLIZA = row.Table.Columns.Contains("INI_POLIZA") ? row["INI_POLIZA"]?.ToString() : null,
                                FIN_POLIZA = row.Table.Columns.Contains("FIN_POLIZA") ? row["FIN_POLIZA"]?.ToString() : null
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetCoverageCrecerPJContra: {ex.Message}\n{ex.StackTrace}");
                throw new Exception($"Error en GetCoverageCrecerPJContra: {ex.Message}", ex);
            }

            return results;
        }

    }
}
