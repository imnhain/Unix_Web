using System.Data;

namespace Unix_Web.Providers
{
    /// <summary>
    /// Kết nối SQL Server 198.1.10.34 (ERP).
    /// Chỉ là lớp mỏng trỏ vào SqlServerDataAccess với tên connection string tương ứng -
    /// logic thực tế nằm ở SqlServerDataAccess (dùng chung với SQLConn33) để tránh trùng lặp code.
    /// Giữ nguyên tên class + chữ ký hàm để các trang .aspx.cs hiện tại không cần sửa.
    /// </summary>
    public class SQLConn34
    {
        private const string ConnectionStringName = "SQLSERVER_34";

        public static object ExecuteScalar(string query, object[] parameter = null)
            => SqlServerDataAccess.ExecuteScalar(ConnectionStringName, query, parameter);

        public static DataTable ExecuteQuery(string query, object[] parameter = null)
            => SqlServerDataAccess.ExecuteQuery(ConnectionStringName, query, parameter);

        public static bool ExecuteNonQuery(string query, object[] parameter = null)
            => SqlServerDataAccess.ExecuteNonQuery(ConnectionStringName, query, parameter);

        public static bool CheckConnectSQL(string ip = null)
            => SqlServerDataAccess.CheckConnect(ConnectionStringName);
    }
}
