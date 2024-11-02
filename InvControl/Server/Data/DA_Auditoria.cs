using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Auditoria
    {
        private readonly string connectionString;

        public DA_Auditoria(string connectionString) => this.connectionString = connectionString;

        public DataTable Obtener(int? idUsuario, int? idtipoEntidad, DateTime fechaDesde, DateTime fechaHasta)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Bitacora";
                if (idUsuario != null) cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
                if (idtipoEntidad != null) cmd.Parameters.AddWithValue("@pIdtipoEntidad", idtipoEntidad);
                cmd.Parameters.AddWithValue("@pFechaDesde", fechaDesde.ToString("dd/MM/yyyy hh:mm:ss"));
                cmd.Parameters.AddWithValue("@pFechaHasta", fechaHasta.ToString("dd/MM/yyyy hh:mm:ss"));
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public void Insertar(string descripcion, DateTime fecha, int idTipoEntidad, int idTipoOperacion, int idUsuario, SqlTransaction transaction = null)
        {
            SqlConnection cnn;
            SqlCommand cmd;
            if (transaction == null)
            {
                cnn = new(connectionString);
                cmd = cnn.CreateCommand();
            }
            else
            {
                cnn = transaction.Connection;
                cmd = cnn.CreateCommand();
                cmd.Transaction = transaction;
            }
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Bitacora";
            cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.Parameters.AddWithValue("@pFecha", fecha);
            cmd.Parameters.AddWithValue("@pIdTipoEntidad", idTipoEntidad);
            cmd.Parameters.AddWithValue("@pIdTipoOperacion", idTipoOperacion);
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            if (transaction == null)
            {
                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
            else
                cmd.ExecuteNonQuery();
        }
    }
}
