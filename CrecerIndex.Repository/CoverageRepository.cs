

//using CrecerIndex.Abstraction.Interfaces.IRepository;
//using CrecerIndex.Entities.Models;
//using Microsoft.Extensions.Configuration;
//using Oracle.ManagedDataAccess.Client;
//using System.Collections.Concurrent;
//using System.Data;

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

//        // Helpers -----------------------------

//        private static string GetStringSafe(IDataRecord r, string col, HashSet<string> cols)
//            => cols.Contains(col) && r[col] != DBNull.Value ? r[col]?.ToString() : null;

//        private static decimal GetDecimalSafe(IDataRecord r, string col, HashSet<string> cols)
//            => cols.Contains(col) && r[col] != DBNull.Value ? Convert.ToDecimal(r[col]) : 0m;

//        private CoverageEntitySigma MapCoverage(IDataRecord r, HashSet<string> cols, bool isCore)
//        {
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

//        // Retry + throttle --------------------

//        private static readonly int[] TransientOracleErrors = { 12570, 12571, 12545, 12170, 3113, 1013 };
//        private static readonly Random _rng = new();
//        private static readonly SemaphoreSlim _dbGate = new(12);

//        private async Task<T> Throttled<T>(Func<Task<T>> run, CancellationToken ct)
//        {
//            await _dbGate.WaitAsync(ct);
//            try { return await run(); }
//            finally { _dbGate.Release(); }
//        }

//        private async Task<T> WithOracleRetry<T>(Func<Task<T>> action, CancellationToken ct, int maxRetries = 3)
//        {
//            int attempt = 0;
//            while (true)
//            {
//                ct.ThrowIfCancellationRequested();
//                try { return await action(); }
//                catch (OracleException ox) when (TransientOracleErrors.Contains(ox.Number) && ++attempt <= maxRetries)
//                {
//                    var baseDelay = (int)Math.Pow(2, attempt - 1) * 250;
//                    var jitter = _rng.Next(0, 150);
//                    Console.WriteLine($"[RETRY] ORA-{ox.Number} attempt {attempt}/{maxRetries} delay {baseDelay + jitter}ms");
//                    await Task.Delay(baseDelay + jitter, ct);
//                }
//            }
//        }

//        private async Task<List<CoverageEntitySigma>> SafeExecute(Func<Task<List<CoverageEntitySigma>>> run, CancellationToken ct)
//        {
//            try { return await WithOracleRetry(run, ct); }
//            catch (OracleException ox) when (TransientOracleErrors.Contains(ox.Number))
//            {
//                Console.WriteLine($"[WARN] Transient error exhausted. Returning empty. ORA-{ox.Number}");
//                return new List<CoverageEntitySigma>();
//            }
//        }

//        // Schema cache ------------------------

//        private static readonly ConcurrentDictionary<string, HashSet<string>> _schemaCache =
//            new(StringComparer.OrdinalIgnoreCase);

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

//        // Core executor -----------------------

//        private async Task<List<CoverageEntitySigma>> ExecuteCoverageProc(
//            string connString,
//            string procName,
//            Action<OracleParameterCollection> fillParams,
//            bool isCore,
//            string cursorParamName,
//            CancellationToken ct
//        )
//        {
//            return await WithOracleRetry(async () =>
//            {
//                var results = new List<CoverageEntitySigma>();

//                using var conn = new OracleConnection(connString);

//                var sw = System.Diagnostics.Stopwatch.StartNew();
//                await conn.OpenAsync(ct);
//                var tOpen = sw.ElapsedMilliseconds;

//                using var cmd = new OracleCommand(procName, conn);
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.BindByName = true;
//                cmd.CommandTimeout = 60; // ojo: esto es a nivel OracleCommand

//                // OUT primero
//                var pCur = cmd.Parameters.Add(cursorParamName, OracleDbType.RefCursor);
//                pCur.Direction = ParameterDirection.Output;

//                // IN después
//                fillParams(cmd.Parameters);

//                sw.Restart();
//                using var reader = (OracleDataReader)await cmd.ExecuteReaderAsync(
//                    CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection, ct);

//                var tExecute = sw.ElapsedMilliseconds;

//                try
//                {
//                    if (!reader.IsClosed && reader.HasRows)
//                    {
//                        var rowSize = Math.Max(reader.RowSize, 1024);
//                        var targetRows = 200;
//                        var fetchBytes = rowSize * targetRows;
//                        reader.FetchSize = Math.Min(fetchBytes, 2 * 1024 * 1024);
//                    }
//                }
//                catch { }

