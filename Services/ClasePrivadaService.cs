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
    public class ClasePrivadaService : IClasesPrivadasService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ClasePrivadaService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }
        public async Task<bool> Create(ClasePrivada clasePrivada)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new
                    {
                        p_Nombre_de_la_clase = clasePrivada.Nombre_de_la_clase,
                        p_Instructor = clasePrivada.Instructor,
                        p_Cedula_instructor = clasePrivada.Cedula_instructor,
                        p_Duracion = clasePrivada.Duracion,
                        p_Precio = clasePrivada.Precio
                    };

                    await connection.ExecuteAsync("create_clase_privada", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating ClasePrivada: {ex.Message}");
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
                        p_ID_Clase = id
                    };

                    await connection.ExecuteAsync("delete_clase_privada", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting ClasePrivada: {ex.Message}");
                return false;
            }
        }

        public async Task<ClasePrivada> Edit(ClasePrivada clasePrivada)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Clase", clasePrivada.ID_Clase, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("p_Nombre_de_la_clase", clasePrivada.Nombre_de_la_clase);
                    parameters.Add("p_Instructor", clasePrivada.Instructor);
                    parameters.Add("p_Cedula_instructor", clasePrivada.Cedula_instructor);
                    parameters.Add("p_Duracion", clasePrivada.Duracion);
                    parameters.Add("p_Precio", clasePrivada.Precio);

                    await connection.ExecuteAsync("edit_clase_privada", parameters, commandType: CommandType.StoredProcedure);

                    return clasePrivada;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating ClasePrivada: {ex.Message}");
                return null;
            }
        }



        public async Task<List<ClasePrivada>> GetAll()
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

                using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_clases_privadas; END;", connection))
                {
                    command.Parameters.Add(resultParameter);
                    command.CommandType = CommandType.Text; 

                    await command.ExecuteNonQueryAsync();

                   
                    var result = (OracleRefCursor)resultParameter.Value;

                   
                    List<ClasePrivada> clasesPrivadas = new List<ClasePrivada>();
                    using (OracleDataReader reader = result.GetDataReader())
                    {
                        while (reader.Read())
                        {
                            var clasePrivada = new ClasePrivada
                            {
                                ID_Clase = reader.GetInt32(reader.GetOrdinal("ID_CLASE")),
                                Nombre_de_la_clase = reader.GetString(reader.GetOrdinal("NOMBRE_DE_LA_CLASE")),
                                Instructor = reader.GetString(reader.GetOrdinal("INSTRUCTOR")),
                                Cedula_instructor = reader.GetString(reader.GetOrdinal("CEDULA_INSTRUCTOR")),
                                Duracion = reader.GetInt32(reader.GetOrdinal("DURACION")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("PRECIO"))
                            };

                            clasesPrivadas.Add(clasePrivada);
                        }
                    }

                    return clasesPrivadas;
                }
            }
        }

        public async Task<ClasePrivada> GetById(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();

                   
                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Clase", id, DbType.Int32, ParameterDirection.Input);

                   
                    parameters.Add("result", dbType: DbType.Object, direction: ParameterDirection.ReturnValue);

                 
                    await connection.ExecuteAsync("BEGIN :result := ClasesPrivadas_Paquete.get_clase_privada_by_id(:p_ID_Clase); END;", parameters, commandType: CommandType.Text);

                   
                    var result = parameters.Get<OracleRefCursor>("result");

                   
                    using (OracleDataReader reader = result.GetDataReader())
                    {
                        if (reader.Read())
                        {
                            var clasePrivada = new ClasePrivada
                            {
                                ID_Clase = reader.GetInt32(reader.GetOrdinal("ID_CLASE")),
                                Nombre_de_la_clase = reader.GetString(reader.GetOrdinal("NOMBRE_DE_LA_CLASE")),
                                Instructor = reader.GetString(reader.GetOrdinal("INSTRUCTOR")),
                                Cedula_instructor = reader.GetString(reader.GetOrdinal("CEDULA_INSTRUCTOR")),
                                Duracion = reader.GetInt32(reader.GetOrdinal("DURACION")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("PRECIO"))
                            };

                            return clasePrivada;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error getting ClasePrivada by ID: {ex.Message}");
               
                return null;
            }
        }
        public async Task<string> GetTotalDuracionClasesPrivadas()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    OracleParameter resultParameter = new OracleParameter
                    {
                        ParameterName = "result",
                        OracleDbType = OracleDbType.Decimal, // Cambiado a Decimal
                        Direction = ParameterDirection.ReturnValue
                    };

                    using (OracleCommand command = new OracleCommand("BEGIN :result := CURSOR_duracion_total_clases_privadas; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        // Convertir el resultado a string
                        string duracionTotal = resultParameter.Value != DBNull.Value ? resultParameter.Value.ToString() : "0";

                        return duracionTotal;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la duración total de las clases privadas: {ex.Message}");
                return "-1"; // o maneja el error según sea necesario
            }
        }

        public async Task<string> ListarClasesPrivadasPrecio()
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := CURSOR_listar_clases_privadas_precio; END;", connection))
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
                Console.WriteLine($"Error al listar clases privadas con precio: {ex.Message}");
                return string.Empty; // o manejar el error según sea necesario
            }
        }


    }
}
