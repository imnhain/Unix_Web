using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using Unix_Web.Helpers;
using Unix_Web.Providers;

namespace Unix_Web.Source.PRD
{
    public partial class MonthLock : System.Web.UI.Page
    {
        string query = "";
        string update = "";
        //test chức năng restore 1 2 3

        // ==========================================
        // KHU VỰC THUỘC TÍNH HỖ TRỢ SẮP XẾP (SORTING)
        // ==========================================
        private string GridViewSortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        private string GridViewSortExpression
        {
            get { return ViewState["SortExpression"] as string ?? string.Empty; }
            set { ViewState["SortExpression"] = value; }
        }

        // ==========================================
        // CÁC HÀM XỬ LÝ SỰ KIỆN CHÍNH
        // ==========================================
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra bảo mật: Chỉ cho phép nhanit truy cập
            if (Session["username"] == null || Session["username"].ToString().ToLower() != "nhanit")
            {
                Response.Redirect("~/Shared/Main.aspx");
                return;
            }

            if (!IsPostBack)
            {
                txtlmonth.Text = Variable.month_chuakhoa();
            }
        }

        // Hàm hiển thị thông báo SweetAlert2
        private void ShowAlert(string msg, string type)
        {
            string script = $"Swal.fire('Thông báo', '{msg}', '{type}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

        // ==========================================
        // XỬ LÝ TÌM KIẾM VÀ LOAD DỮ LIỆU
        // ==========================================
        protected void btnSelect_Click(object sender, EventArgs e)
        {
            string where = " ";

            // Xử lý nhập nhiều DEPNO cách nhau bằng dấu phẩy
            if (!string.IsNullOrWhiteSpace(txtDepno.Text))
            {
                string[] deps = txtDepno.Text.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string depInClause = string.Join(",", deps.Select(d => $"'{d.Trim().ToUpper()}'"));
                where += $" and depno IN ({depInClause}) ";
            }

            // Xử lý nhập nhiều PRGNO cách nhau bằng dấu phẩy
            if (!string.IsNullOrWhiteSpace(txtPrgno.Text))
            {
                string[] prgs = txtPrgno.Text.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string prgInClause = string.Join(",", prgs.Select(p => $"'{p.Trim().ToUpper()}'"));
                where += $" and prgno IN ({prgInClause}) ";
            }

            query = "SELECT prgno,depno,lmonth,indat,intime,usrno FROM erp:prdsysn " +
                    "WHERE subno = '4' and prgno[1,3] <> 'MAS' " + where + " ORDER BY depno, prgno";

            DataTable dt = UnixConn.ExecuteQuery(query);

            // Lưu dữ liệu vào ViewState để dùng cho Sắp xếp
            ViewState["CurrentTable"] = dt;

            dataGridView1.DataSource = dt;
            dataGridView1.DataBind();
        }

        protected void btnSelect_All_Click(object sender, EventArgs e)
        {
            query = "SELECT prgno,depno,lmonth,indat,intime,usrno FROM erp:prdsysn " +
                    "WHERE subno = '4' and prgno[1,3] <> 'MAS' " +
                    "and prgno NOT IN ('PRDA11','PRDA11A','PRDA17','PRD932','PRDA03','PRDA29','PRD915','PRD715') " +
                    "and depno NOT IN ('Z0002','Z0003','Z0004') order by depno ";

            DataTable dt = UnixConn.ExecuteQuery(query);

            // Lưu dữ liệu vào ViewState để dùng cho Sắp xếp
            ViewState["CurrentTable"] = dt;

            dataGridView1.DataSource = dt;
            dataGridView1.DataBind();
        }

        // ==========================================
        // XỬ LÝ SẮP XẾP (SORTING) TRÊN LƯỚI
        // ==========================================
        protected void dataGridView1_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression;

            // Đảo chiều nếu click lại cột cũ, ngược lại mặc định là ASC
            if (GridViewSortExpression == sortExpression)
            {
                GridViewSortDirection = (GridViewSortDirection == "ASC") ? "DESC" : "ASC";
            }
            else
            {
                GridViewSortExpression = sortExpression;
                GridViewSortDirection = "ASC";
            }

            // Lấy dữ liệu từ bộ nhớ tạm ra để sắp xếp
            DataTable dt = ViewState["CurrentTable"] as DataTable;
            if (dt != null)
            {
                DataView dv = new DataView(dt);
                dv.Sort = GridViewSortExpression + " " + GridViewSortDirection;
                dataGridView1.DataSource = dv;
                dataGridView1.DataBind();
            }
        }

