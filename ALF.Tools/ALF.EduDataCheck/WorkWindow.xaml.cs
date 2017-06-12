using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataReport_XP;
using MahApps.Metro.Controls.Dialogs;

namespace DataReport
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
            Title = Tools.Title;
            verText.Text = Tools.Ver;
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
            templateControl.Load();
        }

        public static async Task<MessageDialogResult> showDialog(string title, string content)
        {
            return await Window.ShowMessageAsync(title, content, MessageDialogStyle.AffirmativeAndNegative);
        }

        public static async void ShowInfo(string title, string content)
        {
            try
            {
                await Window.ShowMessageAsync(title, content);
                Cover.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public static async void ShowError( string content)
        {
            try
            {
                await Window.ShowMessageAsync("发生错误", content);
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
