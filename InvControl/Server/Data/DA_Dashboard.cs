using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Dashboard
    {
        private readonly string connectionString;

        public DA_Dashboard(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerRemitosEstados()
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_dash_RemitosEstados";
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}
