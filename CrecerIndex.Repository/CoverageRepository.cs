//using CrecerIndex.Abstraction.Interfaces.IRepository;
//using CrecerIndex.Entities.Models;
//using CrecerIndex.Repository.Context;
//using CrecerIndex.Repository.Extensions;
//using Microsoft.Extensions.Configuration;
//using Oracle.ManagedDataAccess.Client;
//using System;
//using System.Collections.Generic;
//using System.Data;

//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CrecerIndex.Repository
//{
//    public class CoverageRepository : ICoverageRepository
//    {
//        private readonly string _oracleDbConnection;
//        private readonly string _oracleCoreDbConnection;

//        public CoverageRepository(IConfiguration configuration)
//        {
//            _oracleDbConnection = configuration.GetConnectionString("OracleDB");
//            _oracleCoreDbConnection = configuration.GetConnectionString("OracleCoreDB");
//        }


//        //public async Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity _filter)
//        //{
//        //    var results = new List<CoverageEntitySigma>();

//        //    try
//        //    {
//        //        using (var conn = new OracleConnection(_oracleDbConnection))
//        //        using (var cmd = new OracleCommand("PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD", conn))
//        //        {
//        //            cmd.CommandType = CommandType.StoredProcedure;

//        //            cmd.Parameters.Add("STIPO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//        //            cmd.Parameters.Add("SNRO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//        //            cmd.Parameters.Add("SNOMBRE_COMPLETO", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);
//        //            cmd.Parameters.Add("NINDICADOR_PAGINADO", OracleDbType.Int32, 0, ParameterDirection.Input);
//        //            cmd.Parameters.Add("PNPAGESIZE", OracleDbType.Int32, 0, ParameterDirection.Input);
//        //            cmd.Parameters.Add("PNPAGENUM", OracleDbType.Int32, 0, ParameterDirection.Input);
//        //            cmd.Parameters.Add("C_COLUMNS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

//        //            await conn.OpenAsync();

//        //            using (var adapter = new OracleDataAdapter(cmd))
//        //            {
//        //                var dt = new DataTable();
//        //                adapter.Fill(dt);

//        //                foreach (DataRow row in dt.Rows)
//        //                {
//        //                    results.Add(new CoverageEntitySigma
//        //                    {
//        //                        NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
//        //                        SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
//        //                        SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
//        //                        SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),
//        //                        STIPO_DOCUMENTO_ASEGURADO = row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString(),
//        //                        SNRO_DOCUMENTO_ASEGURADO = row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString(),
//        //                        SNOMBRE_COMPLETO = row["SNOMBRE_COMPLETO"]?.ToString(),
//        //                        SNOMBRES_RAZONSOCIAL_ASEGURADO = row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString(),
//        //                        SAPELLIDO_PATERNO_ASEGURADO = row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString(),
//        //                        SAPELLIDO_MATERNO_ASEGURADO = row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString(),
//        //                        SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
//        //                        SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),
//        //                        NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
//        //                        NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
//        //                        SMONEDA = row["SMONEDA"]?.ToString(),
//        //                        SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
//        //                        SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
//        //                        SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),
//        //                        OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,
//        //                        INI_POLIZA = row["INI_POLIZA"]?.ToString(),
//        //                        FIN_POLIZA = row["FIN_POLIZA"]?.ToString(),

//        //                        // Campos no retornados en SP Sigma
//        //                        CONTRATANTE = string.Empty,
//        //                        CANAL = string.Empty,
//        //                        ES_CANAL_VINCULADO = string.Empty,
//        //                        ESTADO_POLIZA = string.Empty,
//        //                        EVENTO_POLIZA = string.Empty,
//        //                        ESTADO_UR = string.Empty,
//        //                    });
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"[ERROR] GetCoverage: {ex.Message}\n{ex.StackTrace}");
//        //        throw new Exception($"Error en GetCoverage: {ex.Message}", ex);
//        //    }

//        //    return results;
//        //}



//        //public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity _filter)
//        //{
//        //    var results = new List<CoverageEntitySigma>();

//        //    try
//        //    {
//        //        using (var conn = new OracleConnection(_oracleCoreDbConnection))
//        //        using (var cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index", conn))
//        //        {
//        //            cmd.CommandType = CommandType.StoredProcedure;

//        //            cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
//        //            cmd.Parameters.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//        //            cmd.Parameters.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//        //            cmd.Parameters.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);

//        //            await conn.OpenAsync();

//        //            using (var adapter = new OracleDataAdapter(cmd))
//        //            {
//        //                var dt = new DataTable();
//        //                adapter.Fill(dt);

//        //                foreach (DataRow row in dt.Rows)
//        //                {
//        //                    results.Add(new CoverageEntitySigma
//        //                    {
//        //                        NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
//        //                        SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
//        //                        SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
//        //                        SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),
//        //                        STIPO_DOCUMENTO_ASEGURADO = row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString(),
//        //                        SNRO_DOCUMENTO_ASEGURADO = row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString(),
//        //                        SNOMBRE_COMPLETO = row["SNOMBRE_COMPLETO"]?.ToString(),
//        //                        SNOMBRES_RAZONSOCIAL_ASEGURADO = row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString(),
//        //                        SAPELLIDO_PATERNO_ASEGURADO = row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString(),
//        //                        SAPELLIDO_MATERNO_ASEGURADO = row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString(),
//        //                        SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
//        //                        SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),
//        //                        NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
//        //                        NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
//        //                        SMONEDA = row["SMONEDA"]?.ToString(),
//        //                        SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
//        //                        SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
//        //                        SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),
//        //                        OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,

