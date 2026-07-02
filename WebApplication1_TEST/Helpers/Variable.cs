using System;

namespace Unix_Web.Helpers // Bạn có thể đổi namespace này theo cấu trúc thư mục thực tế
{
    public static class Variable
    {
        /// <summary>
        /// Lấy tháng chưa khóa (Tháng cần mở/update)
        /// Nếu <= mùng 5: Lùi 2 tháng. Nếu > mùng 5: Lùi 1 tháng.
        /// </summary>
        public static string month_chuakhoa()
        {
            DateTime today = DateTime.Now;
            if (today.Day <= 5)
            {
                return today.AddMonths(-2).ToString("yyyyMM");
            }
            else
            {
                return today.AddMonths(-1).ToString("yyyyMM");
            }
        }

        /// <summary>
        /// Lấy tháng sẽ khóa (Tháng chốt sổ)
        /// Nếu <= mùng 5: Lùi 1 tháng. Nếu > mùng 5: Tháng hiện tại.
        /// </summary>
        public static string month_khoa()
        {
            DateTime today = DateTime.Now;
            if (today.Day <= 5)
            {
                return today.AddMonths(-1).ToString("yyyyMM");
            }
            else
            {
                return today.ToString("yyyyMM");
            }
        }

        /// <summary>
        /// Lấy tháng hiện tại
        /// </summary>
        public static string month_hientai()
        {
            return DateTime.Now.ToString("yyyyMM");
        }
    }
}