//                sw.Restart();
//                var cols = GetColumnSetCached(procName, reader);

//                while (await reader.ReadAsync(ct))
//                    results.Add(MapCoverage(reader, cols, isCore));

//                var tMap = sw.ElapsedMilliseconds;
//                Console.WriteLine($"[PROC] {procName} TIMINGS(ms) -> Open:{tOpen} Execute:{tExecute} Map:{tMap} Rows:{results.Count}");

//                return results;
//            }, ct);
//        }

//        // Public API --------------------------

//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity f, CancellationToken ct = default)
//        {
//            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                _oracleDbConnection,
//                "SIGMA.PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD",
//                ps =>
//                {
//                    ps.Add("STIPO_DOCUMENTO", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
//                    ps.Add("SNRO_DOCUMENTO", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
//                    ps.Add("SNOMBRE_COMPLETO", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
//                    ps.Add("NINDICADOR_PAGINADO", OracleDbType.Int32, 0, ParameterDirection.Input);
//                    ps.Add("PNPAGESIZE", OracleDbType.Int32, 0, ParameterDirection.Input);
//                    ps.Add("PNPAGENUM", OracleDbType.Int32, 0, ParameterDirection.Input);
//                },
//                isCore: false,
//                cursorParamName: "C_COLUMNS",
//                ct: ct
//            ), ct), ct);
//        }

//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity f, CancellationToken ct = default)
//        {
//            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                _oracleCoreDbConnection,
//                "CRECER_SEGURO.PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Prueba",
//                ps =>
//                {
//                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
//                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
//                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
//                },
//                isCore: true,
//                cursorParamName: "C_TABLE",
//                ct: ct
//            ), ct), ct);
//        }

//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity f, CancellationToken ct = default)
//        {
//            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                _oracleCoreDbConnection,
//                "CRECER_SEGURO.PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index",
//                ps =>
//                {
//                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
//                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
//                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
//                },
//                isCore: true,
//                cursorParamName: "C_TABLE",
//                ct: ct
//            ), ct), ct);
//        }

//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPNContra(FilterCoverageEntity f, CancellationToken ct = default)
//        {
//            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                _oracleCoreDbConnection,
//                "CRECER_SEGURO.PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Contra",
//                ps =>
//                {
//                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
//                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
//                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
//                },
//                isCore: true,
//                cursorParamName: "C_TABLE",
//                ct: ct
//            ), ct), ct);
//        }

//        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJContra(FilterCoverageEntity f, CancellationToken ct = default)
//        {
//            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
//                _oracleCoreDbConnection,
//                "CRECER_SEGURO.PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index_Contra",
//                ps =>
//                {
//                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
//                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
//                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
//                },
//                isCore: true,
//                cursorParamName: "C_TABLE",
//                ct: ct
//            ), ct), ct);
//        }
//    }
//}

using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Concurrent;
using System.Data;

namespace CrecerIndex.Repository
{
    public class CoverageRepository : ICoverageRepository
    {
        private readonly string _oracleDbConnection;
        private readonly string _oracleCoreDbConnection;
        private readonly ILogger<CoverageRepository> _logger;

        public CoverageRepository(IConfiguration configuration, ILogger<CoverageRepository> logger)
        {
            _oracleDbConnection = configuration.GetConnectionString("OracleDB");
            _oracleCoreDbConnection = configuration.GetConnectionString("OracleCoreDB");
            _logger = logger;

            // Log de las connection strings (sin password)
            LogConnectionString("OracleDB (SIGMA)", _oracleDbConnection);
            LogConnectionString("OracleCoreDB (CORE)", _oracleCoreDbConnection);
        }

