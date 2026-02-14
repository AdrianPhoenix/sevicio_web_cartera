using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitadorController : ControllerBase
    {
        private readonly GeneradorService _generadorService;
        private readonly DataService _dataService;

        public VisitadorController(GeneradorService generadorService, DataService dataService)
        {
            _generadorService = generadorService;
            _dataService = dataService;
        }

        /// <summary>
        /// Obtiene la lista de todos los visitadores activos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Visitador>>> GetVisitadores()
        {
            try
            {
                var visitadores = await _dataService.BuscarVisitadoresAsync();
                return Ok(visitadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo visitadores: {ex.Message}");
            }
        }
        
                /// <summary>
                /// Genera y descarga el archivo Cartera.txt con sentencias SQL
                /// </summary>
                [HttpGet("{id}/cartera-txt")]
                public async Task<IActionResult> GetCarteraTxt(long id, [FromQuery] int ano = 0, [FromQuery] int ciclo = 1, [FromQuery] bool limpia = false, [FromQuery] bool cicloAbierto = false)
                {
                    try
                    {
                        if (ano == 0) ano = DateTime.Now.Year;
        
                        var carteraScript = await _generadorService.GenerarCarteraAsync(id, ano, ciclo, limpia, cicloAbierto);
                        
                        // Return the script as a downloadable text file named Cartera.txt
                        return File(System.Text.Encoding.UTF8.GetBytes(carteraScript), "text/plain", "Cartera.txt");
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error generando archivo de cartera: {ex.Message}");
                    }
                }
        
        
                /// <summary>
                /// Procesa la sincronización de datos desde la app Android (PENDIENTE DE IMPLEMENTACION COMPLETA)
                /// </summary>
                [HttpPost("{id}/sync")]
                public async Task<ActionResult<SyncResponse>> SyncData(int id, [FromBody] SyncRequest request)
                {
                    try
                    {
                        // La lógica completa de sincronización aún no está implementada
                        return StatusCode(501, new SyncResponse
                        {
                            Success = false,
                            Message = "El endpoint de sincronización está en desarrollo. Utilice el endpoint cartera-txt para obtener el script de actualización."
                        });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new SyncResponse 
                        { 
                            Success = false, 
                            Message = $"Error en sincronización: {ex.Message}" 
                        });
                    }
                }
        /// <summary>
        /// Obtiene los años disponibles para generar carteras
        /// </summary>
        [HttpGet("annios")]
        public async Task<ActionResult<string[]>> GetAnnios()
        {
            try
            {
                var annios = await _dataService.BuscarAnniosAsync();
                return Ok(annios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo años: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el Google Registration ID de un visitador
        /// </summary>
        [HttpGet("{id}/google-registration")]
        public async Task<ActionResult<string>> GetGoogleRegistrationId(long id)
        {
            try
            {
                var registrationId = await _dataService.BuscarGoogleRegistrationIDAsync(id);
                return Ok(registrationId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo Google Registration ID: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene los KPIs de un visitador para un ciclo y año específico
        /// </summary>
        [HttpGet("{id}/kpis")]
        public async Task<ActionResult<KpiResponse>> GetKpis(long id, [FromQuery] int ano = 0, [FromQuery] int ciclo = 1)
        {
            try
            {
                if (ano == 0) ano = DateTime.Now.Year;

                var kpis = await _dataService.ObtenerKpisAsync(id, ano, ciclo);
                
                if (kpis == null)
                {
                    return NotFound($"No se encontraron KPIs para el visitador {id}, año {ano}, ciclo {ciclo}");
                }

                return Ok(kpis);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo KPIs: {ex.Message}");
            }
        }
    }
}
