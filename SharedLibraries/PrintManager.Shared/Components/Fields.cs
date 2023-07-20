using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared
{
    public class Fields
    {
        public const string AppName = "PrintManager";

        public const string AppDataPath = "%appdata%" + "\\" + AppName;

        public const string LogFileName = "log.txt";

        public const string PrintFolderName = "PrintTemplete";

        public const string ConfigFileName = "Config.ini";

        public const string ConfigTPFileName = "TimePrograms.ini";

        //网盘
        public const string CloudFileName = "CPQ customer code.xlsx";

        public const string CloudFilePath = "Z:\\SAP\\CPQ 客户代码维护";

    }
}
