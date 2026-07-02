using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace Unix_Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["OnlineUsers"] = 0;
        }
        void Session_Start(object sender, EventArgs e)
        {
            // Khi có người mới truy cập -> Cộng thêm 1
            Application.Lock();
            Application["OnlineUsers"] = Convert.ToInt32(Application["OnlineUsers"]) + 1;
            Application.UnLock();
        }

        void Session_End(object sender, EventArgs e)
        {
            // Khi người dùng đóng trình duyệt hoặc hết Timeout (mặc định 20 phút) -> Trừ đi 1
            Application.Lock();
            int currentOnline = Convert.ToInt32(Application["OnlineUsers"]);

            // Check > 0 để đề phòng lỗi trừ âm
            if (currentOnline > 0)
            {
                Application["OnlineUsers"] = currentOnline - 1;
            }
            Application.UnLock();
        }
    }
}