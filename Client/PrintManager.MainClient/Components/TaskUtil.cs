using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using PrintManager.MainClient.Models;
using PrintManager.Shared.Utils;
using System.Data;
using Newtonsoft.Json;
using PrintManager.Shared.Helpers;
using PrintManager.Sql.BLL;
using PrintManager.Sql.Models;
using OxyPlot;
using System.Security.Cryptography;
using PrintManager.Shared.Enums;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Reflection;
using PrintManager.Shared;
using Microsoft.Office.Interop.Excel;
using NPOI.OpenXmlFormats.Spreadsheet;
using Spire.Pdf;
using System.Windows.Shell;
using Spire.Pdf.Texts;

namespace PrintManager.MainClient.Components
{
    /// <summary>
    /// 任务
    /// </summary>
    public class TaskUtil
    {
        #region 弃用方法
        /// <summary>
        /// list导出为Excel
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filePath"></param>
        public static void DataExport<T>(List<T> list, string filePath)
        {
            if (list.Count > 0)
            {


                //用于创建文件
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    //用于写入内容
                    using (StreamWriter streamWriter = new StreamWriter(new BufferedStream(fileStream), Encoding.Default))
                    {
                        //表头
                        string tbhead = "";
                        var entity = list[0].GetType();
                        PropertyInfo[] piList = entity.GetProperties();
                        foreach (PropertyInfo pi in piList)
                        {
                            tbhead += pi.Name + "\t";
                        }
                        //取完表头，换行
                        tbhead = tbhead.Substring(0, tbhead.Length - 1) + "\n";
                        //表头写入
                        streamWriter.Write(tbhead);
                        foreach (var v in list)
                        {
                            string tbbody = "导入数据";
                            StringBuilder tbBody = new StringBuilder();
                            for (int i = 0; i < piList.Length; i++)
                            {

                                object value = piList[i].GetMethod.Invoke(v, null);
                                tbBody.Append(value);//内容
                                tbBody.Append("\t");//自动跳到下一单元格

                            }
                            tbbody = tbBody.ToString();
                            tbbody = tbbody.Substring(0, tbbody.Length - 1) + "\n";//取完一行内容，换行
                            streamWriter.Write(tbbody);//内容写入
                        }
                    }
                }
            }
        }

        #endregion

        

