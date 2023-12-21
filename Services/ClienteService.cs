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
    public class ClienteService : IClienteService
    {
        private readonly IConfiguration configuration;
        private readonly string _connectionString;

        public ClienteService(IConfiguration _configuration)
        {
            configuration = _configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }

        public async Task<Cliente> AuthenticacionSession(string correoElectronico, string contrasena)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var parameters = new
                {
                    p_correo_electronico = correoElectronico,
                    p_contrasena = contrasena
                };

                var count = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT validar_inicio_sesion(:p_correo_electronico, :p_contrasena) FROM DUAL",
                    parameters
                );

                if (count > 0)
                {
                    var authenticatedCliente = await connection.QueryFirstOrDefaultAsync<Cliente>(
                        "SELECT * FROM Cliente WHERE Correo_electronico = :p_correo_electronico",
                        parameters
                    );

                    return authenticatedCliente;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<bool> CreateCliente(Cliente cliente)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var parameters = new
                {
                    p_Cedula = cliente.Cedula,
                    p_Nombre = cliente.Nombre,
                    p_Apellido = cliente.Apellido,
                    p_Edad = cliente.Edad,
                    p_Correo_electronico = cliente.Correo_electronico,
                    p_Telefono = cliente.Telefono,
                    p_Contrasena = cliente.Contrasena
                };

                await connection.ExecuteAsync("create_cliente", parameters, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        public async Task<List<Cliente>> GetAllCliente()
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                OracleParameter resultParameter = new OracleParameter
                {
                    ParameterName = "result",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.ReturnValue
                };

                using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_clientes; END;", connection))
                {
                    command.Parameters.Add(resultParameter);
                    command.CommandType = CommandType.Text;

                    await command.ExecuteNonQueryAsync();

                    var result = (OracleRefCursor)resultParameter.Value;

                    List<Cliente> clientes = new List<Cliente>();
                    using (OracleDataReader reader = result.GetDataReader())
                    {
                        while (reader.Read())
                        {
                            var cliente = new Cliente
                            {
                                ID_Cliente = reader.GetInt32(reader.GetOrdinal("ID_CLIENTE")),
                                Cedula = reader.GetString(reader.GetOrdinal("CEDULA")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                Apellido = reader.GetString(reader.GetOrdinal("APELLIDO")),
                                Edad = reader.GetInt32(reader.GetOrdinal("EDAD")),
                                Correo_electronico = reader.GetString(reader.GetOrdinal("CORREO_ELECTRONICO")),
                                Telefono = reader.GetString(reader.GetOrdinal("TELEFONO")),
                                Contrasena = reader.GetString(reader.GetOrdinal("CONTRASENA"))
                            };

                            clientes.Add(cliente);
                        }
                    }

                    return clientes;
                }
            }
        }

        public async Task<bool> UpdateCliente(Cliente cliente)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var parameters = new
                {
                    p_ID_Cliente = cliente.ID_Cliente,
                    p_Cedula = cliente.Cedula,
                    p_Nombre = cliente.Nombre,
                    p_Apellido = cliente.Apellido,
                    p_Edad = cliente.Edad,
                    p_Correo_electronico = cliente.Correo_electronico,
                    p_Telefono = cliente.Telefono,
                    p_Contrasena = cliente.Contrasena
                };

                await connection.ExecuteAsync("edit_cliente", parameters, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        public async Task<bool> DeleteCliente(int idCliente)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var parameters = new { p_ID_Cliente = idCliente };
                await connection.ExecuteAsync("delete_cliente", parameters, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        public async Task<Cliente> GetById(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Cliente", id, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("result", dbType: DbType.Object, direction: ParameterDirection.ReturnValue);

                    await connection.ExecuteAsync("Cliente_Paquete.get_cliente_by_id", parameters, commandType: CommandType.StoredProcedure);

                    var result = parameters.Get<Cliente>("result");

                    Console.WriteLine($"Resultado de la consulta para Cliente ID {id}: {result}");

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting Cliente by ID: {ex.Message}");
                return null;
            }
        }

        public async Task<string> ObtenerPromedioEdadClientes()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    OracleParameter resultParameter = new OracleParameter
                    {
                        ParameterName = "result",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.ReturnValue
                    };

                    using (OracleCommand command = new OracleCommand("BEGIN :result := CURSOR_promedio_edad_clientes; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        // Obtener el resultado como string
                        string promedioEdadString = (resultParameter.Value != DBNull.Value) ? resultParameter.Value.ToString() : "0";

                        return promedioEdadString;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el promedio de edad de los clientes: {ex.Message}");
                return "Error"; // O manejar el error según sea necesario
            }
        }

    }
}
