using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unix_Web.Helpers;
using Unix_Web.Providers;

namespace Unix_Web.Source.CPRD
{
    public partial class cprdv01 : System.Web.UI.Page
    {
        //test chức năng restore 1 2
        // --- CẤU HÌNH PHÂN TRANG ---
        private const int BATCH_SIZE = 20;

        // --- CẤU HÌNH PHÂN QUYỀN ---
        private bool _hasEditPermission = false;

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
            CheckUserPermission();

            if (!IsPostBack)
            {
                lblUserNo.Text = "";
                lblIndat.Text = "";
                ResetToDefaultState();
            }
        }

        // --- HÀM BẢO VỆ DỮ LIỆU ĐẦU VÀO (INFORMIX) ---
        private string SafeStr(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("'", "''").Trim();
        }

        private string GetNum(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "0";
            return input.Trim();
        }

        // --- HÀM KIỂM TRA QUYỀN ---
        private void CheckUserPermission()
        {
            string userID = Session["username"] as string ?? "";
            string currentProgram = "CPRDV01";
            string role = "VIEW";

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
                catch { }
            }

            if (role == "FULL" || role == "ADMIN")
            {
                _hasEditPermission = true;
            }
            else
            {
                _hasEditPermission = false;
            }
        }

        // --- QUẢN LÝ TRẠNG THÁI GIAO DIỆN ---
        private void ResetToDefaultState()
        {
            CurrentMode = ActionMode.NONE;

            if (pnlMainToolbar != null) pnlMainToolbar.Visible = true;
            if (pnlConfirmToolbar != null) pnlConfirmToolbar.Visible = false;

            bool hasData = hdnRowID != null && !string.IsNullOrEmpty(hdnRowID.Value);

            if (pnlNavigationToolbar != null)
            {
                int totalRows = ViewState["TotalCount"] != null ? (int)ViewState["TotalCount"] : 0;
                pnlNavigationToolbar.Visible = totalRows > 0;
            }

            ToggleInput(false);

            if (btnAdd != null)
            {
                btnAdd.Visible = _hasEditPermission;
            }

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
                txtSubno.Text = "4";
                txtFactory.Text = "V";
                txtStype.Text = "T";
                SetDefaultZeros();
            }
            else if (mode == ActionMode.QUERY)
            {
                ClearForm();
                ToggleInput(false);
                if (txtSubno != null) txtSubno.ReadOnly = false;
                if (txtFactory != null) txtFactory.ReadOnly = false;
                if (txtItnbr != null) txtItnbr.ReadOnly = false;
                if (txtVersion != null) txtVersion.ReadOnly = false;
            }
            else if (mode == ActionMode.MODIFY)
            {
                ToggleInput(true);
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
                    if (tb.ID != "txtVersion" && tb.ID != "txtSubno" && tb.ID != "txtFactory" &&
                        tb.ID != "txtTotalm" && tb.ID != "txtTotals")
                        tb.ReadOnly = readOnly;
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
                if (c is TextBox tb)
                {
                    tb.Text = "";
                }
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
        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            if (!_hasEditPermission) return;

            if (string.IsNullOrEmpty(hdnRowID.Value))
            {
                NotificationHelper.Show(this, "Không có dữ liệu để sửa!", "warning");
                return;
            }

            ViewState["OldVersion"] = txtVersion.Text.Trim();

            try
            {
                CalculateVersion();
                EnterActionMode(ActionMode.MODIFY);
                INVMAS();
                lblActionStatus.Text = $"MODE: MODIFY | Old Version: {ViewState["OldVersion"]} → New Version: {txtVersion.Text.Trim()}";
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "Lỗi: " + ex.Message, "error");
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

            if (hdnDeleteConfirmed.Value != "true")
            {
                string script = @"
                    if (confirm('DATA WILL BE DELETED, ARE YOU SURE?')) {
                        document.getElementById('" + hdnDeleteConfirmed.ClientID + @"').value = 'true';
                        " + Page.ClientScript.GetPostBackEventReference(btnDelete, "") + @";
                    }
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "ConfirmDelete", script, true);
                return;
            }

            hdnDeleteConfirmed.Value = "false";

            try
            {
                string recordID = hdnRowID.Value;
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string currentTime = DateTime.Now.ToString("HHmmss");
                string currentUser = Session["username"]?.ToString() ?? "ADMIN";

                // INFORMIX
                DataTable dtRecord = UnixConn.ExecuteQuery($"SELECT * FROM erp:prdevv WHERE ROWID = {recordID}");

                if (dtRecord == null || dtRecord.Rows.Count == 0)
                {
                    NotificationHelper.Show(this, "Không tìm thấy record!", "warning");
                    return;
                }

                DataRow dr = dtRecord.Rows[0];

                // UPDATE state='*' (INFORMIX)
                try
                {
                    UnixConn.ExecuteNonQuery($"UPDATE erp:prdevv SET state = '*' WHERE ROWID = {recordID}");
                }
                catch (Exception exDel)
                {
                    Console.WriteLine("Lỗi Xoá: " + exDel.Message);
                }

                // Backup (INFORMIX)
                StringBuilder sqlBk = new StringBuilder();
                sqlBk.Append("INSERT INTO miskv:prdevvbk (func, intime, subno, factory, itnbr, version, machno, ");
                sqlBk.Append("machm1, machs1, machm2, machs2, machm3, machs3, machm4, machs4, machm5, machs5, ");
                sqlBk.Append("machm6, machs6, machm7, machs7, machm8, machs8, machm9, machs9, machm10, machs10, ");
                sqlBk.Append("machm11, machs11, machm12, machs12, machm13, machs13, machm14, machs14, machm15, machs15, machm16, machs16, ");
                sqlBk.Append("totalm, totals, check1, check2, check3, check4, check5, check6, check7, check8, ");
                sqlBk.Append("check9, check10, check11, check12, check13, check14, check15, check16, ");
                sqlBk.Append("ostoptime, tstoptime, oopentime, topentime, opencheck, oplencheck, cllencheck, speedlen, ");
                sqlBk.Append("opostopcheck, optstopcheck, clostopcheck, cltstopcheck, wopcheck, wclcheck, opchecklen, ");
                sqlBk.Append("inocheck, ouocheck, tocheck, lheight, rheight, ltype, rtype, pressure, lput, rput, ");
                sqlBk.Append("indat, usrno, stype, used, state) VALUES (");
                sqlBk.AppendFormat("'DEL', '{0}', ", currentTime);
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                    dr["subno"].ToString().Trim(), dr["factory"].ToString().Trim(),
                    dr["itnbr"].ToString().Trim(), dr["version"].ToString().Trim(), dr["machno"].ToString().Trim());

                for (int i = 1; i <= 16; i++)
                {
                    sqlBk.AppendFormat("'{0}', '{1}', ",
                        dr[$"machm{i}"].ToString().Trim(), dr[$"machs{i}"].ToString().Trim());
                }

                sqlBk.AppendFormat("'{0}', '{1}', ", dr["totalm"].ToString().Trim(), dr["totals"].ToString().Trim());

                for (int i = 1; i <= 16; i++)
                {
                    sqlBk.AppendFormat("'{0}', ", dr[$"check{i}"].ToString().Trim());
                }

                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                    dr["ostoptime"].ToString().Trim(), dr["tstoptime"].ToString().Trim(),
                    dr["oopentime"].ToString().Trim(), dr["topentime"].ToString().Trim(),
                    dr["opencheck"].ToString().Trim(), dr["oplencheck"].ToString().Trim(),
                    dr["cllencheck"].ToString().Trim(), dr["speedlen"].ToString().Trim());

                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', ",
                    dr["opostopcheck"].ToString().Trim(), dr["optstopcheck"].ToString().Trim(),
                    dr["clostopcheck"].ToString().Trim(), dr["cltstopcheck"].ToString().Trim(),
                    dr["wopcheck"].ToString().Trim(), dr["wclcheck"].ToString().Trim(),
                    dr["opchecklen"].ToString().Trim());

                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', ",
                    dr["inocheck"].ToString().Trim(), dr["ouocheck"].ToString().Trim(),
                    dr["tocheck"].ToString().Trim(), dr["lheight"].ToString().Trim(),
                    dr["rheight"].ToString().Trim(), dr["ltype"].ToString().Trim(),
                    dr["rtype"].ToString().Trim(), dr["pressure"].ToString().Trim(),
                    dr["lput"].ToString().Trim(), dr["rput"].ToString().Trim());

                sqlBk.AppendFormat("'{0}', '{1}', '{2}', 'Y', '*')",
                    currentDate, currentUser, dr["stype"].ToString().Trim());

                try
                {
                    UnixConn.ExecuteNonQuery(sqlBk.ToString());
                }
                catch (Exception exBk)
                {
                    Console.WriteLine("Lỗi Backup: " + exBk.Message);
                }

                // Update prdepv (INFORMIX)
                UnixConn.ExecuteNonQuery($@"
                    UPDATE erp:prdepv 
                    SET pressure = 0, lheight = 0, rheight = 0, ltype = '0', rtype = '0'
                    WHERE itnbr = '{SafeStr(dr["itnbr"].ToString())}' 
                    AND (state IS NULL OR state = ' ' OR state = '')");

                NotificationHelper.Show(this, "DELETE SUCCESS !!!", "success");

                ClearForm();
                hdnRowID.Value = "";
                Session.Remove("CurrentBatchData");

                int totalRows = ViewState["TotalCount"] != null ? (int)ViewState["TotalCount"] : 0;
                if (totalRows > 0)
                {
                    int currentIndex = ViewState["CurrentIndex"] != null ? (int)ViewState["CurrentIndex"] : 0;
                    if (currentIndex >= totalRows - 1) currentIndex = totalRows - 2;
                    ViewState["TotalCount"] = totalRows - 1;

                    if (totalRows > 1 && currentIndex >= 0)
                        ShowRecord(currentIndex);
                    else
                        pnlNavigationToolbar.Visible = false;
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

        // --- LOGIC XỬ LÝ DATABASE ---
        private string BuildWhereClause()
        {
            StringBuilder sb = new StringBuilder();
            string sItnbr = ViewState["Search_Itnbr"] as string ?? "";

            if (!string.IsNullOrEmpty(sItnbr))
            {
                if (sItnbr.Contains("*"))
                    sb.AppendFormat(" AND itnbr LIKE '{0}' ", SafeStr(sItnbr.Replace("*", "%")));
                else
                    sb.AppendFormat(" AND itnbr = '{0}' ", SafeStr(sItnbr));
            }

            sb.Append(" AND subno = '4' ");
            sb.Append(" AND (state = '' OR state = ' ' OR state IS NULL) ");
            return sb.ToString();
        }

        private void ExecuteQuery()
        {
            try
            {
                ViewState["Search_Itnbr"] = txtItnbr.Text.Trim();

                Session.Remove("CurrentBatchData");
                Session.Remove("CurrentBatchStart");

                // INFORMIX COUNT
                string sqlCount = "SELECT COUNT(*) FROM erp:prdevv WHERE 1=1 " + BuildWhereClause();

                DataTable dtCount = UnixConn.ExecuteQuery(sqlCount);
                int totalRows = 0;

                if (dtCount != null && dtCount.Rows.Count > 0)
                    totalRows = Convert.ToInt32(dtCount.Rows[0][0]);

                if (totalRows > 0)
                {
                    ViewState["TotalCount"] = totalRows;
                    ViewState["CurrentIndex"] = 0;
                    INVMAS();
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
            sql.AppendFormat("SELECT SKIP {0} FIRST {1} ROWID, subno, factory, itnbr, version, machno, ", startIndex, BATCH_SIZE);
            sql.Append("machm1, machs1, machm2, machs2, machm3, machs3, machm4, machs4, machm5, machs5, ");
            sql.Append("machm6, machs6, machm7, machs7, machm8, machs8, machm9, machs9, machm10, machs10, ");
            sql.Append("machm11, machs11, machm12, machs12, machm13, machs13, machm14, machs14, machm15, machs15, machm16, machs16, ");
            sql.Append("totalm, totals, check1, check2, check3, check4, check5, check6, check7, check8, ");
            sql.Append("check9, check10, check11, check12, check13, check14, check15, check16, ");
            sql.Append("ostoptime, tstoptime, oopentime, topentime, opencheck, oplencheck, cllencheck, speedlen, ");
            sql.Append("opostopcheck, optstopcheck, clostopcheck, cltstopcheck, wopcheck, wclcheck, opchecklen, ");
            sql.Append("inocheck, ouocheck, tocheck, lheight, rheight, ltype, rtype, pressure, lput, rput, ");
            sql.Append("indat, usrno, stype ");
            sql.Append(" FROM erp:prdevv ");
            sql.Append(" WHERE 1=1 ");
            sql.Append(BuildWhereClause());
            sql.Append(" ORDER BY itnbr, version ");

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

                SetText(txtSubno, dr["subno"]);
                SetText(txtFactory, dr["factory"]);
                SetText(txtItnbr, dr["itnbr"]);
                SetText(txtVersion, dr["version"]);
                SetText(txtMachno, dr["machno"]);

                SetText(txtMachm1, dr["machm1"]); SetText(txtMachs1, dr["machs1"]);
                SetText(txtMachm2, dr["machm2"]); SetText(txtMachs2, dr["machs2"]);
                SetText(txtMachm3, dr["machm3"]); SetText(txtMachs3, dr["machs3"]);
                SetText(txtMachm4, dr["machm4"]); SetText(txtMachs4, dr["machs4"]);
                SetText(txtMachm5, dr["machm5"]); SetText(txtMachs5, dr["machs5"]);
                SetText(txtMachm6, dr["machm6"]); SetText(txtMachs6, dr["machs6"]);
                SetText(txtMachm7, dr["machm7"]); SetText(txtMachs7, dr["machs7"]);
                SetText(txtMachm8, dr["machm8"]); SetText(txtMachs8, dr["machs8"]);
                SetText(txtMachm9, dr["machm9"]); SetText(txtMachs9, dr["machs9"]);
                SetText(txtMachm10, dr["machm10"]); SetText(txtMachs10, dr["machs10"]);
                SetText(txtMachm11, dr["machm11"]); SetText(txtMachs11, dr["machs11"]);
                SetText(txtMachm12, dr["machm12"]); SetText(txtMachs12, dr["machs12"]);
                SetText(txtMachm13, dr["machm13"]); SetText(txtMachs13, dr["machs13"]);
                SetText(txtMachm14, dr["machm14"]); SetText(txtMachs14, dr["machs14"]);
                SetText(txtMachm15, dr["machm15"]); SetText(txtMachs15, dr["machs15"]);
                SetText(txtMachm16, dr["machm16"]); SetText(txtMachs16, dr["machs16"]);

                SetText(txtTotalm, dr["totalm"]);
                SetText(txtTotals, dr["totals"]);

                SetText(txtCheck1, dr["check1"]); SetText(txtCheck2, dr["check2"]);
                SetText(txtCheck3, dr["check3"]); SetText(txtCheck4, dr["check4"]);
                SetText(txtCheck5, dr["check5"]); SetText(txtCheck6, dr["check6"]);
                SetText(txtCheck7, dr["check7"]); SetText(txtCheck8, dr["check8"]);
                SetText(txtCheck9, dr["check9"]); SetText(txtCheck10, dr["check10"]);
                SetText(txtCheck11, dr["check11"]); SetText(txtCheck12, dr["check12"]);
                SetText(txtCheck13, dr["check13"]); SetText(txtCheck14, dr["check14"]);
                SetText(txtCheck15, dr["check15"]); SetText(txtCheck16, dr["check16"]);

                SetText(txtOstoptime, dr["ostoptime"]);
                SetText(txtTstoptime, dr["tstoptime"]);
                SetText(txtOopentime, dr["oopentime"]);
                SetText(txtTopentime, dr["topentime"]);
                SetText(txtOpencheck, dr["opencheck"]);
                SetText(txtOplencheck, dr["oplencheck"]);
                SetText(txtCllencheck, dr["cllencheck"]);
                SetText(txtSpeedlen, dr["speedlen"]);
                SetText(txtOpostopcheck, dr["opostopcheck"]);
                SetText(txtOptstopcheck, dr["optstopcheck"]);
                SetText(txtClostopcheck, dr["clostopcheck"]);
                SetText(txtCltstopcheck, dr["cltstopcheck"]);
                SetText(txtWopcheck, dr["wopcheck"]);
                SetText(txtWclcheck, dr["wclcheck"]);
                SetText(txtOpchecklen, dr["opchecklen"]);
                SetText(txtInocheck, dr["inocheck"]);
                SetText(txtOuocheck, dr["ouocheck"]);
                SetText(txtTocheck, dr["tocheck"]);

                SetText(txtLheight, dr["lheight"]);
                SetText(txtRheight, dr["rheight"]);
                SetText(txtLtype, dr["ltype"]);
                SetText(txtRtype, dr["rtype"]);
                SetText(txtPressure, dr["pressure"]);
                SetText(txtLput, dr["lput"]);
                SetText(txtRput, dr["rput"]);
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

        private void SetDefaultZeros()
        {
            TextBox[] numericFields = new TextBox[]
            {
                txtMachm1, txtMachs1, txtMachm2, txtMachs2, txtMachm3, txtMachs3, txtMachm4, txtMachs4,
                txtMachm5, txtMachs5, txtMachm6, txtMachs6, txtMachm7, txtMachs7, txtMachm8, txtMachs8,
                txtMachm9, txtMachs9, txtMachm10, txtMachs10, txtMachm11, txtMachs11, txtMachm12, txtMachs12,
                txtMachm13, txtMachs13, txtMachm14, txtMachs14, txtMachm15, txtMachs15, txtMachm16, txtMachs16,

                txtCheck1, txtCheck2, txtCheck3, txtCheck4, txtCheck5, txtCheck6, txtCheck7, txtCheck8,
                txtCheck9, txtCheck10, txtCheck11, txtCheck12, txtCheck13, txtCheck14, txtCheck15, txtCheck16,

                txtOstoptime, txtTstoptime, txtOopentime, txtTopentime, txtOpencheck, txtOplencheck,
                txtCllencheck, txtSpeedlen, txtOpostopcheck, txtOptstopcheck, txtClostopcheck, txtCltstopcheck,
                txtWopcheck, txtWclcheck, txtOpchecklen, txtInocheck, txtOuocheck, txtTocheck,

                txtLheight, txtRheight, txtLtype, txtRtype, txtPressure, txtLput, txtRput
            };

            foreach (TextBox tb in numericFields)
            {
                if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = "0";
                }
            }
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

            SetDefaultZeros();
            CalculateTotal();

            // INFORMIX INSERT
            StringBuilder sqlInsert = new StringBuilder();
            sqlInsert.Append("INSERT INTO erp:prdevv (");
            sqlInsert.Append("subno, factory, itnbr, version, machno, ");
            sqlInsert.Append("machm1, machs1, machm2, machs2, machm3, machs3, machm4, machs4, machm5, machs5, ");
            sqlInsert.Append("machm6, machs6, machm7, machs7, machm8, machs8, machm9, machs9, machm10, machs10, ");
            sqlInsert.Append("machm11, machs11, machm12, machs12, machm13, machs13, machm14, machs14, machm15, machs15, machm16, machs16, ");
            sqlInsert.Append("totalm, totals, check1, check2, check3, check4, check5, check6, check7, check8, ");
            sqlInsert.Append("check9, check10, check11, check12, check13, check14, check15, check16, ");
            sqlInsert.Append("ostoptime, tstoptime, oopentime, topentime, opencheck, oplencheck, cllencheck, speedlen, ");
            sqlInsert.Append("opostopcheck, optstopcheck, clostopcheck, cltstopcheck, wopcheck, wclcheck, opchecklen, ");
            sqlInsert.Append("inocheck, ouocheck, tocheck, lheight, rheight, ltype, rtype, pressure, lput, rput, ");
            sqlInsert.Append("indat, usrno, stype, used, state) VALUES (");

            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                SafeStr(txtSubno.Text), SafeStr(txtFactory.Text), SafeStr(txtItnbr.Text),
                SafeStr(txtVersion.Text), SafeStr(txtMachno.Text));

            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm1.Text), GetNum(txtMachs1.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm2.Text), GetNum(txtMachs2.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm3.Text), GetNum(txtMachs3.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm4.Text), GetNum(txtMachs4.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm5.Text), GetNum(txtMachs5.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm6.Text), GetNum(txtMachs6.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm7.Text), GetNum(txtMachs7.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm8.Text), GetNum(txtMachs8.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm9.Text), GetNum(txtMachs9.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm10.Text), GetNum(txtMachs10.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm11.Text), GetNum(txtMachs11.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm12.Text), GetNum(txtMachs12.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm13.Text), GetNum(txtMachs13.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm14.Text), GetNum(txtMachs14.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm15.Text), GetNum(txtMachs15.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm16.Text), GetNum(txtMachs16.Text));

            sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtTotalm.Text), GetNum(txtTotals.Text));

            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', ",
                SafeStr(txtCheck1.Text), SafeStr(txtCheck2.Text), SafeStr(txtCheck3.Text), SafeStr(txtCheck4.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', ",
                SafeStr(txtCheck5.Text), SafeStr(txtCheck6.Text), SafeStr(txtCheck7.Text), SafeStr(txtCheck8.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', ",
                SafeStr(txtCheck9.Text), SafeStr(txtCheck10.Text), SafeStr(txtCheck11.Text), SafeStr(txtCheck12.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', ",
                SafeStr(txtCheck13.Text), SafeStr(txtCheck14.Text), SafeStr(txtCheck15.Text), SafeStr(txtCheck16.Text));

            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                GetNum(txtOstoptime.Text), GetNum(txtTstoptime.Text), GetNum(txtOopentime.Text), GetNum(txtTopentime.Text),
                SafeStr(txtOpencheck.Text), GetNum(txtOplencheck.Text), GetNum(txtCllencheck.Text), GetNum(txtSpeedlen.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', ",
                SafeStr(txtOpostopcheck.Text), SafeStr(txtOptstopcheck.Text), SafeStr(txtClostopcheck.Text), SafeStr(txtCltstopcheck.Text),
                SafeStr(txtWopcheck.Text), SafeStr(txtWclcheck.Text), GetNum(txtOpchecklen.Text));
            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', {3}, {4}, '{5}', '{6}', {7}, '{8}', '{9}', ",
                SafeStr(txtInocheck.Text), SafeStr(txtOuocheck.Text), SafeStr(txtTocheck.Text),
                GetNum(txtLheight.Text), GetNum(txtRheight.Text), SafeStr(txtLtype.Text), SafeStr(txtRtype.Text),
                GetNum(txtPressure.Text), SafeStr(txtLput.Text), SafeStr(txtRput.Text));

            sqlInsert.AppendFormat("'{0}', '{1}', '{2}', 'Y', ' ')", currentDate, currentUser, SafeStr(txtStype.Text));

            bool success = UnixConn.ExecuteNonQuery(sqlInsert.ToString());

            if (!success)
            {
                NotificationHelper.Show(this, "LỖI THÊM LIỆU !!!", "error");
                return;
            }

            // Backup (INFORMIX)
            StringBuilder sqlBk = new StringBuilder();
            sqlBk.Append("INSERT INTO miskv:prdevvbk (func, intime, subno, factory, itnbr, version, machno, ");
            sqlBk.Append("machm1, machs1, machm2, machs2, machm3, machs3, machm4, machs4, machm5, machs5, ");
            sqlBk.Append("machm6, machs6, machm7, machs7, machm8, machs8, machm9, machs9, machm10, machs10, ");
            sqlBk.Append("machm11, machs11, machm12, machs12, machm13, machs13, machm14, machs14, machm15, machs15, machm16, machs16, ");
            sqlBk.Append("totalm, totals, check1, check2, check3, check4, check5, check6, check7, check8, ");
            sqlBk.Append("check9, check10, check11, check12, check13, check14, check15, check16, ");
            sqlBk.Append("ostoptime, tstoptime, oopentime, topentime, opencheck, oplencheck, cllencheck, speedlen, ");
            sqlBk.Append("opostopcheck, optstopcheck, clostopcheck, cltstopcheck, wopcheck, wclcheck, opchecklen, ");
            sqlBk.Append("inocheck, ouocheck, tocheck, lheight, rheight, ltype, rtype, pressure, lput, rput, ");
            sqlBk.Append("indat, usrno, stype, used, state) VALUES (");

            sqlBk.AppendFormat("'ADD', '{0}', ", currentTime);
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                SafeStr(txtSubno.Text), SafeStr(txtFactory.Text), SafeStr(txtItnbr.Text),
                SafeStr(txtVersion.Text), SafeStr(txtMachno.Text));

            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm1.Text), GetNum(txtMachs1.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm2.Text), GetNum(txtMachs2.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm3.Text), GetNum(txtMachs3.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm4.Text), GetNum(txtMachs4.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm5.Text), GetNum(txtMachs5.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm6.Text), GetNum(txtMachs6.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm7.Text), GetNum(txtMachs7.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm8.Text), GetNum(txtMachs8.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm9.Text), GetNum(txtMachs9.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm10.Text), GetNum(txtMachs10.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm11.Text), GetNum(txtMachs11.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm12.Text), GetNum(txtMachs12.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm13.Text), GetNum(txtMachs13.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm14.Text), GetNum(txtMachs14.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm15.Text), GetNum(txtMachs15.Text));
            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm16.Text), GetNum(txtMachs16.Text));

            sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtTotalm.Text), GetNum(txtTotals.Text));

            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck1.Text), SafeStr(txtCheck2.Text), SafeStr(txtCheck3.Text), SafeStr(txtCheck4.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck5.Text), SafeStr(txtCheck6.Text), SafeStr(txtCheck7.Text), SafeStr(txtCheck8.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck9.Text), SafeStr(txtCheck10.Text), SafeStr(txtCheck11.Text), SafeStr(txtCheck12.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck13.Text), SafeStr(txtCheck14.Text), SafeStr(txtCheck15.Text), SafeStr(txtCheck16.Text));

            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                GetNum(txtOstoptime.Text), GetNum(txtTstoptime.Text), GetNum(txtOopentime.Text), GetNum(txtTopentime.Text),
                SafeStr(txtOpencheck.Text), GetNum(txtOplencheck.Text), GetNum(txtCllencheck.Text), GetNum(txtSpeedlen.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', ",
                SafeStr(txtOpostopcheck.Text), SafeStr(txtOptstopcheck.Text), SafeStr(txtClostopcheck.Text), SafeStr(txtCltstopcheck.Text),
                SafeStr(txtWopcheck.Text), SafeStr(txtWclcheck.Text), GetNum(txtOpchecklen.Text));
            sqlBk.AppendFormat("'{0}', '{1}', '{2}', {3}, {4}, '{5}', '{6}', {7}, '{8}', '{9}', ",
                SafeStr(txtInocheck.Text), SafeStr(txtOuocheck.Text), SafeStr(txtTocheck.Text),
                GetNum(txtLheight.Text), GetNum(txtRheight.Text), SafeStr(txtLtype.Text), SafeStr(txtRtype.Text),
                GetNum(txtPressure.Text), SafeStr(txtLput.Text), SafeStr(txtRput.Text));

            sqlBk.AppendFormat("'{0}', '{1}', '{2}', 'Y', ' ')", currentDate, currentUser, SafeStr(txtStype.Text));

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

            try
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string currentTime = DateTime.Now.ToString("HHmmss");
                string currentUser = Session["username"]?.ToString() ?? "ADMIN";

                string oldVersion = ViewState["OldVersion"]?.ToString() ?? "";
                string newVersion = txtVersion.Text.Trim();

                if (string.IsNullOrEmpty(oldVersion))
                {
                    NotificationHelper.Show(this, "Không xác định được version cũ!", "error");
                    return;
                }

                CalculateTotal();

                // Disable old version (INFORMIX)
                UnixConn.ExecuteNonQuery($"UPDATE erp:prdevv SET state = '*' WHERE itnbr = '{SafeStr(txtItnbr.Text)}' AND version = '{SafeStr(oldVersion)}'");

                // Insert new (INFORMIX)
                StringBuilder sqlInsert = new StringBuilder();
                sqlInsert.Append("INSERT INTO erp:prdevv (");
                sqlInsert.Append("subno, factory, itnbr, version, machno, ");
                sqlInsert.Append("machm1, machs1, machm2, machs2, machm3, machs3, machm4, machs4, machm5, machs5, ");
                sqlInsert.Append("machm6, machs6, machm7, machs7, machm8, machs8, machm9, machs9, machm10, machs10, ");
                sqlInsert.Append("machm11, machs11, machm12, machs12, machm13, machs13, machm14, machs14, machm15, machs15, machm16, machs16, ");
                sqlInsert.Append("totalm, totals, check1, check2, check3, check4, check5, check6, check7, check8, ");
                sqlInsert.Append("check9, check10, check11, check12, check13, check14, check15, check16, ");
                sqlInsert.Append("ostoptime, tstoptime, oopentime, topentime, opencheck, oplencheck, cllencheck, speedlen, ");
                sqlInsert.Append("opostopcheck, optstopcheck, clostopcheck, cltstopcheck, wopcheck, wclcheck, opchecklen, ");
                sqlInsert.Append("inocheck, ouocheck, tocheck, lheight, rheight, ltype, rtype, pressure, lput, rput, ");
                sqlInsert.Append("indat, usrno, stype, used, state) VALUES (");

                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                    SafeStr(txtSubno.Text), SafeStr(txtFactory.Text), SafeStr(txtItnbr.Text),
                    SafeStr(newVersion), SafeStr(txtMachno.Text));

                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm1.Text), GetNum(txtMachs1.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm2.Text), GetNum(txtMachs2.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm3.Text), GetNum(txtMachs3.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm4.Text), GetNum(txtMachs4.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm5.Text), GetNum(txtMachs5.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm6.Text), GetNum(txtMachs6.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm7.Text), GetNum(txtMachs7.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm8.Text), GetNum(txtMachs8.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm9.Text), GetNum(txtMachs9.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm10.Text), GetNum(txtMachs10.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm11.Text), GetNum(txtMachs11.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm12.Text), GetNum(txtMachs12.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm13.Text), GetNum(txtMachs13.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm14.Text), GetNum(txtMachs14.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm15.Text), GetNum(txtMachs15.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm16.Text), GetNum(txtMachs16.Text));

                sqlInsert.AppendFormat("'{0}', '{1}', ", GetNum(txtTotalm.Text), GetNum(txtTotals.Text));

                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck1.Text), SafeStr(txtCheck2.Text), SafeStr(txtCheck3.Text), SafeStr(txtCheck4.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck5.Text), SafeStr(txtCheck6.Text), SafeStr(txtCheck7.Text), SafeStr(txtCheck8.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck9.Text), SafeStr(txtCheck10.Text), SafeStr(txtCheck11.Text), SafeStr(txtCheck12.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck13.Text), SafeStr(txtCheck14.Text), SafeStr(txtCheck15.Text), SafeStr(txtCheck16.Text));

                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                    GetNum(txtOstoptime.Text), GetNum(txtTstoptime.Text), GetNum(txtOopentime.Text), GetNum(txtTopentime.Text),
                    SafeStr(txtOpencheck.Text), GetNum(txtOplencheck.Text), GetNum(txtCllencheck.Text), GetNum(txtSpeedlen.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', ",
                    SafeStr(txtOpostopcheck.Text), SafeStr(txtOptstopcheck.Text), SafeStr(txtClostopcheck.Text), SafeStr(txtCltstopcheck.Text),
                    SafeStr(txtWopcheck.Text), SafeStr(txtWclcheck.Text), GetNum(txtOpchecklen.Text));
                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', {3}, {4}, '{5}', '{6}', {7}, '{8}', '{9}', ",
                    SafeStr(txtInocheck.Text), SafeStr(txtOuocheck.Text), SafeStr(txtTocheck.Text),
                    GetNum(txtLheight.Text), GetNum(txtRheight.Text), SafeStr(txtLtype.Text), SafeStr(txtRtype.Text),
                    GetNum(txtPressure.Text), SafeStr(txtLput.Text), SafeStr(txtRput.Text));

                sqlInsert.AppendFormat("'{0}', '{1}', '{2}', 'Y', ' ')", currentDate, currentUser, SafeStr(txtStype.Text));

                bool success = UnixConn.ExecuteNonQuery(sqlInsert.ToString());

                if (!success)
                {
                    NotificationHelper.Show(this, "LỖI CẬP NHẬT !!!", "error");
                    return;
                }

                // Backup
                StringBuilder sqlBk = new StringBuilder();
                sqlBk.Append("INSERT INTO miskv:prdevvbk (func, intime, subno, factory, itnbr, version, machno, ");
                sqlBk.Append("machm1, machs1, machm2, machs2, machm3, machs3, machm4, machs4, machm5, machs5, ");
                sqlBk.Append("machm6, machs6, machm7, machs7, machm8, machs8, machm9, machs9, machm10, machs10, ");
                sqlBk.Append("machm11, machs11, machm12, machs12, machm13, machs13, machm14, machs14, machm15, machs15, machm16, machs16, ");
                sqlBk.Append("totalm, totals, check1, check2, check3, check4, check5, check6, check7, check8, ");
                sqlBk.Append("check9, check10, check11, check12, check13, check14, check15, check16, ");
                sqlBk.Append("ostoptime, tstoptime, oopentime, topentime, opencheck, oplencheck, cllencheck, speedlen, ");
                sqlBk.Append("opostopcheck, optstopcheck, clostopcheck, cltstopcheck, wopcheck, wclcheck, opchecklen, ");
                sqlBk.Append("inocheck, ouocheck, tocheck, lheight, rheight, ltype, rtype, pressure, lput, rput, ");
                sqlBk.Append("indat, usrno, stype, used, state) VALUES (");
                sqlBk.AppendFormat("'MOD', '{0}', ", currentTime);
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",
                    SafeStr(txtSubno.Text), SafeStr(txtFactory.Text), SafeStr(txtItnbr.Text),
                    SafeStr(newVersion), SafeStr(txtMachno.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm1.Text), GetNum(txtMachs1.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm2.Text), GetNum(txtMachs2.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm3.Text), GetNum(txtMachs3.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm4.Text), GetNum(txtMachs4.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm5.Text), GetNum(txtMachs5.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm6.Text), GetNum(txtMachs6.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm7.Text), GetNum(txtMachs7.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm8.Text), GetNum(txtMachs8.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm9.Text), GetNum(txtMachs9.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm10.Text), GetNum(txtMachs10.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm11.Text), GetNum(txtMachs11.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm12.Text), GetNum(txtMachs12.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm13.Text), GetNum(txtMachs13.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm14.Text), GetNum(txtMachs14.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm15.Text), GetNum(txtMachs15.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtMachm16.Text), GetNum(txtMachs16.Text));
                sqlBk.AppendFormat("'{0}', '{1}', ", GetNum(txtTotalm.Text), GetNum(txtTotals.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck1.Text), SafeStr(txtCheck2.Text), SafeStr(txtCheck3.Text), SafeStr(txtCheck4.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck5.Text), SafeStr(txtCheck6.Text), SafeStr(txtCheck7.Text), SafeStr(txtCheck8.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck9.Text), SafeStr(txtCheck10.Text), SafeStr(txtCheck11.Text), SafeStr(txtCheck12.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', ", SafeStr(txtCheck13.Text), SafeStr(txtCheck14.Text), SafeStr(txtCheck15.Text), SafeStr(txtCheck16.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', ",
                    GetNum(txtOstoptime.Text), GetNum(txtTstoptime.Text), GetNum(txtOopentime.Text), GetNum(txtTopentime.Text),
                    SafeStr(txtOpencheck.Text), GetNum(txtOplencheck.Text), GetNum(txtCllencheck.Text), GetNum(txtSpeedlen.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', ",
                    SafeStr(txtOpostopcheck.Text), SafeStr(txtOptstopcheck.Text), SafeStr(txtClostopcheck.Text), SafeStr(txtCltstopcheck.Text),
                    SafeStr(txtWopcheck.Text), SafeStr(txtWclcheck.Text), GetNum(txtOpchecklen.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', ",
                    SafeStr(txtInocheck.Text), SafeStr(txtOuocheck.Text), SafeStr(txtTocheck.Text),
                    GetNum(txtLheight.Text), GetNum(txtRheight.Text), SafeStr(txtLtype.Text), SafeStr(txtRtype.Text),
                    GetNum(txtPressure.Text), SafeStr(txtLput.Text), SafeStr(txtRput.Text));
                sqlBk.AppendFormat("'{0}', '{1}', '{2}', 'Y', ' ')", currentDate, currentUser, SafeStr(txtStype.Text));

                try
                {
                    UnixConn.ExecuteNonQuery(sqlBk.ToString());
                }
                catch (Exception exBk)
                {
                    Console.WriteLine("Lỗi Backup: " + exBk.Message);
                }

                // Update prdepv table (INFORMIX)
                int cc = Convert.ToInt32(UnixConn.ExecuteQuery($"SELECT COUNT(*) FROM erp:prdepv WHERE itnbr = '{SafeStr(txtItnbr.Text)}' AND (state = '' OR state = ' ' OR state IS NULL)").Rows[0][0]);

                if (cc > 0)
                {
                    UnixConn.ExecuteNonQuery($@"
                        UPDATE erp:prdepv 
                        SET pressure = {GetNum(txtPressure.Text)},
                            lheight = {GetNum(txtLheight.Text)},
                            rheight = {GetNum(txtRheight.Text)},
                            ltype = '{SafeStr(txtLtype.Text)}',
                            rtype = '{SafeStr(txtRtype.Text)}',
                            usrno = '{currentUser}',
                            indat = '{currentDate}'
                        WHERE itnbr = '{SafeStr(txtItnbr.Text)}' 
                        AND (state = '' OR state = ' ' OR state IS NULL)");
                }

                NotificationHelper.Show(this, "MODIFY SUCCESS !!!", "success");

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

        // --- TEXTBOX EVENTS ---
        protected void txtItnbr_TextChanged(object sender, EventArgs e)
        {
            if (CurrentMode == ActionMode.ADD || CurrentMode == ActionMode.MODIFY)
            {
                // INVMAS/INVMAT truy vấn qua SQL Server
                int countinv = Convert.ToInt32(UnixConn.ExecuteQuery(
                    $"SELECT COUNT(*) FROM erp:invmas a, erp:invmat b WHERE a.itnbr = '{SafeStr(txtItnbr.Text)}' AND a.itnbr = b.itnbr " +
                    $"AND b.modsta <> 'N'  AND  b.factory = 'E'").Rows[0][0]);
                if (countinv > 0 && CurrentMode == ActionMode.ADD)
                {
                    NotificationHelper.Show(this, "KO CO MA SO NAY!", "warning");
                    txtItnbr.Focus();
                    txtItnbr.Text = "";
                    return;
                }

                INVMAS();

                // PRDEVV đếm trên Informix
                int count = Convert.ToInt32(UnixConn.ExecuteQuery(
                    $"SELECT COUNT(*) FROM erp:prdevv WHERE subno = '4' AND itnbr = '{SafeStr(txtItnbr.Text)}' AND (state = '' OR state = ' ' OR state IS NULL)").Rows[0][0]);

                if (count > 0 && CurrentMode == ActionMode.ADD)
                {
                    NotificationHelper.Show(this, "ITNBR đã tồn tại!", "warning");
                    txtItnbr.Focus();
                    txtItnbr.Text = "";
                    return;
                }

                CalculateVersion();
                CalculateMachNo();
            }
        }

        private void INVMAS()
        {
            DataTable invmas = SQLConn34.ExecuteQuery($"SELECT itdsc, spdsc FROM [erp].[dbo].[invmas] WHERE itnbr = '{SafeStr(txtItnbr.Text)}'");
            if (invmas != null && invmas.Rows.Count > 0)
            {
                txtItdsc.Text = invmas.Rows[0][0].ToString().Trim();
                txtSpdsc.Text = invmas.Rows[0][1].ToString().Trim();
            }
        }

        // --- HELPER METHODS ---
        private void CalculateVersion()
        {
            string newVersion = "01";

            // INFORMIX
            DataTable dtMaxVer = UnixConn.ExecuteQuery(
                $"SELECT MAX(version) FROM erp:prdevv WHERE subno = '4' AND itnbr = '{SafeStr(txtItnbr.Text)}'");

            if (dtMaxVer != null && dtMaxVer.Rows.Count > 0)
            {
                string maxVer = dtMaxVer.Rows[0][0].ToString().Trim();

                if (!string.IsNullOrEmpty(maxVer) && int.TryParse(maxVer, out int verNum))
                {
                    newVersion = (verNum + 1).ToString("D2");
                }
            }

            txtVersion.Text = newVersion;
        }

        private void CalculateMachNo()
        {
            string machnoValue = "";
            string itnbrValue = SafeStr(txtItnbr.Text);

            try
            {
                // INFORMIX
                string sqlGetMoldStyle = $@"
                                            SELECT FIRST 1 mold_style2 
                                            FROM erp:pmspec2 
                                            WHERE bdep = 'V' AND itnbr = '{itnbrValue}' 
                                            ORDER BY rowid DESC";

                DataTable dtMoldStyle = UnixConn.ExecuteQuery(sqlGetMoldStyle);

                if (dtMoldStyle != null && dtMoldStyle.Rows.Count > 0)
                {
                    string moldStyle2 = dtMoldStyle.Rows[0]["mold_style2"].ToString().Trim();

                    if (moldStyle2 == "一體式")
                    {
                        machnoValue = "51";
                    }
                    else if (moldStyle2.Length >= 4)
                    {
                        string substring = moldStyle2.Substring(2, 2);

                        if (substring == "57")
                        {
                            machnoValue = "65";
                        }
                        else
                        {
                            if (int.TryParse(substring, out int substringValue))
                            {
                                if (substringValue >= 0 && substringValue < 57)
                                {
                                    machnoValue = "51";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.Show(this, "LAY TEN MAY BI LOI: " + ex.Message, "error");
            }

            txtMachno.Text = machnoValue;
        }

        private void CalculateTotal()
        {
            int totalMinutes = 0;
            int totalSeconds = 0;

            for (int i = 1; i <= 16; i++)
            {
                TextBox txtM = RecursiveFindControl(pnlInput, $"txtMachm{i}") as TextBox;
                TextBox txtS = RecursiveFindControl(pnlInput, $"txtMachs{i}") as TextBox;

                if (txtM != null && int.TryParse(txtM.Text.Trim(), out int m))
                    totalMinutes += m;

                if (txtS != null && int.TryParse(txtS.Text.Trim(), out int s))
                    totalSeconds += s;
            }

            totalMinutes += totalSeconds / 60;
            totalSeconds = totalSeconds % 60;

            txtTotalm.Text = totalMinutes.ToString("D2");
            txtTotals.Text = totalSeconds.ToString("D2");
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
                // SQL SERVER (YÊU CẦU CỦA BẠN: GIỮ NGUYÊN HÀM NÀY)
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT subno, factory, itnbr, version, machno, ");
                sql.Append("machm1, machs1, machm2, machs2, machm3, machs3, machm4, machs4, machm5, machs5, ");
                sql.Append("machm6, machs6, machm7, machs7, machm8, machs8, machm9, machs9, machm10, machs10, ");
                sql.Append("machm11, machs11, machm12, machs12, machm13, machs13, machm14, machs14, machm15, machs15, machm16, machs16, ");
                sql.Append("totalm, totals, check1, check2, check3, check4, check5, check6, check7, check8, ");
                sql.Append("check9, check10, check11, check12, check13, check14, check15, check16, ");
                sql.Append("ostoptime, tstoptime, oopentime, topentime, opencheck, oplencheck, cllencheck, speedlen, ");
                sql.Append("opostopcheck, optstopcheck, clostopcheck, cltstopcheck, wopcheck, wclcheck, opchecklen, ");
                sql.Append("inocheck, ouocheck, tocheck, lheight, rheight, ltype, rtype, pressure, lput, rput, ");
                sql.Append("stype, indat, usrno, used, state ");
                sql.Append("FROM [erp].[dbo].[prdevv] ");
                sql.Append("WHERE subno = '4' AND (state = '' OR state = ' ' OR state IS NULL) AND itnbr <> '' ");
                sql.Append("ORDER BY itnbr, version");

                DataTable dtAll = SQLConn34.ExecuteQuery(sql.ToString());

                if (dtAll == null || dtAll.Rows.Count == 0)
                {
                    NotificationHelper.Show(this, "Không có dữ liệu để xuất!", "warning");
                    return;
                }

                string fileName = $"CPRDV01_AllData_{DateTime.Now:yyyyMMddHHmmss}";
                ExcelHelper.ExportToExcel(dtAll, fileName, "CPRDV01_Data");

                NotificationHelper.Show(this, $"Đã xuất {dtAll.Rows.Count} dòng dữ liệu!", "success");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất Excel All: " + ex.Message);
            }
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
            btnOK = (LinkButton)RecursiveFindControl(this, "btnOK");
            btnCancel = (LinkButton)RecursiveFindControl(this, "btnCancel");

            btnFetchFirst = (LinkButton)RecursiveFindControl(this, "btnFetchFirst");
            btnFetchPrevious = (LinkButton)RecursiveFindControl(this, "btnFetchPrevious");
            btnFetchNext = (LinkButton)RecursiveFindControl(this, "btnFetchNext");
            btnFetchLast = (LinkButton)RecursiveFindControl(this, "btnFetchLast");

            txtSubno = (TextBox)RecursiveFindControl(this, "txtSubno");
            txtFactory = (TextBox)RecursiveFindControl(this, "txtFactory");
            txtItnbr = (TextBox)RecursiveFindControl(this, "txtItnbr");
            txtVersion = (TextBox)RecursiveFindControl(this, "txtVersion");
            txtMachno = (TextBox)RecursiveFindControl(this, "txtMachno");
            txtStype = (TextBox)RecursiveFindControl(this, "txtStype");

            txtMachm1 = (TextBox)RecursiveFindControl(this, "txtMachm1");
            txtMachs1 = (TextBox)RecursiveFindControl(this, "txtMachs1");
            txtMachm2 = (TextBox)RecursiveFindControl(this, "txtMachm2");
            txtMachs2 = (TextBox)RecursiveFindControl(this, "txtMachs2");
            txtMachm3 = (TextBox)RecursiveFindControl(this, "txtMachm3");
            txtMachs3 = (TextBox)RecursiveFindControl(this, "txtMachs3");
            txtMachm4 = (TextBox)RecursiveFindControl(this, "txtMachm4");
            txtMachs4 = (TextBox)RecursiveFindControl(this, "txtMachs4");
            txtMachm5 = (TextBox)RecursiveFindControl(this, "txtMachm5");
            txtMachs5 = (TextBox)RecursiveFindControl(this, "txtMachs5");
            txtMachm6 = (TextBox)RecursiveFindControl(this, "txtMachm6");
            txtMachs6 = (TextBox)RecursiveFindControl(this, "txtMachs6");
            txtMachm7 = (TextBox)RecursiveFindControl(this, "txtMachm7");
            txtMachs7 = (TextBox)RecursiveFindControl(this, "txtMachs7");
            txtMachm8 = (TextBox)RecursiveFindControl(this, "txtMachm8");
            txtMachs8 = (TextBox)RecursiveFindControl(this, "txtMachs8");
            txtMachm9 = (TextBox)RecursiveFindControl(this, "txtMachm9");
            txtMachs9 = (TextBox)RecursiveFindControl(this, "txtMachs9");
            txtMachm10 = (TextBox)RecursiveFindControl(this, "txtMachm10");
            txtMachs10 = (TextBox)RecursiveFindControl(this, "txtMachs10");
            txtMachm11 = (TextBox)RecursiveFindControl(this, "txtMachm11");
            txtMachs11 = (TextBox)RecursiveFindControl(this, "txtMachs11");
            txtMachm12 = (TextBox)RecursiveFindControl(this, "txtMachm12");
            txtMachs12 = (TextBox)RecursiveFindControl(this, "txtMachs12");
            txtMachm13 = (TextBox)RecursiveFindControl(this, "txtMachm13");
            txtMachs13 = (TextBox)RecursiveFindControl(this, "txtMachs13");
            txtMachm14 = (TextBox)RecursiveFindControl(this, "txtMachm14");
            txtMachs14 = (TextBox)RecursiveFindControl(this, "txtMachs14");
            txtMachm15 = (TextBox)RecursiveFindControl(this, "txtMachm15");
            txtMachs15 = (TextBox)RecursiveFindControl(this, "txtMachs15");
            txtMachm16 = (TextBox)RecursiveFindControl(this, "txtMachm16");
            txtMachs16 = (TextBox)RecursiveFindControl(this, "txtMachs16");

            txtTotalm = (TextBox)RecursiveFindControl(this, "txtTotalm");
            txtTotals = (TextBox)RecursiveFindControl(this, "txtTotals");

            txtCheck1 = (TextBox)RecursiveFindControl(this, "txtCheck1");
            txtCheck2 = (TextBox)RecursiveFindControl(this, "txtCheck2");
            txtCheck3 = (TextBox)RecursiveFindControl(this, "txtCheck3");
            txtCheck4 = (TextBox)RecursiveFindControl(this, "txtCheck4");
            txtCheck5 = (TextBox)RecursiveFindControl(this, "txtCheck5");
            txtCheck6 = (TextBox)RecursiveFindControl(this, "txtCheck6");
            txtCheck7 = (TextBox)RecursiveFindControl(this, "txtCheck7");
            txtCheck8 = (TextBox)RecursiveFindControl(this, "txtCheck8");
            txtCheck9 = (TextBox)RecursiveFindControl(this, "txtCheck9");
            txtCheck10 = (TextBox)RecursiveFindControl(this, "txtCheck10");
            txtCheck11 = (TextBox)RecursiveFindControl(this, "txtCheck11");
            txtCheck12 = (TextBox)RecursiveFindControl(this, "txtCheck12");
            txtCheck13 = (TextBox)RecursiveFindControl(this, "txtCheck13");
            txtCheck14 = (TextBox)RecursiveFindControl(this, "txtCheck14");
            txtCheck15 = (TextBox)RecursiveFindControl(this, "txtCheck15");
            txtCheck16 = (TextBox)RecursiveFindControl(this, "txtCheck16");

            txtOstoptime = (TextBox)RecursiveFindControl(this, "txtOstoptime");
            txtTstoptime = (TextBox)RecursiveFindControl(this, "txtTstoptime");
            txtOopentime = (TextBox)RecursiveFindControl(this, "txtOopentime");
            txtTopentime = (TextBox)RecursiveFindControl(this, "txtTopentime");
            txtOpencheck = (TextBox)RecursiveFindControl(this, "txtOpencheck");
            txtOplencheck = (TextBox)RecursiveFindControl(this, "txtOplencheck");
            txtCllencheck = (TextBox)RecursiveFindControl(this, "txtCllencheck");
            txtSpeedlen = (TextBox)RecursiveFindControl(this, "txtSpeedlen");
            txtOpostopcheck = (TextBox)RecursiveFindControl(this, "txtOpostopcheck");
            txtOptstopcheck = (TextBox)RecursiveFindControl(this, "txtOptstopcheck");
            txtClostopcheck = (TextBox)RecursiveFindControl(this, "txtClostopcheck");
            txtCltstopcheck = (TextBox)RecursiveFindControl(this, "txtCltstopcheck");
            txtWopcheck = (TextBox)RecursiveFindControl(this, "txtWopcheck");
            txtWclcheck = (TextBox)RecursiveFindControl(this, "txtWclcheck");
            txtOpchecklen = (TextBox)RecursiveFindControl(this, "txtOpchecklen");
            txtInocheck = (TextBox)RecursiveFindControl(this, "txtInocheck");
            txtOuocheck = (TextBox)RecursiveFindControl(this, "txtOuocheck");
            txtTocheck = (TextBox)RecursiveFindControl(this, "txtTocheck");

            txtLheight = (TextBox)RecursiveFindControl(this, "txtLheight");
            txtRheight = (TextBox)RecursiveFindControl(this, "txtRheight");
            txtLtype = (TextBox)RecursiveFindControl(this, "txtLtype");
            txtRtype = (TextBox)RecursiveFindControl(this, "txtRtype");
            txtPressure = (TextBox)RecursiveFindControl(this, "txtPressure");
            txtLput = (TextBox)RecursiveFindControl(this, "txtLput");
            txtRput = (TextBox)RecursiveFindControl(this, "txtRput");

            txtItdsc = (TextBox)RecursiveFindControl(this, "txtItdsc");
            txtSpdsc = (TextBox)RecursiveFindControl(this, "txtSpdsc");

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