//        //                        CONTRATANTE = row["CONTRATRANTE"]?.ToString(),
//        //                        CANAL = row["CANAL"]?.ToString(),
//        //                        ES_CANAL_VINCULADO = row["ES_CANAL_VINCULADO"]?.ToString(),
//        //                        ESTADO_POLIZA = row["ESTADO_POLIZA"]?.ToString(),
//        //                        EVENTO_POLIZA = row["EVENTO_POLIZA"]?.ToString(),
//        //                        ESTADO_UR = row["ESTADO_UR"]?.ToString(),


//        //                        INI_POLIZA = row["INI_POLIZA"]?.ToString(),
//        //                        FIN_POLIZA = row["FIN_POLIZA"]?.ToString()
//        //                    });
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"[ERROR] GetCoverageCrecerPN: {ex.Message}\n{ex.StackTrace}");

//        //        // devuélvelo con más contexto
//        //        throw new Exception($"Error en GetCoverageCrecerPN: {ex.Message}", ex);
//        //    }

//        //    return results;
//        //}




//        //public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity _filter)
//        //{
//        //    var results = new List<CoverageEntitySigma>();

//        //    try
//        //    {
//        //        using (var conn = new OracleConnection(_oracleCoreDbConnection))
//        //        using (var cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index", conn))
//        //        {
//        //            cmd.CommandType = CommandType.StoredProcedure;

//        //            cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
//        //            cmd.Parameters.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//        //            cmd.Parameters.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//        //            cmd.Parameters.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);

//        //            await conn.OpenAsync();

//        //            using (var adapter = new OracleDataAdapter(cmd))
//        //            {
//        //                var dt = new DataTable();

//        //                // Puedes medir tiempos si deseas
//        //                // var sw = Stopwatch.StartNew();
//        //                adapter.Fill(dt);
//        //                // sw.Stop();
//        //                // Console.WriteLine($"Tiempo de carga DataTable PJ: {sw.ElapsedMilliseconds} ms");

//        //                foreach (DataRow row in dt.Rows)
//        //                {
//        //                    results.Add(new CoverageEntitySigma
//        //                    {
//        //                        NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
//        //                        SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
//        //                        SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
//        //                        SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),
//        //                        STIPO_DOCUMENTO_ASEGURADO = row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString(),
//        //                        SNRO_DOCUMENTO_ASEGURADO = row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString(),
//        //                        SNOMBRE_COMPLETO = row["SNOMBRE_COMPLETO"]?.ToString(),
//        //                        SNOMBRES_RAZONSOCIAL_ASEGURADO = row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString(),
//        //                        SAPELLIDO_PATERNO_ASEGURADO = row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString(),
//        //                        SAPELLIDO_MATERNO_ASEGURADO = row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString(),
//        //                        SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
//        //                        SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),
//        //                        NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
//        //                        NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
//        //                        SMONEDA = row["SMONEDA"]?.ToString(),
//        //                        SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
//        //                        SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
//        //                        SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),
//        //                        OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,

//        //                        CONTRATANTE = row["CONTRATRANTE"]?.ToString(),
//        //                        CANAL = row["CANAL"]?.ToString(),
//        //                        ES_CANAL_VINCULADO = row["ES_CANAL_VINCULADO"]?.ToString(),
//        //                        ESTADO_POLIZA = row["ESTADO_POLIZA"]?.ToString(),
//        //                        EVENTO_POLIZA = row["EVENTO_POLIZA"]?.ToString(),
//        //                        ESTADO_UR = row["ESTADO_UR"]?.ToString(),


//        //                        INI_POLIZA = row["INI_POLIZA"]?.ToString(),
//        //                        FIN_POLIZA = row["FIN_POLIZA"]?.ToString()
//        //                    });
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"[ERROR] GetCoverageCrecerPJ: {ex.Message}\n{ex.StackTrace}");

//        //        // devuélvelo con más contexto
//        //        throw new Exception($"Error en GetCoverageCrecerPJ: {ex.Message}", ex);
//        //    }

//        //    return results;
//        //}


//        //public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPNContra(FilterCoverageEntity _filter)
//        //{
//        //    var results = new List<CoverageEntitySigma>();

//        //    try
//        //    {
//        //        using (var conn = new OracleConnection(_oracleCoreDbConnection))
//        //        using (var cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Contra", conn))
//        //        {
//        //            cmd.CommandType = CommandType.StoredProcedure;

//        //            cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
//        //            cmd.Parameters.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//        //            cmd.Parameters.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//        //            cmd.Parameters.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);

//        //            await conn.OpenAsync();

//        //            using (var adapter = new OracleDataAdapter(cmd))
//        //            {
//        //                var dt = new DataTable();
//        //                adapter.Fill(dt);

//        //                foreach (DataRow row in dt.Rows)
//        //                {
//        //                    results.Add(new CoverageEntitySigma
//        //                    {
//        //                        NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
//        //                        SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
//        //                        SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
//        //                        SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),

//        //                        STIPO_DOCUMENTO_ASEGURADO = row.Table.Columns.Contains("STIPO_DOCUMENTO_ASEGURADO") ? row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString() : null,
//        //                        SNRO_DOCUMENTO_ASEGURADO = row.Table.Columns.Contains("SNRO_DOCUMENTO_ASEGURADO") ? row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString() : null,
//        //                        SNOMBRE_COMPLETO = row.Table.Columns.Contains("SNOMBRE_COMPLETO") ? row["SNOMBRE_COMPLETO"]?.ToString() : null,
//        //                        SNOMBRES_RAZONSOCIAL_ASEGURADO = row.Table.Columns.Contains("SNOMBRES_RAZONSOCIAL_ASEGURADO") ? row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString() : null,
//        //                        SAPELLIDO_PATERNO_ASEGURADO = row.Table.Columns.Contains("SAPELLIDO_PATERNO_ASEGURADO") ? row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString() : null,
//        //                        SAPELLIDO_MATERNO_ASEGURADO = row.Table.Columns.Contains("SAPELLIDO_MATERNO_ASEGURADO") ? row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString() : null,

