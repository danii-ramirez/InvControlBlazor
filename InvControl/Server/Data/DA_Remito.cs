using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Remito
    {
        private readonly string connectionString;

        public DA_Remito(string connectionString) => this.connectionString = connectionString;

        public DataTable ObtenerRemitos(int? idRemito, string numeroRemito, int? idEstado)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Remitos";
                if (idRemito != null) cmd.Parameters.AddWithValue("@pIdRemito", idRemito);
                if (numeroRemito != null) cmd.Parameters.AddWithValue("@pNumero", numeroRemito);
                if (idEstado != null) cmd.Parameters.AddWithValue("@pIdEstado", idEstado);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable ObtenerRemitosDetalle(int idRemito)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_RemitosDetalle";
                cmd.Parameters.AddWithValue("@pIdRemito", idRemito);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarRemito(string numero, DateTime fecha, int? idTransporte, int? idChofer, int idEstado, int idUsuario,
            DateTime altaRegistro, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Remitos";
            cmd.Parameters.AddWithValue("@pNumero", numero);
            cmd.Parameters.AddWithValue("@pFecha", fecha);
            if (idTransporte != null) cmd.Parameters.AddWithValue("@pIdTransporte", idTransporte);
            if (idChofer != null) cmd.Parameters.AddWithValue("@pIdChofer", idChofer);
            cmd.Parameters.AddWithValue("@pIdEstado", idEstado);
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@pAltaRegistro", altaRegistro);
            SqlParameter returnValue = new("@returnValue", result)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returnValue);
            cmd.ExecuteNonQuery();
            result = (int)returnValue.Value;
            return result;
        }

        public void ActualizarRemito(int idRemito, int idEstado, int? idTransporte, int? idChofer, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_Remitos";
            cmd.Parameters.AddWithValue("@pIdRemito", idRemito);
            cmd.Parameters.AddWithValue("@pIdEstado", idEstado);
            if (idTransporte != null) cmd.Parameters.AddWithValue("@pIdTransporte", idTransporte);
            if (idChofer != null) cmd.Parameters.AddWithValue("@pIdChofer", idChofer);
            cmd.ExecuteNonQuery();
        }

        public void EliminarRemito(int idRemito, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_Remitos";
            cmd.Parameters.AddWithValue("@pIdRemito", idRemito);
            cmd.ExecuteNonQuery();
        }

        public void InsertarRemitoDetalle(int idRemito, int idSku, int codigoSku, string nombreSku, int cantidad, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_RemitosDetalle";
            cmd.Parameters.AddWithValue("@pIdRemito", idRemito);
            cmd.Parameters.AddWithValue("@pIdSKU", idSku);
            cmd.Parameters.AddWithValue("@pCodigoSKU", codigoSku);
            cmd.Parameters.AddWithValue("@pNombreSKU", nombreSku);
            cmd.Parameters.AddWithValue("@pCantidad", cantidad);
            cmd.ExecuteNonQuery();
        }

        public void EliminarRemitoDetalle(int idRemito, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_RemitosDetalle";
            cmd.Parameters.AddWithValue("@pIdRemito", idRemito);
            cmd.ExecuteNonQuery();
        }

        public void ActualziarRemitoEstado(int idRemito, int idEstado, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_RemitosEstado";
            cmd.Parameters.AddWithValue("@pIdRemito", idRemito);
            cmd.Parameters.AddWithValue("@pIdEstado", idEstado);
            cmd.ExecuteNonQuery();
        }
    }
}
