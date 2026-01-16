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
        if (fechaFin.HasValue)
        {
            fechaFin = fechaFin.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
        using (var connection = new SqlConnection(_connectionString))
        {
            // Ajustamos el WHERE para que el BETWEEN sea estricto si hay fechas
            var sql = @"
            SELECT * FROM SYSTEMPES.DBO.Reporte_Pes_Detalle
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




    [HttpGet("buscar2")]
    public async Task<IActionResult> GetFiltrado2(
        [FromQuery] int? numPes,
        [FromQuery] string? usuario,
        [FromQuery] string? sector,
        [FromQuery] string? gerencia,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin)
    {
        // Ajustamos fechaFin para que sea 23:59:59 del día elegido
        DateTime? fechaFinFull = fechaFin?.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

        using (var connection = new SqlConnection(_connectionString))
        {
            // T1: PES_ACTIVOS_SGD01 (Origen)
            // T2: Reporte_Pes_Detalle (Destino/Exclusión)
            var sql = @"
            SELECT T1.* FROM SYSTEMPES02.DBO.vw_PES_ACTIVOS_SGD01 T1
            LEFT JOIN SYSTEMPES.DBO.Reporte_Pes_Detalle T2 ON T1.NUM_PES = T2.NUM_PES AND  DATEPART(YEAR,T1.FECHA_INI) = DATEPART(YEAR,T2.FECHA_INICIO) 
            WHERE T2.NUM_PES IS NULL
              AND (@numPes IS NULL OR T1.NUM_PES = @numPes)
              AND (@usuario IS NULL OR T1.USUARIO LIKE '%' + @usuario + '%')
              AND (@sector IS NULL OR T1.SECTOR = @sector)
              AND (@gerencia IS NULL OR T1.sector = @gerencia)
              AND (
                  (@fechaInicio IS NULL OR T1.FECHA_INI >= @fechaInicio)
                  AND 
                  (@fechaFinFull IS NULL OR T1.FECHA_INI <= @fechaFinFull)
              )";

            var resultados = await connection.QueryAsync<dynamic>(sql, new
            {
                numPes,
                usuario,
                sector,
                gerencia,
                fechaInicio,
                fechaFinFull = fechaFinFull // Usamos la fecha con la hora ajustada
            });

            return Ok(resultados);
        }
    }
}