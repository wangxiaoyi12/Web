using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Common
{
    public class ExcelHelp
    {
        #region 数据导出为EXCEL
        public static void CreateExcel(DataTable dt, string fileName)
        {
            StringBuilder strb = new StringBuilder();
            strb.Append(" <html xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            strb.Append("xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            strb.Append("xmlns=\"http://www.w3.org/TR/REC-html40\">");
            strb.Append(" <head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            strb.Append(" <style>");
            strb.Append(".xl26");
            strb.Append(" {mso-style-parent:style0;");
            strb.Append(" font-family:\"Times New Roman\", serif;");
            strb.Append(" mso-font-charset:0;");
            strb.Append(" mso-number-format:\"@\";}");
            strb.Append(" </style>");
            strb.Append(" <xml>");
            strb.Append(" <x:ExcelWorkbook>");
            strb.Append(" <x:ExcelWorksheets>");
            strb.Append(" <x:ExcelWorksheet>");
            strb.Append(" <x:Name>" + fileName + "</x:Name>");
            strb.Append(" <x:WorksheetOptions>");
            strb.Append(" <x:DefaultRowHeight>285</x:DefaultRowHeight>");
            strb.Append(" <x:Selected/>");
            strb.Append(" <x:Panes>");
            strb.Append(" <x:Pane>");
            strb.Append(" <x:Number>3</x:Number>");
            strb.Append(" <x:ActiveCol>1</x:ActiveCol>");
            strb.Append(" </x:Pane>");
            strb.Append(" </x:Panes>");
            strb.Append(" <x:ProtectContents>False</x:ProtectContents>");
            strb.Append(" <x:ProtectObjects>False</x:ProtectObjects>");
            strb.Append(" <x:ProtectScenarios>False</x:ProtectScenarios>");
            strb.Append(" </x:WorksheetOptions>");
            strb.Append(" </x:ExcelWorksheet>");
            strb.Append(" <x:WindowHeight>6750</x:WindowHeight>");
            strb.Append(" <x:WindowWidth>10620</x:WindowWidth>");
            strb.Append(" <x:WindowTopX>480</x:WindowTopX>");
            strb.Append(" <x:WindowTopY>75</x:WindowTopY>");
            strb.Append(" <x:ProtectStructure>False</x:ProtectStructure>");
            strb.Append(" <x:ProtectWindows>False</x:ProtectWindows>");
            strb.Append(" </x:ExcelWorkbook>");
            strb.Append(" </xml>");
            strb.Append("");
            strb.Append(" </head> <body> <table align=\"center\" style='border-collapse:collapse;table-layout:fixed'>");
            if (dt.Rows.Count > 0)
            {
                strb.Append("<tr>");
                //写列标题   
                int columncount = dt.Columns.Count;
                for (int columi = 0; columi < columncount; columi++)
                {
                    strb.Append(" <td style='text-align:center;'><b>" + dt.Columns[columi].ColumnName.ToString() + "</b></td>");
                }
                strb.Append(" </tr>");
                //写数据   
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strb.Append(" <tr>");

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        strb.Append(" <td class='xl26'>" + dt.Rows[i][j].ToString() + "</td>");
                    }
                    strb.Append(" </tr>");
                }
            }
            strb.Append("</table> </body> </html>");
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;// 
            HttpContext.Current.Response.ContentType = "application/ms-excel";//设置输出文件类型为excel文件。 
            //HttpContext.Current.p.EnableViewState = false;
            HttpContext.Current.Response.Write(strb);
            HttpContext.Current.Response.End();
        }
        #endregion

        #region NPOI导出导入Excel
        //Datatable导出Excel
        /// <summary>
        /// 从DataTable导出Excel
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="strExcelFileName">excel文件路径</param>
        /// <param name="fields">对应要导出的字段</param>
        public static void ToExcelByNPOI(DataTable dt, string strExcelFileName, List<KeyValuePair<string, string>> fields = null)
        {
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                HeadercellStyle.VerticalAlignment = VerticalAlignment.Center;
                //字体
                NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                headerfont.Boldweight = (short)FontBoldWeight.Bold;
                HeadercellStyle.SetFont(headerfont);


                //用column name 作为列名
                int icolIndex = 0;
                IRow headerRow = sheet.CreateRow(0);
                headerRow.HeightInPoints = 30;
                if (fields == null)
                {
                    foreach (DataColumn item in dt.Columns)
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(item.ColumnName);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }
                }
                else
                {
                    foreach (var item in fields)
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(item.Value);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }
                }

                ICellStyle cellStyle = workbook.CreateCellStyle();

                //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;

                NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                cellfont.Boldweight = (short)FontBoldWeight.Normal;
                cellStyle.SetFont(cellfont);

                //建立内容行
                int iRowIndex = 1;
                int iCellIndex = 0;
                foreach (DataRow Rowitem in dt.Rows)
                {
                    IRow DataRow = sheet.CreateRow(iRowIndex);
                    DataRow.HeightInPoints = 25;
                    if (fields != null)
                    {
                        foreach (var Colitem in fields)
                        {

                            ICell cell = DataRow.CreateCell(iCellIndex);
                            var value = Rowitem[Colitem.Key];
                            if (value != null)
                            {
                                cell.SetCellValue(value.ToString());
                            }
                            cell.CellStyle = cellStyle;
                            iCellIndex++;
                        }
                    }
                    else
                    {
                        foreach (DataColumn Colitem in dt.Columns)
                        {

                            ICell cell = DataRow.CreateCell(iCellIndex);
                            cell.SetCellValue(Rowitem[Colitem].ToString());
                            var value = Rowitem[Colitem];
                            if (value != null)
                            {
                                cell.SetCellValue(value.ToString());
                            }
                            cell.CellStyle = cellStyle;
                            iCellIndex++;
                        }
                    }
                    iCellIndex = 0;
                    iRowIndex++;
                }

                //自适应列宽度
                for (int i = 0; i < icolIndex; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                //string excelPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + strExcelFileName + ".xls";

                //写Excel
                FileStream file = new System.IO.FileStream(strExcelFileName, FileMode.OpenOrCreate);
                workbook.Write(file);
                file.Flush();
                file.Close();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 将List集合导出到Excel
        /// </summary>
        /// <typeparam name="T">导出的实体模型</typeparam>
        /// <param name="list">数据列表集合</param>
        /// <param name="strExcelFileName">excel文件路径</param>
        /// <param name="fields">导出的字段集</param>
        public static void ToExcelByNPOI<T>(List<T> list, string strExcelFileName, List<KeyValuePair<string, string>> fields = null)
        {
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                HeadercellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                HeadercellStyle.VerticalAlignment = VerticalAlignment.Center;
                //字体
                NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                headerfont.Boldweight = (short)FontBoldWeight.Bold;
                HeadercellStyle.SetFont(headerfont);


                //用column name 作为列名
                int icolIndex = 0;
                IRow headerRow = sheet.CreateRow(0);
                headerRow.HeightInPoints = 30;
                PropertyInfo[] Properties = typeof(T).GetProperties();

                if (fields == null)
                {
                    foreach (PropertyInfo objProperty in Properties)  //遍历T的属性
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(objProperty.Name);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }
                }
                else
                {
                    foreach (var field in fields)  //遍历T的属性
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(field.Value);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }
                }

                ICellStyle cellStyle = workbook.CreateCellStyle();

                //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;

                NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                cellfont.Boldweight = (short)FontBoldWeight.Normal;
                cellStyle.SetFont(cellfont);

                //建立内容行
                int iRowIndex = 1;
                int iCellIndex = 0;

                foreach (var item in list)
                {
                    IRow DataRow = sheet.CreateRow(iRowIndex);
                    DataRow.HeightInPoints = 25;
                    //PropertyInfo[] PI = typeof(T).GetProperties();
                    if (fields == null)
                    {
                        //循环获取该对象的所有属性和值
                        foreach (PropertyInfo info in Properties)
                        {
                            ICell cell = DataRow.CreateCell(iCellIndex);
                            var value = info.GetValue(item);
                            if (value != null)
                            {
                                cell.SetCellValue(value.ToString());
                            }
                            cell.CellStyle = cellStyle;
                            iCellIndex++;
                        }
                    }
                    else
                    {
                        foreach (var field in fields)
                        {
                            ICell cell = DataRow.CreateCell(iCellIndex);
                            var p = Properties.FirstOrDefault(a => a.Name == field.Key);
                            if (p != null)
                            {
                                var value = p.GetValue(item);
                                if (value != null)
                                {
                                    cell.SetCellValue(value.ToString());
                                }
                            }
                            cell.CellStyle = cellStyle;
                            iCellIndex++;
                        }
                    }
                    iCellIndex = 0;
                    iRowIndex++;
                }

                //自适应列宽度
                for (int i = 0; i < icolIndex; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                //string excelPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + strExcelFileName + ".xls";

                //写Excel
                FileStream file = new System.IO.FileStream(strExcelFileName, FileMode.OpenOrCreate);
                workbook.Write(file);
                file.Flush();
                file.Close();
            }
            catch
            {
                throw;
            }
        }

        public static void ToExcelByNPOI(System.Collections.IEnumerable list, string strExcelFileName, List<KeyValuePair<string, string>> fields = null)
        {
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                HeadercellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                HeadercellStyle.VerticalAlignment = VerticalAlignment.Center;
                //字体
                NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                headerfont.Boldweight = (short)FontBoldWeight.Bold;
                HeadercellStyle.SetFont(headerfont);


                //用column name 作为列名
                int icolIndex = 0;
                IRow headerRow = sheet.CreateRow(0);
                headerRow.HeightInPoints = 30;
                Type type = null;
                foreach (var item in list)
                {
                    type = item.GetType();
                    break;
                }
                PropertyInfo[] Properties = type.GetProperties();

                if (fields == null)
                {
                    foreach (PropertyInfo objProperty in Properties)  //遍历T的属性
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(objProperty.Name);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }
                }
                else
                {
                    foreach (var field in fields)  //遍历T的属性
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(field.Value);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }
                }

                ICellStyle cellStyle = workbook.CreateCellStyle();

                //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;

                NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                cellfont.Boldweight = (short)FontBoldWeight.Normal;
                cellStyle.SetFont(cellfont);

                //建立内容行
                int iRowIndex = 1;
                int iCellIndex = 0;

                foreach (var item in list)
                {
                    IRow DataRow = sheet.CreateRow(iRowIndex);
                    DataRow.HeightInPoints = 25;
                    //PropertyInfo[] PI = typeof(T).GetProperties();
                    if (fields == null)
                    {
                        //循环获取该对象的所有属性和值
                        foreach (PropertyInfo info in Properties)
                        {
                            ICell cell = DataRow.CreateCell(iCellIndex);
                            var value = info.GetValue(item);
                            if (value != null)
                            {
                                cell.SetCellValue(value.ToString());
                            }
                            cell.CellStyle = cellStyle;
                            iCellIndex++;
                        }
                    }
                    else
                    {
                        foreach (var field in fields)
                        {
                            ICell cell = DataRow.CreateCell(iCellIndex);
                            var p = Properties.FirstOrDefault(a => a.Name == field.Key);
                            if (p != null)
                            {
                                var value = p.GetValue(item);
                                if (value != null)
                                {
                                    cell.SetCellValue(value.ToString());
                                }
                            }
                            cell.CellStyle = cellStyle;
                            iCellIndex++;
                        }
                    }
                    iCellIndex = 0;
                    iRowIndex++;
                }

                //自适应列宽度
                for (int i = 0; i < icolIndex; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                //string excelPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + strExcelFileName + ".xls";

                //写Excel
                FileStream file = new System.IO.FileStream(strExcelFileName, FileMode.OpenOrCreate);
                workbook.Write(file);
                file.Flush();
                file.Close();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Excel文件导成Datatable
        /// </summary>
        /// <param name="strFilePath">Excel文件目录地址</param>
        /// <param name="strTableName">Datatable表名</param>
        /// <param name="iSheetIndex">Excel sheet index</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTableByNPOI(string strFilePath, string strTableName, int iSheetIndex)
        {

            string strExtName = Path.GetExtension(strFilePath);

            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(strTableName))
            {
                dt.TableName = strTableName;
            }

            if (strExtName.Equals(".xls") || strExtName.Equals(".xlsx"))
            {
                using (FileStream file = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(file);
                    ISheet sheet = workbook.GetSheetAt(iSheetIndex);

                    //列头
                    foreach (ICell item in sheet.GetRow(sheet.FirstRowNum).Cells)
                    {
                        dt.Columns.Add(item.ToString(), typeof(string));
                    }

                    //写入内容
                    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                    while (rows.MoveNext())
                    {
                        IRow row = (HSSFRow)rows.Current;
                        if (row.RowNum == sheet.FirstRowNum)
                        {
                            continue;
                        }

                        DataRow dr = dt.NewRow();
                        foreach (ICell item in row.Cells)
                        {
                            switch (item.CellType)
                            {
                                case CellType.Boolean:
                                    dr[item.ColumnIndex] = item.BooleanCellValue;
                                    break;
                                case CellType.Error:
                                    dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                    break;
                                case CellType.Formula:
                                    switch (item.CachedFormulaResultType)
                                    {
                                        case CellType.Boolean:
                                            dr[item.ColumnIndex] = item.BooleanCellValue;
                                            break;
                                        case CellType.Error:
                                            dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                            break;
                                        case CellType.Numeric:
                                            if (DateUtil.IsCellDateFormatted(item))
                                            {
                                                dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                            }
                                            else
                                            {
                                                dr[item.ColumnIndex] = item.NumericCellValue;
                                            }
                                            break;
                                        case CellType.String:
                                            string str = item.StringCellValue;
                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                dr[item.ColumnIndex] = str.ToString();
                                            }
                                            else
                                            {
                                                dr[item.ColumnIndex] = null;
                                            }
                                            break;
                                        case CellType.Unknown:
                                        case CellType.Blank:
                                        default:
                                            dr[item.ColumnIndex] = string.Empty;
                                            break;
                                    }
                                    break;
                                case CellType.Numeric:
                                    if (DateUtil.IsCellDateFormatted(item))
                                    {
                                        dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = item.NumericCellValue;
                                    }
                                    break;
                                case CellType.String:
                                    string strValue = item.StringCellValue;
                                    if (!string.IsNullOrEmpty(strValue))
                                    {
                                        dr[item.ColumnIndex] = strValue.ToString();
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = null;
                                    }
                                    break;
                                case CellType.Unknown:
                                case CellType.Blank:
                                default:
                                    dr[item.ColumnIndex] = string.Empty;
                                    break;
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }

            return dt;
        }

        public static List<T> ExcelToListByNPOI<T>(string strFilePath, string strTableName, int iSheetIndex) where T : new()
        {

            string strExtName = Path.GetExtension(strFilePath);

            List<T> list = new List<T>();
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(strTableName))
            {
                dt.TableName = strTableName;
            }

            if (strExtName.Equals(".xls") || strExtName.Equals(".xlsx"))
            {
                using (FileStream file = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(file);
                    ISheet sheet = workbook.GetSheetAt(iSheetIndex);

                    //列头
                    foreach (ICell item in sheet.GetRow(sheet.FirstRowNum).Cells)
                    {

                        dt.Columns.Add(item.ToString(), typeof(string));
                    }

                    //写入内容
                    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                    while (rows.MoveNext())
                    {
                        IRow row = (HSSFRow)rows.Current;
                        if (row.RowNum == sheet.FirstRowNum)
                        {
                            continue;
                        }

                        DataRow dr = dt.NewRow();
                        foreach (ICell item in row.Cells)
                        {
                            switch (item.CellType)
                            {
                                case CellType.Boolean:
                                    dr[item.ColumnIndex] = item.BooleanCellValue;
                                    break;
                                case CellType.Error:
                                    dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                    break;
                                case CellType.Formula:
                                    switch (item.CachedFormulaResultType)
                                    {
                                        case CellType.Boolean:
                                            dr[item.ColumnIndex] = item.BooleanCellValue;
                                            break;
                                        case CellType.Error:
                                            dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                            break;
                                        case CellType.Numeric:
                                            if (DateUtil.IsCellDateFormatted(item))
                                            {
                                                dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                            }
                                            else
                                            {
                                                dr[item.ColumnIndex] = item.NumericCellValue;
                                            }
                                            break;
                                        case CellType.String:
                                            string str = item.StringCellValue;
                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                dr[item.ColumnIndex] = str.ToString();
                                            }
                                            else
                                            {
                                                dr[item.ColumnIndex] = null;
                                            }
                                            break;
                                        case CellType.Unknown:
                                        case CellType.Blank:
                                        default:
                                            dr[item.ColumnIndex] = string.Empty;
                                            break;
                                    }
                                    break;
                                case CellType.Numeric:
                                    if (DateUtil.IsCellDateFormatted(item))
                                    {
                                        dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = item.NumericCellValue;
                                    }
                                    break;
                                case CellType.String:
                                    string strValue = item.StringCellValue;
                                    if (!string.IsNullOrEmpty(strValue))
                                    {
                                        dr[item.ColumnIndex] = strValue.ToString();
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = null;
                                    }
                                    break;
                                case CellType.Unknown:
                                case CellType.Blank:
                                default:
                                    dr[item.ColumnIndex] = string.Empty;
                                    break;
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }

            return DataTableHelp.ConvertTo<T>(dt);
        }

        #endregion
    }
}
