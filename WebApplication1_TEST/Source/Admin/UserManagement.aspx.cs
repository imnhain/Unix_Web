using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;

namespace Unix_Web.Admin
{
    public partial class UserManagement : System.Web.UI.Page
    {
        private string FilePath => Server.MapPath("~/App_Data/Permissions.txt");

        public class PermissionItem
        {
            public string Program { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserDepartment"] == null || Session["UserDepartment"].ToString() != "B22")
            {
                Response.Redirect("~/Shared/Main.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPermissions();
            }
        }

        // ============================================
        // ĐỌC / GHI FILE
        // ============================================

        private List<PermissionItem> ReadPermissions()
        {
            var list = new List<PermissionItem>();

            if (!File.Exists(FilePath))
                return list;

            foreach (string line in File.ReadAllLines(FilePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    continue;

                string[] parts = line.Split('|');
                if (parts.Length >= 3)
                {
                    list.Add(new PermissionItem
                    {
                        Program = parts[0].Trim().ToUpper(),
                        Username = parts[1].Trim().ToLower(),
                        Role = parts[2].Trim().ToUpper()
                    });
                }
            }

            return list;
        }

        private void SavePermissions(List<PermissionItem> list)
        {
            var lines = new List<string>();
            foreach (var item in list)
                lines.Add($"{item.Program}|{item.Username}|{item.Role}");

            File.WriteAllLines(FilePath, lines);
        }

        private void LoadPermissions()
        {
            var list = ReadPermissions();
            rptPermissions.DataSource = list;
            rptPermissions.DataBind();
            lblTotal.Text = list.Count.ToString();
        }

        // ============================================
        // BUTTON EVENTS
        // ============================================

        protected void btnShowAdd_Click(object sender, EventArgs e)
        {
            pnlAddForm.Visible = true;
            ddlProgram.SelectedIndex = 0;
            txtUsername.Text = "";
            ddlRole.SelectedIndex = 0;
            lblNotify.Visible = false;
        }

        protected void btnCancelAdd_Click(object sender, EventArgs e)
        {
            pnlAddForm.Visible = false;
            lblNotify.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // ✅ Lấy từ ddlProgram thay vì txtProgram
            string program = ddlProgram.SelectedValue.Trim().ToUpper();
            string username = txtUsername.Text.Trim().ToLower();
            string role = ddlRole.SelectedValue;

            // Validate
            if (string.IsNullOrEmpty(program))
            {
                ShowNotify("Vui lòng chọn chương trình!", "warning");
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                ShowNotify("Vui lòng nhập username!", "warning");
                return;
            }

            var list = ReadPermissions();

            // Kiểm tra trùng
            foreach (var item in list)
            {
                if (item.Program == program && item.Username == username)
                {
                    ShowNotify($"'{username}' đã có quyền cho '{program}'! Xoá trước rồi thêm lại.", "warning");
                    pnlAddForm.Visible = true;
                    return;
                }
            }

            // Thêm và lưu
            list.Add(new PermissionItem
            {
                Program = program,
                Username = username,
                Role = role
            });

            SavePermissions(list);
            LoadPermissions();

            pnlAddForm.Visible = false;
            ShowNotify($"✅ Đã thêm quyền {role} cho '{username}' trên '{program}'!", "success");
        }

        // ============================================
        // REPEATER - XOÁ
        // ============================================

        protected void rptPermissions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete") return;

            // CommandArgument = "PROGRAM|USERNAME"
            string[] args = e.CommandArgument.ToString().Split('|');
            string program = args[0].Trim();
            string username = args[1].Trim();

            var list = ReadPermissions();
            int removed = list.RemoveAll(x => x.Program == program && x.Username == username);

            if (removed > 0)
            {
                SavePermissions(list);
                LoadPermissions();
                ShowNotify($"✅ Đã xoá quyền của '{username}' trên '{program}'!", "success");
            }
            else
            {
                ShowNotify("Không tìm thấy quyền để xoá!", "warning");
            }
        }

        // ============================================
        // HELPER
        // ============================================

        private void ShowNotify(string message, string type)
        {
            lblNotify.Text = message;
            lblNotify.CssClass = "alert alert-" + type;
            lblNotify.Visible = true;
        }
    }
}