//        //                        SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
//        //                        SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),

//        //                        NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
//        //                        NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
//        //                        SMONEDA = row["SMONEDA"]?.ToString(),

//        //                        SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
//        //                        SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
//        //                        SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),

//        //                        OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,

//        //                        CONTRATANTE = row.Table.Columns.Contains("CONTRATANTE") ? row["CONTRATANTE"]?.ToString(): (row.Table.Columns.Contains("CONTRATRANTE") ? row["CONTRATRANTE"]?.ToString() : null),
//        //                        CANAL = row.Table.Columns.Contains("CANAL") ? row["CANAL"]?.ToString() : null,
//        //                        ES_CANAL_VINCULADO = row.Table.Columns.Contains("ES_CANAL_VINCULADO") ? row["ES_CANAL_VINCULADO"]?.ToString() : null,
//        //                        ESTADO_POLIZA = row.Table.Columns.Contains("ESTADO_POLIZA") ? row["ESTADO_POLIZA"]?.ToString() : null,
//        //                        EVENTO_POLIZA = row.Table.Columns.Contains("EVENTO_POLIZA") ? row["EVENTO_POLIZA"]?.ToString() : null,
//        //                        ESTADO_UR = row.Table.Columns.Contains("ESTADO_UR") ? row["ESTADO_UR"]?.ToString() : null,

//        //                        INI_POLIZA = row.Table.Columns.Contains("INI_POLIZA") ? row["INI_POLIZA"]?.ToString() : null,
//        //                        FIN_POLIZA = row.Table.Columns.Contains("FIN_POLIZA") ? row["FIN_POLIZA"]?.ToString() : null
//        //                    });
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"[ERROR] GetCoverageCrecerPNContra: {ex.Message}\n{ex.StackTrace}");
//        //        throw new Exception($"Error en GetCoverageCrecerPNContra: {ex.Message}", ex);
//        //    }

//        //    return results;
//        //}


//        //public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJContra(FilterCoverageEntity _filter)
//        //{
//        //    var results = new List<CoverageEntitySigma>();

//        //    try
//        //    {
//        //        using (var conn = new OracleConnection(_oracleCoreDbConnection))
//        //        using (var cmd = new OracleCommand("PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index_Contra", conn))
//        //        {
//        //            cmd.CommandType = CommandType.StoredProcedure;

//        //            cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
//        //            cmd.Parameters.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//        //            cmd.Parameters.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//        //            cmd.Parameters.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);

//        //            await conn.OpenAsync();

//        //            using (var adapter = new OracleDataAdapter(cmd))
//        //            {
//        //                var dt = new DataTable();
//        //                adapter.Fill(dt);

//        //                foreach (DataRow row in dt.Rows)
//        //                {
//        //                    results.Add(new CoverageEntitySigma
//        //                    {
//        //                        NID_PRODUCTO = row["NID_PRODUCTO"]?.ToString(),
//        //                        SDESCRIPCION_PRODUCTO = row["SDESCRIPCION_PRODUCTO"]?.ToString(),
//        //                        SNUMERO_POLIZA = row["SNUMERO_POLIZA"]?.ToString(),
//        //                        SNUMERO_CREDITO = row["SNUMERO_CREDITO"]?.ToString(),

//        //                        STIPO_DOCUMENTO_ASEGURADO = row.Table.Columns.Contains("STIPO_DOCUMENTO_ASEGURADO") ? row["STIPO_DOCUMENTO_ASEGURADO"]?.ToString() : null,
//        //                        SNRO_DOCUMENTO_ASEGURADO = row.Table.Columns.Contains("SNRO_DOCUMENTO_ASEGURADO") ? row["SNRO_DOCUMENTO_ASEGURADO"]?.ToString() : null,
//        //                        SNOMBRE_COMPLETO = row.Table.Columns.Contains("SNOMBRE_COMPLETO") ? row["SNOMBRE_COMPLETO"]?.ToString() : null,
//        //                        SNOMBRES_RAZONSOCIAL_ASEGURADO = row.Table.Columns.Contains("SNOMBRES_RAZONSOCIAL_ASEGURADO") ? row["SNOMBRES_RAZONSOCIAL_ASEGURADO"]?.ToString() : null,
//        //                        SAPELLIDO_PATERNO_ASEGURADO = row.Table.Columns.Contains("SAPELLIDO_PATERNO_ASEGURADO") ? row["SAPELLIDO_PATERNO_ASEGURADO"]?.ToString() : null,
//        //                        SAPELLIDO_MATERNO_ASEGURADO = row.Table.Columns.Contains("SAPELLIDO_MATERNO_ASEGURADO") ? row["SAPELLIDO_MATERNO_ASEGURADO"]?.ToString() : null,

//        //                        SINICIO_CIBERTURA = row["SINICIO_CIBERTURA"]?.ToString(),
//        //                        SFIN_COBERTURA = row["SFIN_COBERTURA"]?.ToString(),

//        //                        NMONTO_ASEGURADO = row["NMONTO_ASEGURADO"] != DBNull.Value ? Convert.ToDecimal(row["NMONTO_ASEGURADO"]) : 0,
//        //                        NPRIMA = row["NPRIMA"] != DBNull.Value ? Convert.ToDecimal(row["NPRIMA"]) : 0,
//        //                        SMONEDA = row["SMONEDA"]?.ToString(),

