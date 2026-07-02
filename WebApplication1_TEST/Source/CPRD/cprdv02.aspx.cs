using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unix_Web.Helpers;
using Unix_Web.Providers;

namespace Unix_Web.Source.CPRD
{
    public partial class cprdv02 : System.Web.UI.Page
    {
        // --- CẤU HÌNH PHÂN TRANG ---
        private const int BATCH_SIZE = 20;

        // --- CẤU HÌNH PHÂN QUYỀN (MỚI) ---
        private bool _hasEditPermission = false; // Mặc định là không có quyền

        // Enum quản lý trạng thái
        private enum ActionMode
        {
            NONE, QUERY, ADD, MODIFY
        }

        // Property lấy/gán trạng thái
        private ActionMode CurrentMode
        {
            get
            {
                if (ViewState["CurrentMode"] == null) return ActionMode.NONE;
                return (ActionMode)ViewState["CurrentMode"];
            }
            set
            {
                ViewState["CurrentMode"] = value;
                lblActionStatus.Text = value == ActionMode.NONE ? "" : "MODE: " + value.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MapControls();

            // MỚI: Kiểm tra quyền ngay khi trang vừa tải
            CheckUserPermission();

            if (!IsPostBack)
            {
                lblUserNo.Text = "";
                lblIndat.Text = "";
                LoadColorComboboxes();
                ResetToDefaultState();
            }
        }

        // --- HÀM BẢO VỆ DỮ LIỆU ĐẦU VÀO (INFORMIX) ---
        /// <summary>
        /// Xử lý chuỗi để tránh lỗi Syntax Informix khi người dùng nhập dấu nháy đơn (')
        /// </summary>
        private string SafeStr(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("'", "''").Trim();
        }

        /// <summary>
        /// Đảm bảo các trường số không bị rỗng gây lỗi (, ,) trong câu lệnh Insert
        /// </summary>
        private string GetNum(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "0";
            return input.Trim();
        }

        // --- HÀM KIỂM TRA QUYỀN TỪ FILE TXT (MỚI) ---
        private void CheckUserPermission()
        {
            string userID = Session["username"] as string ?? "";
            string currentProgram = "CPRDV02";
            string role = "VIEW"; // Mặc định chỉ xem

            // Đường dẫn file nằm trong thư mục App_Data
            string filePath = Server.MapPath("~/App_Data/Permissions.txt");

            if (File.Exists(filePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 3)
                        {
                            string fileProg = parts[0].Trim().ToUpper();
                            string fileUser = parts[1].Trim().ToLower();
                            string fileRole = parts[2].Trim().ToUpper();

                            if (fileProg == currentProgram && fileUser == userID.ToLower())
                            {
                                role = fileRole;
                                break;
                            }
                        }
                    }
                }
                catch { /* Lỗi đọc file thì giữ nguyên là VIEW */ }
            }

            // Nếu là ADMIN hoặc FULL thì cấp quyền
            if (role == "FULL" || role == "ADMIN")
            {
                _hasEditPermission = true;
            }
            else
            {
                _hasEditPermission = false;
            }
        }

        private void LoadColorComboboxes()
        {
            string[] colors = { "紅 DO", "橙 CAM", "黃 VANG", "綠 XANH LUC", "白 TRANG", "青 XANH LA", "藍 XANH LAM", "桃紅 HONG" };
            BindCombo(ddlColor, colors);
            BindCombo(ddlColor1, colors);
            BindCombo(ddlColor2, colors);
            BindCombo(ddlColor3, colors);
            BindCombo(ddlCirColor, colors);
        }

