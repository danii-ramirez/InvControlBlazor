using ClosedXML.Excel;
using InvControl.Shared.Models;
using System.Data;

namespace InvControl.Server.Helpers
{
    internal static class Functions
    {
        internal static List<Permiso> ConstruirArbolPermiso(List<Permiso> permisos, int? idpadre = null)
        {
            var permisosHijos = permisos.Where(p => p.IdPadre == idpadre).ToList();
            foreach (var permiso in permisosHijos)
            {
                permiso.Permisos = ConstruirArbolPermiso(permisos, permiso.IdPermiso);
            }
            return permisosHijos;
        }

        internal static DataTable ConvertExcelToDataTable(byte[] excelData)
        {
            var dataTable = new DataTable();
            using (var stream = new MemoryStream(excelData))
            {
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                // Agrega las columnas
                foreach (var headerCell in worksheet.Row(1).CellsUsed())
                {
                    dataTable.Columns.Add(headerCell.Value.ToString());
                }

                // Agrega las filas
                foreach (var row in worksheet.RowsUsed().Skip(1)) // Saltar la fila de encabezados
                {
                    var dataRow = dataTable.NewRow();
                    int i = 0;

                    foreach (var cell in row.Cells())
                    {
                        dataRow[i] = cell.Value;
                        i++;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            return dataTable;
        }
    }
}
