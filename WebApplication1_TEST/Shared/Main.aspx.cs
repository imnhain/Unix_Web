using System;

namespace Unix_Web.Shared
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Trang này kế thừa từ Site.Master
            // Master Page đã kiểm tra đăng nhập rồi

            if (!IsPostBack)
            {
                // Load dữ liệu dashboard
                LoadWelcomeData();
            }
        }

        private void LoadWelcomeData()
        {
            // Lấy username từ session
            if (Session["name"] != null)
            {
                string name = Session["name"].ToString();
                lblWelcomeUser.Text = name;
            }

            // Hiển thị thời gian hiện tại
            lblCurrentTime.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm");
        }
    }
}
