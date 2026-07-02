using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using Unix_Web.Providers;

namespace Unix_Web.Shared
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected string UserDepartment = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/Auth/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string username = Session["username"].ToString();
                string name = Session["name"].ToString();
                lblUsername.Text = name;

                string userIP = GetClientIPAddress();
                lblUserIP.Text = userIP;

                UserDepartment = GetUserDepartment(username);

                Session["UserDepartment"] = UserDepartment;

                if (Application["OnlineUsers"] != null)
                {
                    lblOnlineUsers.Text = Application["OnlineUsers"].ToString();
                }
            }
        }

        /// <summary>
        /// </summary>
        private string GetUserDepartment(string username)
        {
            string department = "";

            try
            {
                string query = @"SELECT SUBSTRING(depno,1,3) as depno 
                                FROM [erp].[dbo].[peremp] 
                                WHERE usrno2 = '" + username + "'";

                DataTable dt = SQLConn33.ExecuteQuery(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    department = dt.Rows[0]["depno"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error getting user department: " + ex.Message);
            }

            return department;
        }

        /// <summary>
        /// </summary>
        private string GetClientIPAddress()
        {
            try
            {
                string ipAddress = string.Empty;

                // 1. Kiểm tra HTTP_X_FORWARDED_FOR (khi đi qua proxy/load balancer)
                ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    // Nếu có nhiều IP (qua nhiều proxy), lấy IP đầu tiên (client gốc)
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length > 0)
                    {
                        ipAddress = addresses[0].Trim();
                    }
                }

                // 2. Nếu không có X-Forwarded-For, kiểm tra REMOTE_ADDR
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                }

                // 3. Fallback: Dùng Request.UserHostAddress
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = Request.UserHostAddress;
                }

                // 4. Kiểm tra localhost/loopback
                if (ipAddress == "::1" || ipAddress == "127.0.0.1")
                {
                    ipAddress = "localhost";
                }

                return !string.IsNullOrEmpty(ipAddress) ? ipAddress : "Unknown";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting client IP: " + ex.Message);
                return "Unknown";
            }
        }

        /// <summary>
        /// </summary>
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            if (Request.Cookies["Username"] != null)
            {
                HttpCookie cookie = new HttpCookie("Username");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            Response.Redirect("~/Auth/Login.aspx");
        }
        protected void tmrOnlineUsers_Tick(object sender, EventArgs e)
        {
            // Cập nhật lại số lượng online mỗi khi Timer tick
            if (Application["OnlineUsers"] != null)
            {
                lblOnlineUsers.Text = Application["OnlineUsers"].ToString();
            }
        }

        // Hàm kiểm tra quyền hiển thị Menu
        public bool CheckMenuPermission(string programCode)
        {
            // Nếu chưa đăng nhập -> Ẩn menu
            if (Session["username"] == null) return false;

            string currentUser = Session["username"].ToString().ToLower();

            // Nếu chưa có cache quyền trong Session, tiến hành đọc file txt 1 lần
            if (Session["UserPermissions"] == null)
            {
                // Thay đổi đường dẫn này trỏ đúng vào nơi bạn để file Permissions.txt
                string filePath = Server.MapPath("~/App_Data/Permissions.txt");
                List<string> perms = new List<string>();

                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length >= 2)
                        {
                            // Lưu vào danh sách theo format: PROGRAMCODE|username
                            perms.Add($"{parts[0].Trim().ToUpper()}|{parts[1].Trim().ToLower()}");
                        }
                    }
                }
                Session["UserPermissions"] = perms; // Lưu vào Session để dùng lại
            }

            // Lấy danh sách quyền từ Session ra kiểm tra
            List<string> userPermissions = (List<string>)Session["UserPermissions"];
            string keyToCheck = $"{programCode.ToUpper()}|{currentUser}";

            // Trả về true nếu user có quyền với programCode này
            return userPermissions.Contains(keyToCheck);
        }
    }
}
