using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Chofer
    {
        private readonly string connectionString;

        public DA_Chofer(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerChoferes(int? idChofer, string nombre, string apellido, bool? activo)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Choferes";
                if (idChofer != null) cmd.Parameters.AddWithValue("@pIdChofer", idChofer);
                if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
                if (apellido != null) cmd.Parameters.AddWithValue("@pApellido", apellido);
                if (activo != null) cmd.Parameters.AddWithValue("@pActivo", activo);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarChofer(string nombre, string apellido, bool activo, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Choferes";
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pApellido", apellido);
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

        public void ModificarChofer(int idChofer, string nombre, string apellido, bool activo, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_Choferes";
            cmd.Parameters.AddWithValue("@pIdChofer", idChofer);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pApellido", apellido);
            cmd.Parameters.AddWithValue("@pActivo", activo);
            cmd.ExecuteNonQuery();
        }

        public bool EliminarChofer(int idChofer, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_Choferes";
            cmd.Parameters.AddWithValue("@pIdChofer", idChofer);
            SqlParameter restult = new("@pResult", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(restult);
            cmd.ExecuteNonQuery();
            return bool.Parse(restult.Value.ToString());
        }
    }
}
