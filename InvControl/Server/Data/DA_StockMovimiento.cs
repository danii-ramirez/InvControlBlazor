using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_StockMovimiento
    {
        public void InsertarStockMovimientos(int idTipoMovimiento, int idSku, int codigoSku, string nombreSku, int cantidad, string referencia,
            DateTime fechaMovimiento, int idUsuario, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_StockMovimientos";
            cmd.Parameters.AddWithValue("@pIdTipoMovimiento", idTipoMovimiento);
            cmd.Parameters.AddWithValue("@pIdSKU", idSku);
            cmd.Parameters.AddWithValue("@pCodigoSKU", codigoSku);
            cmd.Parameters.AddWithValue("@pNombreSKU", nombreSku);
            cmd.Parameters.AddWithValue("@pCantidad", cantidad);
            cmd.Parameters.AddWithValue("@pReferencia", referencia);
            cmd.Parameters.AddWithValue("@pFechaMovimiento", fechaMovimiento);
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cmd.ExecuteNonQuery();
        }
    }
}
