using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiosController : ControllerBase
    {
        private readonly string _estudiosPath;

        public EstudiosController(IConfiguration configuration)
        {
            _estudiosPath = configuration["EstudiosPath"] ?? string.Empty;
        }

        /// <summary>
        /// Lista los archivos PDF disponibles en la carpeta de estudios clínicos
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetEstudios()
        {
            try
            {
                if (!Directory.Exists(_estudiosPath))
                    return StatusCode(500, $"La carpeta de estudios no existe: {_estudiosPath}");

                var archivos = Directory
                    .GetFiles(_estudiosPath, "*.pdf", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                    .Select(nombre => new
                    {
                        nombre,
                        url = $"/api/estudios/{Uri.EscapeDataString(nombre!)}"
                    });

                return Ok(archivos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error listando estudios: {ex.Message}");
            }
        }

        /// <summary>
        /// Descarga un PDF de estudios clínicos por nombre de archivo
        /// </summary>
        [HttpGet("{archivo}")]
        public IActionResult DescargarEstudio(string archivo)
        {
            try
            {
                var nombreSeguro = Path.GetFileName(archivo);
                if (string.IsNullOrWhiteSpace(nombreSeguro))
                    return BadRequest("Nombre de archivo inválido");

                var rutaCompleta = Path.Combine(_estudiosPath, nombreSeguro);
                var raiz = Path.GetFullPath(_estudiosPath);
                var destino = Path.GetFullPath(rutaCompleta);

                if (!destino.StartsWith(raiz, StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Nombre de archivo inválido");

                if (!System.IO.File.Exists(rutaCompleta))
                    return NotFound($"Archivo no encontrado: {nombreSeguro}");

                var stream = System.IO.File.OpenRead(rutaCompleta);
                return File(stream, "application/pdf", nombreSeguro, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error descargando archivo: {ex.Message}");
            }
        }
    }
}
