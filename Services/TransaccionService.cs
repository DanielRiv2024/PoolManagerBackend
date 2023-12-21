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
    public class TransaccionService : ITransacion
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public TransaccionService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }

        public async Task<Transaccion> Create(Transaccion transaccion)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Reserva", transaccion.ID_Reserva, DbType.Int32);
                    parameters.Add("p_ID_Tarjeta", transaccion.ID_Tarjeta, DbType.Int32);
                    parameters.Add("p_Detalle", transaccion.Detalle);
                    parameters.Add("p_Monto", transaccion.Monto, DbType.Decimal);
                    parameters.Add("p_Fecha_de_pago", transaccion.Fecha_de_pago, DbType.DateTime);
                    parameters.Add("p_Estado_de_pago", transaccion.Estado_de_pago, DbType.Int32);
                    parameters.Add("p_Mensaje_de_respuesta", transaccion.Mensaje_de_respuesta);
                    parameters.Add("p_Fecha_de_transaccion", transaccion.Fecha_de_transaccion, DbType.DateTime);

                    await connection.ExecuteAsync("create_pago", parameters, commandType: CommandType.StoredProcedure);

                    return transaccion;
                }
            }
            catch (Exception ex)
            {
                // Log el error
                Console.WriteLine($"Error al crear Transaccion: {ex.Message}");
                return null; // Otra opción es lanzar una excepción personalizada
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
                    parameters.Add("p_ID_Pago", id, DbType.Int32);

                    await connection.ExecuteAsync("delete_pago", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log el error
                Console.WriteLine($"Error al eliminar Transaccion: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Transaccion>> GetAll()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Use OracleRefCursor to handle the output cursor
                    OracleParameter resultParameter = new OracleParameter
                    {
                        ParameterName = "result",
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.ReturnValue
                    };

                    using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_pagos; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text; // Specify that you're calling a function, not a stored procedure

                        await command.ExecuteNonQueryAsync();

                        // Get the output cursor
                        var result = (OracleRefCursor)resultParameter.Value;

                        // Read the results from the cursor
                        List<Transaccion> transacciones = new List<Transaccion>();
                        using (OracleDataReader reader = result.GetDataReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                var transaccion = new Transaccion
                                {
                                    ID_Pago = reader.GetInt32(reader.GetOrdinal("ID_PAGO")),
                                    ID_Reserva = reader.GetInt32(reader.GetOrdinal("ID_RESERVA")),
                                    ID_Tarjeta = reader.GetInt32(reader.GetOrdinal("ID_TARJETA")),
                                    Detalle = reader.GetString(reader.GetOrdinal("DETALLE")),
                                    Monto = reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                    Fecha_de_pago = reader.GetDateTime(reader.GetOrdinal("FECHA_DE_PAGO")),
                                    Estado_de_pago = reader.GetInt32(reader.GetOrdinal("ESTADO_DE_PAGO")) == 1,
                                    Mensaje_de_respuesta = reader.GetString(reader.GetOrdinal("MENSAJE_DE_RESPUESTA")),
                                    Fecha_de_transaccion = reader.GetDateTime(reader.GetOrdinal("FECHA_DE_TRANSACCION"))
                                };

                                transacciones.Add(transaccion);
                            }
                        }

                        return transacciones;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log el error
                Console.WriteLine($"Error al obtener todas las Transacciones: {ex.Message}");
                return null;
            }
        }

        public async Task<Transaccion> Update(Transaccion transaccion)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("p_ID_Pago", transaccion.ID_Pago, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("p_ID_Reserva", transaccion.ID_Reserva, DbType.Int32);
                    parameters.Add("p_ID_Tarjeta", transaccion.ID_Tarjeta, DbType.Int32);
                    parameters.Add("p_Detalle", transaccion.Detalle);
                    parameters.Add("p_Monto", transaccion.Monto, DbType.Decimal);
                    parameters.Add("p_Fecha_de_pago", transaccion.Fecha_de_pago, DbType.DateTime);
                    parameters.Add("p_Estado_de_pago", transaccion.Estado_de_pago ? 1 : 0, DbType.Int32);
                    parameters.Add("p_Mensaje_de_respuesta", transaccion.Mensaje_de_respuesta);
                    parameters.Add("p_Fecha_de_transaccion", transaccion.Fecha_de_transaccion, DbType.DateTime);

                    await connection.ExecuteAsync("edit_pago", parameters, commandType: CommandType.StoredProcedure);

                    return transaccion;
                }
            }
            catch (Exception ex)
            {
                // Log el error
                Console.WriteLine($"Error al editar Transaccion: {ex.Message}");
                return null; // Otra opción es lanzar una excepción personalizada
            }
        }
    }
}
