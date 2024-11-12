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

        public DataTable ObtenerUsuario(int? idUsuario, string user)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Usuarios";
                if (idUsuario != null) cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
                if (user != null) cmd.Parameters.AddWithValue("@pUser", user);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarUsuario(string user, string pass, string nombre, string apellido, bool activo, int idRol, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Usuario";
            cmd.Parameters.AddWithValue("@pUser", user);
            cmd.Parameters.AddWithValue("@pPass", pass);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pApellido", apellido);
            cmd.Parameters.AddWithValue("@pActivo", activo);
            cmd.Parameters.AddWithValue("@pIdRol", idRol);
            SqlParameter returnValue = new("@returnValue", result)
            {
                Direction = ParameterDirection.ReturnValue,
            };
            cmd.Parameters.Add(returnValue);
            cmd.ExecuteNonQuery();
            result = (int)returnValue.Value;
            return result;
        }

        public void ModificarUsuario(int idUsuario, string user, string nombre, string apellido, bool activo, int idRol, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_Usuario";
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@pUser", user);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pApellido", apellido);
            cmd.Parameters.AddWithValue("@pActivo", activo);
            cmd.Parameters.AddWithValue("@pIdRol", idRol);
            cmd.ExecuteNonQuery();
        }

        public bool EliminarUsuario(int idUsuario, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_Usuario";
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            SqlParameter restult = new("@pResult", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(restult);
            cmd.ExecuteNonQuery();
            return bool.Parse(restult.Value.ToString());
        }

        public void RestablecerPass(int idUsuario, string pass)
        {
            SqlConnection cnn = new(connectionString);
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_UsuariosRestablecerPass";
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@pPass", pass);
            cnn.Open();
            cmd.ExecuteNonQuery();
            cnn.Close();
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

        public DataTable ObtenerMenu(int idUsuario)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Menu";
                cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool ValidarAcceso(int idUsuario, string url)
        {
            bool result = false;
            using (var cnn = new SqlConnection(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_ValidarAcceso";
                cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@pUrl", url);
                cnn.Open();
                result = (bool)cmd.ExecuteScalar();
                cnn.Close();
            }

            return result;
        }

        public DataTable ValidarAccion(int idUsuario, string url)
        {
            DataTable dt = new();
            using (SqlConnection connection = new(connectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_ValidarAccion";
                cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@pUrl", url);
                cmd.CommandTimeout = 30;
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}
