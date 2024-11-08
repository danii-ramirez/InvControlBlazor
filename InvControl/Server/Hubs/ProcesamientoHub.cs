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
            DA_CanalVenta daCV = new(connectionString);
            DA_SKU daSKU = new(connectionString);
            DA_Stock daStock = new(connectionString);
            var parametros = new ParametrosController(connectionString: connectionString).ObtenerParametrosBimbo();
            List<MovimientoBimbo> movimientos = new();

            //daStock.EliminarMovimientosBimbo(idUsuario);

            DataTable dt = new();
            using (var stream = new MemoryStream(excelData))
            {
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

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
                            if (int.TryParse(dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.CanalVenta).Descripcion].ToString(), out int canalVenta))
                            {
                                mb.CanalVenta = canalVenta;
                                mb.CanalVentaNoEncontrado = daCV.ObtenerCanalesVentas(mb.CanalVenta, null).Rows.Count == 0;
                            }
                            else
                                mb.CanalVentaNoEncontrado = true;
                        }

                        if (dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.NroRemito).Descripcion] != DBNull.Value)
                            mb.NumeroRemito = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.NroRemito).Descripcion].ToString();

                        if (int.TryParse(dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.CodigoSku).Descripcion].ToString(), out int sku))
                        {
                            mb.CodigoSku = sku;
                            mb.SkuNoEncontrado = daSKU.ObtenerSKU(null, mb.CodigoSku, null, null, null, null).Rows.Count == 0;
                        }
                        else
                            mb.SkuNoEncontrado = true;

                        mb.NombreSku = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.NombreSku).Descripcion].ToString();

                        if (int.TryParse(dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.Cantidad).Descripcion].ToString(), out int cantidad))
                            mb.Cantidad = cantidad;

                        mb.TipoEstoque = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.TipoEstoque).Descripcion].ToString();
                        mb.MotivoAjuste = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.MotivoAjuste).Descripcion].ToString();

                        //daStock.InsertarMovimientosBimbo(idUsuario, mb.CanalVenta, mb.NumeroRemito?.Trim(), mb.CodigoSku, mb.NombreSku.Trim(), mb.Cantidad, mb.TipoEstoque, mb.MotivoAjuste);

                        await Clients.Caller.SendAsync("ActualziarGrid", mb);
                    }

                    await Clients.Caller.SendAsync("ProcesoActualizado", r);
                }
            }

            await Clients.Caller.SendAsync("ProcesoTerminado");
        }
    }
}
