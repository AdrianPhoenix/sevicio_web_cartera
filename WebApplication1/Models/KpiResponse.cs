namespace WebApplication1.Models
{
    public class KpiResponse
    {
        public long VisitadorId { get; set; }
        public int Ano { get; set; }
        public int Ciclo { get; set; }
        public int KpiVisitaMedica { get; set; }
        public int KpiVisitaFarmacia { get; set; }
        public string FechaInicio { get; set; } = string.Empty;
        public string FechaFin { get; set; } = string.Empty;
        public int DiasHabiles { get; set; }
        public string Estatus { get; set; } = string.Empty;
    }
}
