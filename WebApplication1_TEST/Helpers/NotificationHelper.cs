using System.Web.UI;
using Newtonsoft.Json;

namespace Unix_Web.Helpers
{
    public static class NotificationHelper
    {
        public static void Show(Page page, string message, string type = "info")
        {
            if (page == null) return;

            // Dùng JsonConvert.ToString để escape chuỗi an toàn khi nhúng vào JS
            // (tự động xử lý dấu ', ", \, xuống dòng, ... đúng chuẩn thay vì .Replace() thủ công
            // trước đây, vốn chỉ xử lý dấu ' và \n nên vẫn có thể làm vỡ script với input khác).
            string safeMsg = JsonConvert.ToString((message ?? "").Replace("\n", "<br>"));
            string safeType = JsonConvert.ToString(type);

            string title;
            switch (type)
            {
                case "success": title = "Thành công!"; break;
                case "error": title = "Lỗi!"; break;
                case "warning": title = "Cảnh báo"; break;
                case "question": title = "Xác nhận"; break;
                default: title = "Thông báo"; break;
            }
            string safeTitle = JsonConvert.ToString(title);

            string script = $"Swal.fire({{ title: {safeTitle}, html: {safeMsg}, icon: {safeType}, confirmButtonText: 'OK' }});";

            ScriptManager.RegisterStartupScript(page, page.GetType(), "SweetAlert_" + System.Guid.NewGuid(), script, true);
        }
    }
}
