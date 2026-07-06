using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Web;

namespace Unix_Web.Helpers
{
    /// <summary>
    /// Helper class để xuất DataTable ra Excel sử dụng NPOI
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// Xuất DataTable ra file Excel (.xlsx) và tự động download về client
        /// </summary>
        /// <param name="dt">DataTable chứa dữ liệu</param>
        /// <param name="fileName">Tên file (không cần extension .xlsx)</param>
        /// <param name="sheetName">Tên sheet trong Excel</param>
        /// <param name="autoSizeColumns">Tự động điều chỉnh độ rộng cột</param>
        /// <param name="freezeHeader">Đóng băng dòng header</param>
        public static void ExportToExcel(
            DataTable dt,
            string fileName,
            string sheetName = "Sheet1",
            bool autoSizeColumns = true,
            bool freezeHeader = true)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                throw new Exception("Không có dữ liệu để xuất Excel!");
            }

            // 1. Tạo Workbook
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);

            // 2. Tạo style cho header
            ICellStyle headerStyle = CreateHeaderStyle(workbook);

            // 3. Tạo style cho data
            ICellStyle dataStyle = CreateDataStyle(workbook);

            // 4. Tạo header row từ DataTable columns
            IRow headerRow = sheet.CreateRow(0);
            for (int col = 0; col < dt.Columns.Count; col++)
            {
                ICell cell = headerRow.CreateCell(col);
                cell.SetCellValue(dt.Columns[col].ColumnName);
                cell.CellStyle = headerStyle;
            }

            // 5. Ghi dữ liệu
            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                DataRow dataRow = dt.Rows[rowIndex];
                IRow excelRow = sheet.CreateRow(rowIndex + 1);

                for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                {
                    ICell cell = excelRow.CreateCell(colIndex);
                    object value = dataRow[colIndex];

                    SetCellValue(cell, value, dataStyle);
                }
            }

            // 6. Auto-size columns
            if (autoSizeColumns)
            {
                AutoSizeColumns(sheet, dt);
            }

            // 7. Freeze header row
            if (freezeHeader)
            {
                sheet.CreateFreezePane(0, 1);
            }

            // 8. Download file
            DownloadExcel(workbook, fileName);
        }

        /// <summary>
        /// Tạo style cho header (bold, màu nền xám, căn giữa)
        /// </summary>
        private static ICellStyle CreateHeaderStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            // Font
            IFont font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints = 11;
            style.SetFont(font);

            // Background color
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            style.FillPattern = FillPattern.SolidForeground;

            // Alignment
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            // Border
            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;

            return style;
        }

        /// <summary>
        /// Tạo style cho data (căn trái, border)
        /// </summary>
        private static ICellStyle CreateDataStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;

            // Border
            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;

            return style;
        }

        /// <summary>
        /// Set giá trị cho cell theo đúng kiểu dữ liệu
        /// </summary>
        private static void SetCellValue(ICell cell, object value, ICellStyle style)
        {
            cell.CellStyle = style;

            if (value == null || value == DBNull.Value)
            {
                cell.SetCellValue("");
                return;
            }

            // Xử lý theo kiểu dữ liệu
            Type type = value.GetType();

            if (type == typeof(int) || type == typeof(long) || type == typeof(short))
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (type == typeof(DateTime))
            {
                DateTime dt = (DateTime)value;
                cell.SetCellValue(dt.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if (type == typeof(bool))
            {
                cell.SetCellValue((bool)value ? "Yes" : "No");
            }
            else
            {
                cell.SetCellValue(value.ToString().Trim());
            }
        }

        /// <summary>
        /// Tự động điều chỉnh độ rộng cột theo nội dung dài nhất (header hoặc data),
        /// giới hạn tối đa 15000 (đơn vị 1/256 ký tự của NPOI).
        /// Trước đây đoạn logic này được copy-paste giống hệt nhau ở 3 nơi khác nhau trong file này.
        /// </summary>
        private static void AutoSizeColumns(ISheet sheet, DataTable dt)
        {
            IRow headerRow = sheet.GetRow(0);

            for (int col = 0; col < dt.Columns.Count; col++)
            {
                sheet.AutoSizeColumn(col);
                int currentWidth = (int)sheet.GetColumnWidth(col);

                ICell headerCell = headerRow?.GetCell(col);
                int maxLength = headerCell?.StringCellValue?.Length ?? 0;

                foreach (DataRow row in dt.Rows)
                {
                    string cellValue = row[col] != DBNull.Value ? row[col].ToString().Trim() : "";
                    if (cellValue.Length > maxLength)
                    {
                        maxLength = cellValue.Length;
                    }
                }

                int calculatedWidth = (maxLength + 3) * 350;
                int finalWidth = Math.Min(Math.Max(currentWidth, calculatedWidth), 15000);

                sheet.SetColumnWidth(col, finalWidth);
            }
        }

        /// <summary>
        /// Download file Excel về client
        /// </summary>
        private static void DownloadExcel(IWorkbook workbook, string fileName)
        {
            // Thêm extension nếu chưa có
            if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".xlsx";
            }

            HttpResponse response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AddHeader("Content-Disposition", $"attachment;filename={fileName}");

            HttpCookie cookie = new HttpCookie("fileDownload", "true");
            cookie.Path = "/";
            response.Cookies.Add(cookie);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                response.BinaryWrite(memoryStream.ToArray());
            }

            response.Flush();
            response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Xuất DataTable với custom column headers
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="fileName">Tên file</param>
        /// <param name="columnHeaders">Mảng tên cột custom (phải đủ số lượng)</param>
        /// <param name="sheetName">Tên sheet</param>
        public static void ExportToExcelWithCustomHeaders(
            DataTable dt,
            string fileName,
            string[] columnHeaders,
            string sheetName = "Sheet1")
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                throw new Exception("Không có dữ liệu để xuất Excel!");
            }

            if (columnHeaders.Length != dt.Columns.Count)
            {
                throw new Exception("Số lượng custom headers không khớp với số cột!");
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);

            ICellStyle headerStyle = CreateHeaderStyle(workbook);
            ICellStyle dataStyle = CreateDataStyle(workbook);

            // Header với custom names
            IRow headerRow = sheet.CreateRow(0);
            for (int col = 0; col < columnHeaders.Length; col++)
            {
                ICell cell = headerRow.CreateCell(col);
                cell.SetCellValue(columnHeaders[col]);
                cell.CellStyle = headerStyle;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                DataRow dataRow = dt.Rows[rowIndex];
                IRow excelRow = sheet.CreateRow(rowIndex + 1);

                for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                {
                    ICell cell = excelRow.CreateCell(colIndex);
                    SetCellValue(cell, dataRow[colIndex], dataStyle);
                }
            }

            // Auto-size
            AutoSizeColumns(sheet, dt);

            sheet.CreateFreezePane(0, 1);

            DownloadExcel(workbook, fileName);
        }

        /// <summary>
        /// Xuất nhiều DataTable ra nhiều sheet trong một file Excel
        /// </summary>
        public static void ExportMultipleSheetsToExcel(
            System.Collections.Generic.List<DataTable> dtList,
            string fileName,
            System.Collections.Generic.List<string[]> customHeadersList = null)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ICellStyle headerStyle = CreateHeaderStyle(workbook);
            ICellStyle dataStyle = CreateDataStyle(workbook);

            for (int i = 0; i < dtList.Count; i++)
            {
                DataTable dt = dtList[i];
                // Lấy tên máy làm tên Sheet, mặc định nếu trống thì đặt là Sheet1, Sheet2...
                string sheetName = string.IsNullOrEmpty(dt.TableName) ? "Sheet" + (i + 1) : dt.TableName;
                ISheet sheet = workbook.CreateSheet(sheetName);

                // 1. Tạo dòng Header
                IRow headerRow = sheet.CreateRow(0);
                string[] currentHeaders = (customHeadersList != null && customHeadersList.Count > i) ? customHeadersList[i] : null;

                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    ICell cell = headerRow.CreateCell(col);
                    string colName = (currentHeaders != null && col < currentHeaders.Length)
                                     ? currentHeaders[col]
                                     : dt.Columns[col].ColumnName;
                    cell.SetCellValue(colName);
                    cell.CellStyle = headerStyle;
                }

                // 2. Điền dữ liệu
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    IRow row = sheet.CreateRow(r + 1);
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        ICell cell = row.CreateCell(c);
                        string cellValue = dt.Rows[r][c] != DBNull.Value ? dt.Rows[r][c].ToString().Trim() : "";
                        cell.SetCellValue(cellValue);
                        cell.CellStyle = dataStyle;
                    }
                }

                // 3. Auto-size columns (quét cả Header và Data chuẩn Unicode)
                AutoSizeColumns(sheet, dt);

                // 4. Freeze header
                sheet.CreateFreezePane(0, 1);
            }

            // 5. Download file
            DownloadExcel(workbook, fileName);
        }
    }
}