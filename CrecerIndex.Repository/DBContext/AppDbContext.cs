using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Repository.Context
{
    public class AppDbContext
    {
        private readonly string _connectionStringCRECERSEGUROS;
        private readonly string _connectionOracleDB;
        private readonly string _connectionOracleCoreDB;

        public AppDbContext(IConfiguration configuracion)
        {
            _connectionStringCRECERSEGUROS = configuracion.GetConnectionString("CRECERSEGUROS");
            _connectionOracleDB = configuracion.GetConnectionString("OracleDB");
            _connectionOracleCoreDB = configuracion.GetConnectionString("OracleCoreDB");
        }

        public string GetConnectionCRECERSEGUROS()
        {
            return _connectionStringCRECERSEGUROS;
        }

        public string GetConnectionOracleDB()
        {
            return _connectionOracleDB;
        }

        public string GetConnectionOracleCoreDB()
        {
            return _connectionOracleCoreDB;
        }
    }

}
