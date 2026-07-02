using System.Web.UI;

namespace Unix_Web.Helpers
{
    /// <summary>
    /// Helper hiển thị loading spinner khi xử lý
    /// </summary>
    public static class LoadingHelper
    {
        /// <summary>
        /// Hiển thị loading spinner với message tùy chỉnh
        /// </summary>
        /// <param name="page">Page hiện tại (this)</param>
        /// <param name="message">Thông báo hiển thị (mặc định: "Processing...")</param>
        public static void Show(Page page, string message = "Processing...")
        {
            string script = $@"
                if (typeof showLoading === 'function') {{
                    showLoading('{message}');
                }}
            ";

            ScriptManager.RegisterStartupScript(
                page,
                page.GetType(),
                "ShowLoading_" + System.Guid.NewGuid().ToString(),
                script,
                true
            );
        }

        /// <summary>
        /// Ẩn loading spinner
        /// </summary>
        public static void Hide(Page page)
        {
            string script = @"
                if (typeof hideLoading === 'function') {
                    hideLoading();
                }
            ";

            ScriptManager.RegisterStartupScript(
                page,
                page.GetType(),
                "HideLoading_" + System.Guid.NewGuid().ToString(),
                script,
                true
            );
        }
    }
}