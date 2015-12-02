using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using ALF.MSSQL;
using MahApps.Metro.Controls.Dialogs;

namespace ALF.DBFConverter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string _selectedPath = "";
        public static MainWindow TheMainWindow;
        private List<string> _dbfFileList = new List<string>();
        private string _tmp;

        private void pathText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = new FolderBrowserDialog { ShowNewFolderButton = true };
            if (System.Windows.Forms.DialogResult.OK != dialog.ShowDialog())
            {
                Initial();
                return;
            }
            _selectedPath = dialog.SelectedPath;
            pathText.Text = _selectedPath;
            analyzeGrid.Visibility = Visibility.Visible;
            analyzeText.Text = AnaylzeFolder(_selectedPath, out _dbfFileList);
        }

        private void transfer_Click(object sender, RoutedEventArgs e)
        {
            var bgMeet = new BackgroundWorker {WorkerReportsProgress = true};
            bgMeet.DoWork += bgMeet_DoWork;
            bgMeet.RunWorkerAsync();
        }

        private void bgMeet_DoWork(object sender, DoWorkEventArgs e)
        {

            Action<Visibility> coverAction = visibility => { coverGrid.Visibility = visibility; };
            Action<string> action = content => { infoText.Text += content; };

            Dispatcher.Invoke(coverAction, Visibility.Visible);

            string tmp = Tools.ExecSql(@"                                
                                exec sp_configure 'show advanced options',1
                                reconfigure
                                exec sp_configure 'Ad Hoc Distributed Queries',1
                                reconfigure");

            if (tmp != "")
            {
                Dispatcher.Invoke(action, string.Format("开启配置功能失败【{0}】\n\n", tmp));
                Dispatcher.Invoke(coverAction, Visibility.Collapsed);
                return;
            }



            foreach (var path in _dbfFileList)
            {
                Dispatcher.Invoke(action, string.Format("开始转换【{0}】\n", path));

                tmp = TransferSingleData(path);

                if (tmp == "")
                {
                    Dispatcher.Invoke(action, string.Format("转换成功【{0}】\n\n", path));
                    continue;
                }
                Dispatcher.Invoke(action, string.Format("转换失败【{0}】，错误原因【{1}】\n\n", path, tmp));
            }

            Tools.ExecSql(@"
                                exec sp_configure 'show advanced options',0
                                reconfigure
                                exec sp_configure 'Ad Hoc Distributed Queries',0
                                reconfigure");
            Dispatcher.Invoke(action, "***全部转换完成***\n\n\n");

            Dispatcher.Invoke(coverAction, Visibility.Collapsed);
        }

        private void Initial()
        {
            analyzeGrid.Visibility = Visibility.Collapsed;
            _dbfFileList.Clear();
            pathText.Text = "";
            analyzeText.Text = "";
            infoText.Text = "";
        }

        private void InfoText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            infoText.ScrollToEnd();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Tools.DataBaseType = ALF.MSSQL.DataModel.DataBaseEngineType.MsSqlServer;

            TheMainWindow = this;
            var dbNameList = Tools.GetSqlListString("select  name from master..SysDatabases ", out _tmp);
            if (_tmp != "")
            {
                ShowError(_tmp);
                return;
            }
            dbComboBox.ItemsSource = dbNameList;
            dbComboBox.SelectedIndex = 0;


            linkComboBox.ItemsSource = new List<string>() {"VFPOLEDB.1", "MSDASQL", "ACE.OLEDB", "JET.OLEDB"};
            linkComboBox.SelectedIndex = 0;

        }

        private void DbComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tools.DBName=dbComboBox.SelectedItem.ToString();
        }

        public static async void ShowError(string errorMessage)
        {
            await TheMainWindow.ShowMessageAsync("发生错误", errorMessage);
        }

        public static string AnaylzeFolder(string path, out List<string> fileList)
        {
            fileList = Directory.GetFiles(path, "*.dbf").ToList();
            return string.Format("所选目录下共有{0}个DBF文件", fileList.Count());
        }

        public static string TransferSingleData(string path)
        {
            string folderPath = path.Substring(0, path.LastIndexOf('\\'));
            string fileTitle = path.Substring(path.LastIndexOf('\\') + 1, path.LastIndexOf('.') - path.LastIndexOf('\\') - 1);
            Tools.ExecSql(string.Format(@"if exists (select * from sysobjects where name='{0}')
                                        begin 
                                        drop table {0}
                                        end", fileTitle));


            var linkStringList = new List<string>()
            {
                "select * into {1} from openrowset('VFPOLEDB.1','{0}';'admin';'' ,'select * from {1}.DBF')",
                "select * into {1} from openrowset('MSDASQL','Driver=Microsoft Visual FoxPro Driver;SourceType=DBF;SourceDB={0}','select * from {1}.DBF')",
                "select * into {1} from OPENROWSET('Microsoft.ACE.OLEDB.12.0','dBase IV;HDR=NO;IMEX=2;DATABASE={0}','select * from {1}.dbf')",
                "select * into {1} from OPENROWSET('MICROSOFT.JET.OLEDB.4.0','dBase IV;HDR=NO;IMEX=2;DATABASE={0}','select * from {1}.dbf')"
            };
            //select * into {1} from openrowset('VFPOLEDB.1','{0}';'admin';'' ,'select * from {1}.DBF')
            //select * into {1} from openrowset('MSDASQL','Driver=Microsoft Visual FoxPro Driver;SourceType=DBF;SourceDB={0}','select * from {1}.DBF')
            //select * into {1} from OPENROWSET('Microsoft.ACE.OLEDB.12.0','dBase IV;HDR=NO;IMEX=2;DATABASE={0}','select * from {1}.dbf')
            //OPENROWSET('MICROSOFT.JET.OLEDB.4.0','dBase    IV;HDR=NO;IMEX=2;DATABASE=
         //   string command = string.Format("select * into [{1}] from openrowset('MSDASQL','Driver=Microsoft Visual FoxPro Driver;SourceType=DBF;SourceDB={0}','select * from {1}.DBF')", folderPath, fileTitle);

            string command = string.Format(linkStringList[_selectedLinkIndex], folderPath, fileTitle);
            return Tools.ExecSql(command);
        }

        private static int _selectedLinkIndex;
        private void LinkComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedLinkIndex = linkComboBox.SelectedIndex;
        }
    }
}
