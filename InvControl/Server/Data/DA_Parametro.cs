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
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_Parametros";
            cmd.Parameters.AddWithValue("@pIdParametro", idParametro);
            cmd.Parameters.AddWithValue("@pValor", valor);
            cmd.ExecuteNonQuery();
        }

        public DataTable ObtenerParametrosBimbo(int? idParametroBimbo, string nombre, int? idTipoBimboConcepto)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_BimboParametros";
                if (idParametroBimbo != null) cmd.Parameters.AddWithValue("@pIdBimboParametro", idParametroBimbo);
                if (nombre != null) cmd.Parameters.AddWithValue("@pNombre", nombre);
                if (idTipoBimboConcepto != null) cmd.Parameters.AddWithValue("@pIdTipoBimboConcepto", idTipoBimboConcepto);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int InsertarParametrosBimbo(string nombre, string descripcion, int idTipoBimboConcepto, SqlTransaction transaction)
        {
            int result = 0;
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_BimboParametros";
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            if (descripcion != null) cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.Parameters.AddWithValue("@pIdTipoBimboConcepto", idTipoBimboConcepto);
            SqlParameter returnValue = new("@returnValue", result) { Direction = ParameterDirection.ReturnValue };
            cmd.Parameters.Add(returnValue);
            cmd.ExecuteNonQuery();
            result = (int)returnValue.Value;
            return result;
        }

        public void ActualizarParametroBimbo(int idParametroBimbo, string nombre, string descripcion, int idTipoBimboConcepto, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_upd_BimboParametros";
            cmd.Parameters.AddWithValue("@pIdBimboParametro", idParametroBimbo);
            cmd.Parameters.AddWithValue("@pNombre", nombre);
            if (descripcion != null) cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.Parameters.AddWithValue("@pIdTipoBimboConcepto", idTipoBimboConcepto);
            cmd.ExecuteNonQuery();
        }

        public void EliminarParametroBimbo(int idParametroBimbo, int idTipoBimboConcepto, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_del_BimboParametros";
            cmd.Parameters.AddWithValue("@pIdBimboParametro", idParametroBimbo);
            cmd.Parameters.AddWithValue("@pIdTipoBimboConcepto", idTipoBimboConcepto);
            cmd.ExecuteNonQuery();
        }
    }
}
