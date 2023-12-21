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
    public class PiscinaService : IPiscinaService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public PiscinaService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }

        public async Task<bool> CreatePiscina(Piscina piscina)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_Nombre", piscina.Nombre);
                    parameters.Add("p_Direccion", piscina.Direccion);
                    parameters.Add("p_Horario_apertura", piscina.Horario_apertura, DbType.DateTime);
                    parameters.Add("p_Horario_cierre", piscina.Horario_cierre, DbType.DateTime);
                    parameters.Add("p_Capacidad_maxima", piscina.Capacidad_maxima);
                    parameters.Add("p_Costo", piscina.Costo);

                    await connection.ExecuteAsync("create_piscina", parameters, commandType: CommandType.StoredProcedure);

                    Console.WriteLine("Piscina creada exitosamente.");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Piscina: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePiscina(int idPiscina)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new
                    {
                        p_ID_Piscina = idPiscina
                    };

                    await connection.ExecuteAsync("delete_piscina", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting Piscina: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Piscina>> GetAllPiscina()
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_piscinas; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        var result = (OracleRefCursor)resultParameter.Value;
                        using (OracleDataReader reader = result.GetDataReader())
                        {
                            List<Piscina> piscinas = new List<Piscina>();

                            while (await reader.ReadAsync())
                            {
                                var piscina = new Piscina
                                {
                                    ID_Piscina = reader.GetInt32(reader.GetOrdinal("ID_PISCINA")),
                                    Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                    Direccion = reader.GetString(reader.GetOrdinal("DIRECCION")),
                                    Horario_apertura = reader.GetDateTime(reader.GetOrdinal("HORARIO_APERTURA")),
                                    Horario_cierre = reader.GetDateTime(reader.GetOrdinal("HORARIO_CIERRE")),
                                    Capacidad_maxima = reader.GetInt32(reader.GetOrdinal("CAPACIDAD_MAXIMA")),
                                    Costo = reader.GetDecimal(reader.GetOrdinal("COSTO"))
                                };

                                piscinas.Add(piscina);
                            }

                            return piscinas;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all piscinas: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdatePiscina(Piscina piscina)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Piscina", piscina.ID_Piscina, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("p_Nombre", piscina.Nombre);
                    parameters.Add("p_Direccion", piscina.Direccion);
                    parameters.Add("p_Horario_apertura", piscina.Horario_apertura, DbType.DateTime);
                    parameters.Add("p_Horario_cierre", piscina.Horario_cierre, DbType.DateTime);
                    parameters.Add("p_Capacidad_maxima", piscina.Capacidad_maxima);
                    parameters.Add("p_Costo", piscina.Costo);

                    await connection.ExecuteAsync("edit_piscina", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Piscina: {ex.Message}");
                return false;
            }
        }
        public async Task<string> ListarPiscinasCapacidadMaxima()
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := CURSOR_listar_piscinas_capacidad_maxima; END;", connection))
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
                Console.WriteLine($"Error al listar piscinas con capacidad máxima: {ex.Message}");
                return string.Empty; // o manejar el error según sea necesario
            }
        }
    }
}
