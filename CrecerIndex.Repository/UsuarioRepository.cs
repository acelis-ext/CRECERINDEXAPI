using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Entities.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;


namespace CrecerIndex.Repository
{
    public class UsuarioRepository :IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CRECERSEGUROS");
        }

        public Usuario GetByCredentials(string usuarioInput, string passwordInput)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                byte[] encryptedPassword = null;
                Usuario user = null;

                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM usuario WHERE usuario = @usuario", conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", usuarioInput);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            encryptedPassword = (byte[])reader["password"];

                            user = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["idusuario"]),
                                Usuarioname = reader["usuario"].ToString(),
                                //Correo = reader["correo"].ToString(),
                                Nombres = reader["nombres"].ToString(),
                                ApellidoPaterno = reader["apellidopaterno"].ToString(),
                                //ApellidoMaterno = reader["apellidomaterno"].ToString()
                            };
                        }
                    } // 👈 el DataReader se cierra aquí
                }

                if (user != null && encryptedPassword != null)
                {
                    string decryptedPassword = DecryptPassword(conn, encryptedPassword);

                    if (decryptedPassword == passwordInput)
                    {
                        user.Password = decryptedPassword;
                        return user;
                    }
                }
            }

            return null;
        }


        private string DecryptPassword(SqlConnection conn, byte[] encryptedPassword)
        {
            using var cmd = new SqlCommand("USP_Decrypt_Password", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Parámetro de entrada
            cmd.Parameters.Add(new SqlParameter("@clave_encriptada", SqlDbType.VarBinary, -1)
            {
                Value = encryptedPassword
            });

            // Parámetro de salida
            var outputParam = new SqlParameter("@clave_desencriptada", SqlDbType.NVarChar, 400)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            return outputParam.Value?.ToString();
        }

    }
}
