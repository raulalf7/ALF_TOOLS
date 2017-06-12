using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace DailyTools.Controls
{
    /// <summary>
    /// RenameControl.xaml 的交互逻辑
    /// </summary>
    public partial class RenameControl : System.Windows.Controls.UserControl
    {
        public RenameControl()
        {
            InitializeComponent();
        }

        string folder = "";

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if (folder == "")
            {
                return;
            }
            var fileList = GetFiles(folder);
            foreach (var fileName in fileList)
            {
                WriteInfo(string.Format("  处理文件【{0}】", fileName));
                var fileInfo = new FileInfo(fileName);
                var time = fileInfo.CreationTime;
                if (fileInfo.LastAccessTime < time)
                {
                    time = fileInfo.LastAccessTime;
                }
                if (fileInfo.LastWriteTime < time)
                {
                    time = fileInfo.LastWriteTime;
                }
                var dirString = string.Format(@"{0}\{1}",folder, string.Format(time.ToString("yyyy_MM")));
                if (!Directory.Exists(dirString))
                {
                    WriteInfo(string.Format("创建目录【{0}】", dirString));
                    Directory.CreateDirectory(dirString);
                }
                int n = 0;
                var name = string.Format(@"{0}\{1}_{2}.{3}", dirString, time.ToString("yyyyMMdd"), n, fileInfo.Extension);
                while (File.Exists(name))
                {
                    n++;
                    name = string.Format(@"{0}\{1}_{2}.{3}", dirString, time.ToString("yyyyMMdd"), n, fileInfo.Extension);
                }
                File.Move(fileName, name);

            }
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            folder = dialog.SelectedPath;
            WriteInfo(string.Format("选择路径【{0}】", folder));
        }

        private void WriteInfo(string info,int line=1)
        {
            infoText.Text += info;
            int n = 0;
            while (n < line)
            {
                infoText.Text += "\n";
                n++;
            }
            infoText.ScrollToEnd();
        }

        private List<string> GetFiles(string dir)
        {
            var files = Directory.GetFiles(dir);
            var fileList = new List<string>(files);
            var dirs = Directory.GetDirectories(dir);
            foreach (var subDir in dirs)
            {
                fileList.AddRange(GetFiles(subDir));
            }
            return fileList;
        }
    }
}
