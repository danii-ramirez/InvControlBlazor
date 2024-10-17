using System.Security.Claims;
using InvControl.Server.Data;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemitosController : ControllerBase
    {
        private readonly ILogger<RemitosController> _logger;
        private readonly string connectionString;

        public RemitosController(ILogger<RemitosController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpPost]
        public IActionResult PostRemito(Remito remito)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_Remito daRe = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (SqlConnection cnn = new(connectionString))
                {
                    cnn.Open();
                    transaction = cnn.BeginTransaction();

                    remito.IdRemito = daRe.InsertarRemito(remito.Numero.Trim(), (DateTime)remito.Fecha!, remito.IdTransporte, remito.IdChofer, 1,
                        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), DateTime.Now, transaction);

                    foreach (var d in remito.Detalle)
                    {
                        daRe.InsertarRemitoDetalle(remito.IdRemito, d.IdSku, d.NombreSku, (int)d.Cantidad!, transaction);
                    }

                    transaction.Commit();
                    cnn.Close();
                }

                return Ok(remito);
            }
            catch (Exception ex)
            {
                if (transaction != null && transaction.Connection != null)
                    transaction.Rollback();
                _logger.LogError(ex, "{msg}", ex.Message);
                return StatusCode(500, ex);
            }
        }
    }
}
