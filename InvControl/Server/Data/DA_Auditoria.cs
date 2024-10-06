﻿using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Data
{
    public class DA_Auditoria
    {
        private readonly string connectionString;

        public DA_Auditoria(string connectionString) => this.connectionString = connectionString;

        public DataTable Obtener(int? idtipoEntidad)
        {
            DataTable dt = new();
            using (SqlConnection cnn = new(connectionString))
            {
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "prc_get_Bitacora";
                if (idtipoEntidad != null) cmd.Parameters.AddWithValue("@pIdtipoEntidad", idtipoEntidad);
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public void Insertar(string descripcion, DateTime fecha, int idTipoEntidad, int idTipoOperacion, int idUsuario, SqlTransaction transaction)
        {
            var cnn = transaction.Connection;
            var cmd = cnn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "prc_ins_Bitacora";
            cmd.Parameters.AddWithValue("@pDescripcion", descripcion);
            cmd.Parameters.AddWithValue("@pFecha", fecha);
            cmd.Parameters.AddWithValue("@pIdTipoEntidad", idTipoEntidad);
            cmd.Parameters.AddWithValue("@pIdTipoOperacion", idTipoOperacion);
            cmd.Parameters.AddWithValue("@pIdUsuario", idUsuario);
            cmd.ExecuteNonQuery();
        }
    }
}