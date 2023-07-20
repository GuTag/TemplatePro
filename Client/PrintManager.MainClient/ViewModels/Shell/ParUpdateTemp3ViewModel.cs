using PrintManager.MainClient.Components;
using PrintManager.Shared.Utils;
using PrintManager.Shared;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintManager.MainClient.Models;
using PrintManager.UI.Controls;
using PrintManager.Sql.BLL;
using PrintManager.Sql.Models;
using System.Windows;
using Spire.Pdf.Exporting.XPS.Schema.Mc;
using Opc.Ua;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class ParUpdateTemp3ViewModel : ViewModelBase
    {
        public ParUpdateTemp3ViewModel()
        {           
        }

        #region 属性
        public string Index { get => _index; set => Set(ref _index, value); }
        private string _index;
        public string Language_zh { get => _language_zh; set => Set(ref _language_zh, value); }
        private string _language_zh;
        public string Language_cn { get => _language_cn; set => Set(ref _language_cn, value); }
        private string _language_cn;

        public bool IsUpdateItem { get => _isUpdateItem; set => Set(ref _isUpdateItem, value); }
        private bool _isUpdateItem;
        public bool IsAddeItem { get => _isAddItem; set => Set(ref _isAddItem, value); }
        private bool _isAddItem;

        #endregion

        #region 变量
        #endregion

        #region 方法
        private void WriteConfig()
        {
            LanguageText language = new LanguageText()
            {
                Index = Index,
                Language_cn = Language_cn,
                Language_zh = Language_zh,
            };
            
            LanguageTextBLL.UpdateOfIndex(language);
        }


        public void ShowParView(LanguageModel language, bool UpdateItem)
        {
            if (language != null)
            {
                Index = language.Index;
                Language_zh = language.Language_zh;
                Language_cn = language.Language_cn;
            }
            IsUpdateItem = UpdateItem;
            IsAddeItem = !UpdateItem;
        }
        #endregion

        #region 事件
        #endregion

        #region 命令

        public void onUpdateCommand()
        {
            WriteConfig();
            TryClose() ;
        }

        public void onAddCommand()
        {

            var isExit = LanguageTextBLL.Add(new LanguageText()
            {
                Index = Index,
                Language_cn = Language_cn,
                Language_zh = Language_zh,
            });

            if (isExit)
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, "已存在相同地址:" + Index);
            }
            else
            {
                TryClose();
                //productViewModel.UpdateProductOrderList();
            }
        }
        public void onCancelCommand()
        {
            TryClose();
        }
        #endregion

        #region 重写方法
        public override void CanClose(Action<bool> callback)
        {

            base.CanClose(callback);
            #endregion
        }
    }
}