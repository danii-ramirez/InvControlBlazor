using System.Data;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Data.SqlClient;

namespace InvControl.Server.Data
{
    public class DA_Stock
    {
        private readonly string connectionString;

        public DA_Stock(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerStockPorSKU(int idSku, SqlTransaction? transaction = null)
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

        public DataTable ObtenerStock(string? nombre, int? idMarca, bool? especial, int? cantidadMin, int? cantidadMax, DateTime? fechaMin, DateTime? fechaMax)
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
    }
}
