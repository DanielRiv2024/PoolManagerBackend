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
    public class InstructorService : IInstructor
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public InstructorService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }

        public async Task<bool> Create(Instructor instructor)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_Nombre", instructor.Nombre);
                    parameters.Add("p_Cedula", instructor.Cedula);
                    parameters.Add("p_Telefono", instructor.Telefono);
                    parameters.Add("p_Correo_electronico", instructor.Correo_electronico);

                    await connection.ExecuteAsync("create_instructor", parameters, commandType: CommandType.StoredProcedure);

                    Console.WriteLine("Instructor creado exitosamente.");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando Instructor: {ex.Message}");
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

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Instructor", id);

                    await connection.ExecuteAsync("delete_instructor", parameters, commandType: CommandType.StoredProcedure);

                    Console.WriteLine("Instructor eliminado exitosamente.");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando Instructor: {ex.Message}");
                return false;
            }
        }

        public async Task<EvaluacionInstructor> Edit(Instructor instructor)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Instructor", instructor.ID_Instructor);
                    parameters.Add("p_Nombre", instructor.Nombre);
                    parameters.Add("p_Cedula", instructor.Cedula);
                    parameters.Add("p_Telefono", instructor.Telefono);
                    parameters.Add("p_Correo_electronico", instructor.Correo_electronico);

                    await connection.ExecuteAsync("edit_instructor", parameters, commandType: CommandType.StoredProcedure);

                    Console.WriteLine("Instructor editado exitosamente.");

                  
                    return new EvaluacionInstructor { /* información de evaluación, si aplicable */ };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editando Instructor: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Instructor>> GetAll()
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_instructores; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        var result = (OracleRefCursor)resultParameter.Value;
                        using (OracleDataReader reader = result.GetDataReader())
                        {
                            List<Instructor> instructores = new List<Instructor>();

                            while (await reader.ReadAsync())
                            {
                                var instructorResult = new Instructor
                                {
                                    ID_Instructor = reader.GetInt32(reader.GetOrdinal("ID_Instructor")),
                                    Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                    Cedula = reader.GetString(reader.GetOrdinal("CEDULA")),
                                    Telefono = reader.GetString(reader.GetOrdinal("TELEFONO")),
                                    Correo_electronico = reader.GetString(reader.GetOrdinal("CORREO_ELECTRONICO"))
                                };

                                instructores.Add(instructorResult);
                            }

                            return instructores;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo todos los instructores: {ex.Message}");
                return null;
            }
        }
    }
}
