using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_TipoMovimiento
    {
        private readonly string connectionString;

        public DA_TipoMovimiento(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerTipoMovimiento(int? idTipoMovimiento, string nombre, bool? soloLectura, bool? interno)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_TipoMovimiento";
                if (idTipoMovimiento != null) cmd.Parameters.AddWithValue("@pIdTipoMovimiento", idTipoMovimiento);
                if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
                if (soloLectura != null) cmd.Parameters.AddWithValue("@pSoloLectura", soloLectura);
                if (interno != null) cmd.Parameters.AddWithValue("@pInterno", interno);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarTipoMovimiento(string nombre, string tipo, bool soloLectura, bool interno, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_TipoMovimiento";
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pTipo", tipo);
            cmd.Parameters.AddWithValue("@pSoloLectura", soloLectura);
            cmd.Parameters.AddWithValue("@pInterno", interno);
            SqlParameter returnValue = new("@returnValue", result)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returnValue);
            cmd.ExecuteNonQuery();
            result = (int)returnValue.Value;
            return result;
        }

        public void ActualizarTipoMovimiento(int idTipoMovimiento, string nombre, string tipo, bool soloLectura, bool interno, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_TipoMovimiento";
            cmd.Parameters.AddWithValue("@pIdTipoMovimiento", idTipoMovimiento);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pTipo", tipo);
            cmd.Parameters.AddWithValue("@pSoloLectura", soloLectura);
            cmd.Parameters.AddWithValue("@pInterno", interno);
            cmd.ExecuteNonQuery();
        }

        public void EliminarTipoMovimiento(int idTipoMovimiento, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_TipoMovimiento";
            cmd.Parameters.AddWithValue("@pIdTipoMovimiento", idTipoMovimiento);
            cmd.ExecuteNonQuery();
        }
    }
}
