﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ALF.MSSQL.DataModel;
using ALF.SYSTEM.DataModel;

namespace DataReport_XP
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
            serverNameCombo.ItemsSource = new List<object>
                {
                   DataBaseEngineType.MsSqlServer,
                   DataBaseEngineType.SqlExpress,
                   "ServerData"
                };
            serverNameCombo.SelectedIndex = 0;

            typeCombo.ItemsSource = new List<string> { "精简", "高级" };
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

            ALF.MSSQL.Tools.DBName = ALF.EDU.EduTools.EduDBName;
            var windos = new WorkWindow();
            windos.Load(typeCombo.SelectedIndex);
            windos.Show();
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ALF.SYSTEM.WindowsTools.ExecCmd("mailto:caoqian@moe.edu.cn", "");
        }

        private void ServerNameCombo_OnSelectionChanged(object sender, EventArgs e)
        {
            if (serverNameCombo.SelectedIndex == 0)
            {
                ALF.MSSQL.Tools.DataBaseType = ALF.MSSQL.DataModel.DataBaseEngineType.MsSqlServer;
                return;
            }
            if (serverNameCombo.SelectedIndex == 1)
            {
                ALF.MSSQL.Tools.DataBaseType = ALF.MSSQL.DataModel.DataBaseEngineType.SqlExpress;
                return;
            }
            ALF.MSSQL.Tools.DataBaseType = DataBaseEngineType.Remote;
            ALF.MSSQL.Tools.ConnInfo = new ConnInfo() { ConnIp = "192.168.0.10" , ConnPw = "abc123,"};
        }
    }
}
