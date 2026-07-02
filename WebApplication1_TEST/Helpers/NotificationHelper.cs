using System.Web.UI;

namespace Unix_Web.Helpers
{
    public static class NotificationHelper
    {
        public static void Show(Page page, string message, string type = "info")
        {
            if (page == null) return;

            // Xử lý chuỗi an toàn
            string safeMsg = message.Replace("'", "\\'").Replace("\n", "<br>");
            string title = "";

            // Tự động đặt tiêu đề
            switch (type)
            {
                case "success": title = "Thành công!"; break;
                case "error": title = "Lỗi!"; break;
                case "warning": title = "Cảnh báo"; break;
                case "question": title = "Xác nhận"; break;
                default: title = "Thông báo"; break;
            }

            // Tạo script SweetAlert2
            string script = $"Swal.fire({{ title: '{title}', html: '{safeMsg}', icon: '{type}', confirmButtonText: 'OK' }});";

            // Đăng ký script vào Page đang gọi
            ScriptManager.RegisterStartupScript(page, page.GetType(), "SweetAlert", script, true);
        }
    }
}