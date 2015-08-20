using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Alf7.Tools.Library;
using Alf7.Tools.Library.DataModel;

namespace DataCheck_XP
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
            serverNameCombo.ItemsSource = new List<ConnInfo>
                {
                    new ConnInfo {connIp  = ".", connName = "(LOCAL)"},
                    new ConnInfo {connIp = @".\sqlexpress", connName = @".\SQLEXPRESS"}
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
            if (!SystemTools.isServeiceStart(SqlTools.serviceName))
            {
                SystemTools.showError(101);
                return;
            }

            if (!SqlTools.isDBOpen())
            {
                SystemTools.showError(102);
                return;
            }

            if (!File.Exists(Tools.TemplateConfigPath) || !File.Exists(Tools.ArgConfigPath))
            {
                SystemTools.showError(200);
                return;
            }

            var windos = new WorkWindow();
            windos.load(typeCombo.SelectedIndex);
            windos.Show();
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SystemTools.execCmd("mailto:caoqian@moe.edu.cn","");
        }

        private void ServerNameCombo_OnSelectionChanged(object sender, EventArgs e)
        {
            var tmp = serverNameCombo.SelectedItem as ConnInfo;
            if (tmp == null)
            {
                return;
            }
            SqlTools.ConnInfo = tmp;
            SqlTools.RecordYear = "2014";
        }
    }
}