//        //                        SFECHA_DESEMBOLSO_CREDITO = row["SFECHA_DESEMBOLSO_CREDITO"]?.ToString(),
//        //                        SFECHA_VENCIMIENTO_CREDITO = row["SFECHA_VENCIMIENTO_CREDITO"]?.ToString(),
//        //                        SFECHA_PROCESO = row["SFECHA_PROCESO"]?.ToString(),

//        //                        OPERATIONPK = row["OPERATIONPK"] != DBNull.Value ? Convert.ToDecimal(row["OPERATIONPK"]) : 0,

//        //                        CONTRATANTE = row.Table.Columns.Contains("CONTRATANTE") ? row["CONTRATANTE"]?.ToString(): (row.Table.Columns.Contains("CONTRATRANTE") ? row["CONTRATRANTE"]?.ToString() : null),
//        //                        CANAL = row.Table.Columns.Contains("CANAL") ? row["CANAL"]?.ToString() : null,
//        //                        ES_CANAL_VINCULADO = row.Table.Columns.Contains("ES_CANAL_VINCULADO") ? row["ES_CANAL_VINCULADO"]?.ToString() : null,
//        //                        ESTADO_POLIZA = row.Table.Columns.Contains("ESTADO_POLIZA") ? row["ESTADO_POLIZA"]?.ToString() : null,
//        //                        EVENTO_POLIZA = row.Table.Columns.Contains("EVENTO_POLIZA") ? row["EVENTO_POLIZA"]?.ToString() : null,
//        //                        ESTADO_UR = row.Table.Columns.Contains("ESTADO_UR") ? row["ESTADO_UR"]?.ToString() : null,

//        //                        INI_POLIZA = row.Table.Columns.Contains("INI_POLIZA") ? row["INI_POLIZA"]?.ToString() : null,
//        //                        FIN_POLIZA = row.Table.Columns.Contains("FIN_POLIZA") ? row["FIN_POLIZA"]?.ToString() : null
//        //                    });
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"[ERROR] GetCoverageCrecerPJContra: {ex.Message}\n{ex.StackTrace}");
//        //        throw new Exception($"Error en GetCoverageCrecerPJContra: {ex.Message}", ex);
//        //    }

//        //    return results;
//        //}

//        // Helpers -----------------------------

//        private static string GetStringSafe(IDataRecord r, string col, HashSet<string> cols)
//            => cols.Contains(col) && r[col] != DBNull.Value ? r[col]?.ToString() : null;

//        private static decimal GetDecimalSafe(IDataRecord r, string col, HashSet<string> cols)
//            => cols.Contains(col) && r[col] != DBNull.Value ? Convert.ToDecimal(r[col]) : 0m;

//        private static HashSet<string> GetColumnSet(OracleDataReader reader)
//        {
//            var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
//            var schema = reader.GetSchemaTable();
//            foreach (DataRow row in schema.Rows)
//                cols.Add(row["ColumnName"]?.ToString() ?? string.Empty);
//            return cols;
//        }

//        private CoverageEntitySigma MapCoverage(IDataRecord r, HashSet<string> cols, bool isCore)
//        {
//            // Campos base (comunes)
//            var entity = new CoverageEntitySigma
//            {
//                NID_PRODUCTO = GetStringSafe(r, "NID_PRODUCTO", cols),
//                SDESCRIPCION_PRODUCTO = GetStringSafe(r, "SDESCRIPCION_PRODUCTO", cols),
//                SNUMERO_POLIZA = GetStringSafe(r, "SNUMERO_POLIZA", cols),
//                SNUMERO_CREDITO = GetStringSafe(r, "SNUMERO_CREDITO", cols),

//                STIPO_DOCUMENTO_ASEGURADO = GetStringSafe(r, "STIPO_DOCUMENTO_ASEGURADO", cols),
//                SNRO_DOCUMENTO_ASEGURADO = GetStringSafe(r, "SNRO_DOCUMENTO_ASEGURADO", cols),
//                SNOMBRE_COMPLETO = GetStringSafe(r, "SNOMBRE_COMPLETO", cols),
//                SNOMBRES_RAZONSOCIAL_ASEGURADO = GetStringSafe(r, "SNOMBRES_RAZONSOCIAL_ASEGURADO", cols),
//                SAPELLIDO_PATERNO_ASEGURADO = GetStringSafe(r, "SAPELLIDO_PATERNO_ASEGURADO", cols),
//                SAPELLIDO_MATERNO_ASEGURADO = GetStringSafe(r, "SAPELLIDO_MATERNO_ASEGURADO", cols),

//                SINICIO_CIBERTURA = GetStringSafe(r, "SINICIO_CIBERTURA", cols),
//                SFIN_COBERTURA = GetStringSafe(r, "SFIN_COBERTURA", cols),

//                NMONTO_ASEGURADO = GetDecimalSafe(r, "NMONTO_ASEGURADO", cols),
//                NPRIMA = GetDecimalSafe(r, "NPRIMA", cols),
//                SMONEDA = GetStringSafe(r, "SMONEDA", cols),

//                SFECHA_DESEMBOLSO_CREDITO = GetStringSafe(r, "SFECHA_DESEMBOLSO_CREDITO", cols),
//                SFECHA_VENCIMIENTO_CREDITO = GetStringSafe(r, "SFECHA_VENCIMIENTO_CREDITO", cols),
//                SFECHA_PROCESO = GetStringSafe(r, "SFECHA_PROCESO", cols),

