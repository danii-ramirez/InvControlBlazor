using ClosedXML.Excel;
using InvControl.Server.Controllers;
using InvControl.Server.Data;
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
            DA_Stock da = new(connectionString);

            DataTable dt = new();
            using (var stream = new MemoryStream(excelData))
            {
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                foreach (var headerCell in worksheet.Row(1).CellsUsed())
                {
                    dt.Columns.Add(headerCell.Value.ToString());
                }

                int r = 1, totalRow = worksheet.RowsUsed().Count();
                await Clients.Caller.SendAsync("ProgresoActualizado", r, totalRow);

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    int i = 0;
                    var dataRow = dt.NewRow();

                    foreach (var cell in row.Cells())
                    {
                        dataRow[i] = cell.Value;
                        i++;
                    }

                    dt.Rows.Add(dataRow);

                    //da.InsertarMovimientoBimbo((int)dataRow[6], (int)dataRow[12], (string)dataRow[13], (string)dataRow[16], (string)dataRow[17]);

                    r++;
                    await Clients.Caller.SendAsync("ProgresoActualizado", r, totalRow);
                }
            }

            //List<MovimientoBimbo> movimientos = new();

            //using (var stream = new MemoryStream(excelData))
            //{
            //    using (var workbook = new XLWorkbook(stream))
            //    {
            //        var worksheet = workbook.Worksheet(1);
            //        var rows = worksheet.RangeUsed().RowsUsed();

            //        int r = 1, totalRow = worksheet.RowsUsed().Count();

            //        foreach (var dr in rows.Skip(1))
            //        {
            //            MovimientoBimbo mb = new();
            //            //mb.Canal = (int)dr["CANAL"];
            //            mb.CodigoSku = dr.Cell(12).GetValue<int>();

            //            movimientos.Add(mb);

            //            r++;
            //            await Clients.Caller.SendAsync("ProgresoActualizado", r, totalRow);
            //        }
            //    }
            //}

            await Clients.Caller.SendAsync("ProcesamientoCompleto", "Procesamiento terminado");

            //var lst = new ParametrosController(connectionString: connectionString).ObtenerParametrosBimbo();

            //List<MovimientoBimbo> movimientos = new();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    //if (lst.FindAll(x => x.IdTipoBimboConcepto).Exists(x => x.Nombre.ToUpper() == dr["MOTIVO_AJUSTE"].ToString().Trim().ToUpper())
            //    //    && lst.FindAll(x => !x.IdTipoBimboConcepto).Exists(x => x.Nombre.ToUpper() == dr["TIPO_ESTOQUE"].ToString().Trim().ToUpper()))
            //    //{
            //    //    MovimientoBimbo mb = new();
            //    //    if (dr["CANAL"] != DBNull.Value) mb.Canal = dr["CANAL"].ToString();
            //    //    mb.CodigoSku = dr["COD_PRODUCTO"].ToString();
            //    //    mb.NombreSku = dr["NOMBRE_PRODUCTO"].ToString();
            //    //    mb.Cantidad = dr["CANTIDAD"].ToString();
            //    //    mb.TipoEstoque = dr["TIPO_ESTOQUE"].ToString();
            //    //    mb.MotivoAjuste = dr["MOTIVO_AJUSTE"].ToString();

            //    //    movimientos.Add(mb);
            //    //}
            //}

            //await Clients.Caller.SendAsync("MostarTabla", movimientos);
        }
    }
}
