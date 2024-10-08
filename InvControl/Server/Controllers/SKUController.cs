using InvControl.Server.Data;
using InvControl.Server.Helpers;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SKUController : ControllerBase
    {
        private readonly ILogger<SKUController> _logger;
        private readonly string connectionString;

        public SKUController(ILogger<SKUController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet]
        public IActionResult GetSKU(int? codigo, string? nombre, bool? activo, int? idMarca)
        {
            List<SKU> lst = new();
            DA_SKU da = new(connectionString);
            using (DataTable dt = da.ObtenerSKU(null, codigo, nombre))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SKU s = new()
                    {
                        IdSKU = (int)dr["IdSKU"],
                        Codigo = (int)dr["Codigo"],
                        Nombre = (string)dr["Nombre"],
                        Activo = (bool)dr["Activo"],
                        Especial = (bool)dr["Especial"],
                        UnidadesPorBandeja = (int?)dr["UnidadesPorBandeja"],
                        Stock = (int)dr["Stock"],
                        IdMarca = (int)dr["IdMarca"],
                        DescripcionMarca = (string)dr["DescripcionMarca"]
                    };
                    if (dr["Descripcion"] != DBNull.Value) s.Descripcion = (string)dr["Descripcion"];

                    lst.Add(s);
                }
            }

            return Ok(lst);
        }

        [HttpPost]
        public IActionResult PostSKU(SKU sku)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_SKU daSKU = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                if (daSKU.ObtenerSKU(null, sku.Codigo, null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(SKU.Codigo), "Ya existe un SKU con el mismo código");

                if (daSKU.ObtenerSKU(null, null, sku.Nombre.Trim()).Rows.Count > 0)
                    ModelState.AddModelError(nameof(SKU.Nombre), "Ya existe un SKU con el mismo nombre");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        sku.IdSKU = daSKU.InsertarSKU((int)sku.Codigo!, sku.Nombre.Trim(), sku.Descripcion?.Trim()!, sku.Activo, sku.Especial, (int)sku.UnidadesPorBandeja!,
                            sku.IdMarca, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), DateTime.Now, transaction);

                        daAu.Insertar($"Se creó el SKU: {sku.Codigo}", DateTime.Now, (int)TipoEntidad.SKU, (int)TipoOperacion.Creacion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(sku);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                if (transaction != null && transaction.Connection != null)
                    transaction.Rollback();
                _logger.LogError(ex, "{msg}", ex.Message);
                return StatusCode(500, ex);
            }
        }

        [HttpPut]
        public IActionResult PutSKU(SKU sku)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_SKU daSKU = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (DataTable dt = daSKU.ObtenerSKU(null, sku.Codigo, null))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdSKU"] != sku.IdSKU)
                        ModelState.AddModelError(nameof(SKU.Codigo), "Ya existe un SKU con el mismo código");
                }

                using (DataTable dt = daSKU.ObtenerSKU(null, null, sku.Nombre.Trim()))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdSKU"] != sku.IdSKU)
                        ModelState.AddModelError(nameof(SKU.Nombre), "Ya existe un SKU con el mismo nombre");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        daSKU.ActualizarSKU(sku.IdSKU, (int)sku.Codigo!, sku.Nombre.Trim(), sku.Descripcion?.Trim()!, sku.Activo, sku.Especial, (int)sku.UnidadesPorBandeja!,
                           sku.IdMarca, transaction);

                        daAu.Insertar($"Se editó el SKU: {sku.Codigo}", DateTime.Now, (int)TipoEntidad.SKU, (int)TipoOperacion.Edicion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(sku);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                if (transaction != null && transaction.Connection != null)
                    transaction.Rollback();
                _logger.LogError(ex, "{msg}", ex.Message);
                return StatusCode(500, ex);
            }
        }

        [HttpGet("marcas")]
        public IActionResult GetMarcas()
        {
            List<Marca> marcas = new();
            DA_SKU da = new(connectionString);
            using (DataTable dt = da.ObtenerMarcas())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Marca m = new()
                    {
                        IdMarca = (int)dr["IdMarca"],
                        Descripcion = (string)dr["Descripcion"]
                    };
                    marcas.Add(m);
                }
            }
            return Ok(marcas);
        }
    }
}
