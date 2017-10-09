using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;

namespace Sunvalley_PLSystem.GeneralTools
{
    public class ExcelTools
    {
        public const string EXCEL_MIME_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string EXCEL_FORMAT = ".xlsx";

        /// <summary>
        /// Generates an ExcelFile basesd a given DataTable.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="heading"></param>
        /// <param name="showSrNo"></param>
        /// <param name="columnsToAvoid"></param>
        /// <returns></returns>
        public static ExcelPackage exportToExcel(List<TableToExportExcel> dataTables, string title = "")
        {
            ExcelPackage package = new ExcelPackage();

            ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.IsNullOrEmpty(title)?"Report":title);
            int startRowFrom = 0;

            foreach(var tableToExport in dataTables) {
                DataTable dt = tableToExport.table;
                string heading = tableToExport.header;
                startRowFrom += String.IsNullOrEmpty(heading) ? 1 : 2;

                // add the content into the Excel file  
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dt, true);

                // autofit width of cells with small content  
                int columnIndex = 1;
                foreach (DataColumn column in dt.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => cell.Value==null?0:cell.Value.ToString().Count());
                    if (maxLength < 150)
                        workSheet.Column(columnIndex).AutoFit();

                    columnIndex++;
                }

                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dt.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                }

                // format cells - add borders  
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dt.Rows.Count, dt.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                if (!String.IsNullOrEmpty(heading))
                {
                    workSheet.Cells["A"+ (startRowFrom-1)].Value = heading;
                    workSheet.Cells["A" + (startRowFrom - 1)].Style.Font.Size = 20;
                }

                startRowFrom += dt.Rows.Count+1;
            }

            return package;
        }

        public class TableToExportExcel
        {
            public DataTable table { get; }
            public string header { get; }
            public TableToExportExcel() { }
            public TableToExportExcel(DataTable table, string header) {
                this.table = table;
                this.header = header;
            }
        }

        /// <summary>
        /// A partir de una lista de objetos, genera un datatable.
        /// </summary>
        /// <typeparam name="T">Data type of the list items.</typeparam>
        /// <param name="lista">The list to convert.</param>
        /// <returns></returns>
        public static DataTable listToDatatable<T>(List<T> lista)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];
            foreach (T item in lista)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}