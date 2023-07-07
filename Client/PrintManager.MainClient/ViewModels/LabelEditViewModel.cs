using Caliburn.Micro;
using Microsoft.Win32;
using PrintManager.MainClient.Components;
using PrintManager.Shared.Entity;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Helpers;
using PrintManager.Shared.Utils;
using PrintManager.UI;
using PrintManager.UI.Controls;
using PrintManager.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static PrintManager.Shared.Entity.OrderEntity;

namespace PrintManager.MainClient.ViewModels
{
    public class LabelEditViewModel:ViewModelBase
    {
        #region 变量
        private Undoable undoable = new Undoable();
        private PrintTemplate PrindDataLog;
        #endregion
        public LabelEditViewModel()
        {
            PrintData = new PrintTemplate();
            PrintData.Width = 90;
            PrintData.Height = 50;
            PrintData.Background = "#FFFFFF";

            Initialzation();
        }

        public LabelEditViewModel(string filepath)
        {
            PrintData = OrderHelpers.GetPrintTemplate(filepath);
            PrindDataLog = GetNewPrintTemplate(PrintData);
            Initialzation();
        }

        private void Initialzation()
        {
            InstalledFontCollection fonts= new InstalledFontCollection();
            foreach (var font in fonts.Families) 
            {
                FontFamilyList.Add(font.Name);
            }
        }

        #region 属性
        public PrintTemplate PrintData { get => _printData; set => Set(ref _printData, value); }
        private PrintTemplate _printData;

        public double ZoomPercent { get => _zoomPercent; set => Set(ref _zoomPercent, value); }
        private double _zoomPercent = 3.5;

        public List<string> FontFamilyList { get => _fontFamilyList; set => Set(ref _fontFamilyList, value); }
        private List<string> _fontFamilyList = new List<string>();

        public List<string> FontWeightList { get => _fontWeightList; set => Set(ref _fontWeightList, value); }
        private List<string> _fontWeightList = new List<string>() { "Bold", "Regular" };

        public ControlItem SelecteControl { get => _selecteControl;  
            set  
            {  
                if(_selecteControl != null && value == null)
                {
                    PrindDataLog = GetNewPrintTemplate(PrintData);
                }
                if(_selecteControl == null && value != null)
                {
                    UndoableLog();
                }
                Set(ref _selecteControl, value); 
            }  
        }
        private ControlItem _selecteControl;

        public List<string> PrintTypeList { get => _printTypeList; set => Set(ref _printTypeList, value); }
        private List<string> _printTypeList = new List<string>() { "F89", "RPX","Bundle", "F89RPX", "其他" };


        public bool IsUndoableEnable { get => _isUndoableEnable; set { Set(ref _isUndoableEnable, value); } }
        private bool _isUndoableEnable = false;
        public bool IsRedoableEnable { get => _isRedoableEnable; set { Set(ref _isRedoableEnable, value); } }
        private bool _isRedoableEnable = false;
        #endregion

        #region 方法
        private void UpdateCanvasView()
        {
            UndoableLog();
            PrintData.UpdateView();
        }

        private void UndoableLog()
        {
            
            var OldPrindData = PrindDataLog;
            var newPrindData = GetNewPrintTemplate(PrintData);
            undoable.Add(() =>
            {
                //撤销
                PrintData = OldPrindData;
            }, () =>
            {
                //重做
                PrintData = newPrindData;
            });
            IsUndoableEnable = undoable.IsUndoEnable;
            IsRedoableEnable = undoable.IsRedoEnable;
        }

