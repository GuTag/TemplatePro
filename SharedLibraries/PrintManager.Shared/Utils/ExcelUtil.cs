using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Utils
{
    public class ExcelUtil
    {
        public static DataTable ReadExcelToTable(string filepath)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook;
            string fileExt = Path.GetExtension(filepath).ToLower();
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                if (fileExt == ".xlsx") 
                { 
                    workbook = new XSSFWorkbook(fs); 
                } 
                else if (fileExt == ".xls") 
                { 
                    workbook = new HSSFWorkbook(fs); 
                } 
                else 
                { 
                    workbook = null; 
                }
                if (workbook == null) { return null; }
                ISheet sheet = workbook.GetSheetAt(0);

                //表头  
                //IRow header = sheet.GetRow(sheet.FirstRowNum);
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueType(workbook, header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据  
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns)
                    {
                        dr[j] = GetValueType(workbook, sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
                fs.Close();
            }
            return dt;
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell">目标单元格</param>
        /// <returns></returns>
        private static object GetValueType(IWorkbook workbook, ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return null;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;
                case CellType.Formula:
                    //IFormulaEvaluator eva = null;
                    //ICell _cell = null;
                    //if(_cell is XSSFCell)
                    //{
                    //    eva = new XSSFFormulaEvaluator(workbook);
                    //    _cell = (XSSFCell)cell;
                    //}
                    //else
                    //{
                    //    eva = new HSSFFormulaEvaluator(workbook);
                    //    _cell = (HSSFCell)cell;
                    //}
                    //if (eva.Evaluate(_cell).CellType == CellType.Numeric)
                    //{
                    //    return eva.Evaluate(_cell).NumberValue;
                    //}
                    //else
                    //{
                    //    return eva.Evaluate(_cell).StringValue;
                    //}
                default:
                    return cell.StringCellValue;
            }
        }

    }
}
