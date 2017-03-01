using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using DataReport_XP;
using System.Linq;

namespace DataReport
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tools.Initial();
            Title = Tools.Title;
            verText.Text = Tools.Ver;

            serverNameCombo.ItemsSource = new List<object>
                {
                   ALF.MSSQL.DataModel.DataBaseEngineType.MsSqlServer,
                   ALF.MSSQL.DataModel.DataBaseEngineType.SqlExpress
                };
            serverNameCombo.SelectedIndex = 0;

            typeCombo.ItemsSource = new List<string> {"精简", "高级"};
            typeCombo.SelectedIndex = 0;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void enterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ALF.SYSTEM.WindowsTools.IsServeiceStart(ALF.MSSQL.Tools.ServiceName))
            {
                Tools.ShowError(101);
                return;
            }

            if (!ALF.MSSQL.Tools.IsDBOpen())
            {
                Tools.ShowError(102);
                return;
            }

            if (!File.Exists(Tools.TemplateConfigPath) || !File.Exists(Tools.ArgConfigPath))
            {
                Tools.ShowError(200);
                return;
            }
            ALF.EDU.EduTools.RecordYear = "2016";
            ALF.MSSQL.Tools.DBName = ALF.EDU.EduTools.EduDBName;
            var windos = new WorkWindow();
            windos.Load(typeCombo.SelectedIndex);
            windos.Show();
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ALF.SYSTEM.WindowsTools.ExecCmd("mailto:caoqian@moe.edu.cn","");
        }

        private void ServerNameCombo_OnSelectionChanged(object sender, EventArgs e)
        {
            if (serverNameCombo.SelectedIndex == 0)
            {
                ALF.MSSQL.Tools.DataBaseType = ALF.MSSQL.DataModel.DataBaseEngineType.MsSqlServer;
                return;
            }
            ALF.MSSQL.Tools.DataBaseType = ALF.MSSQL.DataModel.DataBaseEngineType.SqlExpress;
        }
    }
}
