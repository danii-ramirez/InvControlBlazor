using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Parametro
    {
        private readonly string connectionString;

        public DA_Parametro(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerParametros(int? idParametro, string nombre)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Parametros";
                if (idParametro != null) cmd.Parameters.AddWithValue("@pIdParametro", idParametro);
                if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public void ActualizarParametro(int idParametro, string valor, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = "prc_upd_Parametros";
            cmd.Parameters.AddWithValue("@pIdParametro", idParametro);
            cmd.Parameters.AddWithValue("@pValor", valor);
            cmd.ExecuteNonQuery();
        }
    }
}
