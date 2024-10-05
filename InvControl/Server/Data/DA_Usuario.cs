using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Usuario
    {
        private readonly string connectionString;

        public DA_Usuario(string connectionString) => this.connectionString = connectionString;

        public DataTable Login(string user)
        {
            DataTable dt = new();
            using (SqlConnection connection = new(connectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Login";
                cmd.Parameters.AddWithValue("@pUser", user);
                cmd.CommandTimeout = 30;
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int IncrementarIntentosFallidos(int idUsuario)
        {
            int value = 0;
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_upd_UsuariosIncrementarIntentosFallidos";
                cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
                SqlParameter returValue = new("@intentosFallidos", value)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(returValue);
                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();
                value = (int)returValue.Value;
            }
            return value;
        }

        public void ResetearIntentosFallidos(int idUsuario)
        {
            SqlConnection cnn = new(connectionString);
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_UsuariosResetearIntentosFallidos";
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cnn.Open();
            cmd.ExecuteNonQuery();
            cnn.Close();
        }

        public DataTable ObtenerUsuario(int idUsuario)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Usuarios";
                cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public void ResetearPass(int idUsuario, string pass)
        {
            SqlConnection cnn = new(connectionString);
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_UsuariosResetearPass";
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@pPass", pass);
            cnn.Open();
            cmd.ExecuteNonQuery();
            cnn.Close();
        }
    }
}
