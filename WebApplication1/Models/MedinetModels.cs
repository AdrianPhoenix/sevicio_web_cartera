namespace WebApplication1.Models
{
    public class Visitador
    {
        public long ID_VisitadoresHistorial { get; set; }
        public string TX_Nombre { get; set; } = string.Empty;
        public string TX_Apellido { get; set; } = string.Empty;
        public string TX_Usuario { get; set; } = string.Empty;
        public string TX_Password { get; set; } = string.Empty;
        public int ID_Empresa { get; set; }
        public int ID_Linea { get; set; }
        public bool BO_Activo { get; set; }
    }

    public class CarteraData
    {
        public List<Visitador> Visitadores { get; set; } = new();
        public string ContenidoCartera { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; }
    }

    public class SyncRequest
    {
        public int VisitadorId { get; set; }
        public string DatosLocales { get; set; } = string.Empty;
        public DateTime UltimaSync { get; set; }
    }

    public class SyncResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CarteraData? DatosActualizados { get; set; }
    }
}

    public class EstadoSincronizacionResponse
    {
        public string Zona { get; set; } = string.Empty;
        public int Ciclo { get; set; }
        public int Ano { get; set; }
        public bool CarteraActualizada { get; set; }
        public string EstadoProcesamiento { get; set; } = string.Empty;
        public UltimaTransmisionInfo? UltimaTransmision { get; set; }
        public ResumenProcesadoInfo? ResumenProcesado { get; set; }
    }

    public class UltimaTransmisionInfo
    {
        public string Fecha { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
    }

    public class ResumenProcesadoInfo
    {
        public string Fecha { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public int Visitados { get; set; }
        public int Fichados { get; set; }
    }
