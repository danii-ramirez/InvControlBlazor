using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_SKU
    {
        private readonly string connectionString;

        public DA_SKU(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerSKU(int? idSku, int? codigo, string? nombre, bool? activo, int? idMarca, SqlTransaction? transaction = null)
        {
            DataTable dt = new();
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
            cmd.CommandText = "prc_get_SKU";
            if (idSku != null) cmd.Parameters.AddWithValue("@pIdSKU", idSku);
            if (codigo != null) cmd.Parameters.AddWithValue("@pCodigo", codigo);
            if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
            if (activo != null) cmd.Parameters.AddWithValue("@pActivo", activo);
            if (idMarca != null) cmd.Parameters.AddWithValue("@pIdMarca", idMarca);
            SqlDataAdapter da = new(cmd);
            da.Fill(dt);
            return dt;
        }

        public int InsertarSKU(int codigo, string nombre, string descripcion, bool activo, bool especial, int idMarca, int idTipoContenedor, int unidadesPorContenedor,
            int idUsuario, DateTime altaRegistro, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_SKU";
            cmd.Parameters.AddWithValue("@pCodigo", codigo);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            if (descripcion != null) cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.Parameters.AddWithValue("@pActivo", activo);
            cmd.Parameters.AddWithValue("@pEspecial", especial);
            cmd.Parameters.AddWithValue("@pIdMarca", idMarca);
            cmd.Parameters.AddWithValue("@pIdTipoContenedor", idTipoContenedor);
            cmd.Parameters.AddWithValue("@pUnidadesPorContenedor", unidadesPorContenedor);
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@pAltaRegistro", altaRegistro);
            SqlParameter returValue = new("@pReturn", result)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returValue);
            cmd.ExecuteNonQuery();
            result = (int)returValue.Value;
            return result;
        }

        public void ActualizarSKU(int idSku, int codigo, string nombre, string descripcion, bool activo, bool especial, int idMarca, int idTipoContenedor, int unidadesPorContenedor,
            SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_SKU";
            cmd.Parameters.AddWithValue("@pIdSKU", idSku);
            cmd.Parameters.AddWithValue("@pCodigo", codigo);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.Parameters.AddWithValue("@pActivo", activo);
            cmd.Parameters.AddWithValue("@pEspecial", especial);
            cmd.Parameters.AddWithValue("@pIdMarca", idMarca);
            cmd.Parameters.AddWithValue("@pIdTipoContenedor", idTipoContenedor);
            cmd.Parameters.AddWithValue("@pUnidadesPorContenedor", unidadesPorContenedor);
            cmd.ExecuteNonQuery();
        }

        public DataTable ObtenerMarcas()
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Marcas";
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable ObtenerTiposContendores()
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_TipoContenedor";
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable ObtenerSugerencias(string sugerencia)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_SKUSugerencia";
                cmd.Parameters.AddWithValue("@pSugerencia", sugerencia);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}
