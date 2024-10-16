using System.Data;
using Microsoft.Data.SqlClient;

namespace InvControl.Server.Data
{
    public class DA_Remito
    {
        private readonly string connectionString;

        public DA_Remito(string connectionString) => this.connectionString = connectionString;

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
            cmd.Parameters.AddWithValue("@pIdEstadoRemito", idEstado);
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

        public void InsertarRemitoDetalle(int idRemito, int idSku, string nombreSku, int cantidad, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_RemitosDetalle";
            cmd.Parameters.AddWithValue("@pIdRemito", idRemito);
            cmd.Parameters.AddWithValue("@pIdSku", idSku);
            cmd.Parameters.AddWithValue("@pNombreSku", nombreSku);
            cmd.Parameters.AddWithValue("@pCantidad", cantidad);
            cmd.ExecuteNonQuery();
        }
    }
}
