using ClosedXML.Excel;
using InvControl.Server.Data;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly string connectionString;

        public StockController(ILogger<StockController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet("consulta")]
        public IActionResult GetStock(string nombre, int? idMarca, bool? especial, int? cantidadMin, int? cantidadMax, DateTime? fechaMin, DateTime? fechaMax)
        {
            List<Stock> stock = new();

            using (DataTable dt = new DA_Stock(connectionString).ObtenerStock(nombre?.Trim(), idMarca, especial, cantidadMin, cantidadMax, fechaMin, fechaMax))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Stock s = new()
                    {
                        IdStock = (int)dr["IdStock"],
                        IdSku = (int)dr["IdSKU"],
                        Codigo = (int)dr["Codigo"],
                        Nombre = (string)dr["Nombre"],
                        Marca = (string)dr["Marca"],
                        Cantidad = (int)dr["Cantidad"]
                    };
                    stock.Add(s);
                }
            }

            return Ok(stock);
        }

        [HttpPost("ExportToExcel")]
        public IActionResult PostExportToExcel([FromBody] List<Stock> skus)
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Datos");

                worksheet.Cell(1, 1).Value = "Código";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Marca";
                worksheet.Cell(1, 4).Value = "Stock";

                for (int i = 0; i < skus.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = skus[i].Codigo;
                    worksheet.Cell(i + 2, 2).Value = skus[i].Nombre;
                    worksheet.Cell(i + 2, 3).Value = skus[i].Marca;
                    worksheet.Cell(i + 2, 4).Value = skus[i].Cantidad;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportedData.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
