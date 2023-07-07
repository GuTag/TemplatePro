using PrintManager.MainClient.Components;
using PrintManager.MainClient.Models;
using PrintManager.Shared;
using PrintManager.UI;
using PrintManager.UI.Controls;
using PrintManager.UI.Helpers;
using System;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class PrintViewModel : ViewModelBase
    {
        public PrintViewModel()
        {
        }

        #region 变量


        #endregion

        #region 数据
        private string filePath { get; set; }

        public PrintCanvas PrintCanvas { get => _printCanvas; set => Set(ref _printCanvas, value); }
        private PrintCanvas _printCanvas;
        #endregion


        #region 方法

        public void ShowPrintView(string filepath, ProductOrderModel orderModel)
        {
            this.filePath = filepath;
            PrintCanvas = new PrintCanvas(filepath, orderModel?.ItemNo, orderModel?.MO, orderModel.ComplatedNum, orderModel.SOItem, orderModel.MtlNo, orderModel.ProductOrderType, orderModel.Desc, orderModel.NewItemNo);
        }

        public void updatePrintView(string filepath)
        {
            this.filePath = filepath;
            updatePrintCanvas();
            ;
        }
        private void updatePrintCanvas()
        {
            try
            {
                if (filePath == null && PrintCanvas?.Filepath != null)
                {
                    PrintCanvas.UpdateCanvasView();
                }
                else
                {
                    PrintCanvas = new PrintCanvas(filePath);
                }
            }
            catch (Exception e)
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, $"模板文件错误：{e}");
            }
            

        }
        #endregion

        #region 事件与命令
        public void onEditCommand()
        {
            try
            {
                if(string.IsNullOrEmpty(filePath)) { return; }
                var viewModel = new LabelEditViewModel(filePath);
                dynamic settings = new ExpandoObject();
                settings.Title = "编辑器";
                settings.Height = 850;
                settings.Width = 1400;
                settings.SizeToContent = SizeToContent.Manual;
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                WindowManager.ShowDialog(viewModel, null, settings);
            }
            catch (Exception e)
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, $"编辑器错误：{e}");
            }
            updatePrintCanvas();
        }
        public void onRefreshCommand()
        {
            updatePrintCanvas();
        }

        public void CreateImageCommand()
        {
            //生成canvas image，打印完成后删除
            var iamgepath = Path.Combine(Environments.AppDataPath, Path.GetFileName(filePath)).Replace("pm", "png");

            using (Stream stream = File.Create(iamgepath))
            {
                var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(CanvasHelpers.ConvertToBitmapSource(PrintCanvas)));
                encoder.Save(stream);
            }
            //Bitmap source = new Bitmap(iamgepath);
            //Bitmap img = new Bitmap(source, 1080, 600);
            //source.Dispose();
            //img.Save(Path.Combine(Environments.AppDataPath, "aa.png"));
            //File.Delete(iamgepath);

        }
        #endregion
    }
}