//                OPERATIONPK = GetDecimalSafe(r, "OPERATIONPK", cols),
//                INI_POLIZA = GetStringSafe(r, "INI_POLIZA", cols),
//                FIN_POLIZA = GetStringSafe(r, "FIN_POLIZA", cols),
//            };

//            if (isCore)
//            {
//                // Core trae info adicional. Maneja el typo "CONTRATRANTE".
//                entity.CONTRATANTE = GetStringSafe(r, "CONTRATANTE", cols) ?? GetStringSafe(r, "CONTRATRANTE", cols);
//                entity.CANAL = GetStringSafe(r, "CANAL", cols);
//                entity.ES_CANAL_VINCULADO = GetStringSafe(r, "ES_CANAL_VINCULADO", cols);
//                entity.ESTADO_POLIZA = GetStringSafe(r, "ESTADO_POLIZA", cols);
//                entity.EVENTO_POLIZA = GetStringSafe(r, "EVENTO_POLIZA", cols);
//                entity.ESTADO_UR = GetStringSafe(r, "ESTADO_UR", cols);
//                entity.ROL = GetStringSafe(r, "ROL", cols);

//            }
//            else
//            {
//                // Sigma no los retorna: deja string.Empty como en tu implementación
//                entity.CONTRATANTE = string.Empty;
//                entity.CANAL = string.Empty;
//                entity.ES_CANAL_VINCULADO = string.Empty;
//                entity.ESTADO_POLIZA = string.Empty;
//                entity.EVENTO_POLIZA = string.Empty;
//                entity.ESTADO_UR = string.Empty;
//                entity.ROL = string.Empty;

//            }

//            return entity;
//        }

//        private static readonly int[] TransientOracleErrors = { 12570, 12571, 12545, 12170, 3113, 1013 };
//        private static readonly Random _rng = new();

//        private async Task<T> WithOracleRetry<T>(Func<Task<T>> action, int maxRetries = 3)
//        {
//            int attempt = 0;
//            while (true)
//            {
//                try { return await action(); }
//                catch (OracleException ox) when (TransientOracleErrors.Contains(ox.Number) && ++attempt <= maxRetries)
//                {
//                    var baseDelay = (int)Math.Pow(2, attempt - 1) * 250; // 250,500,1000,2000,4000
//                    var jitter = _rng.Next(0, 150);                      // +0..150ms
//                    Console.WriteLine($"[RETRY] ORA-{ox.Number} attempt {attempt}/{maxRetries} delay {baseDelay + jitter}ms");
//                    await Task.Delay(baseDelay + jitter);
//                }
//            }
//        }


//        private async Task<List<CoverageEntitySigma>> SafeExecute(Func<Task<List<CoverageEntitySigma>>> run)
//        {
//            try { return await WithOracleRetry(run); }
//            catch (OracleException ox) when (TransientOracleErrors.Contains(ox.Number))
//            {
//                Console.WriteLine($"[WARN] Transient error exhausted. Returning empty. ORA-{ox.Number}");
//                return new List<CoverageEntitySigma>();
//            }
//        }



//        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, HashSet<string>> _schemaCache
//    = new System.Collections.Concurrent.ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

//        private static HashSet<string> GetColumnSetCached(string key, OracleDataReader reader)
//        {
//            if (_schemaCache.TryGetValue(key, out var cols))
//                return cols;

//            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
//            var schema = reader.GetSchemaTable();
//            foreach (DataRow row in schema.Rows)
//                set.Add(row["ColumnName"]?.ToString() ?? string.Empty);

//            _schemaCache[key] = set;
//            return set;
//        }


//        private static readonly SemaphoreSlim _dbGate = new(12); // 6–8 es sano

//        private async Task<T> Throttled<T>(Func<Task<T>> run)
//        {
//            await _dbGate.WaitAsync();
//            try { return await run(); }
//            finally { _dbGate.Release(); }
//        }


//        //private async Task<List<CoverageEntitySigma>> ExecuteCoverageProc(
//        //    string connString,
//        //    string procName,
//        //    Action<OracleParameterCollection> fillParams,
//        //    bool isCore,
//        //    string cursorParamName // "C_COLUMNS" en Sigma, "C_TABLE" en Core
//        //)
//        //{
//        //    return await WithOracleRetry(async () =>
//        //    {
//        //        var results = new List<CoverageEntitySigma>();

//        //        using (var conn = new OracleConnection(connString))
//        //        using (var cmd = new OracleCommand(procName, conn))
//        //        {
//        //            cmd.CommandType = CommandType.StoredProcedure;
//        //            cmd.BindByName = true;
//        //            cmd.CommandTimeout = 180;

//        //            // OUT primero
//        //            var pCur = cmd.Parameters.Add(cursorParamName, OracleDbType.RefCursor);
//        //            pCur.Direction = ParameterDirection.Output;

//        //            // IN después
//        //            fillParams(cmd.Parameters);

//        //            var sw = System.Diagnostics.Stopwatch.StartNew();
//        //            await conn.OpenAsync();
//        //            var tOpen = sw.ElapsedMilliseconds;

//        //            sw.Restart();
//        //            using (var reader = (OracleDataReader)await cmd.ExecuteReaderAsync(
//        //                CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection))
//        //            {
//        //                var tExecute = sw.ElapsedMilliseconds;
//        //                try
//        //                {
//        //                    if (!reader.IsClosed && reader.HasRows)
//        //                    {
//        //                        var rowSize = Math.Max(reader.RowSize, 1024);
//        //                        var targetRows = 200; // 150–300
//        //                        var fetchBytes = rowSize * targetRows;
//        //                        reader.FetchSize = Math.Min(fetchBytes, 2 * 1024 * 1024); // 2MB tope
//        //                    }
//        //                }
//        //                catch { /* ignore */ }

