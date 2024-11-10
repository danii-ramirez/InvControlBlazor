using ClosedXML.Excel;
using InvControl.Server.Controllers;
using InvControl.Server.Data;
using InvControl.Shared.Helpers;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Data;

namespace InvControl.Server.Hubs
{
    public class ProcesamientoHub : Hub
    {
        private readonly ILogger<ProcesamientoHub> _logger;
        private readonly string connectionString;

        public ProcesamientoHub(ILogger<ProcesamientoHub> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("Cliente conectado: {ConnectionId}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogWarning("Cliente desconectado: {ConnectionId}", Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task ProcesarMovimientosBimbo(byte[] excelData)
        {
            try
            {
                DA_CanalVenta daCV = new(connectionString);
                DA_SKU daSKU = new(connectionString);
                DA_Stock daStock = new(connectionString);
                var parametros = new ParametrosController(connectionString: connectionString).ObtenerParametrosBimbo();
                List<MovimientoBimbo> movimientos = new();

                DataTable dt = new();
                using (var stream = new MemoryStream(excelData))
                {
                    using var workbook = new XLWorkbook(stream);
                    var worksheet = workbook.Worksheets.First();

                    if (!TieneColumnasDuplicadas(worksheet.Row(1).CellsUsed()))
                    {
                        foreach (var headerCell in worksheet.Row(1).CellsUsed())
                        {
                            dt.Columns.Add(headerCell.Value.ToString());
                        }

                        int r = 0;
                        await Clients.Caller.SendAsync("ProcesoInicial", worksheet.RowsUsed().Skip(1).Count());

                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            r++;

                            int i = 0;
                            var dataRow = dt.NewRow();

                            foreach (var cell in row.Cells())
                            {
                                dataRow[i] = cell.Value;
                                i++;
                            }

                            //dt.Rows.Add(dataRow);

                            if (parametros.FindAll(x => x.IdTipoBimboConcepto == (int)BimboConcepto.MotivoAjuste).Exists(x => x.Nombre.ToUpper() == dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.MotivoAjuste).Descripcion].ToString().Trim().ToUpper())
                                && parametros.FindAll(x => x.IdTipoBimboConcepto == (int)BimboConcepto.TipoEstoque).Exists(x => x.Nombre.ToUpper() == dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.TipoEstoque).Descripcion].ToString().Trim().ToUpper()))
                            {
                                MovimientoBimbo mb = new();

                                if (dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.CanalVenta).Descripcion] != DBNull.Value)
                                {
                                    mb.CanalVenta = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.CanalVenta).Descripcion].ToString()?.Trim();

                                    if (int.TryParse(mb.CanalVenta, out int canalVenta))
                                    {
                                        var dtCV = daCV.ObtenerCanalesVentas(canalVenta, null);
                                        if (dtCV.Rows.Count > 0)
                                            mb.IdCanalVenta = (int?)dtCV.Rows[0]["IdCanalVenta"];
                                    }
                                }

                                if (dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.NroRemito).Descripcion] != DBNull.Value
                                    && string.IsNullOrEmpty(dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.NroRemito).Descripcion].ToString()?.Trim()))
                                    mb.NumeroRemito = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.NroRemito).Descripcion].ToString()?.Trim();

                                mb.CodigoSku = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.CodigoSku).Descripcion].ToString()?.Trim();
                                if (int.TryParse(mb.CodigoSku, out int sku))
                                {
                                    var dtSku = daSKU.ObtenerSKU(null, sku, null, null, null, null);
                                    if (dtSku.Rows.Count > 0)
                                    {
                                        mb.IdSku = (int?)dtSku.Rows[0]["IdSKU"];
                                        mb.NombreSkuOriginal = (string)dtSku.Rows[0]["Nombre"];
                                    }
                                }

                                mb.NombreSku = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.NombreSku).Descripcion].ToString();
                                mb.Cantidad = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.Cantidad).Descripcion].ToString()?.Trim();
                                mb.TipoEstoque = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.TipoEstoque).Descripcion].ToString()?.Trim();
                                mb.MotivoAjuste = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.MotivoAjuste).Descripcion].ToString()?.Trim();

                                await Clients.Caller.SendAsync("ActualziarGrid", mb);
                            }

                            await Clients.Caller.SendAsync("ProcesoActualizado", r);
                        }
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("ColumnasDuplicadas");
                        return;
                    }
                }

                await Clients.Caller.SendAsync("ProcesoTerminado");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ProcesoConError", ex);
            }
        }

        public static bool TieneColumnasDuplicadas(IXLCells cells)
        {
            HashSet<string> nombresColumnas = new();
            foreach (var cell in cells)
            {
                string nombreColumna = cell.GetString();
                if (!string.IsNullOrEmpty(nombreColumna))
                {
                    if (!nombresColumnas.Add(nombreColumna))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
