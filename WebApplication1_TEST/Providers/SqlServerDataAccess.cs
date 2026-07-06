using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Unix_Web.Helpers;

namespace Unix_Web.Providers
{
    /// <summary>
    /// Lớp dùng chung cho các kết nối SQL Server (được dùng bởi SQLConn33, SQLConn34, ...).
    /// Trước đây mỗi server có 1 class riêng (SQLConn33, SQLConn34) với code giống hệt nhau,
    /// chỉ khác connection string -> gom về đây để sửa 1 lần dùng cho tất cả.
    ///
    /// Connection string được đọc từ Web.config (connectionStrings) thay vì hardcode trong code,
    /// để đổi IP/user/pass không cần build lại project, và tách được config theo môi trường
    /// (Web.Debug.config / Web.Release.config).
    /// </summary>
    internal static class SqlServerDataAccess
    {
        // Khớp các tham số dạng @paramName trong câu SQL (đúng chuẩn SqlClient).
        // Trước đây code cũ tìm ký tự '?' bằng query.Split(' '), vốn không phải cú pháp
        // tham số hợp lệ của SqlCommand nên không thực sự hoạt động.
        private static readonly Regex ParamTokenRegex = new Regex(@"@\w+", RegexOptions.Compiled);

        private static string GetConnectionString(string connectionStringName)
        {
            var setting = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (setting == null)
            {
                throw new ConfigurationErrorsException(
                    $"Không tìm thấy connection string '{connectionStringName}' trong Web.config.");
            }
            return setting.ConnectionString;
        }

        /// <summary>
        /// Gán tham số vào SqlCommand theo đúng thứ tự token @param xuất hiện trong câu query.
        /// Nếu query không chứa token @param nào (đa số trường hợp hiện tại trong project, do
        /// các trang tự nối chuỗi sẵn), thì bỏ qua - giữ hành vi tương thích ngược.
        /// </summary>
        private static void BindParameters(SqlCommand cmd, string query, object[] parameters)
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

        public static object ExecuteScalar(string connectionStringName, string query, object[] parameters = null)
        {
            string connectionString = GetConnectionString(connectionStringName);
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                try
                {
                    BindParameters(cmd, query, parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{connectionStringName}.ExecuteScalar", ex);
                    return null;
                }
            }
        }

        public static DataTable ExecuteQuery(string connectionStringName, string query, object[] parameters = null)
        {
            string connectionString = GetConnectionString(connectionStringName);
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                try
                {
                    BindParameters(cmd, query, parameters);
                    var dt = new DataTable();
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{connectionStringName}.ExecuteQuery", ex);
                    return new DataTable();
                }
            }
        }

        public static bool ExecuteNonQuery(string connectionStringName, string query, object[] parameters = null)
        {
            string connectionString = GetConnectionString(connectionStringName);
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                try
                {
                    BindParameters(cmd, query, parameters);
                    conn.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows > 0;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{connectionStringName}.ExecuteNonQuery", ex);
                    return false;
                }
            }
        }

        public static bool CheckConnect(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{connectionStringName}.CheckConnect", ex);
                    return false;
                }
            }
        }
    }
}
