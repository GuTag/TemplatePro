using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Utils
{
    public class FileUtil
    {
        /// <summary>
        /// 拷贝文件到另一个文件夹下
        /// </summary>
        /// <param name="sourceName">源文件路径</param>
        /// <param name="folderPath">目标路径（目标文件夹）</param>
        public static bool CopyToFile(string sourceName, string folderPath)
        {
            //例子：
            //源文件路径
            //string sourceName = @"D:\Source\Test.txt";
            //目标路径:项目下的NewTest文件夹,(如果没有就创建该文件夹)
            //string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NewTest");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (!File.Exists(sourceName))
            {
                return false;
            }

            try
            {
                //当前文件如果不用新的文件名，那么就用原文件文件名
                string fileName = Path.GetFileName(sourceName);
                //这里可以给文件换个新名字，如下：
                //string fileName = string.Format("{0}.{1}", "newFileText", "txt");

                //目标整体路径
                string targetPath = Path.Combine(folderPath, fileName);

                //Copy到新文件下
                FileInfo file = new FileInfo(sourceName);
                if (file.Exists)
                {
                    //true 为覆盖已存在的同名文件，false 为不覆盖
                    file.CopyTo(targetPath, true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 复制文件夹以及其内所有内容
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="aimPath"></param>
        public static bool CopyDir(string srcPath, string targetPath)
        {
            // 检查目标目录是否以目录分割字符结束如果不是则添加
            if (targetPath[targetPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                targetPath += System.IO.Path.DirectorySeparatorChar;
            }
            // 判断目标目录是否存在如果不存在则新建
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
            // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
            // string[] fileList = Directory.GetFiles（srcPath）；
            string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
            // 遍历所有的文件和目录
            foreach (string file in fileList)
            {
                // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                if (System.IO.Directory.Exists(file))
                {
                    CopyDir(file, targetPath + System.IO.Path.GetFileName(file));
                }
                // 否则直接Copy文件
                else
                {
                    System.IO.File.Copy(file, targetPath + System.IO.Path.GetFileName(file), true);
                }
            }
            return true;
        }
    }
}