        private void LogConnectionString(string name, string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
            {
                _logger.LogError("❌ ConnectionString '{Name}' está VACÍA o NULL!", name);
                return;
            }

            // Ocultar password para el log
            var safeConnStr = connStr;
            if (connStr.Contains("Password=", StringComparison.OrdinalIgnoreCase))
            {
                var start = connStr.IndexOf("Password=", StringComparison.OrdinalIgnoreCase);
                var end = connStr.IndexOf(";", start);
                if (end == -1) end = connStr.Length;
                safeConnStr = connStr.Substring(0, start) + "Password=***" + connStr.Substring(end);
            }
            _logger.LogInformation("✅ ConnectionString '{Name}' configurada: {ConnStr}", name, safeConnStr);
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

        private static readonly int[] TransientOracleErrors = { 12570, 12571, 12545, 12170, 3113, 1013, 12154, 12514, 12541, 12543 };
        private static readonly Random _rng = new();
        private static readonly SemaphoreSlim _dbGate = new(12);

        private async Task<T> Throttled<T>(Func<Task<T>> run, CancellationToken ct)
        {
            await _dbGate.WaitAsync(ct);
            try { return await run(); }
            finally { _dbGate.Release(); }
        }

        private async Task<T> WithOracleRetry<T>(Func<Task<T>> action, string procName, CancellationToken ct, int maxRetries = 3)
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
                    _logger.LogWarning("[RETRY] {Proc} ORA-{Number} attempt {Attempt}/{MaxRetries} delay {Delay}ms - {Message}",
                        procName, ox.Number, attempt, maxRetries, baseDelay + jitter, ox.Message);
                    await Task.Delay(baseDelay + jitter, ct);
                }
            }
        }

        private async Task<List<CoverageEntitySigma>> SafeExecute(Func<Task<List<CoverageEntitySigma>>> run, string procName, CancellationToken ct)
        {
            try
            {
                return await WithOracleRetry(run, procName, ct);
            }
            catch (OracleException ox) when (TransientOracleErrors.Contains(ox.Number))
            {
                _logger.LogError(ox, "❌ [WARN] {Proc} Transient error exhausted. Returning empty. ORA-{Number}: {Message}",
                    procName, ox.Number, ox.Message);
                return new List<CoverageEntitySigma>();
            }
            catch (OracleException ox)
            {
                _logger.LogError(ox, "❌ [ERROR] {Proc} Oracle error ORA-{Number}: {Message}",
                    procName, ox.Number, ox.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [ERROR] {Proc} Error general: {Message}", procName, ex.Message);
                throw;
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
            var dbType = isCore ? "CORE" : "SIGMA";
            _logger.LogInformation("=== [{DbType}] Ejecutando {Proc} ===", dbType, procName);

            // Validar connection string
            if (string.IsNullOrWhiteSpace(connString))
            {
                _logger.LogError("❌ [{DbType}] ConnectionString está VACÍA para {Proc}!", dbType, procName);
                throw new InvalidOperationException($"ConnectionString para {dbType} no está configurada");
            }

            return await WithOracleRetry(async () =>
            {
                var results = new List<CoverageEntitySigma>();

                using var conn = new OracleConnection(connString);

                var sw = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    _logger.LogInformation("[{DbType}] Abriendo conexión Oracle...", dbType);
                    await conn.OpenAsync(ct);
                    var tOpen = sw.ElapsedMilliseconds;
                    _logger.LogInformation("✅ [{DbType}] Conexión abierta en {Time}ms - Server: {Server}",
                        dbType, tOpen, conn.DataSource);

                    using var cmd = new OracleCommand(procName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandTimeout = 60;

                    // OUT primero
                    var pCur = cmd.Parameters.Add(cursorParamName, OracleDbType.RefCursor);
                    pCur.Direction = ParameterDirection.Output;

                    // IN después
                    fillParams(cmd.Parameters);

                    // Log de parámetros
                    foreach (OracleParameter p in cmd.Parameters)
                    {
                        if (p.Direction == ParameterDirection.Input)
                        {
                            _logger.LogInformation("[{DbType}] Param {Name} = '{Value}'",
                                dbType, p.ParameterName, p.Value?.ToString() ?? "NULL");
                        }
                    }

                    sw.Restart();
                    _logger.LogInformation("[{DbType}] Ejecutando SP...", dbType);

                    using var reader = (OracleDataReader)await cmd.ExecuteReaderAsync(
                        CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection, ct);

                    var tExecute = sw.ElapsedMilliseconds;
                    _logger.LogInformation("✅ [{DbType}] SP ejecutado en {Time}ms", dbType, tExecute);

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
                    _logger.LogInformation("✅ [{DbType}] {Proc} completado - Open:{TOpen}ms Execute:{TExec}ms Map:{TMap}ms Rows:{Rows}",
                        dbType, procName, tOpen, tExecute, tMap, results.Count);

                    return results;
                }
                catch (OracleException ox)
                {
                    _logger.LogError(ox, "❌ [{DbType}] Oracle Error en {Proc} - ORA-{Number}: {Message}",
                        dbType, procName, ox.Number, ox.Message);
                    _logger.LogError("[{DbType}] Oracle Error Detail - Source: {Source}, DataSource: {DS}",
                        dbType, ox.Source, conn.DataSource);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ [{DbType}] Error general en {Proc}: {Message}",
                        dbType, procName, ex.Message);
                    throw;
                }
            }, procName, ct);
        }

        // Public API --------------------------

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverage(FilterCoverageEntity f, CancellationToken ct = default)
        {
            _logger.LogInformation("=== GetCoverage (SIGMA) - DocType:{DocType} DocNum:{DocNum} Name:{Name} ===",
                f.sdocumenttype, f.sdocumentnumber, f.sname);

            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleDbConnection,
                "SIGMA.PKG_SIGMA_INFO_ASEGURABILIDAD_INDEX.SP_GET_INFO_ASEGURABILIDAD",
                ps =>
                {
                    ps.Add("STIPO_DOCUMENTO", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("SNRO_DOCUMENTO", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("SNOMBRE_COMPLETO", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                    ps.Add("NINDICADOR_PAGINADO", OracleDbType.Int32, 0, ParameterDirection.Input);
                    ps.Add("PNPAGESIZE", OracleDbType.Int32, 0, ParameterDirection.Input);
                    ps.Add("PNPAGENUM", OracleDbType.Int32, 0, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_COLUMNS",
                ct: ct
            ), "SIGMA.SP_GET_INFO_ASEGURABILIDAD", ct), ct);
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPN(FilterCoverageEntity f, CancellationToken ct = default)
        {
            _logger.LogInformation("=== GetCoverageCrecerPN (CORE) - DocType:{DocType} DocNum:{DocNum} Name:{Name} ===",
                f.sdocumenttype, f.sdocumentnumber, f.sname);

            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleCoreDbConnection,
                "CRECER_SEGURO.PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Prueba",
                ps =>
                {
                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_TABLE",
                ct: ct
            ), "CORE.open_ListaPN_index_Prueba", ct), ct);
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJ(FilterCoverageEntity f, CancellationToken ct = default)
        {
            _logger.LogInformation("=== GetCoverageCrecerPJ (CORE) - DocType:{DocType} DocNum:{DocNum} Name:{Name} ===",
                f.sdocumenttype, f.sdocumentnumber, f.sname);

            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleCoreDbConnection,
                "CRECER_SEGURO.PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index",
                ps =>
                {
                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_TABLE",
                ct: ct
            ), "CORE.open_ListaPJ_index", ct), ct);
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPNContra(FilterCoverageEntity f, CancellationToken ct = default)
        {
            _logger.LogInformation("=== GetCoverageCrecerPNContra (CORE) - DocType:{DocType} DocNum:{DocNum} Name:{Name} ===",
                f.sdocumenttype, f.sdocumentnumber, f.sname);

            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleCoreDbConnection,
                "CRECER_SEGURO.PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPN_index_Contra",
                ps =>
                {
                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_TABLE",
                ct: ct
            ), "CORE.open_ListaPN_index_Contra", ct), ct);
        }

        public async Task<IEnumerable<CoverageEntitySigma>> GetCoverageCrecerPJContra(FilterCoverageEntity f, CancellationToken ct = default)
        {
            _logger.LogInformation("=== GetCoverageCrecerPJContra (CORE) - DocType:{DocType} DocNum:{DocNum} Name:{Name} ===",
                f.sdocumenttype, f.sdocumentnumber, f.sname);

            return await Throttled(() => SafeExecute(() => ExecuteCoverageProc(
                _oracleCoreDbConnection,
                "CRECER_SEGURO.PKG_VISTA_ASEGURABILIDAD_INDEX.open_ListaPJ_index_Contra",
                ps =>
                {
                    ps.Add("tipoDocumento", OracleDbType.Varchar2, f.sdocumenttype, ParameterDirection.Input);
                    ps.Add("nroDocumento", OracleDbType.Varchar2, f.sdocumentnumber, ParameterDirection.Input);
                    ps.Add("nombreRazonSocial", OracleDbType.Varchar2, f.sname, ParameterDirection.Input);
                },
                isCore: true,
                cursorParamName: "C_TABLE",
                ct: ct
            ), "CORE.open_ListaPJ_index_Contra", ct), ct);
        }
    }
}