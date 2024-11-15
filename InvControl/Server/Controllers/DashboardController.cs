using InvControl.Server.Data;
using InvControl.Shared.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly string connectionString;

        public DashboardController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet("remitos/estados")]
        public IActionResult GetRemitosEstados()
        {
            List<RemitoEstadoDash> remitos = new();
            using (DataTable dt = new DA_Dashboard(connectionString).ObtenerRemitosEstados())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    RemitoEstadoDash r = new()
                    {
                        IdEstado = (int)dr["IdEstado"],
                        Cantidad = (int)dr["Cantidad"]
                    };
                    remitos.Add(r);
                }
            }
            return Ok(remitos);
        }
    }
}
