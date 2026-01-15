using ConsultPesApi.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ConsultPesApi.Models;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly string _connectionString;

    public ReportesController(IConfiguration configuration)
    {
        // Esto lee la conexión que pusimos en el paso 3
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportePesDetalle>>> GetReportes()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var sql = "SELECT top 1 * FROM Reporte_Pes_Detalle";
            var lista = await connection.QueryAsync<ReportePesDetalle>(sql);
            return Ok(lista);
        }
    }


    [HttpGet("buscar")]
    public async Task<IActionResult> GetFiltrado(
      [FromQuery] int? numPes,
      [FromQuery] string? usuario,
      [FromQuery] string? sector,
      [FromQuery] string? gerencia,
      [FromQuery] DateTime? fechaInicio,
      [FromQuery] DateTime? fechaFin)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            // Ajustamos el WHERE para que el BETWEEN sea estricto si hay fechas
            var sql = @"
            SELECT top 100 * FROM Reporte_Pes_Detalle
            WHERE (@numPes IS NULL OR NUM_PES = @numPes)
            AND (@usuario IS NULL OR USUARIO LIKE '%' + @usuario + '%')
            AND (@sector IS NULL OR SECTOR = @sector)
            AND (@gerencia IS NULL OR PROVINCIA = @gerencia)
            AND (
                (@fechaInicio IS NULL AND @fechaFin IS NULL) 
                OR (FECHA_INICIO BETWEEN ISNULL(@fechaInicio, '1900-01-01') AND ISNULL(@fechaFin, '2099-12-31'))
            )";

            var resultados = await connection.QueryAsync<dynamic>(sql, new
            {
                numPes,
                usuario,
                sector,
                gerencia,
                fechaInicio,
                fechaFin
            });

            return Ok(resultados);
        }
    }


}