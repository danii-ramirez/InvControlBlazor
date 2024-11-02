using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_CanalVenta
    {
        private readonly string connectionString;

        public DA_CanalVenta(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerCanalesVentas(int? codigo, string nombre)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_CanalesVentas";
                if (codigo != null) cmd.Parameters.AddWithValue("@pCodigo", codigo);
                if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarCanalesVentas(int codigo, string nombre, string descripcion, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_CanalesVentas";
            cmd.Parameters.AddWithValue("@pCodigo", codigo);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            if (descripcion != null) cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            SqlParameter returnValue = new("@returnValue", result)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returnValue);
            cmd.ExecuteNonQuery();
            result = (int)returnValue.Value;
            return result;
        }

        public void ModificarCanalesVentas(int idCanalVenta, int codigo, string nombre, string descripcion, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_CanalesVentas";
            cmd.Parameters.AddWithValue("@pIdCanalVenta", idCanalVenta);
            cmd.Parameters.AddWithValue("@pCodigo", codigo);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            if (descripcion != null) cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.ExecuteNonQuery();
        }
    }
}
