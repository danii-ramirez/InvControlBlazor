using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_TipoContenedor
    {
        private readonly string connectionString;

        public DA_TipoContenedor(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerTiposContendores(int? idTipoContenedor, string nombre)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_TipoContenedor";
                if (idTipoContenedor != null) cmd.Parameters.AddWithValue("@pIdTipoContenedor", idTipoContenedor);
                if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarTipoContenedor(string nombre, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_TipoContenedor";
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            SqlParameter returnValue = new("@returnValue", result)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returnValue);
            cmd.ExecuteNonQuery();
            result = (int)returnValue.Value;
            return result;
        }

        public void ActualziarTipoContenedor(int idTipoContenedor, string nombre, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_TipoContenedor";
            cmd.Parameters.AddWithValue("@pIdTipoContenedor", idTipoContenedor);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.ExecuteNonQuery();
        }

        public void EliminarTipoContenedor(int idTipoContenedor, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_TipoContenedor";
            cmd.Parameters.AddWithValue("@pIdTipoContenedor", idTipoContenedor);
            cmd.ExecuteNonQuery();
        }
    }
}
