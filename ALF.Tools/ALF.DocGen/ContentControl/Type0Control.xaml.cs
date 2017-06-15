using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ALF.DocGen.ContentControl
{
    /// <summary>
    /// Type0Control.xaml 的交互逻辑
    /// </summary>
    public partial class Type0Control : UserControl
    {
        public Type0Control()
        {
            InitializeComponent();
        }
        List<string> attachList = new List<string>();
        string fileType="部发文";
        int NFValue;
        
        private void GenType0Doc()
        {
            if (!CheckFormat())
            {
                return;
            }
            
            string FJName="";
            var FJlist = FJ.Value.Split(';');
            for (int i = 0; i < FJlist.Length; i++)
            {
                FJName += string.Format("{0}.{1}\r\n", (i + 1), FJlist[i]);
            }
            FJName = FJName.Substring(0, FJName.Length - 1);

            var valueList = new Dictionary<string, string>();

            valueList.Add("WH", String.Format("{0}﹝{1}﹞", FWDZ.Value,NF.Value));

            valueList.Add("FWDZ", FWDZ.Value);
            valueList.Add("ZS", ZS.Value);
            valueList.Add("CS", CS.Value);
            valueList.Add("ZBDW", ZBDW.Value);
            valueList.Add("MJ", MJ.SelectedItem.ToString() == "无" ? "" : "★" + MJ.SelectedItem.ToString());
            valueList.Add("BT", BT.Value);
            valueList.Add("FJ", FJName);
            valueList.Add("HJ", HJ.SelectedItem.ToString());
            valueList.Add("CHUSHI", CHUSHI.SelectedItem.ToString());
            valueList.Add("RQ", ((DateTime)RQ.SelectedDate).ToString("yyyy年MM月dd日"));


            var fileInfo = new FileInfo(string.Format(@"{0}\{1}.docx",Environment.CurrentDirectory,DateTime.Now.ToString("yyyyMMddhhmmss")));

            int fwtype = 4;
            if (fileType.Substring(0, 1) == "部")
            {
                fwtype = 2;
            }
            if (fileType.Substring(0, 1) == "厅")
            {
                fwtype = 3;
            }


            var qbPath = Tools.Gen(valueList, 0,true);

            if (fwtype == 4)
            {
                var zwPath = Tools.Gen(valueList, fwtype, true);
                Tools.CopyWordDoc(ZW.Text, zwPath, false);
                Tools.CopyWordDoc(zwPath, qbPath, true);
            }

            Tools.ConvertWordToImage(qbPath, valueList, 0);

            //string logInfo = string.Format("\r\n\r\n\r\n制作时间：{0}\r\n", DateTime.Now.ToString("yyyyMMdd hh:mm:ss"));
            //foreach (var item in valueList)
            //{
            //    logInfo += string.Format("【{0}】:{1}\r\n", item.Key, item.Value);
            //}

            //var log = SYSTEM.WindowsTools.ReadFromTxt(Tools.folder+@"\Log.txt", System.Text.Encoding.UTF8);
            //SYSTEM.WindowsTools.WriteToTxt(Tools.folder + @"\Log.txt", log + logInfo, System.Text.Encoding.UTF8);


        }

        private void SecondChangeAction(string type)
        {
            fileType = type;
            ComboChange();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Gen = () => GenType0Doc();
            MainWindow.SecondChange = (ss) => SecondChangeAction(ss);
            NF.Value = DateTime.Now.Year.ToString();
            RQ.SelectedDate = DateTime.Now;
            ZBDW.Value = "国际司";
            CHUSHI.Items = "办公室,亚非处,欧洲处,美大处,欧亚处,政规处,国际处,留学处,办学处,港澳台,机制办";
            MJ.Items = "无,绝密,机密,绝密";
            HJ.Items = "普通,紧急,特急";
            HJ.SelectedIndex = 0;
            MJ.SelectedIndex = 0;
            CHUSHI.SelectedIndex = 0;
        }

        private bool CheckFormat()
        {
            var message = "";
            if (FWDZ.Value == "")
            {
                message += "发文代字未填写\n";
            }
            if (!int.TryParse(NF.Value, out NFValue) || NF.Value.Length != 4)
            {
                message += "年份格式不正确\n";
            }
            if (ZS.Value == "")
            {
                message += "主送未填写\n";
            }
            if (ZBDW.Value == "")
            {
                message += "主办单位未填写\n";
            }
            if (BT.Value == "")
            {
                message += "标题未填写\n";
            }
            if (RQ.SelectedDate==null)
            {
                message += "日期格式不正确\n";
            }
            //if (FJ.Text != "" && !File.Exists(FJ.Text))
            //{
            //    message += "附件文件不存在\n";
            //}
            if (!File.Exists(ZW.Text))
            {
                message += "正文文件不存在\n";
            }
            if (message != "")
            {
                MessageBox.Show(message);
            }
            return message == "";
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new OpenFileDialog()
        //    {
        //        Filter = "WORD文件(*.docx)|*.docx|EXCEL文件(*.xlsx)|*.xlsx",
        //        Multiselect = true,
        //        AddExtension = true
        //    };

        //    if ((bool)dialog.ShowDialog())
        //    {
        //        foreach (var fileName in dialog.FileNames)
        //        {
        //            attachList.Add(fileName);
        //            FJ.Text += string.Format("{0}.{1}\n", attachList.Count, ALF.SYSTEM.WindowsTools.GetBasicName(fileName));
        //        }
        //        FJ.Text = FJ.Text.Substring(0, FJ.Text.Length - 1);
        //    }
        //}

        private void ButtonDoc_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "WORD文件(*.docx)|*.docx",
                AddExtension = true
            };
            if ((bool)dialog.ShowDialog())
            {
                ZW.Text = dialog.FileName;
            }
        }

        private void CHUSHI_SelectionChanged(object sender, EventArgs e)
        {
            ComboChange();
        }

        private void ComboChange()
        {
            switch (fileType)
            {
                case "部发文":
                    FWDZ.Value = "教外";
                    break;
                case "部发函":
                    FWDZ.Value = "教外函";
                    break;
                case "厅发文":
                    FWDZ.Value = "教外厅";
                    break;
                case "厅发函":
                    FWDZ.Value = "教外厅函";
                    break;
                case "司发文":
                    FWDZ.Value = "教外";
                    break;
                case "司发函":
                    FWDZ.Value = "教外函";
                    break;
            }


            if (fileType.Substring(0, 1) == "司")
            {
                switch (CHUSHI.SelectedItem.ToString())
                {
                    case "办公室":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "综");
                        break;
                    case "亚非处":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "亚非");
                        break;
                    case "欧洲处":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "欧");
                        break;
                    case "美大处":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "美大");
                        break;
                    case "欧亚处":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "欧亚");
                        break;
                    case "政规处":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "政规");
                        break;
                    case "国际处":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "国际");
                        break;
                    case "留学处":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "留");
                        break;
                    case "办学处":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "办");
                        break;
                    case "港澳台":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "港澳台");
                        break;
                    case "机制办":
                        FWDZ.Value = FWDZ.Value.Insert(FWDZ.Value.IndexOf("外") + 1, "机制");
                        break;
                }
            }
        }
    }
}
