using System;
using System.Data;
using System.Web.UI;
using Unix_Web.Helpers;
using Unix_Web.Providers; // Đảm bảo đúng namespace của ExcelHelper

namespace Unix_Web.Source
{
    public partial class DownloadData : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra quyền truy cập nếu cần
            //test chức năng restore 1
            if (Session["username"] == null)
            {
                Response.Redirect("~/Auth/Login.aspx");
            }
        }

        protected void btnDownloadINV035_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = @"SELECT subno, factory, itnbr, itdsc, spdsc, dotcode, indat, usrno 
                               FROM erp:invdco 
                               WHERE subno = 4 AND factory = 'V' 
                               ORDER BY indat DESC";

                // Giả sử bạn có một lớp DatabaseHelper để thực thi SQL trả về DataTable
                DataTable dt = UnixConn.ExecuteQuery(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Sử dụng ExcelHelper để xuất file
                    ExcelHelper.ExportToExcel(dt, "INV035_Export_" + DateTime.Now.ToString("yyyyMMdd"));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "Swal.fire('Thông báo', 'Không có dữ liệu để xuất!', 'info');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"Swal.fire('Lỗi', '{ex.Message}', 'error');", true);
            }
        }

        protected void btnDownloadCRDSI801_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "SELECT * FROM erp:rdsudb";

                DataTable dt = UnixConn.ExecuteQuery(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    ExcelHelper.ExportToExcel(dt, "CRDSI801_Export_" + DateTime.Now.ToString("yyyyMMdd"));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "Swal.fire('Thông báo', 'Không có dữ liệu để xuất!', 'info');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"Swal.fire('Lỗi', '{ex.Message}', 'error');", true);
            }
        }

        protected void btnDownloadRDSMEST_Click(object sender, EventArgs e)
        {
            try
            {
                System.Collections.Generic.List<string> selectedMachines = new System.Collections.Generic.List<string>();
                if (chkTR0001.Checked) selectedMachines.Add("TR0001");
                if (chkTR0002.Checked) selectedMachines.Add("TR0002");
                if (chkSW0001.Checked) selectedMachines.Add("SW0001");

                if (selectedMachines.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "Swal.fire('Thông báo', 'Vui lòng chọn ít nhất 1 mã máy (machno).', 'warning');", true);
                    return;
                }

                System.Collections.Generic.List<DataTable> listDt = new System.Collections.Generic.List<DataTable>();
                System.Collections.Generic.List<string[]> listHeaders = new System.Collections.Generic.List<string[]>();

                // 1. HEADER MÁY TR (Chuẩn 115 cột theo cấu trúc bảng)
                string[] TR_HEADERS = new string[] {
                    "集團別", "廠別", "機台號碼", "中間製品代號", "版本",
                    "類別", "SPEC編號", "上流道膠號", "中流道膠號", "下流道膠號",
                    "板規編號", "內口金編號", "上流道編號", "中流道編號", "下流道編號",
                    "中心色線", "色線2 (右側)", "色線3 (右側)", "風扇1", "風扇2",
                    "風扇3", "風扇4", "上螺桿溫度Φ90", "上機筒一溫度Φ90", "上機筒二溫度Φ90",
                    "中螺桿溫度Φ200", "中機筒一溫度Φ200", "中機筒二溫度Φ200", "下螺桿溫度Φ150", "下機筒一溫度Φ150",
                    "下機筒二溫度Φ150", "上機頭溫度", "中機頭溫度", "下機頭溫度", "預成型座溫度1",
                    "預成型座溫度2", "上螺桿轉速 (rpm)", "中螺桿轉速 (rpm)", "下螺桿轉速 (rpm)", "速度 (m/min)",
                    "上擠出機服務速度", "中擠出機服務速度", "下擠出機服務速度", "接取段速度", "收縮段1速度",
                    "收縮段2速度", "收縮段3速度", "接取段比率", "收縮段1比率", "收縮段2比率",
                    "收縮段3比率", "上擠出機預警壓力bar", "上擠出機膠料壓力bar", "中擠出機預警壓力bar", "中擠出機膠料壓力bar",
                    "下擠出機預警壓力bar", "下擠出機膠料壓力bar", "上擠出機膠料溫度℃", "中擠出機膠料溫度℃", "下擠出機膠料溫度℃",
                    "連續秤(㎏/m)", "擷取膠料溫度", "寬度 (mm)", "連秤重+公差", "連秤重-公差",
                    "連秤重++公差", "連秤重--公差", "寬度+公差", "寬度-公差", "寬度++公差",
                    "寬度--公差", "色線2 (右側)距離", "色線3 (右側)距離", "Dancer1 壓力設定(%)", "Dancer2 壓力設定(%)",
                    "Dancer3 壓力設定(%)", "Dancer4 壓力設定(%)", "Dancer5 壓力設定(%)", "Dancer6 壓力設定(%)", "Dancer7 壓力設定(%)",
                    "Dancer8 壓力設定(%)", "Dancer9 壓力設定(%)", "Dancer10 壓力設定(%)", "Dancer11 壓力設定(%)", "Dancer12 壓力設定(%)",
                    "Dancer13 壓力設定(%)", "Dancer14 壓力設定(%)", "Dancer1 位置設定(%)", "Dancer2 位置設定(%)", "Dancer3 位置設定(%)",
                    "Dancer4 位置設定(%)", "Dancer5 位置設定(%)", "Dancer6 位置設定(%)", "Dancer7 位置設定(%)", "Dancer8 位置設定(%)",
                    "Dancer9 位置設定(%)", "Dancer10 位置設定(%)", "Dancer11 位置設定(%)", "Dancer12 位置設定(%)", "Dancer13 位置設定(%)",
                    "Dancer14 位置設定(%)", "裁刀溫度", "捲取長度1", "捲取長度2", "墊布張力1",
                    "墊布張力2", "時間", "鍵入日期", "使用者", "審核時間",
                    "審核日期", "審核人員", "鎖機狀態", "噴字內容", "狀態"
                };

                // 2. HEADER MÁY SW (Chuẩn 41 cột giữ đúng thứ tự xuất hiện trong bảng rdsmest)
                string[] SW_HEADERS = new string[] {
                    "集團別", "廠別", "機台號碼", "中間製品代號", "版本",
                    "類別", "SPEC編號", "上流道膠料", "下流道膠料", "板規編號",
                    "內口金編號", "上流道編號", "下流道編號", "單卷長度", "溫控機頭上模溫度",
                    "溫控機頭下模溫度", "溫控上擠出機200機筒1段溫度", "溫控上擠出機200機筒2段溫度", "溫控上擠出機200螺杆段溫度", "溫控下擠出機150機筒1段溫度",
                    "溫控下擠出機150機筒2段溫度", "溫控下擠出機150螺杆段溫度", "擠出胎邊總寬度", "擠出胎邊寬度誤差", "擠出胎邊每米重量",
                    "擠出胎邊每米重量誤差", "每卷裁斷長度", "200上擠出機速度", "150下擠出機進度", "冷卻線速度",
                    "收縮1#收縮比%", "收縮2#收縮比%", "收縮3#收縮比%", "收縮4#收縮比%", "浮動輥氣壓",
                    "上擠出機膠料壓力", "下擠出機膠料壓力", "維護時間", "維護日期", "維護人員",
                    "狀態"
                };

                // 3. Vòng lặp tách query riêng cho từng máy để đẩy ra Sheet tương ứng
                foreach (string machine in selectedMachines)
                {
                    string sql = "";

                    // SQL CHO MÁY TR (Lấy đủ 115 trường)
                    if (machine.StartsWith("TR"))
                    {
                        sql = $@"
                            SELECT 
                                subno, factory, machno, partno, version, stype, specno, acompound, bcompound, ccompound, 
                                preformer, finaldie, ainsert, binsert, cinsert, 
                                CASE astripecol WHEN '84003' THEN 'DO' WHEN '84015' THEN 'CAM' WHEN '84006' THEN 'VANG' WHEN '84001' THEN 'XANH LUC' WHEN '84004' THEN 'TRANG' WHEN '84009' THEN 'XANH LA' WHEN '84008' THEN 'XANH LAM' WHEN '84002' THEN 'HONG' ELSE astripecol END AS astripecol, 
                                CASE bstripecol WHEN '84003' THEN 'DO' WHEN '84015' THEN 'CAM' WHEN '84006' THEN 'VANG' WHEN '84001' THEN 'XANH LUC' WHEN '84004' THEN 'TRANG' WHEN '84009' THEN 'XANH LA' WHEN '84008' THEN 'XANH LAM' WHEN '84002' THEN 'HONG' ELSE bstripecol END AS bstripecol, 
                                CASE cstripecol WHEN '84003' THEN 'DO' WHEN '84015' THEN 'CAM' WHEN '84006' THEN 'VANG' WHEN '84001' THEN 'XANH LUC' WHEN '84004' THEN 'TRANG' WHEN '84009' THEN 'XANH LA' WHEN '84008' THEN 'XANH LAM' WHEN '84002' THEN 'HONG' ELSE cstripecol END AS cstripecol, 
                                ablower, bblower, cblower, dblower, aezatemp, aezbtemp, aezctemp, bezatemp, bezbtemp, 
                                bezctemp, cezatemp, cezbtemp, cezctemp, ahztemp, bhztemp, chztemp, apreformer, bpreformer, 
                                aspeed, bspeed, cspeed, shrinkspeed, aspeedser, bspeedser, cspeedser, takespeed, ashrinkspeed, 
                                bshrinkspeed, cshrinkspeed, takespeedre, ashspeedre, bshspeedre, cshspeedre, acompalarm, 
                                acomppres, bcompalarm, bcomppres, ccompalarm, ccomppres, acomptemp, bcomptemp, ccomptemp, 
                                weight, irtemp, width, weight1, weight2, weight3, weight4, width1, 
                                width2, width3, width4, amarkpos, bmarkpos, dancer1, dancer2, dancer3, dancer4, dancer5, 
                                dancer6, dancer7, dancer8, dancer9, dancer10, dancer11, dancer12, dancer13, dancer14, posdan1, 
                                posdan2, posdan3, posdan4, posdan5, posdan6, posdan7, posdan8, posdan9, posdan10, posdan11, 
                                posdan12, posdan13, posdan14, knife, alength, blength, aliner, bliner, intime, indat, 
                                usrno, sintime, sindat, susrno, lock, ptdata, state 
                            FROM [erp].[dbo].[rdsmest] 
                            WHERE machno = '{machine}'";
                    }
                    // SQL CHO MÁY SW (Chỉ lấy đúng 41 trường tương ứng với SW_HEADERS)
                    else if (machine.StartsWith("SW"))
                    {
                        sql = $@"
                            SELECT 
                                subno, factory, machno, partno, version, stype, specno, acompound, ccompound, preformer, 
                                finaldie, ainsert, cinsert, ablower, bblower, dblower, aezatemp, aezbtemp, aezctemp, bezatemp, 
                                bezbtemp, bezctemp, cezatemp, cezctemp, bhztemp, chztemp, apreformer, bpreformer, aspeed, bspeed, 
                                cspeed, shrinkspeed, aspeedser, bspeedser, cspeedser, takespeed, ashrinkspeed, intime, indat, usrno, 
                                state
                            FROM [erp].[dbo].[rdsmest] 
                            WHERE machno = '{machine}'";
                    }

                    // Thực thi query riêng biệt
                    DataTable dtMachine = SQLConn34.ExecuteQuery(sql);

                    // Nếu DB trả về dữ liệu, lưu DataTable vào listDt để chuẩn bị xuất thành Sheet
                    if (dtMachine != null && dtMachine.Rows.Count > 0)
                    {
                        dtMachine.TableName = machine; // Dùng machno làm tên Sheet (VD: Sheet "TR0001")
                        listDt.Add(dtMachine);
                        listHeaders.Add(machine.StartsWith("SW") ? SW_HEADERS : TR_HEADERS);
                    }
                }

                // Xuất file nếu có ít nhất 1 DataTable (ít nhất 1 Sheet)
                if (listDt.Count > 0)
                {
                    // Hàm này ở file ExcelHelper sẽ lặp qua listDt, tạo ra N sheet tương ứng.
                    ExcelHelper.ExportMultipleSheetsToExcel(listDt, "RDSMEST_Data_" + DateTime.Now.ToString("yyyyMMdd"), listHeaders);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "Swal.fire('Thông báo', 'Không tìm thấy dữ liệu trên DB cho các máy đã chọn!', 'info');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"Swal.fire('Lỗi', '{ex.Message}', 'error');", true);
            }
        }
    }
}