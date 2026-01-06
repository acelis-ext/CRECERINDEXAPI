using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace CrecerIndex.Services
{


    public class OracleKeepAliveHostedService : BackgroundService
    {
        private readonly IConfiguration _cfg;

        public OracleKeepAliveHostedService(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var csSigma = _cfg.GetConnectionString("OracleDB");
            var csCore = _cfg.GetConnectionString("OracleCoreDB");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Ping(csSigma, stoppingToken);
                await Ping(csCore, stoppingToken);

                // Mantiene vivas conexiones idle que infraestructura/NAT suele matar
                await Task.Delay(TimeSpan.FromSeconds(90), stoppingToken);
            }
        }

        private static async Task Ping(string cs, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cs)) return;

            try
            {
                using var conn = new OracleConnection(cs);
                await conn.OpenAsync(ct);

                using var cmd = new OracleCommand("SELECT 1 FROM DUAL", conn);
                cmd.CommandTimeout = 3;
                await cmd.ExecuteScalarAsync(ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KEEPALIVE] Oracle ping failed: {ex.Message}");
            }
        }
    }

}
