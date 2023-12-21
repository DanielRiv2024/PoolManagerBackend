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
    public class RegistroService : IRegistroCliente
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public RegistroService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }

        public async Task<bool> Create(RegistroCliente registroCliente)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Cliente", registroCliente.ID_Cliente);
                    parameters.Add("p_Tipo_Actividad", registroCliente.Tipo_Actividad);
                    parameters.Add("p_Fecha_hora_inicio", registroCliente.Fecha_hora_inicio, DbType.DateTime);
                    parameters.Add("p_Fecha_hora_fin", registroCliente.Fecha_hora_fin, DbType.DateTime);

                    await connection.ExecuteAsync("create_actividad_cliente", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Actividad Cliente: {ex.Message}");
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

                    var parameters = new
                    {
                        p_ID_Actividad = id
                    };

                    await connection.ExecuteAsync("delete_actividad_cliente", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting Actividad Cliente: {ex.Message}");
                return false;
            }
        }

        public async Task<RegistroCliente> Edit(RegistroCliente registroCliente)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Actividad", registroCliente.ID_Actividad);
                    parameters.Add("p_ID_Cliente", registroCliente.ID_Cliente);
                    parameters.Add("p_Tipo_Actividad", registroCliente.Tipo_Actividad);
                    parameters.Add("p_Fecha_hora_inicio", registroCliente.Fecha_hora_inicio, DbType.DateTime);
                    parameters.Add("p_Fecha_hora_fin", registroCliente.Fecha_hora_fin, DbType.DateTime);

                    await connection.ExecuteAsync("edit_actividad_cliente", parameters, commandType: CommandType.StoredProcedure);

                    return registroCliente;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing Actividad Cliente: {ex.Message}");
                return null;
            }
        }

        public async Task<List<RegistroCliente>> GetAll()
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_actividades_cliente; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        var result = (OracleRefCursor)resultParameter.Value;
                        using (OracleDataReader reader = result.GetDataReader())
                        {
                            List<RegistroCliente> actividadesClientes = new List<RegistroCliente>();

                            while (await reader.ReadAsync())
                            {
                                var actividadCliente = new RegistroCliente
                                {
                                    ID_Actividad = reader.GetInt32(reader.GetOrdinal("ID_ACTIVIDAD")),
                                    ID_Cliente = reader.GetInt32(reader.GetOrdinal("ID_CLIENTE")),
                                    Tipo_Actividad = reader.GetString(reader.GetOrdinal("TIPO_ACTIVIDAD")),
                                    Fecha_hora_inicio = reader.GetDateTime(reader.GetOrdinal("FECHA_HORA_INICIO")),
                                    Fecha_hora_fin = reader.GetDateTime(reader.GetOrdinal("FECHA_HORA_FIN"))
                                };

                                actividadesClientes.Add(actividadCliente);
                            }

                            return actividadesClientes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all Actividades Cliente: {ex.Message}");
                return null;
            }
        }
    }
}
