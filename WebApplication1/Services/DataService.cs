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
    }
}
