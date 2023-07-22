using Caliburn.Micro;
using PrintManager.Shared;
using PrintManager.UI;
using PrintManager.MainClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using PrintManager.Sql.IBLL;
using PrintManager.Sql.BLL;
using PrintManager.Shared.Components;
using PrintManager.Shared.Utils;
using System.IO;
using PrintManager.MainClient.Properties;
using System.Windows.Media;
using System.Windows;
using System.Dynamic;
using System.Timers;
using PrintManager.Shared.Helpers;
using System.Globalization;

namespace PrintManager.MainClient.Components
{
    public class AppBootstrapper : BootstrapperBase
    {
        #region Fields
        private CompositionContainer _container;

        private IWindowManager _windowManager;

        private IEventAggregator _eventAggregator;

        private ILogger _logger;

        private IThemeManager _themeManager;

        //private IProductOrderBLL _productOrderBLL;
        #endregion

        #region Ctor
        public AppBootstrapper()
        {
            Initialize();
        }
        #endregion

        #region Overrides
        protected override void Configure()
        {
            var aggregateCatalog = new AggregateCatalog(
                              AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());

            var batch = new CompositionBatch();

            _container = new CompositionContainer(aggregateCatalog);

            batch.AddExportedValue(_container);

            //注入IoC
            _windowManager = new WindowManager();
            batch.AddExportedValue(_windowManager);
            _eventAggregator = new EventAggregator();
            batch.AddExportedValue(_eventAggregator);
            _logger = new Logger();
            batch.AddExportedValue(_logger);
            _themeManager = new ThemeManager();
            batch.AddExportedValue(_themeManager);
            //数据库
            //_productOrderBLL = new ProductOrderBLL();
            //batch.AddExportedValue(_productOrderBLL);


            _container.Compose(batch);


            CaliburnActionMessage.EnableNestedViewModelActionBinding();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
        }

        protected override object GetInstance(Type service, string key)
        {
            string contract = string.IsNullOrEmpty(key)
               ? AttributedModelServices.GetContractName(service)
               : key;

            var exports = _container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception($"找不到实例 {contract}。");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(service));
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetEntryAssembly(),
                Assembly.GetExecutingAssembly(),
            };

            return assemblies.Where(x => x != null)
                .Distinct();
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce((ComposablePart)instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            //初始化配置文件
            if (!File.Exists(Environments.ConfigFilePath))
            {
                File.Create(Environments.ConfigFilePath).Close();
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ConnectionString", "Server=localhost;Database=db_client;Trusted_Connection=True");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "LocalIPAdr", "127.0.0.1:5500");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "OPCAdr", "opc.tcp://192.168.0.1:4840");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ClientName", "PLC_1");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "AutoConnectDB", "TRUE");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "AutoConnectClient", "TRUE");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ServerAdr", "192.168.0.10:8080");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "RefreshTime", "1000");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "Language", "zh-CN");
            };

            //Loading System Language
            switch (IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "Language"))
            {
                case "zh-CN":
                    GlobalData.Language = "zh-CN";
                    break;
                case "en-US":
                    GlobalData.Language = "en-US";
                    break;
                default:
                    GlobalData.Language = "zh-CN";
                    break;
            }

            LanguageManager.Instance.ChangeLanguage(new CultureInfo(GlobalData.Language));

            var viewModel = new ShellViewModel();
            _windowManager.ShowWindow(viewModel);

            
        }
        #endregion
    }
}
