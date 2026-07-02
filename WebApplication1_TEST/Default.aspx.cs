using System;

namespace Unix_Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra xem user đã đăng nhập chưa
            if (Session["Username"] == null || Session["IsAuthenticated"] == null)
            {
                // Chưa đăng nhập, redirect về trang login
                Response.Redirect("~/Auth/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Hiển thị thông tin user
                lblUsername.Text = Session["Username"].ToString();
                lblLoginTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Xóa session
            Session.Clear();
            Session.Abandon();

            // Xóa cookie remember me nếu có
            if (Request.Cookies["Username"] != null)
            {
                Response.Cookies["Username"].Expires = DateTime.Now.AddDays(-1);
            }

            // Redirect về trang login
            Response.Redirect("~/Auth/Login.aspx");
        }
    }
}