        // ==========================================
        // XỬ LÝ KHÓA / MỞ KHÓA
        // ==========================================
        protected void btnKhoa_Click(object sender, EventArgs e)
        {
            ProcessLockUnlock(true); // Gửi cờ true = Khóa
        }

        protected void btnMoKhoa_Click(object sender, EventArgs e)
        {
            ProcessLockUnlock(false); // Gửi cờ false = Mở khóa
        }

        private void ProcessLockUnlock(bool isLock)
        {
            string targetMonth = isLock ? Variable.month_khoa() : Variable.month_chuakhoa();
            int count = 0;
            StringBuilder sbUpdate = new StringBuilder();

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            string currentUser = Session["username"]?.ToString() ?? "ADMIN";

            foreach (System.Web.UI.WebControls.GridViewRow row in dataGridView1.Rows)
            {
                if (row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
                {
                    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)row.FindControl("chkSelect");

                    if (chk != null && chk.Checked)
                    {
                        string prgno = dataGridView1.DataKeys[row.RowIndex].Values["prgno"].ToString().Trim();
                        string depno = dataGridView1.DataKeys[row.RowIndex].Values["depno"].ToString().Trim();

                        sbUpdate.Append($"UPDATE erp:prdsysn set lmonth = '{targetMonth}' " +
                                        $"WHERE subno = '4' and depno = '{depno}' and prgno = '{prgno}'; ");
                        count++;
                    }
                }
            }

            if (count == 0)
            {
                ShowAlert("Vui lòng tick chọn ít nhất 1 dòng trong bảng để thực hiện!", "warning");
                return;
            }

            bool isSuccess = UnixConn.ExecuteNonQuery(sbUpdate.ToString());
            if (!isSuccess)
            {
                ShowAlert("Lỗi cập nhật dữ liệu!", "error");
            }
            else
            {
                string msg = isLock ? "Khóa" : "Mở khóa";
                ShowAlert($"{msg} thành công {count} dòng!", "success");
                btnSelect_Click(null, null);
            }
        }

        // ==========================================
        // XỬ LÝ UPDATE BATCH (PRDA01 & PRDA11)
        // ==========================================
        protected void btnUpPRDA01_Click(object sender, EventArgs e)
        {
            ExecuteBatchUpdate("PRDA01");
        }

        protected void btnUpPRDA11_Click(object sender, EventArgs e)
        {
            ExecuteBatchUpdate("PRDA11");
        }

        private void ExecuteBatchUpdate(string prgnoTarget)
        {
            string selectQuery = $"select prgno,depno,lmonth,indat,intime,usrno from erp:prdsysn where subno = 4 and lmonth = '{Variable.month_chuakhoa()}' and prgno = '{prgnoTarget}'";
            DataTable dt = UnixConn.ExecuteQuery(selectQuery);

            ViewState["CurrentTable"] = dt;
            dataGridView1.DataSource = dt;
            dataGridView1.DataBind();

            if (dt.Rows.Count == 0)
            {
                ShowAlert($"Không có dữ liệu {prgnoTarget} để update!", "info");
                return;
            }

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            string targetMonth = Variable.month_khoa();

            int successCount = 0;
            int errorCount = 0;

            foreach (DataRow row in dt.Rows)
            {
                string prgno = row["prgno"].ToString().Trim();
                string depno = row["depno"].ToString().Trim();
                string lmonth = row["lmonth"].ToString().Trim();
                string usrno = GetUserByDepno(depno);

                string updateSql = $"UPDATE erp:prdsysn SET lmonth = '{targetMonth}', usrno = '{usrno}', indat = '{currentDate}', intime = '{currentTime}' " +
                                   $"WHERE subno = '4' AND lmonth = '{lmonth}' AND depno = '{depno}' AND prgno = '{prgno}'";

                bool isSuccess = UnixConn.ExecuteNonQuery(updateSql);

                if (isSuccess)
                {
                    successCount++;
                }
                else
                {
                    errorCount++;
                }
            }

            if (errorCount > 0)
            {
                ShowAlert($"Cập nhật xong: {successCount} thành công, {errorCount} dòng thất bại!", "warning");
            }
            else
            {
                ShowAlert($"Update {prgnoTarget} thành công toàn bộ {successCount} dòng!", "success");

                query = $"SELECT prgno,depno,lmonth,indat,intime,usrno FROM erp:prdsysn WHERE subno = '4' and prgno[1,3] <> 'MAS' and prgno = '{prgnoTarget}'";
                DataTable newDt = UnixConn.ExecuteQuery(query);

                ViewState["CurrentTable"] = newDt;
                dataGridView1.DataSource = newDt;
                dataGridView1.DataBind();
            }
        }