//        //                sw.Restart();
//        //                var cols = GetColumnSetCached(procName, reader);

//        //                while (await reader.ReadAsync())
//        //                    results.Add(MapCoverage(reader, cols, isCore));

//        //                var tMap = sw.ElapsedMilliseconds;
//        //                Console.WriteLine($"[PROC] {procName} TIMINGS(ms) -> Open:{tOpen} Execute:{tExecute} Map:{tMap} Rows:{results.Count}");
//        //            }
//        //        }

//        //        return results;
//        //    });
//        //}


//        private async Task<List<CoverageEntitySigma>> ExecuteCoverageProc(
//    string connString,
//    string procName,
//    Action<OracleParameterCollection> fillParams,
//    bool isCore,
//    string cursorParamName
//)
//        {
//            return await WithOracleRetry(async () =>
//            {
//                var results = new List<CoverageEntitySigma>();

//                using (var conn = new OracleConnection(connString))
//                {
//                    var sw = System.Diagnostics.Stopwatch.StartNew();
//                    await conn.OpenAsync();

//                    // PING rápido para validar que la conexión está viva
//                    try
//                    {
//                        using var pingCmd = new OracleCommand("SELECT 1 FROM DUAL", conn);
//                        pingCmd.CommandTimeout = 5;
//                        await pingCmd.ExecuteScalarAsync();
//                    }
//                    catch (OracleException)
//                    {
//                        // Conexión muerta, cerrar y dejar que el retry abra una nueva
//                        conn.Close();
//                        throw;
//                    }

//                    var tOpen = sw.ElapsedMilliseconds;

//                    using (var cmd = new OracleCommand(procName, conn))
//                    {
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.BindByName = true;
//                        cmd.CommandTimeout = 60;

//                        // OUT primero
//                        var pCur = cmd.Parameters.Add(cursorParamName, OracleDbType.RefCursor);
//                        pCur.Direction = ParameterDirection.Output;

//                        // IN después
//                        fillParams(cmd.Parameters);

//                        sw.Restart();
//                        using (var reader = (OracleDataReader)await cmd.ExecuteReaderAsync(
//                            CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection))
//                        {
//                            var tExecute = sw.ElapsedMilliseconds;

//                            try
//                            {
//                                if (!reader.IsClosed && reader.HasRows)
//                                {
//                                    var rowSize = Math.Max(reader.RowSize, 1024);
//                                    var targetRows = 200;
//                                    var fetchBytes = rowSize * targetRows;
//                                    reader.FetchSize = Math.Min(fetchBytes, 2 * 1024 * 1024);
//                                }
//                            }
//                            catch { /* ignore */ }

//                            sw.Restart();
//                            var cols = GetColumnSetCached(procName, reader);

//                            while (await reader.ReadAsync())
//                                results.Add(MapCoverage(reader, cols, isCore));

//                            var tMap = sw.ElapsedMilliseconds;
//                            Console.WriteLine($"[PROC] {procName} TIMINGS(ms) -> Open:{tOpen} Execute:{tExecute} Map:{tMap} Rows:{results.Count}");
//                        }
//                    }
//                }

//                return results;
//            });
//        }

//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity _filter)
//        {
//            try
//            {
//                return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                    _oracleDbConnection,
//                    "PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD",
//                    ps =>
//                    {
//                        ps.Add("STIPO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//                        ps.Add("SNRO_DOCUMENTO", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//                        ps.Add("SNOMBRE_COMPLETO", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);
//                        ps.Add("NINDICADOR_PAGINADO", OracleDbType.Int32, 0, ParameterDirection.Input);
//                        ps.Add("PNPAGESIZE", OracleDbType.Int32, 0, ParameterDirection.Input);
//                        ps.Add("PNPAGENUM", OracleDbType.Int32, 0, ParameterDirection.Input);
//                    },
//                    isCore: false,
//                    cursorParamName: "C_COLUMNS"
//                )));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[ERROR] GetCoverage: {ex.Message}\n{ex.StackTrace}");
//                throw new Exception($"Error en GetCoverage: {ex.Message}", ex);
//            }
//        }
//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity _filter)
//        {
//            try
//            {
//                return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                    _oracleCoreDbConnection,
//                    "PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Prueba",
//                    ps =>
//                    {
//                        ps.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//                        ps.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//                        ps.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);
//                    },
//                    isCore: true,
//                    cursorParamName: "C_TABLE"
//                )));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[ERROR] GetCoverageCrecerPN: {ex.Message}\n{ex.StackTrace}");
//                throw new Exception($"Error en GetCoverageCrecerPN: {ex.Message}", ex);
//            }
//        }
//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity _filter)
//        {
//            try
//            {
//                return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                    _oracleCoreDbConnection,
//                    "PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index",
//                    ps =>
//                    {
//                        ps.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//                        ps.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//                        ps.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);
//                    },
//                    isCore: true,
//                    cursorParamName: "C_TABLE"
//                )));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[ERROR] GetCoverageCrecerPJ: {ex.Message}\n{ex.StackTrace}");
//                throw new Exception($"Error en GetCoverageCrecerPJ: {ex.Message}", ex);
//            }
//        }

