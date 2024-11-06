using ClosedXML.Excel;
using Microsoft.AspNetCore.SignalR;
using System.Data;

namespace InvControl.Server.Hubs
{
    public class ProcesamientoHub : Hub
    {
        private readonly ILogger<ProcesamientoHub> _logger;

        public ProcesamientoHub(ILogger<ProcesamientoHub> logger)
        {
            _logger = logger;
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

        public async Task ProcesarExcel(byte[] excelData)
        {
            DataTable dataTable = new();
            using (var stream = new MemoryStream(excelData))
            {
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                foreach (var headerCell in worksheet.Row(1).CellsUsed())
                {
                    dataTable.Columns.Add(headerCell.Value.ToString());
                }

                int r = 1, totalRow = worksheet.RowsUsed().Count();
                await Clients.Caller.SendAsync("ProgresoActualizado", r, totalRow);

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    int i = 0;
                    var dataRow = dataTable.NewRow();

                    foreach (var cell in row.Cells())
                    {
                        dataRow[i] = cell.Value;
                        i++;
                    }

                    dataTable.Rows.Add(dataRow);

                    r++;
                    await Clients.Caller.SendAsync("ProgresoActualizado", r, totalRow);
                }
            }

            await Clients.Caller.SendAsync("ProcesamientoCompleto", "Procesamiento terminado");
        }
    }
}
