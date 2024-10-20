using System.Data;
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
    }
}
