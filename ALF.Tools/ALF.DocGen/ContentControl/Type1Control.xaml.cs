using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ALF.DocGen.ContentControl
{
    /// <summary>
    /// Type1Control.xaml 的交互逻辑
    /// </summary>
    public partial class Type1Control : UserControl
    {
        public Type1Control()
        {
            InitializeComponent();
        }

        public void GenType1Doc()
        {
            var valueList = new Dictionary<string, string>();
            valueList.Add("CB", CB.Value);
            valueList.Add("SY", SY.Text);
            valueList.Add("ZBDW", ZBDW.Value);
            valueList.Add("JBCS", JBCS.SelectedItem.ToString());
            valueList.Add("SFFW", SFFW.SelectedItem.ToString());

            var filePath =  Tools.Gen(valueList, 1);
            Tools.wordApp.Visible = true;
            Tools.wordApp.Documents.Add(filePath);

            string logInfo = string.Format("\r\n\r\n\r\n制作时间：{0}\r\n", DateTime.Now.ToLongTimeString());
            foreach (var item in valueList)
            {
                logInfo += string.Format("【{0}】:{1}\r\n", item.Key, item.Value);
            }

            var log = SYSTEM.WindowsTools.ReadFromTxt(Tools.folder + @"\Log.txt", System.Text.Encoding.UTF8);
            SYSTEM.WindowsTools.WriteToTxt(Tools.folder + @"\Log.txt", log + logInfo, System.Text.Encoding.UTF8);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Gen = () => GenType1Doc();
            ZBDW.Value = "国际司";
            SFFW.Items = "否,是";
            SFFW.SelectedIndex = 0;
            JBCS.Items = "办公室,亚非处,欧洲处,美大处,欧亚处,政规处,国际处,留学处,办学处,港澳台,机制办";
            JBCS.SelectedIndex = 0;

        }
    }
}
