using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;



namespace DataReport_XP
{
    /// <summary>
    /// WorkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WorkWindow 
    {
        public WorkWindow()
        {
            InitializeComponent();
        }

        public static Grid Cover;

        public static WorkWindow Window;

        public void Load(int index)
        {
            Cover = coverGrid;
            Window = this;

            regionControl.CreateAction += () =>
            {
                wordControl.Load(false);
                tab.SelectedItem = wordControl;
            };

            wordControl.Load(true);
            if (index == 0)
            {
                string tmpResult;

                var templateInfos = Tools.GetLocalTemplateFileList(out tmpResult);

                if (tmpResult != "")
                {
                    ShowError(tmpResult);
                    return;
                }

                tab.Items.RemoveAt(0);
                regionControl.Load(templateInfos);
                return;
            }
            templateControl.SelectAction += list =>
            {
                regionControl.Load(list);
                tab.SelectedItem = regionControl;
            };
            templateControl.load();
        }

        public static MessageBoxResult showDialog(string title, string content)
        {
            return  MessageBox.Show(content, title, MessageBoxButton.OKCancel);
        }

        public static void ShowInfo(string title, string content)
        {
            try
            {
                MessageBox.Show(content,title);
                Cover.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public static  void ShowError( string content)
        {
            try
            {
                MessageBox.Show(content, "发生错误");
                Cover.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ALF.SYSTEM.WindowsTools.ExecCmd("mailto:caoqian@moe.edu.cn", "");
        }
    }
}
