//using CrecerIndex.Abstraction.Interfaces.IRepository;
//using CrecerIndex.Entities.Models;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Data;
//using Microsoft.Data.SqlClient;


//namespace CrecerIndex.Repository
//{
//    public class UsuarioRepository :IUsuarioRepository
//    {
//        private readonly string _connectionString;

//        public UsuarioRepository(IConfiguration configuration)
//        {
//            _connectionString = configuration.GetConnectionString("CRECERSEGUROS");
//        }

//        public Usuario GetByCredentials(string usuarioInput, string passwordInput)
//        {
//            using (SqlConnection conn = new SqlConnection(_connectionString))
//            {
//                conn.Open();

//                byte[] encryptedPassword = null;
//                Usuario user = null;

//                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM usuario WHERE usuario = @usuario", conn))
//                {
//                    cmd.Parameters.AddWithValue("@usuario", usuarioInput);

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            encryptedPassword = (byte[])reader["password"];

//                            user = new Usuario
//                            {
//                                IdUsuario = Convert.ToInt32(reader["idusuario"]),
//                                Usuarioname = reader["usuario"].ToString(),
//                                //Correo = reader["correo"].ToString(),
//                                Nombres = reader["nombres"].ToString(),
//                                ApellidoPaterno = reader["apellidopaterno"].ToString(),
//                                //ApellidoMaterno = reader["apellidomaterno"].ToString()
//                            };
//                        }
//                    } // 👈 el DataReader se cierra aquí
//                }

//                if (user != null && encryptedPassword != null)
//                {
//                    string decryptedPassword = DecryptPassword(conn, encryptedPassword);

//                    if (decryptedPassword == passwordInput)
//                    {
//                        user.Password = decryptedPassword;
//                        return user;
//                    }
//                }
//            }

//            return null;
//        }


//        private string DecryptPassword(SqlConnection conn, byte[] encryptedPassword)
//        {
//            using var cmd = new SqlCommand("USP_Decrypt_Password", conn);
//            cmd.CommandType = CommandType.StoredProcedure;

//            // Parámetro de entrada
//            cmd.Parameters.Add(new SqlParameter("@clave_encriptada", SqlDbType.VarBinary, -1)
//            {
//                Value = encryptedPassword
//            });

//            // Parámetro de salida
//            var outputParam = new SqlParameter("@clave_desencriptada", SqlDbType.NVarChar, 400)
//            {
//                Direction = ParameterDirection.Output
//            };
//            cmd.Parameters.Add(outputParam);

//            cmd.ExecuteNonQuery();

//            return outputParam.Value?.ToString();
//        }

//    }
//}

using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CrecerIndex.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(IConfiguration configuration, ILogger<UsuarioRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("CRECERSEGUROS");
            _logger = logger;

            // Log de la connection string (sin password)
            var safeConnStr = _connectionString?.Contains("Password=") == true
                ? _connectionString.Substring(0, _connectionString.IndexOf("Password=")) + "Password=***"
                : _connectionString;
            _logger.LogInformation("ConnectionString configurada: {ConnStr}", safeConnStr);
        }

        public Usuario GetByCredentials(string usuarioInput, string passwordInput)
        {
            _logger.LogInformation("=== GetByCredentials INICIO ===");
            _logger.LogInformation("Buscando usuario: {Usuario}", usuarioInput);

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                _logger.LogError("❌ ConnectionString está vacía o nula!");
                throw new InvalidOperationException("ConnectionString 'CRECERSEGUROS' no está configurada");
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    _logger.LogInformation("Abriendo conexión a SQL Server...");
                    _logger.LogInformation("Server: {Server}", conn.DataSource);
                    _logger.LogInformation("Database: {DB}", conn.Database);

                    conn.Open();
                    _logger.LogInformation("✅ Conexión abierta exitosamente");

                    byte[] encryptedPassword = null;
                    Usuario user = null;

                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM usuario WHERE usuario = @usuario", conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuarioInput);
                        cmd.CommandTimeout = 30; // 30 segundos timeout

                        _logger.LogInformation("Ejecutando query SELECT...");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _logger.LogInformation("✅ Usuario encontrado en BD");

                                encryptedPassword = (byte[])reader["password"];
                                user = new Usuario
                                {
                                    IdUsuario = Convert.ToInt32(reader["idusuario"]),
                                    Usuarioname = reader["usuario"].ToString(),
                                    Nombres = reader["nombres"].ToString(),
                                    ApellidoPaterno = reader["apellidopaterno"].ToString(),
                                };

                                _logger.LogInformation("IdUsuario: {Id}, Nombres: {Nombres}",
                                    user.IdUsuario, user.Nombres);
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ Usuario NO encontrado en BD: {Usuario}", usuarioInput);
                            }
                        }
                    }

                    if (user != null && encryptedPassword != null)
                    {
                        _logger.LogInformation("Desencriptando password...");

                        try
                        {
                            string decryptedPassword = DecryptPassword(conn, encryptedPassword);

                            if (decryptedPassword == passwordInput)
                            {
                                _logger.LogInformation("✅ Password válido");
                                user.Password = decryptedPassword;
                                return user;
                            }
                            else
                            {
                                _logger.LogWarning("❌ Password NO coincide");
                            }
                        }
                        catch (Exception exDecrypt)
                        {
                            _logger.LogError(exDecrypt, "❌ Error desencriptando password: {Message}", exDecrypt.Message);
                            throw;
                        }
                    }
                }

                _logger.LogInformation("Retornando null (usuario no encontrado o password incorrecto)");
                return null;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "❌ SQL Exception: {Message}", sqlEx.Message);
                _logger.LogError("SQL Error Number: {Number}", sqlEx.Number);
                _logger.LogError("SQL State: {State}", sqlEx.State);

                foreach (SqlError error in sqlEx.Errors)
                {
                    _logger.LogError("SQL Error Detail - Number: {Num}, Message: {Msg}, Server: {Server}",
                        error.Number, error.Message, error.Server);
                }

                throw new Exception($"Error de base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general en GetByCredentials: {Message}", ex.Message);
                _logger.LogError("StackTrace: {Stack}", ex.StackTrace);
                throw;
            }
        }

        private string DecryptPassword(SqlConnection conn, byte[] encryptedPassword)
        {
            _logger.LogInformation("Ejecutando SP USP_Decrypt_Password...");

            using var cmd = new SqlCommand("USP_Decrypt_Password", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 30;

            cmd.Parameters.Add(new SqlParameter("@clave_encriptada", SqlDbType.VarBinary, -1)
            {
                Value = encryptedPassword
            });

            var outputParam = new SqlParameter("@clave_desencriptada", SqlDbType.NVarChar, 400)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            var result = outputParam.Value?.ToString();
            _logger.LogInformation("✅ SP ejecutado correctamente");

            return result;
        }
    }
}