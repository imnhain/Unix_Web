using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Text.RegularExpressions;
using Unix_Web.Helpers;

namespace Unix_Web.Providers
{
    /// <summary>
    /// Kết nối Informix (Unix ERP) qua ODBC.
    /// Connection string được đọc từ Web.config (appSettings["UnixConn.ConnectionString"])
    /// thay vì hardcode trong code.
    /// </summary>
    public class UnixConn
    {
        private static readonly Regex ParamTokenRegex = new Regex(@"@\w+", RegexOptions.Compiled);

        private static string ConnectionString
        {
            get
            {
                string cs = ConfigurationManager.AppSettings["UnixConn.ConnectionString"];
                if (string.IsNullOrEmpty(cs))
                {
                    throw new ConfigurationErrorsException(
                        "Không tìm thấy 'UnixConn.ConnectionString' trong Web.config (appSettings).");
                }
                return cs;
            }
        }

        private static void BindParameters(OdbcCommand cmd, string query, object[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return;

            var tokens = new List<string>();
            foreach (Match m in ParamTokenRegex.Matches(query))
            {
                if (!tokens.Contains(m.Value))
                    tokens.Add(m.Value);
            }

            if (tokens.Count == 0) return;

            int count = Math.Min(tokens.Count, parameters.Length);
            for (int i = 0; i < count; i++)
            {
                cmd.Parameters.AddWithValue(tokens[i], parameters[i] ?? DBNull.Value);
            }
        }

        public static DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            using (var conn = new OdbcConnection(ConnectionString))
            using (var cmd = new OdbcCommand(query, conn))
            {
                try
                {
                    BindParameters(cmd, query, parameter);
                    conn.Open();
                    var dt = new DataTable();
                    using (var adapter = new OdbcDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    Logger.LogError("UnixConn.ExecuteQuery", ex);
                    return new DataTable();
                }
            }
        }

        public static bool ExecuteNonQuery(string query, object[] parameter = null)
        {
            using (var conn = new OdbcConnection(ConnectionString))
            using (var cmd = new OdbcCommand(query, conn))
            {
                try
                {
                    BindParameters(cmd, query, parameter);
                    conn.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows > 0;
                }
                catch (Exception ex)
                {
                    Logger.LogError("UnixConn.ExecuteNonQuery", ex);
                    return false;
                }
            }
        }
    }
}