//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPNContra(FilterCoverageEntity _filter)
//        {
//            try
//            {
//                return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                    _oracleCoreDbConnection,
//                    "PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Contra",
//                    ps =>
//                    {
//                        ps.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//                        ps.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//                        ps.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);
//                    },
//                    isCore: true,
//                    cursorParamName: "C_TABLE"
//                )));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[ERROR] GetCoverageCrecerPNContra: {ex.Message}\n{ex.StackTrace}");
//                throw new Exception($"Error en GetCoverageCrecerPNContra: {ex.Message}", ex);
//            }
//        }

//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJContra(FilterCoverageEntity _filter)
//        {
//            try
//            {
//                return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                    _oracleCoreDbConnection,
//                    "PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index_Contra",
//                    ps =>
//                    {
//                        ps.Add("tipoDocumento", OracleDbType.Varchar2, _filter.sdocumenttype, ParameterDirection.Input);
//                        ps.Add("nroDocumento", OracleDbType.Varchar2, _filter.sdocumentnumber, ParameterDirection.Input);
//                        ps.Add("nombreRazonSocial", OracleDbType.Varchar2, _filter.sname, ParameterDirection.Input);
//                    },
//                    isCore: true,
//                    cursorParamName: "C_TABLE"
//            )));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[ERROR] GetCoverageCrecerPJContra: {ex.Message}\n{ex.StackTrace}");
//                throw new Exception($"Error en GetCoverageCrecerPJContra: {ex.Message}", ex);
//            }
//        }




//    }
//}