        public static string GetItemNodeType(int index)
        {
            string result = string.Empty;

            switch (index)
            {
                case 0:
                    result = "T";
                    break;
                case 1:
                    result = "F";
                    break;
                case 2:
                    result = "W";
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 产品DB 转Excel 
        /// </summary>
        /// <param name="range"></param>
        public static void ExportDBToExcel(string range)
        {
            List<ProductOrderLog> items = new List<ProductOrderLog>();
            switch (range)
            {
                case "0":
                    items = ProductOrderLogBLL.GetToDayAll();
                    break;
                case "1":
                    items= ProductOrderLogBLL.GetToLastAndDayAll();
                    break;
                case "2":
                    items=ProductOrderLogBLL.GetToLastMonthAll();
                    break;
                case "3":
                    items=ProductOrderLogBLL.GetToLastWeekAll();
                    break;
                case "4":
                    items= ProductOrderLogBLL.GetToAll();
                    break;
                default:
                    break;
            }

            string headerfile = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Chart", "ChartExcelSavePath");
            string lastfile = DateTime.Now.Date.ToString("yyyyMMdd");
            string filename = Path.Combine(headerfile, $"F89_{lastfile}.xlsx");

            if (!File.Exists(filename))
            {
                //DataExport(items, filename);
                GenerateAttachmentDataExport(items, filename);
            }
            else
            {
                System.IO.File.Delete(filename);
                //DataExport(items, filename);
                GenerateAttachmentDataExport(items, filename);
            }

        }

        /// <summary>
        /// 生成附件（使用Microsoft.Office.Interop.Excel组件的方式）
        /// </summary>
        /// <param name="DT"></param>
        /// <returns></returns>
        public static void GenerateAttachmentDataExport(List<ProductOrderLog> DT,string savePath)
        {
            //需要添加 Microsoft.Office.Interop.Excel引用 
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            if (app == null)//服务器上缺少Excel组件，需要安装Office软件
            {
                return;
            }
            app.Visible = false;
            app.UserControl = true;
            //string strTempPath = "C:\\Users\\F89.isvprod\\AppData\\Roaming\\PrintManager\\100.xlsx";//模版excel
            Microsoft.Office.Interop.Excel.Workbooks workbooks = app.Workbooks;
            Microsoft.Office.Interop.Excel._Workbook workbook = workbooks.Add(true); //加载模板               
            Microsoft.Office.Interop.Excel.Sheets sheets = workbook.Sheets;
            Microsoft.Office.Interop.Excel._Worksheet worksheet = (Microsoft.Office.Interop.Excel._Worksheet)sheets.get_Item(1); //第一个工作薄。
            if (worksheet == null)//工作薄中没有工作表
            {
                return;
            }

            //1、获取数据
            if (DT.Count < 1)//没有取到数据
            {
                return;
            }

            ////选择数据所在区域
            //Microsoft.Office.Interop.Excel.Range excelRange = worksheet.get_Range(worksheet.Cells[1,1], worksheet.Cells[9,9]);
            ////设置宽度
            //excelRange.ColumnWidth = 30;
            ////加边框
            //excelRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThick,XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.Black.ToArgb());
            //excelRange.Select();
            //Pictures pics = excelRange.CopyPicture("C:\\Users\\Administrator\\Desktop\\Images\\2022071309223596435.png")

            //写入Title
            string title = "F89 生产数据";
            RangeBuild(worksheet, "A1", "H1", title);

            //写入Header
            worksheet.Cells[2, 1] = "时间";
            worksheet.Cells[2, 2] = "Mo";
            worksheet.Cells[2, 3] = "生产线";
            worksheet.Cells[2, 4] = "产品类型";
            worksheet.Cells[2, 5] = "物料描述";
            worksheet.Cells[2, 6] = "ItemNo";
            worksheet.Cells[2, 7] = "需求数量";
            worksheet.Cells[2, 8] = "完成数量";

            var listLine1 = DT.FindAll(o => "一线".Equals(o.Line)).ToList();
            var listLine2 = DT.FindAll(o => "二线".Equals(o.Line)).ToList();
            var listLine3 = DT.FindAll(o => "三线".Equals(o.Line)).ToList();

            int rowIndex = 3;  //Excel模板上表头占了1行 
            //写入Body
            for (int i = 0; i < listLine1.Count; i++)
            {
                    
                worksheet.Cells[rowIndex, 1] = listLine1[i].AddTime.ToString();
                worksheet.Cells[rowIndex, 2] = listLine1[i].MO.ToString();
                worksheet.Cells[rowIndex, 3] = listLine1[i].Line.ToString();
                worksheet.Cells[rowIndex, 4] = listLine1[i].ProdType.ToString();
                worksheet.Cells[rowIndex, 5] = listLine1[i].Desc.ToString();
                worksheet.Cells[rowIndex, 6] = listLine1[i].ItemNo.ToString();
                worksheet.Cells[rowIndex, 7] = listLine1[i].RequestNum.ToString();
                worksheet.Cells[rowIndex, 8] = listLine1[i].ComplatedNum.ToString();
                rowIndex++;
            }
            RangeBuildText(worksheet, $"A{rowIndex}", $"F{rowIndex}", "一线总计");
            worksheet.Cells[rowIndex, 7] = listLine1.Select(it => it.RequestNum).Sum();
            worksheet.Cells[rowIndex, 8] = listLine1.Select(it => it.ComplatedNum).Sum();
            rowIndex++;
            //二线
            for (int i = 0; i < listLine2.Count; i++)
            {

                worksheet.Cells[rowIndex, 1] = listLine2[i].AddTime.ToString();
                worksheet.Cells[rowIndex, 2] = listLine2[i].MO.ToString();
                worksheet.Cells[rowIndex, 3] = listLine2[i].Line.ToString();
                worksheet.Cells[rowIndex, 4] = listLine2[i].ProdType.ToString();
                worksheet.Cells[rowIndex, 5] = listLine2[i].Desc.ToString();
                worksheet.Cells[rowIndex, 6] = listLine2[i].ItemNo.ToString();
                worksheet.Cells[rowIndex, 7] = listLine2[i].RequestNum.ToString();
                worksheet.Cells[rowIndex, 8] = listLine2[i].ComplatedNum.ToString();
                rowIndex++;
            }
            RangeBuildText(worksheet, $"A{rowIndex}", $"F{rowIndex}", "二线总计");
            worksheet.Cells[rowIndex, 7] = listLine2.Select(it => it.RequestNum).Sum();
            worksheet.Cells[rowIndex, 8] = listLine2.Select(it => it.ComplatedNum).Sum();
            rowIndex++;
            //三线
            for (int i = 0; i < listLine3.Count; i++)
            {

                worksheet.Cells[rowIndex, 1] = listLine3[i].AddTime.ToString();
                worksheet.Cells[rowIndex, 2] = listLine3[i].MO.ToString();
                worksheet.Cells[rowIndex, 3] = listLine3[i].Line.ToString();
                worksheet.Cells[rowIndex, 4] = listLine3[i].ProdType.ToString();
                worksheet.Cells[rowIndex, 5] = listLine3[i].Desc.ToString();
                worksheet.Cells[rowIndex, 6] = listLine3[i].ItemNo.ToString();
                worksheet.Cells[rowIndex, 7] = listLine3[i].RequestNum.ToString();
                worksheet.Cells[rowIndex, 8] = listLine3[i].ComplatedNum.ToString();
                rowIndex++;
            }
            RangeBuildText(worksheet, $"A{rowIndex}", $"F{rowIndex}", "三线总计");
            worksheet.Cells[rowIndex, 7] = listLine3.Select(it => it.RequestNum).Sum();
            worksheet.Cells[rowIndex, 8] = listLine3.Select(it => it.ComplatedNum).Sum();
            rowIndex++;


            //for (int i = 1; i <= rowCount; i++)
            //{
            //    int row_ = 2 + i;  //Excel模板上表头占了1行 
            //    //foreach (var item in DT)
            //    //{
            //        worksheet.Cells[row_, 1] = DT[i].AddTime.ToString();
            //        worksheet.Cells[row_, 2] = DT[i].MO.ToString();
            //        worksheet.Cells[row_, 3] = DT[i].Line.ToString();
            //        worksheet.Cells[row_, 4] = DT[i].ProdType.ToString();
            //        worksheet.Cells[row_, 5] = DT[i].Desc.ToString();
            //        worksheet.Cells[row_, 6] = DT[i].ItemNo.ToString();
            //        worksheet.Cells[row_, 7] = DT[i].RequestNum.ToString();
            //        worksheet.Cells[row_, 8] = DT[i].ComplatedNum.ToString();
            //    //}
            //}

            worksheet.Columns.AutoFit(); //自动调整列宽
            //3、保存生成的Excel文件
            //Missing在System.Reflection命名空间下
            //string savePath = "C:\\Users\\F89.isvprod\\AppData\\Roaming\\PrintManager\\200.xlsx";
            workbook.SaveAs(savePath, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            //4、按顺序释放资源
            NAR(worksheet);
            NAR(sheets);
            NAR(workbook);
            NAR(workbooks);
            app.Quit();
            NAR(app);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="o"></param>
        public static void NAR(object o)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(o);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                o = null;
            }
        }


        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="oSheet"></param>
        /// <param name="startcell"></param>
        /// <param name="endcell"></param>
        /// <param name="value"></param>
        private static void RangeBuildText(_Worksheet oSheet, string startcell, string endcell, string value)
        {
            ///创建一个区域对象。第一个参数是开始格子号，第二个参数是终止格子号。比如选中A1——D3这个区域。
            Range range = (Range)oSheet.get_Range(startcell, endcell);
            ///合并方法，0的时候直接合并为一个单元格
            range.Merge(0);
            ///合并单元格之后，设置其中的文本
            range.Value = value;
            //横向居中
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft ;
            range.VerticalAlignment = XlVAlign.xlVAlignBottom;
            ///字体大小
            range.Font.Size = 11;
            ///字体
            //range.Font.Name = "黑体";
            ///行高
            range.RowHeight = 15;
            //自动调整列宽
            range.EntireColumn.AutoFit();
            //填充颜色
            //range.Interior.ColorIndex = 20;
            //设置单元格边框的粗细
            //range.Cells.Borders.LineStyle = 1;
        }

        private static void RangeBuild(_Worksheet oSheet, string startcell, string endcell, string value)
        {
            ///创建一个区域对象。第一个参数是开始格子号，第二个参数是终止格子号。比如选中A1——D3这个区域。
            Range range = (Range)oSheet.get_Range(startcell, endcell);
            ///合并方法，0的时候直接合并为一个单元格
            range.Merge(0);
            ///合并单元格之后，设置其中的文本
            range.Value = value;
            //横向居中
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            ///字体大小
            range.Font.Size = 18;
            ///字体
            //range.Font.Name = "黑体";
            ///行高
            range.RowHeight = 24;
            //自动调整列宽
            range.EntireColumn.AutoFit();
            //填充颜色
            //range.Interior.ColorIndex = 20;
            //设置单元格边框的粗细
            //range.Cells.Borders.LineStyle = 1;
        }


        /// <summary>
        /// 文本转Excel
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        public static void TxtToExcel(string sourceFile, string targetFile)
        {
            FileStream fileStream = new FileStream(sourceFile, FileMode.Open);
            StreamReader sr = new StreamReader(fileStream, System.Text.Encoding.GetEncoding("GB2312"));
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet1");
            string line;
            int index = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] lines = line.Split('\t');
                var dataRow = sheet.CreateRow(index);
                for (int j = 0; j < lines.Length; j++)
                {
                    var cell = dataRow.CreateCell(j);
                    cell.SetCellValue(lines[j].Trim());
                }
                index++;
            }
            sr.Close();
            fileStream.Close();
            using (FileStream fs = File.OpenWrite(targetFile)) //打开一个xls文件，如果没有则自行创建，如果存在myxls.xls文件则在创建是不要打开该文件！
            {
                workbook.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
            }
            workbook.Close();
        }

