using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Transporte
    {
        private readonly string connectionString;

        public DA_Transporte(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerTransportes(string nombre, string patente, bool? activo)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Transportes";
                if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
                if (patente != null) cmd.Parameters.AddWithValue("@pPatente", patente);
                if (activo != null) cmd.Parameters.AddWithValue("@pActivo", activo);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarTransportes(string nombre, string patente, bool activo, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Transportes";
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pPatente", patente);
            cmd.Parameters.AddWithValue("@pActivo", activo);
            SqlParameter returnValue = new("@returnValue", result)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returnValue);
            cmd.ExecuteNonQuery();
            result = (int)returnValue.Value;
            return result;
        }

        public void ModificarTransportes(int idTransporte, string nombre, string patente, bool activo, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_Transportes";
            cmd.Parameters.AddWithValue("@pIdTransporte", idTransporte);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pPatente", patente);
            cmd.Parameters.AddWithValue("@pActivo", activo);
            cmd.ExecuteNonQuery();
        }
    }
}
