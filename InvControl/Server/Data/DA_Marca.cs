using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Marca
    {
        private readonly string connectionString;

        public DA_Marca(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerMarcas(int? idMarca, string descripcion)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Marcas";
                if (idMarca != null) cmd.Parameters.AddWithValue("@pIdMarca", idMarca);
                if (descripcion != null) cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarMarca(string descripcion, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Marcas";
            cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            SqlParameter returnValue = new("@returnValue", result)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returnValue);
            cmd.ExecuteNonQuery();
            result = (int)returnValue.Value;
            return result;
        }

        public void ActualziarMarca(int idMarca, string descripcion, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_Marcas";
            cmd.Parameters.AddWithValue("@pIdMarca", idMarca);
            cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.ExecuteNonQuery();
        }

        public void EliminarMarca(int idMarca, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_Marcas";
            cmd.Parameters.AddWithValue("@pIdMarca", idMarca);
            cmd.ExecuteNonQuery();
        }
    }
}