        /// <summary>
        /// 解析公盘Excel数据
        /// </summary>
        /// <param name="filepath"></param>
        public static void RequestExcelData(string filepath) 
        {
            System.Data.DataTable data = ExcelUtil.ReadExcelToTable(filepath);
            List<ProductOrder> orderList = new List<ProductOrder>();
            if (data != null && data.Rows.Count > 0)
            {
                orderList.Clear();
                foreach (DataRow row in data.Rows)
                {
                    var model = RowToOrder(row);
                    orderList.Add(model);
                }
                ProductOrderBLL.AddList(orderList);
            }
        }

        public static void RequestExcelLanguageTextData(string filepath)
        {
            System.Data.DataTable data = ExcelUtil.ReadExcelToTable(filepath);
            List<LanguageText> languageTexts = new List<LanguageText>();
            if (data != null && data.Rows.Count > 0)
            {
                languageTexts.Clear();
                foreach (DataRow row in data.Rows)
                {
                    var model = RowToOrderLanguage(row);
                    languageTexts.Add(model);
                }
                LanguageTextBLL.AddList(languageTexts);
            }

        }

        public static void RequestExcelAnalogTextData(string filepath)
        {
            System.Data.DataTable data = ExcelUtil.ReadExcelToTable(filepath);
            List<Analog> languageTexts = new List<Analog>();
            if (data != null && data.Rows.Count > 0)
            {
                languageTexts.Clear();
                foreach (DataRow row in data.Rows)
                {
                    var model = RowToOrderAnalog(row);
                    languageTexts.Add(model);
                }
                AnalogBLL.AddList(languageTexts);
            }

        }

