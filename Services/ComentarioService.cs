using Dapper;
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
    public class ComentarioService : IComentario
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ComentarioService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }

        public async Task<bool> Create(Comentario comentario)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new
                    {
                        p_ID_Cliente = comentario.ID_Cliente,
                        p_Detalle = comentario.Detalle,
                        p_Fecha_comentario = comentario.Fecha_comentario
                    };

                    await connection.ExecuteAsync("create_comentario", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Comentario: {ex.Message}");
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
                        p_ID_Comentario = id
                    };

                    await connection.ExecuteAsync("delete_comentario", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting Comentario: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Comentario>> GetAll()
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_comentarios; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        var result = (OracleRefCursor)resultParameter.Value;
                        using (OracleDataReader reader = result.GetDataReader())
                        {
                            List<Comentario> comentarios = new List<Comentario>();

                            while (await reader.ReadAsync())
                            {
                                var comentario = new Comentario
                                {
                                    ID_Comentario = reader.GetInt32(reader.GetOrdinal("ID_Comentario")),
                                    ID_Cliente = reader.GetInt32(reader.GetOrdinal("ID_Cliente")),
                                    Detalle = reader.GetString(reader.GetOrdinal("Detalle")),
                                    Fecha_comentario = reader.GetDateTime(reader.GetOrdinal("Fecha_comentario"))
                                };

                                comentarios.Add(comentario);
                            }

                            return comentarios;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all comentarios: {ex.Message}");
                return null;
            }
        }

        public async Task<Comentario> Edit(Comentario comentario)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new
                    {
                        p_ID_Comentario = comentario.ID_Comentario,
                        p_ID_Cliente = comentario.ID_Cliente,
                        p_Detalle = comentario.Detalle,
                        p_Fecha_comentario = comentario.Fecha_comentario
                    };

                    await connection.ExecuteAsync("edit_comentario", parameters, commandType: CommandType.StoredProcedure);

                    return comentario;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing Comentario: {ex.Message}");
                return null;
            }
        }

        public async Task<string> MostrarComentariosCliente(int idCliente)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    OracleParameter idClienteParameter = new OracleParameter
                    {
                        ParameterName = "p_id_cliente",
                        OracleDbType = OracleDbType.Decimal,
                        Value = idCliente,
                        Direction = ParameterDirection.Input
                    };

                    OracleParameter resultParameter = new OracleParameter
                    {
                        ParameterName = "result",
                        OracleDbType = OracleDbType.Varchar2,
                        Size = 4000,
                        Direction = ParameterDirection.ReturnValue
                    };

                    using (OracleCommand command = new OracleCommand("BEGIN :result := CURSOR_mostrar_comentarios_cliente(:p_id_cliente); END;", connection))
                    {
                        command.Parameters.Add(idClienteParameter);
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        // Obtener el resultado como cadena de texto
                        string resultado = (resultParameter.Value != DBNull.Value) ? resultParameter.Value.ToString() : string.Empty;

                        // Imprimir el resultado en la consola
                        Console.WriteLine(resultado);

                        return resultado;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mostrar comentarios del cliente: {ex.Message}");
                return string.Empty; // o manejar el error según sea necesario
            }
        }



      
    }
}

