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
        private readonly List<ParametroBimbo> parametros;
        private readonly DA_CanalVenta daCV;
        private readonly DA_SKU daSKU;
        private const int batchSize = 10000;
        private const int maxConcurrentTasks = 4;
        private readonly SemaphoreSlim semaphore;

        public ProcesamientoHub(ILogger<ProcesamientoHub> logger, IConfiguration configuration)
        {
            _logger = logger;
            var connectionString = configuration.GetConnectionString("InvControlDB");
            parametros = new ParametrosController(connectionString: connectionString).ObtenerParametrosBimbo();
            daCV = new(connectionString);
            daSKU = new(connectionString);
            semaphore = new SemaphoreSlim(maxConcurrentTasks);
        }

        public async Task ProcesarMovimientosBimbo(byte[] excelData)
        {
            try
            {
                using (var stream = new MemoryStream(excelData))
                {
                    using var workbook = new XLWorkbook(stream);
                    var worksheet = workbook.Worksheets.First();

                    if (!TieneColumnasDuplicadas(worksheet.Row(1).CellsUsed()))
                    {
                        var mapeoColumnas = worksheet.Row(1).CellsUsed()
                            .Where(cell => parametros.FindAll(x => x.IdTipoBimboConcepto == (int)BimboConcepto.NombreColumna).Select(x => x.Descripcion)
                            .Contains(cell.Value.ToString()))
                            .ToDictionary(cell => cell.Value.ToString(), cell => cell.Address.ColumnNumber);

                        await Clients.Caller.SendAsync("ProcesoInicial", worksheet.RowsUsed().Skip(1).Count());

                        int processedRows = 0;
                        var tasks = new List<Task>();

                        foreach (var rowBatch in worksheet.RowsUsed().Skip(1).Chunk(batchSize))
                        {
                            await semaphore.WaitAsync();

                            var task = Task.Run(async () =>
                            {
                                try
                                {
                                    foreach (var row in rowBatch)
                                    {
                                        var dataRow = new Dictionary<string, string>();
                                        foreach (var columna in mapeoColumnas)
                                        {
                                            var cell = row.Cell(columna.Value);
                                            dataRow[columna.Key] = cell?.GetString()?.Trim();
                                        }

                                        if (FiltrarMovimiento(dataRow))
                                        {
                                            var movimiento = CrearMovimientoBimboAsync(dataRow);
                                            await Clients.Caller.SendAsync("ActualziarGrid", movimiento);
                                        }

                                        Interlocked.Increment(ref processedRows);
                                        await Clients.Caller.SendAsync("ProcesoActualizado", processedRows);
                                    }
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            });

                            tasks.Add(task);
                        }

                        await Task.WhenAll(tasks);
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

        private MovimientoBimbo CrearMovimientoBimboAsync(Dictionary<string, string> dataRow)
        {
            MovimientoBimbo mb = new();

            if (dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.CanalVenta).Descripcion] != null)
            {
                mb.CanalVenta = dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.CanalVenta).Descripcion].ToString()?.Trim();

                if (int.TryParse(mb.CanalVenta, out int canalVenta))
                {
                    var dtCV = daCV.ObtenerCanalesVentas(null, canalVenta, null);
                    if (dtCV.Rows.Count > 0)
                        mb.IdCanalVenta = (int?)dtCV.Rows[0]["IdCanalVenta"];
                }
            }

            if (dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.NroRemito).Descripcion] != null
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

            return mb;
        }

        private bool FiltrarMovimiento(Dictionary<string, string> dataRow)
        {
            return parametros.FindAll(x => x.IdTipoBimboConcepto == (int)BimboConcepto.MotivoAjuste)
                             .Exists(x => x.Nombre.ToUpper() == dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.MotivoAjuste).Descripcion].ToString().Trim().ToUpper())
                && parametros.FindAll(x => x.IdTipoBimboConcepto == (int)BimboConcepto.TipoEstoque)
                             .Exists(x => x.Nombre.ToUpper() == dataRow[parametros.Find(x => x.IdParametroBimbo == (int)BimboNombreColumna.TipoEstoque).Descripcion].ToString().Trim().ToUpper());
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