        public static Analog RowToOrderAnalog(DataRow row)
        {
            var model = new Analog();

            model.ClientName = row["ClientName"].ToString();
            model.NodeType = row["NodeType"].ToString();
            model.NodeTypeView = row["NodeTypeView"].ToString();
            model.NodeAdr = row["NodeAdr"].ToString();
            model.NodeValHH =  int.Parse(row["NodeValHH"].ToString());
            model.NodeValH = int.Parse(row["NodeValH"].ToString());
            model.NodeValLL = int.Parse(row["NodeValLL"].ToString());
            model.NodeValL = int.Parse(row["NodeValL"].ToString());
            model.NodeLanguageHH = row["NodeLanguageHH"].ToString();
            model.NodeLanguageH = row["NodeLanguageH"].ToString();
            model.NodeLanguageLL = row["NodeLanguageLL"].ToString();
            model.NodeLanguageL = row["NodeLanguageL"].ToString();
            model.NodeUnit = row["NodeUnit"].ToString();
            model.NodeDes = row["NodeDes"].ToString();
            model.AddTime = DateTime.Now;
            return model;
        }
        public static LanguageText RowToOrderLanguage(DataRow row)
        {
            var model = new LanguageText();

            model.Index = row["Language_Index"].ToString();
            model.Language_zh = row["Language_zh"].ToString();
            model.Language_cn = row["Language_cn"].ToString();
            model.AddTime = DateTime.Now;
            return model;
        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="row"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static ProductOrder RowToOrder(DataRow row)
        {
            var model = new ProductOrder();

            model.ClientName = row["ClientName"].ToString();
            model.NodeType = row["NodeType"].ToString();
            model.NodeTypeView = row["NodeTypeView"].ToString();
            model.NodeIndexLang = row["NodeIndexLang"].ToString();
            model.NodeAdr = row["NodeAdr"].ToString();
            model.NodeDes = row["NodeDes"].ToString();
            model.AddTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            return model;
        }




        /// <summary>
        /// 解析公盘CPQ数据
        /// </summary>
        /// <param name="filepath"></param>
        public static void RequestExcelCPQ(string filepath)
        {
            System.Data.DataTable data = ExcelUtil.ReadExcelToTable(filepath);
            List<string> codes = new List<string>();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow row in data.Rows)
                {

                    string code = row["Customer Code"].ToString();
                    if (!string.IsNullOrEmpty(code))
                    {
                        codes.Add(code);
                    }
                }
                CustomerBLL.AddList(codes);
            }
        }

        public static void RequestPDFData(string filepath)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(filepath);

            List<string> moList = new List<string>();
            List<string> numList = new List<string>();
            List<string> soList = new List<string>();
            //遍历文档所有PDF页面，提取文本
            foreach (PdfPageBase page in doc.Pages)
            {
                var pdfText = new PdfTextExtractor(page);
                var context = pdfText.ExtractText(new PdfTextExtractOptions() { IsExtractAllText = true });
                //var lines = page.ExtractText().Split('\n');
                var lines = context.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains("MO生产单号"))
                    {
                        moList.Add(line.Split('：').LastOrDefault().Trim());
                    }
                    if (line.Contains("生产订单数量"))
                    {
                        numList.Add(line.Split('：').LastOrDefault().Trim().Split('P').FirstOrDefault().Trim());
                    }
                    if (line.Contains("SO剩余数量"))
                    {
                        soList.Add(line.Split('S').FirstOrDefault().Trim());
                    }
                }
            }
            if(moList.Count != soList.Count || moList.Count != numList.Count) 
            {
                throw new Exception("解析PDF数据错误，订单数量不匹配");
            }
            for (int i = 0; i < moList.Count; i++)
            {
                var items = ProductOrderBLL.GetItemOfSO(soList[i]);
                
                if (items != null && items.Count > 0)
                {
                    GlobalData.Instance.MOList.Add(moList[i]);
                    bool isExist = false;
                    foreach (var item in items)
                    {
                        if (item.MO.Equals(moList[i]))
                        {
                            isExist = true;
                        }
                    }
                    if(!isExist)
                    {
                        var newItem = new ProductOrder()
                        {
                            MO= moList[i],
                            SOItem= soList[i],
                            RequestNum = Convert.ToInt32(numList[i]),
                            ComplatedNum = 0,
                            IsOK = false,
                            ItemNo = items[0].ItemNo,
                            Desc = items[0].Desc,
                            MtlNo= items[0].MtlNo,
                            NewItemNO= items[0].NewItemNO,
                            CustomerCode= items[0].CustomerCode,
                            CPQCode= items[0].CPQCode,
                            ProductOrderType= items[0].ProductOrderType,
                        };
                        //System.Diagnostics.Debug.WriteLine($"{newItem.MO},{newItem.SOItem}");
                        ProductOrderBLL.Add(newItem);
                    }
                }
            }
        }
    }
}