using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Entities.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Concurrent;
using System.Data;

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

        // Helpers -----------------------------

        private static string GetStringSafe(IDataRecord r, string col, HashSet<string> cols)
            => cols.Contains(col) && r[col] != DBNull.Value ? r[col]?.ToString() : null;

        private static decimal GetDecimalSafe(IDataRecord r, string col, HashSet<string> cols)
            => cols.Contains(col) && r[col] != DBNull.Value ? Convert.ToDecimal(r[col]) : 0m;

        private CoverageEntitySigma MapCoverage(IDataRecord r, HashSet<string> cols, bool isCore)
        {
            var entity = new CoverageEntitySigma
            {
                NID_PRODUCTO = GetStringSafe(r, "NID_PRODUCTO", cols),
                SDESCRIPCION_PRODUCTO = GetStringSafe(r, "SDESCRIPCION_PRODUCTO", cols),
                SNUMERO_POLIZA = GetStringSafe(r, "SNUMERO_POLIZA", cols),
                SNUMERO_CREDITO = GetStringSafe(r, "SNUMERO_CREDITO", cols),

                STIPO_DOCUMENTO_ASEGURADO = GetStringSafe(r, "STIPO_DOCUMENTO_ASEGURADO", cols),
                SNRO_DOCUMENTO_ASEGURADO = GetStringSafe(r, "SNRO_DOCUMENTO_ASEGURADO", cols),
                SNOMBRE_COMPLETO = GetStringSafe(r, "SNOMBRE_COMPLETO", cols),
                SNOMBRES_RAZONSOCIAL_ASEGURADO = GetStringSafe(r, "SNOMBRES_RAZONSOCIAL_ASEGURADO", cols),
                SAPELLIDO_PATERNO_ASEGURADO = GetStringSafe(r, "SAPELLIDO_PATERNO_ASEGURADO", cols),
                SAPELLIDO_MATERNO_ASEGURADO = GetStringSafe(r, "SAPELLIDO_MATERNO_ASEGURADO", cols),

                SINICIO_CIBERTURA = GetStringSafe(r, "SINICIO_CIBERTURA", cols),
                SFIN_COBERTURA = GetStringSafe(r, "SFIN_COBERTURA", cols),

                NMONTO_ASEGURADO = GetDecimalSafe(r, "NMONTO_ASEGURADO", cols),
                NPRIMA = GetDecimalSafe(r, "NPRIMA", cols),
                SMONEDA = GetStringSafe(r, "SMONEDA", cols),

                SFECHA_DESEMBOLSO_CREDITO = GetStringSafe(r, "SFECHA_DESEMBOLSO_CREDITO", cols),
                SFECHA_VENCIMIENTO_CREDITO = GetStringSafe(r, "SFECHA_VENCIMIENTO_CREDITO", cols),
                SFECHA_PROCESO = GetStringSafe(r, "SFECHA_PROCESO", cols),

                OPERATIONPK = GetDecimalSafe(r, "OPERATIONPK", cols),
                INI_POLIZA = GetStringSafe(r, "INI_POLIZA", cols),
                FIN_POLIZA = GetStringSafe(r, "FIN_POLIZA", cols),
            };

            if (isCore)
            {
                entity.CONTRATANTE = GetStringSafe(r, "CONTRATANTE", cols) ?? GetStringSafe(r, "CONTRATRANTE", cols);
                entity.CANAL = GetStringSafe(r, "CANAL", cols);
                entity.ES_CANAL_VINCULADO = GetStringSafe(r, "ES_CANAL_VINCULADO", cols);
                entity.ESTADO_POLIZA = GetStringSafe(r, "ESTADO_POLIZA", cols);
                entity.EVENTO_POLIZA = GetStringSafe(r, "EVENTO_POLIZA", cols);
                entity.ESTADO_UR = GetStringSafe(r, "ESTADO_UR", cols);
                entity.ROL = GetStringSafe(r, "ROL", cols);
            }
            else
            {
                entity.CONTRATANTE = string.Empty;
                entity.CANAL = string.Empty;
                entity.ES_CANAL_VINCULADO = string.Empty;
                entity.ESTADO_POLIZA = string.Empty;
                entity.EVENTO_POLIZA = string.Empty;
                entity.ESTADO_UR = string.Empty;
                entity.ROL = string.Empty;
            }

            return entity;
        }

        // Retry + throttle --------------------

        private static readonly int[] TransientOracleErrors = { 12570, 12571, 12545, 12170, 3113, 1013 };
        private static readonly Random _rng = new();
        private static readonly SemaphoreSlim _dbGate = new(12);

        private async Task<T> Throttled<T>(Func<Task<T>> run, CancellationToken ct)
        {
            await _dbGate.WaitAsync(ct);
            try { return await run(); }
            finally { _dbGate.Release(); }
        }

        private async Task<T> WithOracleRetry<T>(Func<Task<T>> action, CancellationToken ct, int maxRetries = 3)
        {
            int attempt = 0;
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                try { return await action(); }
                catch (OracleException ox) when (TransientOracleErrors.Contains(ox.Number) && ++attempt <= maxRetries)
                {
                    var baseDelay = (int)Math.Pow(2, attempt - 1) * 250;
                    var jitter = _rng.Next(0, 150);
                    Console.WriteLine($"[RETRY] ORA-{ox.Number} attempt {attempt}/{maxRetries} delay {baseDelay + jitter}ms");
                    await Task.Delay(baseDelay + jitter, ct);
                }
            }
        }

        private async Task<List<CoverageEntitySigma>> SafeExecute(Func<Task<List<CoverageEntitySigma>>> run, CancellationToken ct)
        {
            try { return await WithOracleRetry(run, ct); }
            catch (OracleException ox) when (TransientOracleErrors.Contains(ox.Number))
            {
                Console.WriteLine($"[WARN] Transient error exhausted. Returning empty. ORA-{ox.Number}");
                return new List<CoverageEntitySigma>();
            }
        }

        // Schema cache ------------------------

        private static readonly ConcurrentDictionary<string, HashSet<string>> _schemaCache =
            new(StringComparer.OrdinalIgnoreCase);

        private static HashSet<string> GetColumnSetCached(string key, OracleDataReader reader)
        {
            if (_schemaCache.TryGetValue(key, out var cols))
                return cols;

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var schema = reader.GetSchemaTable();
            foreach (DataRow row in schema.Rows)
                set.Add(row["ColumnName"]?.ToString() ?? string.Empty);

            _schemaCache[key] = set;
            return set;
        }

        // Core executor -----------------------

        private async Task<List<CoverageEntitySigma>> ExecuteCoverageProc(
            string connString,
            string procName,
            Action<OracleParameterCollection> fillParams,
            bool isCore,
            string cursorParamName,
            CancellationToken ct
        )
        {
            return await WithOracleRetry(async () =>
            {
                var results = new List<CoverageEntitySigma>();

                using var conn = new OracleConnection(connString);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                await conn.OpenAsync(ct);
                var tOpen = sw.ElapsedMilliseconds;

                using var cmd = new OracleCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.CommandTimeout = 60; // ojo: esto es a nivel OracleCommand

                // OUT primero
                var pCur = cmd.Parameters.Add(cursorParamName, OracleDbType.RefCursor);
                pCur.Direction = ParameterDirection.Output;

                // IN después
                fillParams(cmd.Parameters);

                sw.Restart();
                using var reader = (OracleDataReader)await cmd.ExecuteReaderAsync(
                    CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection, ct);

                var tExecute = sw.ElapsedMilliseconds;

                try
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        var rowSize = Math.Max(reader.RowSize, 1024);
                        var targetRows = 200;
                        var fetchBytes = rowSize * targetRows;
                        reader.FetchSize = Math.Min(fetchBytes, 2 * 1024 * 1024);
                    }
                }
                catch { }

                sw.Restart();
                var cols = GetColumnSetCached(procName, reader);

                while (await reader.ReadAsync(ct))
                    results.Add(MapCoverage(reader, cols, isCore));

                var tMap = sw.ElapsedMilliseconds;
                Console.WriteLine($"[PROC] {procName} TIMINGS(ms) -> Open:{tOpen} Execute:{tExecute} Map:{tMap} Rows:{results.Count}");

                return results;
            }, ct);
        }

        // Public API --------------------------

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity f, CancellationToken ct = default)
        {
            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleDbConnection,
                "PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD",
                ps =>
                {
                    ps.Add("STIPO_DOCUMENTO", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("SNRO_DOCUMENTO", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("SNOMBRE_COMPLETO", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                    ps.Add("NINDICADOR_PAGINADO", OracleDbType.Int32, 0, ParameterDirection.Input);
                    ps.Add("PNPAGESIZE", OracleDbType.Int32, 0, ParameterDirection.Input);
                    ps.Add("PNPAGENUM", OracleDbType.Int32, 0, ParameterDirection.Input);
                },
                isCore: false,
                cursorParamName: "C_COLUMNS",
                ct: ct
            ), ct), ct);
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity f, CancellationToken ct = default)
        {
            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleCoreDbConnection,
                "PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Prueba",
                ps =>
                {
                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_TABLE",
                ct: ct
            ), ct), ct);
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity f, CancellationToken ct = default)
        {
            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleCoreDbConnection,
                "PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index",
                ps =>
                {
                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_TABLE",
                ct: ct
            ), ct), ct);
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPNContra(FilterCoverageEntity f, CancellationToken ct = default)
        {
            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleCoreDbConnection,
                "PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Contra",
                ps =>
                {
                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_TABLE",
                ct: ct
            ), ct), ct);
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJContra(FilterCoverageEntity f, CancellationToken ct = default)
        {
            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleCoreDbConnection,
                "PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index_Contra",
                ps =>
                {
                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_TABLE",
                ct: ct
            ), ct), ct);
        }
    }
}