        private void BindCombo(DropDownList ddl, string[] data)
        {
            ddl.DataSource = data;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("", ""));
        }

        // --- QUẢN LÝ TRẠNG THÁI GIAO DIỆN ---

        private void ResetToDefaultState()
        {
            CurrentMode = ActionMode.NONE;

            // 1. Toolbar logic
            if (pnlMainToolbar != null) pnlMainToolbar.Visible = true;
            if (pnlConfirmToolbar != null) pnlConfirmToolbar.Visible = false;

            // 2. Kiểm tra dữ liệu
            bool hasData = hdnRowID != null && !string.IsNullOrEmpty(hdnRowID.Value);

            // 3. Xử lý thanh phân trang
            if (pnlNavigationToolbar != null)
            {
                int totalRows = ViewState["TotalCount"] != null ? (int)ViewState["TotalCount"] : 0;
                pnlNavigationToolbar.Visible = totalRows > 0;
            }

            // 4. Khóa Input
            ToggleInput(false);

            // 5. Xử lý ẨN/HIỆN nút dựa trên quyền hạn 

            // Nút ADD: Chỉ hiện nếu có quyền
            if (btnAdd != null)
            {
                btnAdd.Visible = _hasEditPermission;
            }

            // Nút MODIFY & DELETE: Chỉ hiện nếu có dữ liệu VÀ có quyền
            if (btnModify != null)
            {
                bool canModify = hasData && _hasEditPermission;
                btnModify.Enabled = canModify;
                btnModify.Visible = _hasEditPermission; // Ẩn hẳn nếu không có quyền
                btnModify.Style["opacity"] = canModify ? "1" : "0.5";
            }

            if (btnDelete != null)
            {
                bool canDelete = hasData && _hasEditPermission;
                btnDelete.Enabled = canDelete;
                btnDelete.Visible = _hasEditPermission; // Ẩn hẳn nếu không có quyền
                btnDelete.Style["opacity"] = canDelete ? "1" : "0.5";
            }
        }

        private void EnterActionMode(ActionMode mode)
        {
            CurrentMode = mode;
            pnlMainToolbar.Visible = false;
            pnlConfirmToolbar.Visible = true;
            pnlNavigationToolbar.Visible = false;

            ToggleInput(true);

            if (mode == ActionMode.ADD)
            {
                ClearForm();
                SetText(lblIndat, DateTime.Now.ToString("yyyyMMdd"));
                SetText(lblUserNo, Session["username"]?.ToString() ?? "ADMIN");
            }
            else if (mode == ActionMode.QUERY)
            {
                ClearForm();
                ToggleInput(false);
                if (txtMachNo != null) txtMachNo.ReadOnly = false;
                if (txtItnbr != null) txtItnbr.ReadOnly = false;
            }
            else if (mode == ActionMode.MODIFY)
            {
                ToggleInput(true);
                if (txtMachNo != null) txtMachNo.ReadOnly = true;
                if (txtItnbr != null) txtItnbr.ReadOnly = true;
            }
        }

        private void ToggleInput(bool enable)
        {
            RecursiveSetReadOnly(pnlInput, !enable);
        }

        private void RecursiveSetReadOnly(Control parent, bool readOnly)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox tb)
                {
                    if (tb.ID != "txtVersion" && tb.ID != "txtStype" && tb.ID != "txtSpecPci")
                        tb.ReadOnly = readOnly;
                }
                else if (c is DropDownList ddl)
                {
                    ddl.Enabled = !readOnly;
                    if (!readOnly) ddl.Attributes.Remove("disabled");
                }
                if (c.HasControls()) RecursiveSetReadOnly(c, readOnly);
            }
        }

        private void ClearForm()
        {
            if (pnlInput != null) ClearRecursive(pnlInput);

            if (CurrentMode == ActionMode.ADD || CurrentMode == ActionMode.QUERY)
            {
                if (hdnRowID != null) hdnRowID.Value = "";
                if (lblIndex != null) lblIndex.Text = "0";
                if (lblCount != null) lblCount.Text = "0";
                if (lblIndat != null) lblIndat.Text = "";
                if (lblUserNo != null) lblUserNo.Text = "";
            }
        }

        private void ClearRecursive(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox tb) tb.Text = "";
                else if (c is DropDownList ddl) ddl.SelectedIndex = -1;
                if (c.HasControls()) ClearRecursive(c);
            }
        }

        // --- BUTTON EVENTS ---

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            EnterActionMode(ActionMode.QUERY);
            ClearForm();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!_hasEditPermission) return;

            EnterActionMode(ActionMode.ADD);
            LoadColorComboboxes();
        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            if (!_hasEditPermission) return;

            if (string.IsNullOrEmpty(hdnRowID.Value))
            {
                NotificationHelper.Show(this, "Không có dữ liệu để sửa!", "warning");
                return;
            }

            // LƯU VERSION CŨ vào ViewState
            ViewState["OldVersion"] = txtVersion.Text.Trim();

            try
            {
                LoadData(txtItnbr.Text.Trim(), txtMachNo.Text.Trim());

                // Chuyển sang mode MODIFY
                EnterActionMode(ActionMode.MODIFY);

                // Hiển thị thông báo cho user
                lblActionStatus.Text = $"MODE: MODIFY | Old Version: {ViewState["OldVersion"]} → New Version: {txtVersion.Text.Trim()}";
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "Lỗi: " + ex.Message, "error");
            }
        }

        private void LoadData(string itnbr, string machno)
        {
            try
            {
                string newVersion = "01";

                // Lấy MAX version (Dùng UnixConn cho Informix)
                DataTable dtMaxVer = UnixConn.ExecuteQuery(
                    $"SELECT MAX(version) FROM erp:prdepv WHERE machno = '{SafeStr(machno)}' AND itnbr = '{SafeStr(itnbr)}'");

                if (dtMaxVer != null && dtMaxVer.Rows.Count > 0)
                {
                    string maxVer = dtMaxVer.Rows[0][0].ToString().Trim();

                    if (!string.IsNullOrEmpty(maxVer) && int.TryParse(maxVer, out int verNum))
                    {
                        // Version mới = MAX + 1
                        newVersion = (verNum + 1).ToString("D2");
                    }
                }

                txtVersion.Text = newVersion;

                int l_curemin = 0, l_curesec = 0, l_actmin = 0, l_actsec = 0;
                int force = 0;

                // 1. Load từ pmspec2
                DataTable dt_pmspec2 = UnixConn.ExecuteQuery(
                    $"SELECT dot,mold_style2,cmno,ringno,moldpres,curemin,curesec,actmin,actsec,indat " +
                    $"FROM erp:pmspec2 WHERE bdep = 'V' AND itnbr = '{SafeStr(itnbr)}' " +
                    $"AND rowid IN (SELECT MAX(rowid) FROM erp:pmspec2 WHERE bdep = 'V' AND itnbr = '{SafeStr(itnbr)}')");

                if (dt_pmspec2 != null && dt_pmspec2.Rows.Count > 0)
                {
                    txtStructCod.Text = dt_pmspec2.Rows[0]["dot"].ToString().Trim();
                    txtMoldStyle.Text = dt_pmspec2.Rows[0]["mold_style2"].ToString().Trim();
                    txtLBatchSize.Text = dt_pmspec2.Rows[0]["cmno"].ToString().Trim();
                    txtRBatchSize.Text = txtLBatchSize.Text;
                    txtRingNo.Text = dt_pmspec2.Rows[0]["ringno"].ToString().Trim();

                    int.TryParse(dt_pmspec2.Rows[0]["moldpres"].ToString().Trim(), out force);
                    int.TryParse(dt_pmspec2.Rows[0]["curemin"].ToString().Trim(), out l_curemin);
                    int.TryParse(dt_pmspec2.Rows[0]["curesec"].ToString().Trim(), out l_curesec);
                    int.TryParse(dt_pmspec2.Rows[0]["actmin"].ToString().Trim(), out l_actmin);
                    int.TryParse(dt_pmspec2.Rows[0]["actsec"].ToString().Trim(), out l_actsec);

                    // Tính TimePCI
                    int timepci = p_cprdv02_timepci(l_curemin, l_curesec, l_actmin, l_actsec);
                    txtTimePci.Text = timepci.ToString();

                    // Force
                    txtForce.Text = (force * 10).ToString();
                    txtForceKn.Text = txtForce.Text;
                }

                // 2. Load từ rdssdh (moldsize)
                DataTable dt_rdssdh = UnixConn.ExecuteQuery(
                    $"SELECT moldno FROM erp:rdssdh WHERE factory = 'V' AND itnbr = '{SafeStr(itnbr)}' GROUP BY 1");

                if (dt_rdssdh != null && dt_rdssdh.Rows.Count > 0)
                {
                    txtLMoldSize.Text = dt_rdssdh.Rows[0]["moldno"].ToString().Trim();
                    txtRMoldSize.Text = txtLMoldSize.Text;

                    // Get spec từ moldno
                    string spec = GetSpec(txtLMoldSize.Text);
                    txtSpecPci.Text = spec;
                }

                // 3. Load từ pspec2 (tireno2)
                DataTable dt_pspec2 = UnixConn.ExecuteQuery(
                    $"SELECT tireno2,indat FROM erp:pspec2 WHERE bdep = 'V' AND opno[3,10] = '{SafeStr(itnbr)}' " +
                    $"AND rowid IN (SELECT MAX(rowid) FROM erp:pspec2 WHERE bdep = 'V' AND opno[3,10] = '{SafeStr(itnbr)}')");

                if (dt_pspec2 != null && dt_pspec2.Rows.Count > 0)
                {
                    txtTireNo2.Text = dt_pspec2.Rows[0]["tireno2"].ToString().Trim();
                }

                // 4. Load từ prdevv
                int cc = Convert.ToInt32(UnixConn.ExecuteQuery(
                    $"SELECT COUNT(*) FROM erp:prdevv WHERE itnbr = '{SafeStr(itnbr)}' AND (state = '' OR state = ' ' OR state IS NULL)").Rows[0][0]);

                if (cc > 0)
                {
                    DataTable dt_prdevv = UnixConn.ExecuteQuery(
                        $"SELECT lheight,rheight,ltype,rtype,pressure,lput,rput FROM erp:prdevv " +
                        $"WHERE itnbr = '{SafeStr(itnbr)}' AND version IN (SELECT MAX(version) FROM erp:prdevv WHERE itnbr = '{SafeStr(itnbr)}')");

                    if (dt_prdevv != null && dt_prdevv.Rows.Count > 0)
                    {
                        txtLHeight.Text = dt_prdevv.Rows[0]["lheight"].ToString().Trim();
                        txtRHeight.Text = dt_prdevv.Rows[0]["rheight"].ToString().Trim();
                        txtLType.Text = dt_prdevv.Rows[0]["ltype"].ToString().Trim();
                        txtRType.Text = dt_prdevv.Rows[0]["rtype"].ToString().Trim();
                        txtPressure.Text = dt_prdevv.Rows[0]["pressure"].ToString().Trim();
                        txtLPut.Text = dt_prdevv.Rows[0]["lput"].ToString().Trim();
                        txtRPut.Text = dt_prdevv.Rows[0]["rput"].ToString().Trim();
                    }
                }

                // 5. Set các giá trị mặc định
                lblIndat.Text = DateTime.Now.ToString("yyyyMMdd");
                lblUserNo.Text = Session["username"]?.ToString() ?? "ADMIN";
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "Lỗi load dữ liệu: " + ex.Message, "error");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!_hasEditPermission) return;

            if (string.IsNullOrEmpty(hdnRowID.Value))
            {
                NotificationHelper.Show(this, "MSTP KHÔNG TÌM THẤY !!!", "warning");
                return;
            }
            // Kiểm tra đã confirm chưa
            if (hdnDeleteConfirmed.Value != "true")
            {
                // Chưa confirm → Show confirm và set flag
                string script = @"
                    if (confirm('DATA WILL BE DELETED, ARE YOU SURE?')) {
                        document.getElementById('" + hdnDeleteConfirmed.ClientID + @"').value = 'true';
                        " + Page.ClientScript.GetPostBackEventReference(btnDelete, "") + @";
                    }
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "ConfirmDelete", script, true);
                return;
            }

            // Reset flag
            hdnDeleteConfirmed.Value = "false";
            // ĐÃ CONFIRM → THỰC HIỆN XÓA
            try
            {
                string recordID = hdnRowID.Value;
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string currentTime = DateTime.Now.ToString("HHmmss");
                string currentUser = Session["username"]?.ToString() ?? "ADMIN";

                // Lấy thông tin record trên Informix
                string sqlSelect = $"SELECT * FROM erp:prdepv WHERE ROWID = {recordID}";
                DataTable dtRecord = UnixConn.ExecuteQuery(sqlSelect);

                if (dtRecord == null || dtRecord.Rows.Count == 0)
                {
                    NotificationHelper.Show(this, "Không tìm thấy record để xóa!", "warning");
                    return;
                }

                DataRow dr = dtRecord.Rows[0];

                // UPDATE state = '*' (INFORMIX)
                string sqlUpdate = $"UPDATE erp:prdepv SET state = '*' WHERE ROWID = {recordID}";
                UnixConn.ExecuteNonQuery(sqlUpdate);

                // Backup (INFORMIX)
                StringBuilder sqlBk = new StringBuilder();
                sqlBk.Append("INSERT INTO miskv:prdepvbk (");
                sqlBk.Append("func, intime, machno, version, lmoldsize, rmoldsize, moldstyle, ");
                sqlBk.Append("lbatchsize, rbatchsize, batchclamp, itnbr, ringno, spec, ");
                sqlBk.Append("tireno2, trcircolor, trrcolor1, trrcolor2, trrcolor3, color, ");
                sqlBk.Append("tread, speed, sidewall, structcod, dot, maxload, note1, note2, ");
                sqlBk.Append("scansta, ltemp, rtemp, leptemp, reptemp, ");
                sqlBk.Append("lheight, rheight, ltype, rtype, force, ");
                sqlBk.Append("forcekn, heightpci, pressure, ");
                sqlBk.Append("timepci, lput, rput, stype, used, state, indat, usrno");
                sqlBk.Append(") VALUES (");
                sqlBk.AppendFormat("'DEL', '{0}', ", currentTime);
                sqlBk.AppendFormat("'{0}', '{1}', ", dr["machno"].ToString().Trim(), dr["version"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", dr["lmoldsize"].ToString().Trim(), dr["rmoldsize"].ToString().Trim(), dr["moldstyle"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", dr["lbatchsize"].ToString().Trim(), dr["rbatchsize"].ToString().Trim(), dr["batchclamp"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", dr["itnbr"].ToString().Trim(), dr["ringno"].ToString().Trim(), dr["spec"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', ",
                    dr["tireno2"].ToString().Trim(), dr["trcircolor"].ToString().Trim(), dr["trrcolor1"].ToString().Trim(),
                    dr["trrcolor2"].ToString().Trim(), dr["trrcolor3"].ToString().Trim(), dr["color"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                    dr["tread"].ToString().Trim(), dr["speed"].ToString().Trim(), dr["sidewall"].ToString().Trim(),
                    dr["structcod"].ToString().Trim(), dr["dot"].ToString().Trim(), dr["maxload"].ToString().Trim(),
                    dr["note1"].ToString().Trim(), dr["note2"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                    dr["scansta"].ToString().Trim(), dr["ltemp"].ToString().Trim(), dr["rtemp"].ToString().Trim(),
                    dr["leptemp"].ToString().Trim(), dr["reptemp"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                    dr["lheight"].ToString().Trim(), dr["rheight"].ToString().Trim(), dr["ltype"].ToString().Trim(),
                    dr["rtype"].ToString().Trim(), dr["force"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', ",
                    dr["forcekn"].ToString().Trim(), dr["heightpci"].ToString().Trim(), dr["pressure"].ToString().Trim());
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', 'Y', '*', '{4}', '{5}'",
                    dr["timepci"].ToString().Trim(), dr["lput"].ToString().Trim(), dr["rput"].ToString().Trim(),
                    dr["stype"].ToString().Trim(), currentDate, currentUser);
                sqlBk.Append(")");

                try
                {
                    UnixConn.ExecuteNonQuery(sqlBk.ToString());
                }
                catch (Exception exBk)
                {
                    Console.WriteLine("Lỗi Backup: " + exBk.Message);
                }

                NotificationHelper.Show(this, "DELETE SUCCESS !!!", "success");

                // Clear và reload
                ClearForm();
                hdnRowID.Value = "";

                int totalRows = ViewState["TotalCount"] != null ? (int)ViewState["TotalCount"] : 0;
                if (totalRows > 0)
                {
                    int currentIndex = ViewState["CurrentIndex"] != null ? (int)ViewState["CurrentIndex"] : 0;

                    Session.Remove("CurrentBatchData");
                    Session.Remove("CurrentBatchStart");

                    if (currentIndex >= totalRows - 1)
                    {
                        currentIndex = totalRows - 2;
                    }

                    ViewState["TotalCount"] = totalRows - 1;

                    if (totalRows > 1 && currentIndex >= 0)
                    {
                        ShowRecord(currentIndex);
                    }
                    else
                    {
                        ViewState["TotalCount"] = 0;
                        if (pnlNavigationToolbar != null) pnlNavigationToolbar.Visible = false;
                    }
                }

                ResetToDefaultState();
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "Lỗi: " + ex.Message, "error");
            }
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            switch (CurrentMode)
            {
                case ActionMode.QUERY: ExecuteQuery(); break;
                case ActionMode.ADD: ExecuteInsert(); break;
                case ActionMode.MODIFY: ExecuteUpdate(); break;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            ResetToDefaultState();
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdnRowID.Value))
            {
                NotificationHelper.Show(this, "Không có dữ liệu!", "warning");
                return;
            }

            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "Lỗi: " + ex.Message, "error");
            }
        }

        private void ExportToExcel()
        {
            try
            {
                // 1. KIỂM TRA FILE MẪU
                string templatePath = Server.MapPath("~/App_Data/KV_PCR05.xlsx");
                if (!File.Exists(templatePath))
                {
                    NotificationHelper.Show(this, "Không tìm thấy file mẫu KV_PCR05.xlsx!", "error");
                    return;
                }

                IWorkbook workbook;
                // Mở file mẫu
                using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(file); // XSSFWorkbook dành cho .xlsx
                }

                ISheet sheet = workbook.GetSheet("PCR");
                if (sheet == null) sheet = workbook.GetSheetAt(0); // Nếu không thấy tên PCR thì lấy sheet đầu tiên

                // 2. LẤY DỮ LIỆU TỪ DATABASE

                // --- A. Query Bảng INV MAS (Kích thước) ---
                string tempSize = "";
                string sqlInv = "SELECT size, loading, speed, patt, pnum, tttl FROM [erp].[dbo].[invmas] WHERE itnbr = '" + SafeStr(txtItnbr.Text) + "'";
                DataTable dtInv = SQLConn34.ExecuteQuery(sqlInv);

                if (dtInv != null && dtInv.Rows.Count > 0)
                {
                    DataRow dr = dtInv.Rows[0];
                    List<string> parts = new List<string>();

                    if (!string.IsNullOrEmpty(dr["size"].ToString().Trim())) parts.Add(dr["size"].ToString().Trim());
                    if (!string.IsNullOrEmpty(dr["loading"].ToString().Trim())) parts.Add(dr["loading"].ToString().Trim());
                    if (!string.IsNullOrEmpty(dr["speed"].ToString().Trim())) parts.Add(dr["speed"].ToString().Trim());
                    if (!string.IsNullOrEmpty(dr["patt"].ToString().Trim())) parts.Add(dr["patt"].ToString().Trim());
                    if (!string.IsNullOrEmpty(dr["pnum"].ToString().Trim())) parts.Add(dr["pnum"].ToString().Trim() + "P");
                    if (!string.IsNullOrEmpty(dr["tttl"].ToString().Trim())) parts.Add(dr["tttl"].ToString().Trim());

                    tempSize = string.Join(" ", parts);
                }

                // --- B. Query Bảng PRDEVV (Máy & Thời gian) ---
                string machNo = "";
                string totalTime = "00:00";
                string[] machTimes = new string[11];
                for (int k = 0; k < 11; k++) machTimes[k] = "00:00"; // Reset mảng

                string sqlEvv = "SELECT machno, totalm, totals, " +
                                "machm1, machs1, machm2, machs2, machm3, machs3, machm4, machs4, machm5, machs5, " +
                                "machm6, machs6, machm7, machs7, machm8, machs8, machm9, machs9, machm10, machs10, machm11, machs11 " +
                                "FROM [erp].[dbo].[prdevv] " +
                                "WHERE subno = '4' AND (state = '' OR state IS NULL) AND itnbr = '" + SafeStr(txtItnbr.Text) + "'";

                DataTable dtEvv = SQLConn34.ExecuteQuery(sqlEvv);
                if (dtEvv != null && dtEvv.Rows.Count > 0)
                {
                    DataRow drEvv = dtEvv.Rows[0];
                    machNo = drEvv["machno"].ToString().Trim() + " 型";

                    // Format Tổng thời gian
                    int.TryParse(drEvv["totalm"].ToString(), out int tM);
                    int.TryParse(drEvv["totals"].ToString(), out int tS);
                    totalTime = string.Format("{0:D2}:{1:D2}", tM, tS);

                    // Format thời gian từng máy (1-11)
                    for (int i = 1; i <= 11; i++)
                    {
                        int.TryParse(drEvv["machm" + i].ToString(), out int mM);
                        int.TryParse(drEvv["machs" + i].ToString(), out int mS);
                        machTimes[i - 1] = string.Format("{0:D2}:{1:D2}", mM, mS);
                    }
                }

                // 3. ĐIỀN DỮ LIỆU VÀO EXCEL (NPOI dùng index 0: Dòng 1 là index 0, Cột A là index 0)
                // K = 10, O = 14, S = 18, T = 19, W = 22, AA = 26, AE = 30, AI = 34, AM = 38, AQ = 42, AU = 46, AY = 50, AT = 45

                // -- Header Info --
                SetCell(sheet, 4, 10, txtItnbr.Text.Trim());      // K5
                SetCell(sheet, 6, 10, tempSize);                  // K7
                SetCell(sheet, 4, 30, "*" + txtItnbr.Text.Trim() + "*"); // AE5 (Barcode)
                SetCell(sheet, 9, 10, machNo);                    // K10

                // -- Dropdown Colors --
                SetCell(sheet, 6, 34, ddlCirColor.SelectedItem?.Text); // AI7
                SetCell(sheet, 6, 42, ddlColor1.SelectedItem?.Text);   // AQ7
                SetCell(sheet, 6, 50, ddlColor2.SelectedItem?.Text);   // AY7
                SetCell(sheet, 8, 42, ddlColor.SelectedItem?.Text);    // AQ9

                // -- Thông số --
                SetCell(sheet, 10, 30, txtHeightPci.Text.Trim()); // AE11
                SetCell(sheet, 12, 30, txtPressure.Text.Trim());  // AE13
                SetCell(sheet, 10, 50, txtLType.Text.Trim());     // AY11
                SetCell(sheet, 11, 50, txtLHeight.Text.Trim());   // AY12
                SetCell(sheet, 12, 50, txtLPut.Text.Trim());      // AY13
                SetCell(sheet, 13, 10, txtForceKn.Text.Trim() + " ± 50"); // K14

                // -- Thời gian --
                SetCell(sheet, 11, 30, totalTime); // AE12
                SetCell(sheet, 24, 10, totalTime); // K25

                // -- Mảng thời gian (11 máy) --
                // Các cột: K(10), O(14), S(18), W(22), AA(26), AE(30), AI(34), AM(38), AQ(42), AU(46), AY(50)
                int[] timeCols = { 10, 14, 18, 22, 26, 30, 34, 38, 42, 46, 50 };
                for (int i = 0; i < 11; i++)
                {
                    SetCell(sheet, 15, timeCols[i], machTimes[i]); // Dòng 16 (index 15)
                }

                // -- Mold & Batch --
                SetCell(sheet, 25, 10, txtRingNo.Text.Trim());      // K26
                SetCell(sheet, 26, 10, txtLBatchSize.Text.Trim());  // K27
                SetCell(sheet, 25, 42, txtBatchClamp.Text.Trim());  // AQ26
                SetCell(sheet, 26, 42, txtMoldStyle.Text.Trim());   // AQ27

                // -- Chi tiết cuối --
                SetCell(sheet, 27, 19, txtTread.Text.Trim());     // T28
                SetCell(sheet, 28, 19, txtSpeed.Text.Trim());     // T29
                SetCell(sheet, 29, 19, txtDot.Text.Trim());       // T30
                SetCell(sheet, 30, 19, txtMaxLoad.Text.Trim());   // T31
                SetCell(sheet, 31, 19, txtNote1.Text.Trim());     // T32
                SetCell(sheet, 32, 19, txtNote2.Text.Trim());     // T33
                SetCell(sheet, 27, 45, txtSidewall.Text.Trim());  // AT28
                SetCell(sheet, 28, 45, txtStructCod.Text.Trim()); // AT29


                // 4. XUẤT FILE RA TRÌNH DUYỆT
                string fileName = "PCR_" + txtItnbr.Text.Trim() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);

                HttpCookie cookie = new HttpCookie("fileDownload", "true");
                cookie.Path = "/";
                Response.Cookies.Add(cookie);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    Response.BinaryWrite(memoryStream.ToArray());
                }

                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "Lỗi xuất Excel: " + ex.Message, "error");
            }
        }

        private void SetCell(ISheet sheet, int rowIdx, int colIdx, string value)
        {
            var row = sheet.GetRow(rowIdx) ?? sheet.CreateRow(rowIdx); // Lấy dòng, nếu chưa có thì tạo mới
            var cell = row.GetCell(colIdx) ?? row.CreateCell(colIdx);  // Lấy ô, nếu chưa có thì tạo mới
            cell.SetCellValue(value ?? ""); // Gán giá trị
        }

        // --- btnExcelAll ---
        protected void btnExcelAll_Click(object sender, EventArgs e)
        {
            try
            {
                ExportAllToExcel();
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "Lỗi xuất Excel: " + ex.Message, "error");
            }
        }

        private void ExportAllToExcel()
        {
            try
            {
                // 1. LẤY DỮ LIỆU TỪ INFORMIX
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT machno, version, itnbr, lmoldsize, rmoldsize, moldstyle, ");
                sql.Append("lbatchsize, rbatchsize, batchclamp, ringno, spec, tireno2, ");
                sql.Append("trcircolor, trrcolor1, trrcolor2, trrcolor3, color, ");
                sql.Append("tread, speed, sidewall, structcod, dot, maxload, note1, note2, ");
                sql.Append("scansta, ltemp, rtemp, leptemp, reptemp, ");
                sql.Append("lheight, rheight, ltype, rtype, force, forcekn, heightpci, pressure, ");
                sql.Append("timepci, lput, rput, stype, used, state, indat, usrno ");
                sql.Append("FROM [erp].[dbo].[prdepv] ");
                sql.Append("WHERE (state = '' OR state = ' ' OR state IS NULL) AND itnbr <> ''");
                sql.Append("ORDER BY itnbr, machno, version");

                DataTable dtAll = SQLConn34.ExecuteQuery(sql.ToString());

                if (dtAll == null || dtAll.Rows.Count == 0)
                {
                    NotificationHelper.Show(this, "Không có dữ liệu để xuất!", "warning");
                    return;
                }

                // 2. XUẤT EXCEL - 
                string fileName = $"CPRDV02_AllData_{DateTime.Now:yyyyMMddHHmmss}";
                ExcelHelper.ExportToExcel(dtAll, fileName, "CPRDV02_Data");

                // Thông báo thành công
                NotificationHelper.Show(this, $"Đã xuất {dtAll.Rows.Count} dòng dữ liệu!", "success");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất Excel All: " + ex.Message);
            }
        }

        // --- LOGIC XỬ LÝ DATABASE (BATCH PAGING) ---

        private string BuildWhereClause()
        {
            StringBuilder sb = new StringBuilder();
            string sMachNo = ViewState["Search_MachNo"] as string ?? "";
            string sItnbr = ViewState["Search_Itnbr"] as string ?? "";

            if (!string.IsNullOrEmpty(sMachNo))
            {
                if (sMachNo.Contains("*"))
                    sb.AppendFormat(" AND machno LIKE '{0}' ", SafeStr(sMachNo.Replace("*", "%")));
                else
                    sb.AppendFormat(" AND machno = '{0}' ", SafeStr(sMachNo));
            }

            if (!string.IsNullOrEmpty(sItnbr))
            {
                if (sItnbr.Contains("*"))
                    sb.AppendFormat(" AND itnbr LIKE '{0}' ", SafeStr(sItnbr.Replace("*", "%")));
                else
                    sb.AppendFormat(" AND itnbr = '{0}' ", SafeStr(sItnbr));
            }

            sb.Append(" AND (state = '' OR state = ' ' OR state IS NULL) ");
            return sb.ToString();
        }

        private void ExecuteQuery()
        {
            try
            {
                ViewState["Search_MachNo"] = txtMachNo.Text.Trim();
                ViewState["Search_Itnbr"] = txtItnbr.Text.Trim();

                Session.Remove("CurrentBatchData");
                Session.Remove("CurrentBatchStart");

                // Cú pháp INFORMIX
                string sqlCount = "SELECT COUNT(*) FROM erp:prdepv WHERE 1=1 " + BuildWhereClause();

                DataTable dtCount = UnixConn.ExecuteQuery(sqlCount);
                int totalRows = 0;

                if (dtCount != null && dtCount.Rows.Count > 0)
                    totalRows = Convert.ToInt32(dtCount.Rows[0][0]);

                if (totalRows > 0)
                {
                    ViewState["TotalCount"] = totalRows;
                    ViewState["CurrentIndex"] = 0;
                    ShowRecord(0);
                    if (pnlNavigationToolbar != null) pnlNavigationToolbar.Visible = true;
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Không tìm thấy dữ liệu!');", true);
                    ClearForm();
                    ViewState["TotalCount"] = 0;
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Lỗi: {ex.Message}');", true);
            }
            finally
            {
                ResetToDefaultState();
            }
        }

        private void FetchBatchFromDB(int startIndex)
        {
            StringBuilder sql = new StringBuilder();
            // CÚ PHÁP PHÂN TRANG INFORMIX
            sql.AppendFormat("SELECT SKIP {0} FIRST {1} ROWID, machno, version, ", startIndex, BATCH_SIZE);
            sql.Append("lmoldsize, rmoldsize, moldstyle, lbatchsize, rbatchsize, batchclamp, itnbr, ");
            sql.Append("ringno, spec, tireno2, trcircolor, trrcolor1, trrcolor2, trrcolor3, color, ");
            sql.Append("tread, speed, sidewall, structcod, dot, maxload, note1, note2, ");
            sql.Append("scansta, ltemp, rtemp, leptemp, reptemp, lheight, rheight, ");
            sql.Append("ltype, rtype, force, forcekn, heightpci, pressure, ");
            sql.Append("timepci, lput, rput, stype, indat, usrno ");
            sql.Append("FROM erp:prdepv ");
            sql.Append("WHERE 1=1 ");
            sql.Append(BuildWhereClause());
            sql.Append(" ORDER BY itnbr, machno, version ");

            DataTable dt = UnixConn.ExecuteQuery(sql.ToString());

            Session["CurrentBatchData"] = dt;
            Session["CurrentBatchStart"] = startIndex;
        }

        private DataRow GetRecordByBatch(int globalIndex)
        {
            int batchPageNumber = globalIndex / BATCH_SIZE;
            int batchStartIndex = batchPageNumber * BATCH_SIZE;

            DataTable currentBatch = Session["CurrentBatchData"] as DataTable;
            int? sessionBatchStart = Session["CurrentBatchStart"] as int?;

            if (currentBatch == null || sessionBatchStart != batchStartIndex)
            {
                FetchBatchFromDB(batchStartIndex);
                currentBatch = Session["CurrentBatchData"] as DataTable;
            }

            if (currentBatch != null)
            {
                int localIndex = globalIndex % BATCH_SIZE;
                if (localIndex < currentBatch.Rows.Count)
                {
                    return currentBatch.Rows[localIndex];
                }
            }
            return null;
        }

        private void ShowRecord(int index)
        {
            int totalRows = ViewState["TotalCount"] != null ? (int)ViewState["TotalCount"] : 0;
            if (index < 0 || index >= totalRows) return;

            DataRow dr = GetRecordByBatch(index);

            if (dr != null)
            {
                ViewState["CurrentIndex"] = index;

                SetText(txtMachNo, dr["machno"]);
                SetText(txtItnbr, dr["itnbr"]);
                SetText(txtVersion, dr["version"]);
                SetText(txtLBatchSize, dr["lbatchsize"]);
                SetText(txtRBatchSize, dr["rbatchsize"]);
                SetText(txtBatchClamp, dr["batchclamp"]);
                SetText(txtRingNo, dr["ringno"]);
                SetText(txtSpecPci, dr["spec"]);
                SetText(txtTireNo2, dr["tireno2"]);
                SetText(txtLMoldSize, dr["lmoldsize"]);
                SetText(txtRMoldSize, dr["rmoldsize"]);
                SetText(txtMoldStyle, dr["moldstyle"]);

                SetDropdownValue(ddlColor, dr["color"].ToString().Trim());
                SetDropdownValue(ddlColor1, dr["trrcolor1"].ToString().Trim());
                SetDropdownValue(ddlColor2, dr["trrcolor2"].ToString().Trim());
                SetDropdownValue(ddlColor3, dr["trrcolor3"].ToString().Trim());
                SetDropdownValue(ddlCirColor, dr["trcircolor"].ToString().Trim());

                SetText(txtTread, dr["tread"]);
                SetText(txtSidewall, dr["sidewall"]);
                SetText(txtSpeed, dr["speed"]);
                SetText(txtStructCod, dr["structcod"]);
                SetText(txtDot, dr["dot"]);
                SetText(txtMaxLoad, dr["maxload"]);
                SetText(txtNote1, dr["note1"]);
                SetText(txtNote2, dr["note2"]);

                SetText(txtScanSta, dr["scansta"]);
                SetText(txtForce, dr["force"]);
                SetText(txtForceKn, dr["forcekn"]);
                SetText(txtHeightPci, dr["heightpci"]);
                SetText(txtTimePci, dr["timepci"]);
                SetText(txtPressure, dr["pressure"]);

                SetText(txtLTemp, dr["ltemp"]);
                SetText(txtRTemp, dr["rtemp"]);
                SetText(txtLEpTemp, dr["leptemp"]);
                SetText(txtREpTemp, dr["reptemp"]);
                SetText(txtLHeight, dr["lheight"]);
                SetText(txtRHeight, dr["rheight"]);
                SetText(txtLType, dr["ltype"]);
                SetText(txtRType, dr["rtype"]);
                SetText(txtLPut, dr["lput"]);
                SetText(txtRPut, dr["rput"]);
                SetText(txtStype, dr["stype"]);

                if (hdnRowID != null) hdnRowID.Value = dr["ROWID"].ToString().Trim();

                if (lblIndex != null) lblIndex.Text = (index + 1).ToString();
                if (lblCount != null) lblCount.Text = totalRows.ToString();
                if (lblNavIndex != null) lblNavIndex.Text = (index + 1).ToString();
                if (lblNavCount != null) lblNavCount.Text = totalRows.ToString();

                SetText(lblIndat, dr["indat"]);
                SetText(lblUserNo, dr["usrno"]);

                UpdateNavigationButtons();
            }
        }

        private void SetText(WebControl control, object value)
        {
            if (control == null) return;
            string valStr = value != null ? value.ToString().Trim() : "";
            if (control is TextBox txt) txt.Text = valStr;
            else if (control is Label lbl) lbl.Text = valStr;
        }

        private void SetDropdownValue(DropDownList ddl, string value)
        {
            if (ddl != null && !string.IsNullOrEmpty(value))
            {
                string valStr = value.Trim();
                ListItem existingItem = ddl.Items.FindByValue(valStr);
                if (existingItem == null)
                {
                    ddl.Items.Add(new ListItem(valStr, valStr));
                }
                try { ddl.SelectedValue = valStr; }
                catch { ddl.SelectedIndex = -1; }
            }
            else if (ddl != null) ddl.SelectedIndex = -1;
        }

        private void UpdateNavigationButtons()
        {
            int totalRows = ViewState["TotalCount"] != null ? (int)ViewState["TotalCount"] : 0;
            int currentIndex = ViewState["CurrentIndex"] != null ? (int)ViewState["CurrentIndex"] : 0;

            btnFetchFirst.Enabled = currentIndex > 0;
            btnFetchPrevious.Enabled = currentIndex > 0;
            btnFetchNext.Enabled = currentIndex < totalRows - 1;
            btnFetchLast.Enabled = currentIndex < totalRows - 1;

            bool hasData = totalRows > 0;

            // Xử lý ẩn hiện nút dựa trên Cả DATA và QUYỀN HẠN
            if (btnModify != null)
            {
                bool canModify = hasData && _hasEditPermission;
                btnModify.Enabled = canModify;
                btnModify.Visible = _hasEditPermission;
                btnModify.Style["opacity"] = canModify ? "1" : "0.5";
            }

            if (btnDelete != null)
            {
                bool canDelete = hasData && _hasEditPermission;
                btnDelete.Enabled = canDelete;
                btnDelete.Visible = _hasEditPermission;
                btnDelete.Style["opacity"] = canDelete ? "1" : "0.5";
            }
        }

        // --- NAVIGATION BUTTON EVENTS ---

        protected void btnFetchFirst_Click(object sender, EventArgs e)
        {
            Thread.Sleep(100);
            ShowRecord(0);
        }

        protected void btnFetchPrevious_Click(object sender, EventArgs e)
        {
            Thread.Sleep(100);
            int currentIndex = ViewState["CurrentIndex"] != null ? (int)ViewState["CurrentIndex"] : 0;
            if (currentIndex > 0) ShowRecord(currentIndex - 1);
        }

        protected void btnFetchNext_Click(object sender, EventArgs e)
        {
            Thread.Sleep(100);
            int currentIndex = ViewState["CurrentIndex"] != null ? (int)ViewState["CurrentIndex"] : 0;
            int totalRows = ViewState["TotalCount"] != null ? (int)ViewState["TotalCount"] : 0;
            if (currentIndex < totalRows - 1) ShowRecord(currentIndex + 1);
        }

        protected void btnFetchLast_Click(object sender, EventArgs e)
        {
            Thread.Sleep(100);
            int totalRows = ViewState["TotalCount"] != null ? (int)ViewState["TotalCount"] : 0;
            if (totalRows > 0) ShowRecord(totalRows - 1);
        }

        private void ExecuteInsert()
        {
            if (string.IsNullOrEmpty(txtMachNo.Text.Trim()))
            {
                NotificationHelper.Show(this, "MACHNO RỖNG !!!", "warning");
                txtMachNo.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtItnbr.Text.Trim()))
            {
                NotificationHelper.Show(this, "ITNBR RỖNG !!!", "warning");
                txtItnbr.Focus();
                return;
            }

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string currentTime = DateTime.Now.ToString("HHmmss");
            string currentUser = Session["username"] as string ?? lblUserNo.Text;
            if (string.IsNullOrEmpty(currentUser)) currentUser = "ADMIN";

            // CÚ PHÁP INFORMIX
            StringBuilder sqlInsert = new StringBuilder();
            sqlInsert.Append("INSERT INTO erp:prdepv (");
            sqlInsert.Append("machno, version, lmoldsize, rmoldsize, moldstyle, ");
            sqlInsert.Append("lbatchsize, rbatchsize, batchclamp, itnbr, ringno, spec, ");
            sqlInsert.Append("tireno2, trcircolor, trrcolor1, trrcolor2, trrcolor3, color, ");
            sqlInsert.Append("tread, speed, sidewall, structcod, dot, maxload, note1, note2, ");
            sqlInsert.Append("scansta, ltemp, rtemp, leptemp, reptemp, ");
            sqlInsert.Append("lheight, rheight, ltype, rtype, force, ");
            sqlInsert.Append("forcekn, heightpci, pressure, ");
            sqlInsert.Append("timepci, lput, rput, stype, used, state, indat, usrno");
            sqlInsert.Append(") VALUES (");

            // Bỏ tiền tố N, dùng SafeStr, GetNum bảo vệ lỗi Syntax
            sqlInsert.AppendFormat("'{0}', '{1}', ", SafeStr(txtMachNo.Text), SafeStr(txtVersion.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtLMoldSize.Text), SafeStr(txtRMoldSize.Text), SafeStr(txtMoldStyle.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtLBatchSize.Text), SafeStr(txtRBatchSize.Text), SafeStr(txtBatchClamp.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtItnbr.Text), SafeStr(txtRingNo.Text), SafeStr(txtSpecPci.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', ",
                SafeStr(txtTireNo2.Text), SafeStr(ddlCirColor.SelectedValue), SafeStr(ddlColor1.SelectedValue),
                SafeStr(ddlColor2.SelectedValue), SafeStr(ddlColor3.SelectedValue), SafeStr(ddlColor.SelectedValue));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                SafeStr(txtTread.Text), SafeStr(txtSpeed.Text), SafeStr(txtSidewall.Text),
                SafeStr(txtStructCod.Text), SafeStr(txtDot.Text), SafeStr(txtMaxLoad.Text),
                SafeStr(txtNote1.Text), SafeStr(txtNote2.Text));
            sqlInsert.AppendFormat("{0}, {1}, {2}, {3}, {4}, ",
                GetNum(txtScanSta.Text), GetNum(txtLTemp.Text), GetNum(txtRTemp.Text),
                GetNum(txtLEpTemp.Text), GetNum(txtREpTemp.Text));
            sqlInsert.AppendFormat("{0}, {1}, '{2}', '{3}', {4}, ",
                GetNum(txtLHeight.Text), GetNum(txtRHeight.Text), SafeStr(txtLType.Text),
                SafeStr(txtRType.Text), GetNum(txtForce.Text));
            sqlInsert.AppendFormat("{0}, {1}, {2}, ",
                GetNum(txtForceKn.Text), GetNum(txtHeightPci.Text), GetNum(txtPressure.Text));
            sqlInsert.AppendFormat("{0}, '{1}', '{2}', '{3}', 'Y', ' ', '{4}', '{5}'",
                GetNum(txtTimePci.Text), SafeStr(txtLPut.Text), SafeStr(txtRPut.Text),
                SafeStr(txtStype.Text), currentDate, currentUser);
            sqlInsert.Append(")");

            bool in_epv = UnixConn.ExecuteNonQuery(sqlInsert.ToString());
            if (!in_epv)
            {
                NotificationHelper.Show(this, "LỖI THÊM LIỆU !!!", "error");
                return;
            }

            // BƯỚC 3: Backup (Informix miskv:prdepvbk)
            StringBuilder sqlBk = new StringBuilder();
            sqlBk.Append("INSERT INTO miskv:prdepvbk (");
            sqlBk.Append("func, intime, ");
            sqlBk.Append("machno, version, lmoldsize, rmoldsize, moldstyle, ");
            sqlBk.Append("lbatchsize, rbatchsize, batchclamp, itnbr, ringno, spec, ");
            sqlBk.Append("tireno2, trcircolor, trrcolor1, trrcolor2, trrcolor3, color, ");
            sqlBk.Append("tread, speed, sidewall, structcod, dot, maxload, note1, note2, ");
            sqlBk.Append("scansta, ltemp, rtemp, leptemp, reptemp, ");
            sqlBk.Append("lheight, rheight, ltype, rtype, force, ");
            sqlBk.Append("forcekn, heightpci, pressure, ");
            sqlBk.Append("timepci, lput, rput, stype, used, state, indat, usrno");
            sqlBk.Append(") VALUES (");
            sqlBk.AppendFormat("'ADD', '{0}', ", currentTime);
            sqlBk.AppendFormat("'{0}', '{1}', ", SafeStr(txtMachNo.Text), SafeStr(txtVersion.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtLMoldSize.Text), SafeStr(txtRMoldSize.Text), SafeStr(txtMoldStyle.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtLBatchSize.Text), SafeStr(txtRBatchSize.Text), SafeStr(txtBatchClamp.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtItnbr.Text), SafeStr(txtRingNo.Text), SafeStr(txtSpecPci.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', ",
                SafeStr(txtTireNo2.Text), SafeStr(ddlCirColor.SelectedValue), SafeStr(ddlColor1.SelectedValue),
                SafeStr(ddlColor2.SelectedValue), SafeStr(ddlColor3.SelectedValue), SafeStr(ddlColor.SelectedValue));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                SafeStr(txtTread.Text), SafeStr(txtSpeed.Text), SafeStr(txtSidewall.Text),
                SafeStr(txtStructCod.Text), SafeStr(txtDot.Text), SafeStr(txtMaxLoad.Text),
                SafeStr(txtNote1.Text), SafeStr(txtNote2.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                SafeStr(txtScanSta.Text), SafeStr(txtLTemp.Text), SafeStr(txtRTemp.Text),
                SafeStr(txtLEpTemp.Text), SafeStr(txtREpTemp.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                SafeStr(txtLHeight.Text), SafeStr(txtRHeight.Text), SafeStr(txtLType.Text),
                SafeStr(txtRType.Text), SafeStr(txtForce.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', ",
                SafeStr(txtForceKn.Text), SafeStr(txtHeightPci.Text), SafeStr(txtPressure.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', 'Y', ' ', '{4}', '{5}'",
                SafeStr(txtTimePci.Text), SafeStr(txtLPut.Text), SafeStr(txtRPut.Text),
                SafeStr(txtStype.Text), currentDate, currentUser);
            sqlBk.Append(")");

            try
            {
                UnixConn.ExecuteNonQuery(sqlBk.ToString());
            }
            catch (Exception exBk)
            {
                Console.WriteLine("Lỗi Backup: " + exBk.Message);
            }
            NotificationHelper.Show(this, "THÊM LIỆU THÀNH CÔNG !!!", "success");
            ClearForm();
            ResetToDefaultState();
        }

        private void ExecuteUpdate()
        {
            if (string.IsNullOrWhiteSpace(txtItnbr.Text))
            {
                NotificationHelper.Show(this, "KHÔNG TÌM THẤY MSTP !!!", "warning");
                return;
            }

            if (!ValidateRequiredFields())
            {
                return;
            }

            try
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string currentTime = DateTime.Now.ToString("HHmmss");
                string currentUser = Session["username"]?.ToString() ?? "ADMIN";

                // Lấy version cũ và mới từ ViewState và TextBox
                string oldVersion = ViewState["OldVersion"]?.ToString() ?? "";
                string newVersion = txtVersion.Text.Trim();

                if (string.IsNullOrEmpty(oldVersion))
                {
                    NotificationHelper.Show(this, "Không xác định được version cũ!", "error");
                    return;
                }

                // BƯỚC 1: Disable version cũ (INFORMIX)
                string sqlDisable = $"UPDATE erp:prdepv SET state = '*' WHERE itnbr = '{SafeStr(txtItnbr.Text)}' AND version = '{SafeStr(oldVersion)}'";
                UnixConn.ExecuteNonQuery(sqlDisable);

                // BƯỚC 2: Insert record mới với version mới
                StringBuilder sqlInsert = new StringBuilder();
                sqlInsert.Append("INSERT INTO erp:prdepv (");
                sqlInsert.Append("machno, version, lmoldsize, rmoldsize, moldstyle, ");
                sqlInsert.Append("lbatchsize, rbatchsize, batchclamp, itnbr, ringno, spec, ");
                sqlInsert.Append("tireno2, trcircolor, trrcolor1, trrcolor2, trrcolor3, color, ");
                sqlInsert.Append("tread, speed, sidewall, structcod, dot, maxload, note1, note2, ");
                sqlInsert.Append("scansta, ltemp, rtemp, leptemp, reptemp, ");
                sqlInsert.Append("lheight, rheight, ltype, rtype, force, ");
                sqlInsert.Append("forcekn, heightpci, pressure, ");
                sqlInsert.Append("timepci, lput, rput, stype, used, state, indat, usrno");
                sqlInsert.Append(") VALUES (");

                sqlInsert.AppendFormat("'{0}', '{1}', ", SafeStr(txtMachNo.Text), SafeStr(newVersion));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtLMoldSize.Text), SafeStr(txtRMoldSize.Text), SafeStr(txtMoldStyle.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtLBatchSize.Text), SafeStr(txtRBatchSize.Text), SafeStr(txtBatchClamp.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtItnbr.Text), SafeStr(txtRingNo.Text), SafeStr(txtSpecPci.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', ",
                    SafeStr(txtTireNo2.Text), SafeStr(ddlCirColor.SelectedValue), SafeStr(ddlColor1.SelectedValue),
                    SafeStr(ddlColor2.SelectedValue), SafeStr(ddlColor3.SelectedValue), SafeStr(ddlColor.SelectedValue));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                    SafeStr(txtTread.Text), SafeStr(txtSpeed.Text), SafeStr(txtSidewall.Text),
                    SafeStr(txtStructCod.Text), SafeStr(txtDot.Text), SafeStr(txtMaxLoad.Text),
                    SafeStr(txtNote1.Text), SafeStr(txtNote2.Text));
                sqlInsert.AppendFormat("{0}, {1}, {2}, {3}, {4}, ",
                    GetNum(txtScanSta.Text), GetNum(txtLTemp.Text), GetNum(txtRTemp.Text),
                    GetNum(txtLEpTemp.Text), GetNum(txtREpTemp.Text));
                sqlInsert.AppendFormat("{0}, {1}, '{2}', '{3}', {4}, ",
                    GetNum(txtLHeight.Text), GetNum(txtRHeight.Text), SafeStr(txtLType.Text),
                    SafeStr(txtRType.Text), GetNum(txtForce.Text));
                sqlInsert.AppendFormat("{0}, {1}, {2}, ",
                    GetNum(txtForceKn.Text), GetNum(txtHeightPci.Text), GetNum(txtPressure.Text));
                sqlInsert.AppendFormat("{0}, '{1}', '{2}', '{3}', 'Y', ' ', '{4}', '{5}'",
                    GetNum(txtTimePci.Text), SafeStr(txtLPut.Text), SafeStr(txtRPut.Text),
                    SafeStr(txtStype.Text), currentDate, currentUser);
                sqlInsert.Append(")");

                bool in_epv = UnixConn.ExecuteNonQuery(sqlInsert.ToString());
                if (!in_epv)
                {
                    NotificationHelper.Show(this, "LỖI THÊM LIỆU !!!", "error");
                    return;
                }

                // BƯỚC 3: Backup
                StringBuilder sqlBk = new StringBuilder();
                sqlBk.Append("INSERT INTO miskv:prdepvbk (");
                sqlBk.Append("func, intime, ");
                sqlBk.Append("machno, version, lmoldsize, rmoldsize, moldstyle, ");
                sqlBk.Append("lbatchsize, rbatchsize, batchclamp, itnbr, ringno, spec, ");
                sqlBk.Append("tireno2, trcircolor, trrcolor1, trrcolor2, trrcolor3, color, ");
                sqlBk.Append("tread, speed, sidewall, structcod, dot, maxload, note1, note2, ");
                sqlBk.Append("scansta, ltemp, rtemp, leptemp, reptemp, ");
                sqlBk.Append("lheight, rheight, ltype, rtype, force, ");
                sqlBk.Append("forcekn, heightpci, pressure, ");
                sqlBk.Append("timepci, lput, rput, stype, used, state, indat, usrno");
                sqlBk.Append(") VALUES (");
                sqlBk.AppendFormat("'MOD', '{0}', ", currentTime);
                sqlBk.AppendFormat("'{0}', '{1}', ", SafeStr(txtMachNo.Text), SafeStr(newVersion));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtLMoldSize.Text), SafeStr(txtRMoldSize.Text), SafeStr(txtMoldStyle.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtLBatchSize.Text), SafeStr(txtRBatchSize.Text), SafeStr(txtBatchClamp.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', ", SafeStr(txtItnbr.Text), SafeStr(txtRingNo.Text), SafeStr(txtSpecPci.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', ",
                    SafeStr(txtTireNo2.Text), SafeStr(ddlCirColor.SelectedValue), SafeStr(ddlColor1.SelectedValue),
                    SafeStr(ddlColor2.SelectedValue), SafeStr(ddlColor3.SelectedValue), SafeStr(ddlColor.SelectedValue));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                    SafeStr(txtTread.Text), SafeStr(txtSpeed.Text), SafeStr(txtSidewall.Text),
                    SafeStr(txtStructCod.Text), SafeStr(txtDot.Text), SafeStr(txtMaxLoad.Text),
                    SafeStr(txtNote1.Text), SafeStr(txtNote2.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                    SafeStr(txtScanSta.Text), SafeStr(txtLTemp.Text), SafeStr(txtRTemp.Text),
                    SafeStr(txtLEpTemp.Text), SafeStr(txtREpTemp.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                    SafeStr(txtLHeight.Text), SafeStr(txtRHeight.Text), SafeStr(txtLType.Text),
                    SafeStr(txtRType.Text), SafeStr(txtForce.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', ",
                    SafeStr(txtForceKn.Text), SafeStr(txtHeightPci.Text), SafeStr(txtPressure.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', 'Y', ' ', '{4}', '{5}'",
                    SafeStr(txtTimePci.Text), SafeStr(txtLPut.Text), SafeStr(txtRPut.Text),
                    SafeStr(txtStype.Text), currentDate, currentUser);
                sqlBk.Append(")");

                try
                {
                    UnixConn.ExecuteNonQuery(sqlBk.ToString());
                }
                catch (Exception exBk)
                {
                    Console.WriteLine("Lỗi Backup: " + exBk.Message);
                }

                NotificationHelper.Show(this, "MODIFY SUCCESS !!!", "success");

                // Clear cache
                Session.Remove("CurrentBatchData");
                Session.Remove("CurrentBatchStart");
                ViewState.Remove("OldVersion");

                ClearForm();
                ResetToDefaultState();
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "Lỗi: " + ex.Message, "error");
            }
        }

        /// <summary>
        /// Validate các trường bắt buộc (giống AFTER FIELD trong 4GL)
        /// </summary>
        private bool ValidateRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(txtForce.Text))
            {
                NotificationHelper.Show(this, "FORCE IS NULL !!!", "warning");
                txtForce.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtForceKn.Text))
            {
                NotificationHelper.Show(this, "FORCEKN IS NULL !!!", "warning");
                txtForceKn.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtHeightPci.Text))
            {
                NotificationHelper.Show(this, "HEIGHTPCI IS NULL !!!", "warning");
                txtHeightPci.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTimePci.Text))
            {
                NotificationHelper.Show(this, "TIMEPCI IS NULL !!!", "warning");
                txtTimePci.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPressure.Text))
            {
                NotificationHelper.Show(this, "PRESSURE IS NULL !!!", "warning");
                txtPressure.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLTemp.Text))
            {
                NotificationHelper.Show(this, "LTEMP IS NULL !!!", "warning");
                txtLTemp.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRTemp.Text))
            {
                NotificationHelper.Show(this, "RTEMP IS NULL !!!", "warning");
                txtRTemp.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLEpTemp.Text))
            {
                NotificationHelper.Show(this, "LEPTEMP IS NULL !!!", "warning");
                txtLEpTemp.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtREpTemp.Text))
            {
                NotificationHelper.Show(this, "REPTEMP IS NULL !!!", "warning");
                txtREpTemp.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLHeight.Text))
            {
                NotificationHelper.Show(this, "LHEIGHT IS NULL !!!", "warning");
                txtLHeight.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRHeight.Text))
            {
                NotificationHelper.Show(this, "RHEIGHT IS NULL !!!", "warning");
                txtRHeight.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLType.Text))
            {
                NotificationHelper.Show(this, "LTYPE IS NULL !!!", "warning");
                txtLType.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRType.Text))
            {
                NotificationHelper.Show(this, "RTYPE IS NULL !!!", "warning");
                txtRType.Focus();
                return false;
            }

            return true;
        }

        protected void txtMachNo_TextChanged(object sender, EventArgs e)
        {
            if (txtMachNo.Text.Trim().Length != 3)
            {
                NotificationHelper.Show(this, "MACHNO 3 KÝ TỰ!!!", "warning");
                txtMachNo.Focus();
                txtMachNo.Text = "";
                return;
            }

            if (txtMachNo.Text.Trim() != "C01")
            {
                NotificationHelper.Show(this, "MACHNO <> 'C01' !!", "warning");
                txtMachNo.Focus();
                txtMachNo.Text = "";
                return;
            }
        }

        protected void txtItnbr_TextChanged(object sender, EventArgs e)
        {
            if (CurrentMode == ActionMode.ADD || CurrentMode == ActionMode.MODIFY)
            {
                int count = Convert.ToInt32(UnixConn.ExecuteQuery("SELECT COUNT(*) FROM erp:prdepv WHERE machno = '" + SafeStr(txtMachNo.Text) + "' " +
                                                    " AND itnbr = '" + SafeStr(txtItnbr.Text) + "' AND (state = '' OR state = ' ' OR state IS NULL)").Rows[0][0]);
                if (count > 0)
                {
                    NotificationHelper.Show(this, "Máy " + txtMachNo.Text + " đã tồn tại " + txtItnbr.Text, "warning");
                    txtItnbr.Focus();
                    txtItnbr.Text = "";
                    return;
                }

                DataTable dt_invmat = UnixConn.ExecuteQuery("SELECT * FROM erp:invmat WHERE factory = 'E' AND modsta != 'N' AND itnbr = '" + SafeStr(txtItnbr.Text) + "'");
                if (dt_invmat == null || dt_invmat.Rows.Count == 0)
                {
                    NotificationHelper.Show(this, "KV2 KHÔNG SẢN XUẤT MSTP NÀY !!!", "warning");
                    txtItnbr.Focus();
                    txtItnbr.Text = "";
                    return;
                }

                int count_evv = Convert.ToInt32(SQLConn34.ExecuteQuery("SELECT COUNT(*) FROM [erp].[dbo].[prdevv] WHERE " +
                                                    " itnbr = '" + SafeStr(txtItnbr.Text) + "' AND (state = '' OR state = ' ' OR state IS NULL)").Rows[0][0]);
                if (count_evv == 0)
                {
                    NotificationHelper.Show(this, "Mã này chưa nhập CPRDV01", "warning");
                    txtItnbr.Focus();
                    txtItnbr.Text = "";
                    return;
                }

                LoadData(txtItnbr.Text.Trim(), txtMachNo.Text.Trim());

                txtLTemp.Text = "182";
                txtRTemp.Text = "182";
                txtLEpTemp.Text = "182";
                txtREpTemp.Text = "182";
                txtHeightPci.Text = "8.0";
                txtScanSta.Text = "1";

                txtStype.Text = "T";
            }
        }

        private int p_cprdv02_timepci(int? cureMin, int? cureSec, int? actMin, int? actSec)
        {
            int lCureMin = cureMin ?? 0;
            int lCureSec = cureSec ?? 0;
            int lActMin = actMin ?? 0;
            int lActSec = actSec ?? 0;

            // Tính TimePCI
            int timePci = (lCureMin * 60 + lCureSec) + (lActMin * 60 + lActSec);

            return timePci;
        }

        private string GetSpec(string moldno)
        {
            string spec = string.Empty;

            if (string.IsNullOrEmpty(moldno))
                return spec;

            for (int i = 0; i < moldno.Length; i++)
            {
                if (moldno[i] == 'R')
                {
                    // Lấy 2 ký tự sau 'R' (nếu đủ độ dài)
                    if (i + 2 < moldno.Length)
                    {
                        spec = moldno.Substring(i + 1, 2);
                    }
                    else if (i + 1 < moldno.Length)
                    {
                        // Chỉ còn 1 ký tự sau 'R'
                        spec = moldno.Substring(i + 1, 1);
                    }
                    break;
                }
            }

            return spec;
        }

        private void MapControls()
        {
            pnlInput = (Panel)RecursiveFindControl(this, "pnlInput");
            pnlMainToolbar = (Panel)RecursiveFindControl(this, "pnlMainToolbar");
            pnlConfirmToolbar = (Panel)RecursiveFindControl(this, "pnlConfirmToolbar");
            pnlNavigationToolbar = (Panel)RecursiveFindControl(this, "pnlNavigationToolbar");

            btnQuery = (LinkButton)RecursiveFindControl(this, "btnQuery");
            btnAdd = (LinkButton)RecursiveFindControl(this, "btnAdd");
            btnModify = (LinkButton)RecursiveFindControl(this, "btnModify");
            btnDelete = (LinkButton)RecursiveFindControl(this, "btnDelete");
            btnExcel = (LinkButton)RecursiveFindControl(this, "btnExcel");
            btnExcelAll = (LinkButton)RecursiveFindControl(this, "btnExcelAll");
            btnOK = (LinkButton)RecursiveFindControl(this, "btnOK");
            btnCancel = (LinkButton)RecursiveFindControl(this, "btnCancel");

            btnFetchFirst = (LinkButton)RecursiveFindControl(this, "btnFetchFirst");
            btnFetchPrevious = (LinkButton)RecursiveFindControl(this, "btnFetchPrevious");
            btnFetchNext = (LinkButton)RecursiveFindControl(this, "btnFetchNext");
            btnFetchLast = (LinkButton)RecursiveFindControl(this, "btnFetchLast");

            ddlColor = (DropDownList)RecursiveFindControl(this, "ddlColor");
            ddlColor1 = (DropDownList)RecursiveFindControl(this, "ddlColor1");
            ddlColor2 = (DropDownList)RecursiveFindControl(this, "ddlColor2");
            ddlColor3 = (DropDownList)RecursiveFindControl(this, "ddlColor3");
            ddlCirColor = (DropDownList)RecursiveFindControl(this, "ddlCirColor");

            txtMachNo = (TextBox)RecursiveFindControl(this, "txtMachNo");
            txtItnbr = (TextBox)RecursiveFindControl(this, "txtItnbr");
            txtLBatchSize = (TextBox)RecursiveFindControl(this, "txtLBatchSize");
            txtRBatchSize = (TextBox)RecursiveFindControl(this, "txtRBatchSize");
            txtBatchClamp = (TextBox)RecursiveFindControl(this, "txtBatchClamp");
            txtVersion = (TextBox)RecursiveFindControl(this, "txtVersion");
            txtStype = (TextBox)RecursiveFindControl(this, "txtStype");
            txtSpecPci = (TextBox)RecursiveFindControl(this, "txtSpecPci");
            txtMoldStyle = (TextBox)RecursiveFindControl(this, "txtMoldStyle");
            txtRingNo = (TextBox)RecursiveFindControl(this, "txtRingNo");
            txtTireNo2 = (TextBox)RecursiveFindControl(this, "txtTireNo2");
            txtLMoldSize = (TextBox)RecursiveFindControl(this, "txtLMoldSize");
            txtRMoldSize = (TextBox)RecursiveFindControl(this, "txtRMoldSize");

            txtTread = (TextBox)RecursiveFindControl(this, "txtTread");
            txtSidewall = (TextBox)RecursiveFindControl(this, "txtSidewall");
            txtSpeed = (TextBox)RecursiveFindControl(this, "txtSpeed");
            txtStructCod = (TextBox)RecursiveFindControl(this, "txtStructCod");
            txtDot = (TextBox)RecursiveFindControl(this, "txtDot");
            txtMaxLoad = (TextBox)RecursiveFindControl(this, "txtMaxLoad");
            txtNote1 = (TextBox)RecursiveFindControl(this, "txtNote1");
            txtNote2 = (TextBox)RecursiveFindControl(this, "txtNote2");

            txtScanSta = (TextBox)RecursiveFindControl(this, "txtScanSta");
            txtForce = (TextBox)RecursiveFindControl(this, "txtForce");
            txtForceKn = (TextBox)RecursiveFindControl(this, "txtForceKn");
            txtHeightPci = (TextBox)RecursiveFindControl(this, "txtHeightPci");
            txtTimePci = (TextBox)RecursiveFindControl(this, "txtTimePci");
            txtPressure = (TextBox)RecursiveFindControl(this, "txtPressure");

            txtLTemp = (TextBox)RecursiveFindControl(this, "txtLTemp");
            txtRTemp = (TextBox)RecursiveFindControl(this, "txtRTemp");
            txtLEpTemp = (TextBox)RecursiveFindControl(this, "txtLEpTemp");
            txtREpTemp = (TextBox)RecursiveFindControl(this, "txtREpTemp");
            txtLHeight = (TextBox)RecursiveFindControl(this, "txtLHeight");
            txtRHeight = (TextBox)RecursiveFindControl(this, "txtRHeight");
            txtLType = (TextBox)RecursiveFindControl(this, "txtLType");
            txtRType = (TextBox)RecursiveFindControl(this, "txtRType");
            txtLPut = (TextBox)RecursiveFindControl(this, "txtLPut");
            txtRPut = (TextBox)RecursiveFindControl(this, "txtRPut");

            hdnRowID = (HiddenField)RecursiveFindControl(this, "hdnRowID");
            lblActionStatus = (Label)RecursiveFindControl(this, "lblActionStatus");
            lblIndex = (Label)RecursiveFindControl(this, "lblIndex");
            lblCount = (Label)RecursiveFindControl(this, "lblCount");
            lblNavIndex = (Label)RecursiveFindControl(this, "lblNavIndex");
            lblNavCount = (Label)RecursiveFindControl(this, "lblNavCount");
            lblIndat = (Label)RecursiveFindControl(this, "lblIndat");
            lblUserNo = (Label)RecursiveFindControl(this, "lblUserNo");
        }

        private Control RecursiveFindControl(Control root, string id)
        {
            if (root.ID == id) return root;
            foreach (Control c in root.Controls)
            {
                Control t = RecursiveFindControl(c, id);
                if (t != null) return t;
            }
            return null;
        }
    }
}