        private string GetUserByDepno(string depno)
        {
            switch (depno)
            {
                case "P1300": case "P1320": case "Z0001": return "阮氏香";
                case "P1410": case "P1420": case "P1421": return "阮氏春鳳";
                case "P1500": case "P1600": return "陳氏香";
                case "P1700": return "Thanh";
                case "P1800": return "Anh";
                case "P1910": case "P1920": case "P1930": case "P1940": return "Phuong";
                case "P3100": return "黎光忠";
                case "P3410": case "P3420": case "P3430": return "黎氏清水";
                case "P5200": case "P5230": return "Mai";
                case "P5310": case "P5320": case "P5330": return "Tu";
                case "P5400": case "P5500": return "Thuy";
                case "P5720": case "P5730": return "Van";
                case "P8200": case "P8500": return "黎氏容";
                case "P8300": case "P8310": case "P8320": case "P8330": return "Hang";
                case "P8400": return "范氏選";
                case "P8600": return "Hoa";
                case "P8830": return "Thu Tran";
                case "P9100": return "Minh";
                default: return "";
            }
        }

        protected void btn_maspakh_Click(object sender, EventArgs e)
        {
            string datenow01 = DateTime.Now.ToString("yyyyMM") + "01";
            string insert = $"insert into erp:maspakh values('4','{Variable.month_khoa()}','N','{datenow01}','','nhanit','N','{datenow01}','','nhanit','N','{datenow01}','','nhanit','N','{datenow01}','','nhanit','N','{datenow01}','','nhanit')";
            bool in_mas = UnixConn.ExecuteNonQuery(insert);

            update = $"update erp:maspakh set packdp = 'Y',packag = 'Y',packot = 'Y',packhf = 'Y',packbb = 'Y' where yymm = '{Variable.month_chuakhoa()}' and subno = '4'";
            bool up_mas = UnixConn.ExecuteNonQuery(update);

            if (!up_mas || !in_mas)
                ShowAlert("Lỗi cập nhật MASPAKH!", "error");
            else
                ShowAlert("Cập nhật MASPAKH OK!", "success");
        }

        protected void btnPRI_QC_Click(object sender, EventArgs e)
        {
            string selectedDepno = ddlPRI_QC.SelectedValue;

            query = $"SELECT * FROM erp:prdpri WHERE subno = 4 AND depno = '{selectedDepno}' AND slipdate[1,6] = '{Variable.month_khoa()}'";
            DataTable dtqc_pri = UnixConn.ExecuteQuery(query);

            if (dtqc_pri.Rows.Count > 0)
            {
                update = $"UPDATE erp:prdpri set warehouse = '1' WHERE subno = 4 AND depno = '{selectedDepno}' AND slipdate[1,6] = '{Variable.month_khoa()}'";
                bool up_qc_pri = UnixConn.ExecuteNonQuery(update);

                if (!up_qc_pri)
                    ShowAlert("Lỗi cập nhật QC PRI!", "error");
                else
                    ShowAlert("Cập nhật QC PRI thành công!", "success");
            }
            else
            {
                ShowAlert("Không tìm thấy dữ liệu QC để khóa!", "info");
            }
        }

