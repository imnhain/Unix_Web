using System;
using System.Net;
using Unix_Web.Providers;

namespace Unix_Web.Auth
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Đặt nút mặc định khi nhấn Enter
            Form.DefaultButton = btnLogin.UniqueID;

            // Nếu đã đăng nhập, chuyển đến trang Main
            if (!IsPostBack && Session["username"] != null)
            {
                Response.Redirect("~/Shared/Main.aspx");
            }

            // Hiển thị lỗi nếu có từ session
            if (Session["errorAdminLogin"] != null)
            {
                ShowError(Session["errorAdminLogin"].ToString());
                Session["errorAdminLogin"] = null; // Xóa sau khi hiển thị
            }
        }

        protected void Loginclick(object sender, EventArgs e)
        {
            string username = txtusername.Text.Trim();
            string password = txtpassword.Text.Trim();

            // Kiểm tra input rỗng
            if (username.Length == 0 || password.Length == 0)
            {
                ShowError("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu");
                return;
            }

            // Xác thực với Unix server
            string unixLoginResult = UnixLogin(username, password);
            if (unixLoginResult != "")
            {
                ShowError(unixLoginResult);
                return;
            }

            // Đăng nhập thành công
            Session["username"] = username;
            string name = SQLConn34.ExecuteQuery("SELECT name FROM [erp].[dbo].[peremp] WHERE usrno2 = '" + username + "'").Rows[0][0].ToString().Trim();
            Session["name"] = name;

            // Xử lý Remember Me
            if (chkRemember.Checked)
            {
                // Tạo cookie để ghi nhớ đăng nhập (30 ngày)
                Response.Cookies["Username"].Value = username;
                Response.Cookies["Username"].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                // Xóa cookie nếu không chọn remember me
                if (Request.Cookies["Username"] != null)
                {
                    Response.Cookies["Username"].Expires = DateTime.Now.AddDays(-1);
                }
            }

            // Chuyển hướng đến trang Main
            Response.Redirect("~/Shared/Main.aspx");
        }

        /// <summary>
        /// Xác thực người dùng qua FTP Unix server
        /// </summary>
        private static string UnixLogin(string id, string pw)
        {
            DateTime startTime = DateTime.Now;

            try
            {
                FtpWebRequest fwr = (FtpWebRequest)WebRequest.Create("ftp://192.1.1.1/");
                fwr.Method = WebRequestMethods.Ftp.ListDirectory;
                fwr.Credentials = new NetworkCredential(id, pw);
                fwr.Timeout = 10000; // 10 seconds timeout

                using (FtpWebResponse fwre = (FtpWebResponse)fwr.GetResponse())
                {
                    // Đăng nhập thành công
                    return "";
                }
            }
            catch (WebException ex)
            {
                DateTime endTime = DateTime.Now;
                TimeSpan duration = endTime - startTime;

                // Kiểm tra lỗi timeout
                if (duration.TotalSeconds >= 10)
                {
                    return "Không thể kết nối đến máy chủ. Vui lòng kiểm tra kết nối mạng.";
                }

                // Kiểm tra loại lỗi
                if (ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.NotLoggedIn)
                    {
                        return "Tên đăng nhập hoặc mật khẩu không đúng";
                    }
                }

                // Lỗi chung
                return "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            catch (Exception ex)
            {
                // Lỗi không xác định
                return "Đã xảy ra lỗi: " + ex.Message;
            }
        }

        /// <summary>
        /// Hiển thị thông báo lỗi
        /// </summary>
        private void ShowError(string message)
        {
            lblResult.Text = message;
            pnlError.Visible = true;
        }
    }
}