        #endregion
        private PrintTemplate GetNewPrintTemplate(PrintTemplate template) 
        {
            if(template == null) return new PrintTemplate();
            PrintTemplate newTemplate = new PrintTemplate()
            {
                Name= template.Name,
                Width= template.Width,
                Height= template.Height,
                Background= template.Background,
                DPI= template.DPI,
                FilePath= template.FilePath,
                Num= template.Num,
                Type= template.Type,
                UpdateViewEvent= template.UpdateViewEvent,
            };
            newTemplate.ControlItems = new List<ControlItem>();
            foreach(var control in template.ControlItems)
            {
                newTemplate.ControlItems.Add(new ControlItem()
                {
                    ControlType = control.ControlType,
                    DisplayName = control.DisplayName,
                    IsAssociation= control.IsAssociation,
                    FontFamily=control.FontFamily,
                    FontSize=control.FontSize,
                    FontStyle=control.FontStyle,
                    FontWeight=control.FontWeight,
                    Height= control.Height,
                    Width= control.Width,
                    Image= control.Image,
                    IsSelected= control.IsSelected,
                    MousePoint= control.MousePoint,
                    PosX= control.PosX,
                    PosY= control.PosY,
                    VarName= control.VarName,
                    ID = control.ID,
                    ImageData= control.ImageData,
                    WidthFactor= control.WidthFactor,
                    Spacing= control.Spacing,
                });
            }
            return newTemplate;
        }

        #region 方法
        

        #endregion

        #region 事件

        #endregion

        #region 命令
        public void NewTemplateCommand()
        {
            if(PrintData != null && PrintData.ControlItems.Count > 0)
            {
                if(WindowManagerExtension.ShowAckDialog(WindowManager, "保存", "是否保存当前模板？") == true)
                {
                    SaveCommand();
                }
            }
            PrintData = new PrintTemplate()
            {
                Width = 90,
                Height = 50,
                Background = "#FFFFFF",
            };
        }
        public void SaveCommand()
        {
            if (string.IsNullOrEmpty(PrintData.FilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "保存模板文件",
                    Filter = "PM文件|*.pm;",
                };
                if(saveFileDialog.ShowDialog() == true)
                {
                    PrintData.FilePath = saveFileDialog.FileName;
                    OrderHelpers.SavePrintTemplate(PrintData);
                }
            }
            else
            {
                try
                {
                    PrindDataLog = GetNewPrintTemplate(PrintData);
                    OrderHelpers.SavePrintTemplate(PrintData);
                    undoable.Clean();
                    IsUndoableEnable = undoable.IsUndoEnable;
                    IsRedoableEnable = undoable.IsRedoEnable;
                }
                catch (Exception e)
                {
                    WindowManagerExtension.ShowMessageDialog(WindowManager, e);
                }
            }
        }

