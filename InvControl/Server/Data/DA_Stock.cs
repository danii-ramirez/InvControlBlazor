using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Stock
    {
        private readonly string connectionString;

        public DA_Stock(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerStockPorSKU(int idSku, SqlTransaction transaction = null)
        {
            DataTable dt = new();
            SqlConnection cnn;
            SqlCommand cmd;
            if (transaction == null)
            {
                cnn = new SqlConnection(connectionString);
                cmd = cnn.CreateCommand();
            }
            else
            {
                cnn = transaction.Connection;
                cmd = cnn.CreateCommand();
                cmd.Transaction = transaction;
            }
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_get_StockPorSKU";
            cmd.Parameters.AddWithValue("@pIdSKU", idSku);
            SqlDataAdapter da = new(cmd);
            da.Fill(dt);
            return dt;
        }

        public void InsertarStock(int idSku, int cantidad, DateTime fechaActualizacion, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Stock";
            cmd.Parameters.AddWithValue("@pIdSKU", idSku);
            cmd.Parameters.AddWithValue("@pCantidad", cantidad);
            cmd.Parameters.AddWithValue("@pFechaActualizacion", fechaActualizacion);
            cmd.ExecuteNonQuery();
        }

        public DataTable ObtenerStock(string nombre, int? idMarca, bool? especial, int? cantidadMin, int? cantidadMax, DateTime? fechaMin, DateTime? fechaMax)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Stock";
                if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
                if (idMarca != null) cmd.Parameters.AddWithValue("@pIdMarca", idMarca);
                if (especial != null) cmd.Parameters.AddWithValue("@pEspecial", especial);
                if (cantidadMin != null) cmd.Parameters.AddWithValue("@pCantidadMin", cantidadMin);
                if (cantidadMax != null) cmd.Parameters.AddWithValue("@pCantidadMax", cantidadMax);
                if (fechaMin != null) cmd.Parameters.AddWithValue("@pFechaMin", fechaMin);
                if (fechaMax != null) cmd.Parameters.AddWithValue("@pFechaMax", fechaMax);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

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

        public void InsertarMovimientosBimbo(int idUsuario, int? canalVenta, string numeroRemito, int codigoSku, string nombreSku, int cantidad, string tipoEstoque,
            string motivoAjuste)
        {
            var cnn = new SqlConnection(connectionString);
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_BimboMovimientos";
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            if (canalVenta != null) cmd.Parameters.AddWithValue("@pCanalVenta", canalVenta);
            if (numeroRemito != null) cmd.Parameters.AddWithValue("@pNumeroRemito", numeroRemito);
            cmd.Parameters.AddWithValue("@pCodigoSKU", codigoSku);
            cmd.Parameters.AddWithValue("@pNombreSKU", nombreSku);
            cmd.Parameters.AddWithValue("@pCantidad", cantidad);
            cmd.Parameters.AddWithValue("@pTipoEstoque", tipoEstoque);
            cmd.Parameters.AddWithValue("@pMotivoAjuste", motivoAjuste);
            cnn.Open();
            cmd.ExecuteNonQuery();
            cnn.Close();
        }

        public void EliminarMovimientosBimbo(int idUsuario)
        {
            var cnn = new SqlConnection(connectionString);
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_BimboMovimientos";
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cnn.Open();
            cmd.ExecuteNonQuery();
            cnn.Close();
        }
    }
}
