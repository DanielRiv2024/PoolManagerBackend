using Oracle.ManagedDataAccess.Client;
using PoolManagerBackend.Models;
using System.Configuration;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Interface;
using Dapper.Oracle;
using System.Data;
using Oracle.ManagedDataAccess.Types;

namespace PoolManagerBackend.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IConfiguration configuration;
        private readonly string _connectionString;

        public ReservaService(IConfiguration _configuration)
        {
            configuration = _configuration;
            _connectionString = "User Id=poolmanager;Password=123456;Data Source=localhost:1521/orcl;";
        }
        public async Task<bool> CreateReserva(Reserva reserva)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new
                    {
                        p_ID_Cliente = reserva.ID_Cliente,
                        p_ID_Piscina = reserva.ID_Piscina,
                        p_ID_Clase = reserva.ID_Clase,
                        p_Fecha_hora_inicio = reserva.Fecha_hora_inicio,
                        p_Fecha_hora_fin = reserva.Fecha_hora_fin,
                        p_Costo = reserva.Costo,
                        p_Pago_en_linea = reserva.Pago_en_linea
                    };

                    await connection.ExecuteAsync("create_reserva", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la reserva: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateReserva(Reserva reserva)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string query = "BEGIN edit_reserva(:IN_ID_Reserva, :IN_ID_Cliente, :IN_ID_Piscina, :IN_ID_Clase, :IN_FechaInicio, :IN_FechaFin, :IN_Costo, :IN_PagoEnLinea); END;";
                            var result = await connection.ExecuteAsync(query, new
                            {
                                IN_ID_Reserva = reserva.ID_Reserva,
                                IN_ID_Cliente = reserva.ID_Cliente,
                                IN_ID_Piscina = reserva.ID_Piscina,
                                IN_ID_Clase = reserva.ID_Clase,
                                IN_FechaInicio = reserva.Fecha_hora_inicio,
                                IN_FechaFin = reserva.Fecha_hora_fin,
                                IN_Costo = reserva.Costo,
                                IN_PagoEnLinea = reserva.Pago_en_linea
                            }, transaction);

                            transaction.Commit();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error al actualizar la reserva: {ex.Message}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la conexión: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteReserva(int idReserva)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new
                    {
                        p_ID_Reserva = idReserva
                    };

                    await connection.ExecuteAsync("delete_reserva", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la reserva: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Reserva>> GetAllReserva()
        {
            try
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := get_all_reservas; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        var result = (OracleRefCursor)resultParameter.Value;

                        List<Reserva> reservas = new List<Reserva>();
                        using (OracleDataReader reader = result.GetDataReader())
                        {
                            while (reader.Read())
                            {
                                var reserva = new Reserva
                                {
                                    ID_Reserva = reader.GetInt32(reader.GetOrdinal("ID_Reserva")),
                                    ID_Cliente = reader.GetInt32(reader.GetOrdinal("ID_Cliente")),
                                    ID_Piscina = reader.GetInt32(reader.GetOrdinal("ID_Piscina")),
                                    ID_Clase = reader.GetInt32(reader.GetOrdinal("ID_Clase")),
                                    Fecha_hora_inicio = reader.GetDateTime(reader.GetOrdinal("Fecha_hora_inicio")),
                                    Fecha_hora_fin = reader.GetDateTime(reader.GetOrdinal("Fecha_hora_fin")),
                                    Costo = reader.GetDecimal(reader.GetOrdinal("Costo")),
                                    Pago_en_linea = reader.GetString(reader.GetOrdinal("Pago_en_linea")),
                                };

                                reservas.Add(reserva);
                            }
                        }

                        return reservas;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener todas las reservas: {ex.Message}");
                return null;
            }
        }
        public async Task<string> GetTotalIngresosReservas()
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := CURSOR_calcular_total_ingresos_reservas; END;", connection))
                    {
                        command.Parameters.Add(resultParameter);
                        command.CommandType = CommandType.Text;

                        await command.ExecuteNonQueryAsync();

                        // Convertir el resultado a cadena directamente
                        string totalIngresos = resultParameter.Value.ToString();

                        return totalIngresos;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el total de ingresos: {ex.Message}");
                return "-1"; // o maneja el error según sea necesario
            }
        }

        public async Task<string> ObtenerReservasPagadasEnLinea()
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

                    using (OracleCommand command = new OracleCommand("BEGIN :result := CURSOR_reservas_pagadas_en_linea; END;", connection))
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
                Console.WriteLine($"Error al obtener reservas pagadas en línea: {ex.Message}");
                return string.Empty; // o manejar el error según sea necesario
            }
        }


    }
}
