using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PoolManagerBackend.Services
{
    public class TarjetaUsuarioService : ITarjetaUsuario
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public TarjetaUsuarioService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }

        public async Task<bool> Create(TarjetaUsuario tarjetaUsuario)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Cliente", tarjetaUsuario.ID_Cliente);
                    parameters.Add("p_Numero_de_tarjeta", tarjetaUsuario.Numero_de_tarjeta);
                    parameters.Add("p_Nombre_en_la_tarjeta", tarjetaUsuario.Nombre_en_la_tarjeta);
                    parameters.Add("p_Fecha_de_expiracion", tarjetaUsuario.Fecha_de_expiracion, DbType.DateTime);
                    parameters.Add("p_CVV", tarjetaUsuario.CVV);

                    await connection.ExecuteAsync("create_tarjeta_usuario", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating TarjetaUsuario: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new { p_ID_Tarjeta = id };
                    await connection.ExecuteAsync("delete_tarjeta_usuario", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting TarjetaUsuario: {ex.Message}");
                return false;
            }
        }

        public async Task<TarjetaUsuario> Edit(TarjetaUsuario tarjetaUsuario)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Tarjeta", tarjetaUsuario.ID_Tarjeta, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("p_ID_Cliente", tarjetaUsuario.ID_Cliente, DbType.Int32);
                    parameters.Add("p_Numero_de_tarjeta", tarjetaUsuario.Numero_de_tarjeta);
                    parameters.Add("p_Nombre_en_la_tarjeta", tarjetaUsuario.Nombre_en_la_tarjeta);
                    parameters.Add("p_Fecha_de_expiracion", tarjetaUsuario.Fecha_de_expiracion, DbType.DateTime);
                    parameters.Add("p_CVV", tarjetaUsuario.CVV);

                    connection.Execute("edit_tarjeta_usuario", parameters, commandType: CommandType.StoredProcedure);

                    return tarjetaUsuario;
                }
            }
            catch (Exception ex)
            {
            
                Console.WriteLine($"Error al editar TarjetaUsuario: {ex.Message}");
                return null; 
            }
        }


        public async Task<List<TarjetaUsuario>> GetAll()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    OracleParameter resultParameter = new OracleParameter
                    {
                        ParameterName = "result",
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.ReturnValue
                    };

                    using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_tarjetas_usuario; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        var result = (OracleRefCursor)resultParameter.Value;

                        List<TarjetaUsuario> tarjetasUsuario = new List<TarjetaUsuario>();

                        using (OracleDataReader reader = result.GetDataReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                var tarjeta = new TarjetaUsuario
                                {
                                    ID_Tarjeta = reader.GetInt32(reader.GetOrdinal("ID_TARJETA")),
                                    ID_Cliente = reader.GetInt32(reader.GetOrdinal("ID_CLIENTE")),
                                    Numero_de_tarjeta = reader.GetString(reader.GetOrdinal("NUMERO_DE_TARJETA")),
                                    Nombre_en_la_tarjeta = reader.GetString(reader.GetOrdinal("NOMBRE_EN_LA_TARJETA")),
                                    Fecha_de_expiracion = reader.GetDateTime(reader.GetOrdinal("FECHA_DE_EXPIRACION")),
                                    CVV = reader.GetString(reader.GetOrdinal("CVV"))
                                };

                                tarjetasUsuario.Add(tarjeta);
                            }
                        }

                        return tarjetasUsuario;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all TarjetasUsuario: {ex.Message}");
                return null;
            }
        }

        public async Task<string> VerificarTarjetas()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    OracleParameter resultParameter = new OracleParameter
                    {
                        ParameterName = "result",
                        OracleDbType = OracleDbType.Varchar2,
                        Size = 4000,
                        Direction = ParameterDirection.ReturnValue
                    };

                    using (OracleCommand command = new OracleCommand("BEGIN :result := CURSOR_verificar_cursor_tarjetas; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        // Obtener el resultado como cadena de texto
                        string resultado = (resultParameter.Value != DBNull.Value) ? resultParameter.Value.ToString() : string.Empty;

                        return resultado;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar tarjetas: {ex.Message}");
                return string.Empty; // o manejar el error según sea necesario
            }
        }

    }
}