        protected void btnPRD923_Click(object sender, EventArgs e)
        {
            // Lấy thông tin thời gian và người dùng hiện tại
            string currentYYMM = DateTime.Now.ToString("yyyyMM");
            string prevYYMM = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            string currentUser = Session["username"]?.ToString() ?? "ADMIN";

            char[] deptyList = { 'M', 'P', 'B', 'Z' };
            StringBuilder finalMsg = new StringBuilder("<b>📊 TỔNG KẾT CHUYỂN DỮ LIỆU NHÂN SỰ:</b><br/><hr/>");
            bool hasError = false;
            int totalInserted = 0;

            foreach (char depty in deptyList)
            {
                try
                {
                    // 1. Xóa dữ liệu cũ của bộ phận trong tháng hiện tại để tránh trùng lặp[cite: 1]
                    string deleteSql = $"DELETE FROM miskv:prdper WHERE yymm = '{currentYYMM}' AND depno[1] = '{depty}'";
                    UnixConn.ExecuteNonQuery(deleteSql);

                    // 2. Xây dựng câu lệnh Insert Bulk (xử lý hàng ngàn dòng trong 1 lệnh)
                    string insertSql = "";
                    if (depty == 'P')
                    {
                        // Bộ phận P lấy từ bảng prdper của tháng trước[cite: 1]
                        insertSql = $@"INSERT INTO miskv:prdper (yymm, depno, empno, cod, indat, intime, usrno)
                               SELECT '{currentYYMM}', depno, empno, 'N', '{currentDate}', '{currentTime}', '{currentUser}'
                               FROM miskv:prdper 
                               WHERE yymm = '{prevYYMM}' AND depno[1] = 'P'";
                    }
                    else
                    {
                        // Các bộ phận khác lấy từ danh mục nhân viên erp:peremp[cite: 1]
                        insertSql = $@"INSERT INTO miskv:prdper (yymm, depno, empno, cod, indat, intime, usrno)
                               SELECT '{currentYYMM}', depno, empno, 'N', '{currentDate}', '{currentTime}', '{currentUser}'
                               FROM erp:peremp
                               WHERE othcod1 NOT MATCHES '[23]' 
                                 AND levdat = '' 
                                 AND subno = '4' 
                                 AND depno[1] = '{depty}'";
                    }

                    // 3. Thực thi lệnh chèn dữ liệu
                    bool success = UnixConn.ExecuteNonQuery(insertSql);

                    if (success)
                    {
                        // 4. Đếm số dòng thực tế đã chèn để báo cáo
                        string countQuery = $"SELECT COUNT(*) FROM miskv:prdper WHERE yymm = '{currentYYMM}' AND depno[1] = '{depty}'";
                        DataTable dt = UnixConn.ExecuteQuery(countQuery);
                        int count = (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : 0;

                        totalInserted += count;
                        finalMsg.Append($"✅ <b>Bộ phận {depty}:</b> đã thêm {count} dòng.<br/>");
                    }
                    else
                    {
                        finalMsg.Append($"❌ <b>Bộ phận {depty}:</b> thực thi thất bại.<br/>");
                        hasError = true;
                    }
                }
                catch (Exception ex)
                {
                    finalMsg.Append($"⚠️ <b>Bộ phận {depty}:</b> lỗi hệ thống ({ex.Message}).<br/>");
                    hasError = true;
                }
            }

            // 5. Hiển thị thông báo tổng kết một lần duy nhất
            finalMsg.Append("<hr/>");
            finalMsg.Append($"<b>Tổng cộng: {totalInserted} nhân viên đã được chuyển.</b>");

            ShowAlert(finalMsg.ToString(), hasError ? "warning" : "success");
        }

        protected void btnCheckMaterialQty_Click(object sender, EventArgs e)
        {
            try
            {
                string lastMonthYYMM = txtlmonth.Text;
                string targetSubno = "4";

                string sqlGetDeparts = $@"
                    SELECT DISTINCT depart 
                    FROM erp:prddpth 
                    WHERE yymm = '{lastMonthYYMM}' AND subno = '{targetSubno}' AND depart <> ''";

                DataTable dtDeparts = UnixConn.ExecuteQuery(sqlGetDeparts);

                if (dtDeparts == null || dtDeparts.Rows.Count == 0)
                {
                    ShowAlert($"Tháng {lastMonthYYMM} không tìm thấy dữ liệu bộ phận (depart) nào để đối chiếu!", "info");
                    return;
                }

                StringBuilder sbErrorMessages = new StringBuilder();
                int errorCount = 0;

                foreach (DataRow rowDep in dtDeparts.Rows)
                {
                    string currentDepart = rowDep["depart"].ToString().Trim();

                    DataTable dtCompare = new DataTable();
                    dtCompare.Columns.Add("itnbr", typeof(string));
                    dtCompare.Columns.Add("qty1", typeof(decimal));
                    dtCompare.Columns.Add("qty2", typeof(decimal));

                    string sqlGdtOut = $@"
                        SELECT itnbr, qty 
                        FROM erp:gdtprih 
                        WHERE depno = '{currentDepart}' 
                          AND sliptype MATCHES 'B2[ABCDER]' 
                          AND inout = '1' 
                          AND yymm = '{lastMonthYYMM}' 
                          AND subno = '{targetSubno}'";

                    DataTable dtGdtOut = UnixConn.ExecuteQuery(sqlGdtOut);
                    if (dtGdtOut != null)
                    {
                        foreach (DataRow r in dtGdtOut.Rows)
                        {
                            decimal qty = r["qty"] != DBNull.Value ? Convert.ToDecimal(r["qty"]) : 0;
                            dtCompare.Rows.Add(r["itnbr"].ToString().Trim(), qty, 0);
                        }
                    }

                    string sqlGdtReturn = $@"
                        SELECT itnbr, qty 
                        FROM erp:gdtprih 
                        WHERE infact = '{currentDepart}' 
                          AND sliptype[1,2] = 'B3' 
                          AND yymm = '{lastMonthYYMM}' 
                          AND subno = '{targetSubno}'";

                    DataTable dtGdtReturn = UnixConn.ExecuteQuery(sqlGdtReturn);
                    if (dtGdtReturn != null)
                    {
                        foreach (DataRow r in dtGdtReturn.Rows)
                        {
                            decimal qty = r["qty"] != DBNull.Value ? Convert.ToDecimal(r["qty"]) : 0;
                            dtCompare.Rows.Add(r["itnbr"].ToString().Trim(), -qty, 0);
                        }
                    }

                    string sqlPrd = $@"
                        SELECT dmitnbr, getinqty 
                        FROM erp:prddpth 
                        WHERE depart = '{currentDepart}' 
                          AND yymm = '{lastMonthYYMM}' 
                          AND subno = '{targetSubno}'";

                    DataTable dtPrd = UnixConn.ExecuteQuery(sqlPrd);
                    if (dtPrd != null)
                    {
                        foreach (DataRow r in dtPrd.Rows)
                        {
                            decimal getinqty = r["getinqty"] != DBNull.Value ? Convert.ToDecimal(r["getinqty"]) : 0;
                            dtCompare.Rows.Add(r["dmitnbr"].ToString().Trim(), 0, getinqty);
                        }
                    }

                    System.Collections.Generic.List<string> distinctItnbrs = new System.Collections.Generic.List<string>();
                    foreach (DataRow r in dtCompare.Rows)
                    {
                        string itnbr = r["itnbr"].ToString();
                        if (!distinctItnbrs.Contains(itnbr) && !string.IsNullOrEmpty(itnbr))
                        {
                            distinctItnbrs.Add(itnbr);
                        }
                    }

                    foreach (string itnbr in distinctItnbrs)
                    {
                        decimal sumQty1 = 0;
                        decimal sumQty2 = 0;

                        foreach (DataRow r in dtCompare.Rows)
                        {
                            if (r["itnbr"].ToString() == itnbr)
                            {
                                sumQty1 += Convert.ToDecimal(r["qty1"]);
                                sumQty2 += Convert.ToDecimal(r["qty2"]);
                            }
                        }

                        if (sumQty1 != sumQty2)
                        {
                            errorCount++;
                            sbErrorMessages.Append($"<b>[Bộ phận: {currentDepart}]</b> 原料代號: {itnbr} <br/>")
                                           .Append($"GDT310數量: {sumQty1:0.00} <br/>")
                                           .Append($"PRDA06數量: {sumQty2:0.00} <br/>")
                                           .Append($"<span style='color:#ef4444;'>數量不符,請再確認!!!</span><br/><br/>");
                        }
                    }
                }

                if (errorCount > 0)
                {
                    string finalAlertMsg = sbErrorMessages.ToString();

                    if (finalAlertMsg.Length > 2000)
                    {
                        finalAlertMsg = finalAlertMsg.Substring(0, 2000) + "<br/><i>... (Còn tiếp và nhiều mã khác bị lệch)</i>";
                    }

                    string script = $"Swal.fire({{ title: 'Phát hiện sai lệch ({errorCount} lỗi)', html: \"{finalAlertMsg.Replace("\"", "'")}\", icon: 'error', width: '600px' }});";
                    ScriptManager.RegisterStartupScript(this, GetType(), "CheckQtyError", script, true);
                }
                else
                {
                    ShowAlert($"Kiểm tra hoàn tất. Toàn bộ các bộ phận đều khớp số lượng giữa GDT310 và PRDA06 trong tháng {lastMonthYYMM}!", "success");
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Lỗi hệ thống: {ex.Message.Replace("'", "''")}", "error");
            }
        }
    }
}