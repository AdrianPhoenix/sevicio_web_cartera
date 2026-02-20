using System.Data.SqlClient;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class DataService
    {
        private readonly string _connectionString = string.Empty;

        public DataService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MedinetWeb") ?? string.Empty;
        }
        
        public async Task<Visitador?> BuscarVisitadorAsync(long visitadorId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Este query une las tablas para obtener todos los datos necesarios del visitador
            var query = @"SELECT 
                              vh.ID_VisitadoresHistorial,
                              u.TX_Nombre,
                              u.TX_Apellido,
                              vh.ID_Region,
                              vh.ID_Linea
                          FROM 
                              MW_VisitadoresHistorial vh
                          JOIN 
                              MW_Usuarios u ON vh.ID_Visitador = u.ID_Usuario
                          WHERE 
                              vh.ID_VisitadoresHistorial = @visitadorId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@visitadorId", visitadorId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Visitador
                {
                    ID_VisitadoresHistorial = reader.GetInt64(0),
                    TX_Nombre = reader.GetString(1) + " " + reader.GetString(2),
                    TX_Apellido = reader.GetString(2), // Apellido solo
                    // Mapeo basado en el código original y el nuevo modelo
                    ID_Empresa = reader.GetInt16(3), // ID_Region se mapea a ID_Empresa
                    ID_Linea = reader.GetInt16(4),   // ID_Linea
                    BO_Activo = true
                };
            }

            return null; // Retorna null si no se encuentra el visitador
        }


        public async Task<List<Visitador>> BuscarVisitadoresAsync()
        {
            var visitadores = new List<Visitador>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Usar el mismo stored procedure que el código original
            using var command = new SqlCommand("SP_GBD_VisitadoresActivos", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ano", DateTime.Now.Year);
            command.Parameters.AddWithValue("@ciclo", 1);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Basado en el código original: GetInt64(0), GetString(1) + " " + GetString(2), GetInt16(3), GetInt16(4)
                visitadores.Add(new Visitador
                {
                    ID_VisitadoresHistorial = reader.GetInt64(0),
                    TX_Nombre = reader.GetString(1) + " " + reader.GetString(2), // Nombre completo
                    TX_Apellido = "", // No usado en el original
                    TX_Usuario = "", // No usado en el original  
                    TX_Password = "", // No usado en el original
                    ID_Empresa = reader.GetInt16(3), // Region
                    ID_Linea = reader.GetInt16(4), // Linea
                    BO_Activo = true
                });
            }

            return visitadores;
        }

        public async Task<string[]> BuscarAnniosAsync()
        {
            var annios = new List<string>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT DISTINCT NU_Ano FROM MW_Ciclos WHERE NU_Estatus=1 OR NU_Estatus=2 ORDER BY 1 DESC";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                annios.Add(reader.GetInt16(0).ToString());
            }

            return annios.ToArray();
        }

        public async Task<string> GetDebugInfoAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT TOP 1 * FROM MW_VisitadoresHistorial";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            var info = new StringBuilder();
            if (await reader.ReadAsync())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i);
                    info.AppendLine($"Column {i}: {reader.GetName(i)} = {value} ({value?.GetType().Name})");
                }
            }

            return info.ToString();
        }

        public async Task<string> BuscarGoogleRegistrationIDAsync(long idVisitador)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT dbo.MW_Usuarios.TX_IDRegistroGoogle 
                         FROM dbo.MW_Usuarios 
                         INNER JOIN dbo.MW_VisitadoresHistorial ON dbo.MW_Usuarios.ID_Usuario = dbo.MW_VisitadoresHistorial.ID_Visitador 
                         WHERE (dbo.MW_VisitadoresHistorial.ID_VisitadoresHistorial = @idVisitador) 
                         AND (dbo.MW_VisitadoresHistorial.BO_Activo = 1)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@idVisitador", idVisitador);

            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }

        public async Task<KpiResponse?> ObtenerKpisAsync(long visitadorId, int ano, int ciclo)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT 
                            ID_Ciclo,
                            NU_Ano,
                            NU_Ciclo,
                            FE_CicloIni,
                            FE_CicloFin,
                            NU_DiasHabiles,
                            NU_Estatus,
                            KPI_Visita_Medica,
                            KPI_Visita_Farmacia
                          FROM MW_Ciclos 
                          WHERE NU_Ano = @ano AND NU_Ciclo = @ciclo";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ano", ano);
            command.Parameters.AddWithValue("@ciclo", ciclo);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                // Determinar el estatus basado en NU_Estatus
                // 1 = Activo (A), 2 = Cerrado (C), 3 = Pendiente (P)
                int estatusNum = reader.GetInt16(6);
                string estatus = estatusNum switch
                {
                    1 => "A", // Activo
                    2 => "C", // Cerrado
                    _ => "P"  // Pendiente
                };

                return new KpiResponse
                {
                    VisitadorId = visitadorId,
                    Ano = reader.GetInt16(1),
                    Ciclo = reader.GetInt16(2),
                    FechaInicio = reader.GetDateTime(3).ToString("dd/MM/yyyy"),
                    FechaFin = reader.GetDateTime(4).ToString("dd/MM/yyyy"),
                    DiasHabiles = reader.GetInt16(5),
                    Estatus = estatus,
                    KpiVisitaMedica = reader.GetInt32(7),
                    KpiVisitaFarmacia = reader.GetInt32(8)
                };
            }

            return null;
        }

        public async Task<EstadoSincronizacionResponse?> ObtenerEstadoSincronizacionAsync(string zona, int ano, int ciclo)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // PASO 1: Obtener última transmisión de MD_Transmision
            var queryTransmision = @"SELECT TOP 1 FECHA, HORA, TIPO 
                                     FROM MD_Transmision 
                                     WHERE ZONA = @zona AND CICLO = @ciclo AND NU_ANO = @ano 
                                     ORDER BY FECHA DESC, HORA DESC";

            string? fechaTransmision = null;
            string? horaTransmision = null;
            string? tipoTransmision = null;

            using (var command = new SqlCommand(queryTransmision, connection))
            {
                command.Parameters.AddWithValue("@zona", zona);
                command.Parameters.AddWithValue("@ciclo", ciclo);
                command.Parameters.AddWithValue("@ano", ano);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    fechaTransmision = reader.GetString(0);
                    horaTransmision = reader.GetString(1);
                    tipoTransmision = reader.GetString(2);
                }
            }

            // Si no hay transmisión, retornar null
            if (fechaTransmision == null || horaTransmision == null)
            {
                return null;
            }

            // PASO 2: Verificar en MD_Resumen_Transmision
            var queryResumen = @"SELECT Fecha, Hora, Visitados, Fichados 
                                 FROM MD_Resumen_Transmision 
                                 WHERE Zona = @zona AND CICLO = @ciclo AND NU_ANO = @ano";

            bool existeEnResumen = false;
            string? fechaResumen = null;
            string? horaResumen = null;
            int visitados = 0;
            int fichados = 0;

            using (var command = new SqlCommand(queryResumen, connection))
            {
                command.Parameters.AddWithValue("@zona", zona);
                command.Parameters.AddWithValue("@ciclo", ciclo);
                command.Parameters.AddWithValue("@ano", ano);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    fechaResumen = reader.GetString(0);
                    horaResumen = reader.GetString(1);
                    
                    // Usar Convert para manejar tanto Int16 como Int32
                    visitados = Convert.ToInt32(reader.GetValue(2));
                    fichados = Convert.ToInt32(reader.GetValue(3));

                    // Verificar si la fecha coincide y la hora está dentro del rango de ±3 segundos
                    if (fechaResumen == fechaTransmision && CompararHorasConTolerancia(horaResumen, horaTransmision, 3))
                    {
                        existeEnResumen = true;
                    }
                }
            }

            // PASO 3: Verificar en MD_Resumen_Transmision_Acumulada
            var queryAcumulada = @"SELECT TOP 1 Fecha, Hora 
                                   FROM MD_Resumen_Transmision_Acumulada 
                                   WHERE Zona = @zona AND CICLO = @ciclo AND NU_ANO = @ano 
                                   ORDER BY Fecha DESC, Hora DESC";

            bool existeEnAcumulada = false;

            using (var command = new SqlCommand(queryAcumulada, connection))
            {
                command.Parameters.AddWithValue("@zona", zona);
                command.Parameters.AddWithValue("@ciclo", ciclo);
                command.Parameters.AddWithValue("@ano", ano);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    string fechaAcumulada = reader.GetString(0);
                    string horaAcumulada = reader.GetString(1);

                    // Verificar si la fecha coincide y la hora está dentro del rango de ±3 segundos
                    if (fechaAcumulada == fechaTransmision && CompararHorasConTolerancia(horaAcumulada, horaTransmision, 3))
                    {
                        existeEnAcumulada = true;
                    }
                }
            }

            // PASO 4: Determinar estado
            bool carteraActualizada = existeEnResumen && existeEnAcumulada;
            string estadoProcesamiento = carteraActualizada ? "COMPLETADO" : "PROCESANDO";

            // Construir timestamp ISO 8601
            string timestamp = ConvertirATimestamp(fechaTransmision, horaTransmision);

            return new EstadoSincronizacionResponse
            {
                Zona = zona,
                Ciclo = ciclo,
                Ano = ano,
                CarteraActualizada = carteraActualizada,
                EstadoProcesamiento = estadoProcesamiento,
                UltimaTransmision = new UltimaTransmisionInfo
                {
                    Fecha = fechaTransmision,
                    Hora = horaTransmision,
                    Tipo = tipoTransmision ?? "",
                    Timestamp = timestamp
                },
                ResumenProcesado = fechaResumen != null ? new ResumenProcesadoInfo
                {
                    Fecha = fechaResumen,
                    Hora = horaResumen ?? "",
                    Visitados = visitados,
                    Fichados = fichados
                } : null
            };
        }

        private bool CompararHorasConTolerancia(string hora1, string hora2, int toleranciaSegundos)
        {
            try
            {
                // Normalizar las horas (eliminar punto final si existe)
                hora1 = hora1.TrimEnd('.');
                hora2 = hora2.TrimEnd('.');

                // Convertir a TimeSpan
                var time1 = ParsearHora(hora1);
                var time2 = ParsearHora(hora2);

                if (time1 == null || time2 == null)
                    return false;

                // Calcular diferencia en segundos
                var diferencia = Math.Abs((time1.Value - time2.Value).TotalSeconds);

                return diferencia <= toleranciaSegundos;
            }
            catch
            {
                return false;
            }
        }

        private TimeSpan? ParsearHora(string hora)
        {
            try
            {
                // Formato esperado: "08:18:57 P.M" o "08:18:57 A.M"
                var partes = hora.Split(' ');
                if (partes.Length != 2)
                    return null;

                var tiempoPartes = partes[0].Split(':');
                if (tiempoPartes.Length != 3)
                    return null;

                int horas = int.Parse(tiempoPartes[0]);
                int minutos = int.Parse(tiempoPartes[1]);
                int segundos = int.Parse(tiempoPartes[2]);

                // Ajustar para formato 12 horas
                string ampm = partes[1].ToUpper();
                if (ampm.StartsWith("P") && horas != 12)
                    horas += 12;
                else if (ampm.StartsWith("A") && horas == 12)
                    horas = 0;

                return new TimeSpan(horas, minutos, segundos);
            }
            catch
            {
                return null;
            }
        }

        private string ConvertirATimestamp(string fecha, string hora)
        {
            try
            {
                // Parsear fecha (formato: "2026-02-19")
                var fechaPartes = fecha.Split('-');
                int year = int.Parse(fechaPartes[0]);
                int month = int.Parse(fechaPartes[1]);
                int day = int.Parse(fechaPartes[2]);

                // Parsear hora
                var timeSpan = ParsearHora(hora);
                if (timeSpan == null)
                    return $"{fecha}T00:00:00";

                var dateTime = new DateTime(year, month, day, timeSpan.Value.Hours, timeSpan.Value.Minutes, timeSpan.Value.Seconds);
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            catch
            {
                return $"{fecha}T00:00:00";
            }
        }
    }
}
