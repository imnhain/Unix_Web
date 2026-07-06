using System;
using System.IO;
using System.Text;
using System.Web;

namespace Unix_Web.Helpers
{
    /// <summary>
    /// Logger đơn giản, ghi lỗi ra file text theo ngày trong App_Data/Logs.
    /// Dùng để thay thế các khối "catch (Exception ex) { return ...; }" nuốt lỗi âm thầm,
    /// giúp còn dấu vết khi cần điều tra sự cố (đặc biệt là lỗi kết nối DB).
    /// </summary>
    public static class Logger
    {
        private static readonly object _lock = new object();

        private static string LogFolder =>
            HttpContext.Current != null
                ? HttpContext.Current.Server.MapPath("~/App_Data/Logs")
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Logs");

        /// <summary>
        /// Ghi log lỗi. "context" là tên hàm/nơi phát sinh lỗi, giúp dễ tìm hơn khi grep log.
        /// </summary>
        public static void LogError(string context, Exception ex)
        {
            WriteLine("ERROR", context, ex.ToString());
        }

        public static void LogWarning(string context, string message)
        {
            WriteLine("WARN", context, message);
        }

        public static void LogInfo(string context, string message)
        {
            WriteLine("INFO", context, message);
        }

        private static void WriteLine(string level, string context, string message)
        {
            try
            {
                lock (_lock)
                {
                    string folder = LogFolder;
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string filePath = Path.Combine(folder, $"log-{DateTime.Now:yyyyMMdd}.txt");

                    var sb = new StringBuilder();
                    sb.Append('[').Append(DateTime.Now.ToString("HH:mm:ss.fff")).Append("] ");
                    sb.Append('[').Append(level).Append("] ");
                    sb.Append('[').Append(context).Append("] ");
                    sb.Append(message);

                    File.AppendAllText(filePath, sb.ToString() + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // Logger tự thân không được phép làm sập request đang xử lý.
                // Nếu ghi log thất bại (vd. thiếu quyền ghi ổ đĩa), im lặng bỏ qua ở đây là chấp nhận được,
                // vì đây là "log của log" - không còn nơi nào khác để báo lỗi này nữa.
            }
        }
    }
}
