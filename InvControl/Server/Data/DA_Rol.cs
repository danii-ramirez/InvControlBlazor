using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Rol
    {
        private readonly string connectionString;

        public DA_Rol(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerRoles(int? idRol, string descripcion)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Roles";
                if (idRol != null) cmd.Parameters.AddWithValue("@pIdRol", idRol);
                if (descripcion != null) cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarRol(string descripcion, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Rol";
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

        public void ActualizarRol(int idRol, string descripcion, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_Rol";
            cmd.Parameters.AddWithValue("@pIdRol", idRol);
            cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.ExecuteNonQuery();
        }

        public bool EliminarRol(int idRol, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_Rol";
            cmd.Parameters.AddWithValue("@pIdRol", idRol);
            SqlParameter restult = new("@pResult", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(restult);
            cmd.ExecuteNonQuery();
            return bool.Parse(restult.Value.ToString());
        }

        public DataTable ObtenerPermisosPorRol(int idRol)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_RolesPorPermiso";
                cmd.Parameters.AddWithValue("@pIdRol", idRol);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public void InsertarRolesPorPermiso(int idRol, int idPermiso, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_RolesPorPermiso";
            cmd.Parameters.AddWithValue("@pIdRol", idRol);
            cmd.Parameters.AddWithValue("@pIdPermiso", idPermiso);
            cmd.ExecuteNonQuery();
        }

        public void EliminarRolesPorPermiso(int idRol, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_RolesPorPermiso";
            cmd.Parameters.AddWithValue("@pIdRol", idRol);
            cmd.ExecuteNonQuery();
        }

        public DataTable ObtenerPermisos()
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Permisos";
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}