        public void SaveOtherCommand()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "保存模板文件",
                Filter = "PM文件|*.pm;",
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                PrintData.FilePath = saveFileDialog.FileName;
                OrderHelpers.SavePrintTemplate(PrintData);
            }
        }
        public void ImportTemplateCommand()
        {
            if (undoable.IsUndoEnable)
            {
                if (WindowManagerExtension.ShowAckDialog(WindowManager, "保存", "是否保存当前模板？") == true)
                {
                    SaveCommand();
                }
            }
            //打开文件选择
            var dialog = new OpenFileDialog
            {
                Title = "选择模板文件",
                Filter = "PrintManager文件|*.pm;",
            };
            if ((bool)dialog.ShowDialog())
            {
                try
                {
                    SelecteControl = null;
                    PrintData = OrderHelpers.GetPrintTemplate(dialog.FileName);
                }
                catch (Exception e)
                {
                    PrintData.FilePath = null;
                    WindowManagerExtension.ShowMessageDialog(WindowManager, e);
                }
            }

        }

        public void RefreshViewCommand()
        {
            UpdateCanvasView();
        }

        /// <summary>
        /// 控件调整
        /// </summary>
        /// <param name="type"></param>
        public void ContorlOperateCommand(int type)
        {
            if(type > 0 && type <99)
            {
                ControlItem firstControl = null;
                PrindDataLog = GetNewPrintTemplate(PrintData);
                foreach (var control in PrintData.ControlItems)
                {
                    if (control.IsSelected)
                    {
                        if(firstControl == null) 
                        { 
                            if(SelecteControl != null)
                            {
                                firstControl = SelecteControl;
                            }
                            else
                            {
                                firstControl = control;
                            }
                        }

                        if(firstControl != null)
                        {
                            switch (type)
                            {
                                case 1://上对齐
                                    control.PosY = firstControl.PosY;break;
                                case 2://下对齐
                                    control.PosY = firstControl.PosY + firstControl.Height - control.Height; break;
                                case 3://左对齐
                                    control.PosX = firstControl.PosX; break;
                                case 4://右对齐
                                    control.PosX = firstControl.PosX + firstControl.Width - control.Width; break;
                                case 5://垂直居中对齐
                                    control.PosX = firstControl.PosX + firstControl.Width/2 - control.Width/2; break;
                                case 6://水平居中对齐
                                    control.PosY = firstControl.PosY + firstControl.Height/2 - control.Height/2; break;
                            }
                        }
                    }
                }
                UpdateCanvasView();
            }
            else
            {
                IsUndoableEnable = undoable.IsUndoEnable;
                IsRedoableEnable = undoable.IsRedoEnable;
                switch (type)
                {
                    case 99:
                        undoable.Undo();
                        break;
                    case 101:
                        undoable.Redo();
                        break;
                }
            }
        }

        /// <summary>
        /// 放大缩小
        /// </summary>
        /// <param name="target">“+” || “-”</param>
        public void ScaleCommand(string target)
        {
            switch (target)
            {
                case "+":
                    ZoomPercent += 0.05;
                    if (ZoomPercent > 10) ZoomPercent = 10;
                    break;
                case "-":
                    ZoomPercent -= 0.05;
                    if (ZoomPercent < 0.5) ZoomPercent = 0.5;
                    break; 
                default:
                    break;
            }
        }

        /// <summary>
        /// 增加控件
        /// </summary>
        /// <param name="type"></param>
        public void AddControlCommand(string type)
        {
            try
            {
                PrindDataLog = GetNewPrintTemplate(PrintData);
                //获取枚举值
                ControlType controlType;
                Enum.TryParse(type, out controlType);
                switch (controlType)
                {
                    case ControlType.Text:
                        PrintData.ControlItems.Add(new ControlItem() 
                        { 
                            ControlType= controlType ,
                            DisplayName= "Text",
                        });
                        UpdateCanvasView();
                        break;
                    case ControlType.Image:
                        //打开文件选择
                        var dialog = new OpenFileDialog
                        {
                            Title = "选择图片文件",
                            Filter = "Ini文件|*.png;*.jpg;*.jpeg",
                        };
                        if ((bool)dialog.ShowDialog())
                        {
                            PrintData.ControlItems.Add(new ControlItem()
                            {
                                ControlType = controlType,
                                DisplayName = "Image",
                                Image = dialog.FileName,
                                ImageData = ImageHelpers.ImageConvertString(dialog.FileName)
                            });
                            UpdateCanvasView();
                        }
                        break;
                    case ControlType.BarCode:
                        PrintData.ControlItems.Add(new ControlItem()
                        {
                            ControlType = controlType,
                            DisplayName = "BarCode",
                            Width = 40,
                            Height = 15,
                        });
                        UpdateCanvasView();
                        break;
                    case ControlType.QRCode:
                        PrintData.ControlItems.Add(new ControlItem()
                        {
                            ControlType = controlType,
                            DisplayName = "QRCode",
                            Width = 20,
                            Height= 20,
                        });
                        UpdateCanvasView();
                        break;
                    case ControlType.Line:
                        PrintData.ControlItems.Add(new ControlItem()
                        {
                            ControlType = controlType,
                            DisplayName = "Line",
                        });
                        UpdateCanvasView();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, e);
            }
        }

        /// <summary>
        /// 属性修改按钮
        /// </summary>
        public void SureModifyControl()
        {
            UpdateCanvasView();
        }

        /// <summary>
        /// 修改属性时，回车更新画面
        /// </summary>
        /// <param name="context"></param>
        public void onInputTextSearch(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                UpdateCanvasView();
            }
        }

        #endregion

        #region 重写方法
        public override void CanClose(Action<bool> callback)
        {
            if (undoable.IsUndoEnable)
            {
                if (WindowManagerExtension.ShowAckDialog(WindowManager, "关闭编辑器", "是否保存？") == true)
                {
                    try
                    {
                        SaveCommand();
                        base.CanClose(callback);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    base.CanClose(callback);
                }
            }
            else
            {
                base.CanClose(callback);
            }
        }
        #endregion
    }


}
