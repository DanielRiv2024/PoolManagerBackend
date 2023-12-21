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
    public class EvaluacionService : IEvalucionInstructor
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public EvaluacionService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }

        public async Task<List<EvaluacionInstructor>> GetAll()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var resultParameter = new OracleParameter
                    {
                        ParameterName = "result",
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.ReturnValue
                    };

                    using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_evaluaciones_instructor; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        var result = (OracleRefCursor)resultParameter.Value;
                        using (OracleDataReader reader = result.GetDataReader())
                        {
                            List<EvaluacionInstructor> evaluaciones = new List<EvaluacionInstructor>();

                            while (await reader.ReadAsync())
                            {
                                var evaluacion = new EvaluacionInstructor
                                {
                                    ID_Evaluacion = reader.GetInt32(reader.GetOrdinal("ID_EVALUACION")),
                                    ID_Cliente = reader.GetInt32(reader.GetOrdinal("ID_CLIENTE")),
                                    ID_Instructor = reader.GetInt32(reader.GetOrdinal("ID_INSTRUCTOR")),
                                    Calificacion = reader.GetDecimal(reader.GetOrdinal("CALIFICACION")),
                                    Comentario = reader.GetString(reader.GetOrdinal("COMENTARIO")),
                                    Fecha_evaluacion = reader.GetDateTime(reader.GetOrdinal("FECHA_EVALUACION"))
                                };

                                evaluaciones.Add(evaluacion);
                            }

                            return evaluaciones;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all EvaluacionesInstructor: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Create(EvaluacionInstructor evaluacionInstructor)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Cliente", evaluacionInstructor.ID_Cliente);
                    parameters.Add("p_ID_Instructor", evaluacionInstructor.ID_Instructor);
                    parameters.Add("p_Calificacion", evaluacionInstructor.Calificacion);
                    parameters.Add("p_Comentario", evaluacionInstructor.Comentario);
                    parameters.Add("p_Fecha_evaluacion", evaluacionInstructor.Fecha_evaluacion, DbType.DateTime);

                    await connection.ExecuteAsync("create_evaluacion_instructor", parameters, commandType: CommandType.StoredProcedure);

                    Console.WriteLine("Evaluación del instructor creada exitosamente.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating EvaluacionInstructor: {ex.Message}");
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
                        p_ID_Evaluacion = id
                    };

                    await connection.ExecuteAsync("delete_evaluacion_instructor", parameters, commandType: CommandType.StoredProcedure);

                    Console.WriteLine("Evaluación del instructor eliminada exitosamente.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting EvaluacionInstructor: {ex.Message}");
                return false;
            }
        }

        public async Task<EvaluacionInstructor> Edit(EvaluacionInstructor evaluacionInstructor)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Evaluacion", evaluacionInstructor.ID_Evaluacion);
                    parameters.Add("p_ID_Cliente", evaluacionInstructor.ID_Cliente);
                    parameters.Add("p_ID_Instructor", evaluacionInstructor.ID_Instructor);
                    parameters.Add("p_Calificacion", evaluacionInstructor.Calificacion);
                    parameters.Add("p_Comentario", evaluacionInstructor.Comentario);
                    parameters.Add("p_Fecha_evaluacion", evaluacionInstructor.Fecha_evaluacion, DbType.DateTime);

                    await connection.ExecuteAsync("edit_evaluacion_instructor", parameters, commandType: CommandType.StoredProcedure);

                    Console.WriteLine("Evaluación del instructor editada exitosamente.");
                    return evaluacionInstructor;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing EvaluacionInstructor: {ex.Message}");
                return null;
            }
        }
        public async Task<EvaluacionInstructor> GetEvaluacionInstructorById(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Evaluacion", id);
                    parameters.Add("result", OracleDbType.RefCursor, dbType: DbType.Object, direction: ParameterDirection.ReturnValue);




                    var result = await connection.QueryFirstOrDefaultAsync<EvaluacionInstructor>("BEGIN :result := get_evaluacion_instructor_by_id(:p_ID_Evaluacion); END;", parameters);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting EvaluacionInstructor by ID: {ex.Message}");
                return null;
            }
        }
    }
}
