using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ALF.DocGen
{
    /// <summary>
    /// PicWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PicWindow : Window
    {
        public PicWindow()
        {
            InitializeComponent();
        }
        Dictionary<string, string> _valueList;
        int genType;
        private List<string> picList;
        int current;

        public void Load(List<string> list,Dictionary<string,string> valueList,int type)
        {
            _valueList = valueList;
            genType = type;
            picList = list;
            LoadImage();
        }
        
        private void LoadImage()
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(picList[current], UriKind.Absolute);
            bi.EndInit();
            imageControl.Source = bi;
            setButtonEnable();
        }


        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Last_Click(object sender, RoutedEventArgs e)
        {
            current--;
            LoadImage();
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            current++;
            LoadImage();
        }

        private void setButtonEnable()
        {
            last.IsEnabled = true;
            next.IsEnabled = true;

            if (current == 0)
            {
                last.IsEnabled = false;
            }

            if (current + 1 == picList.Count)
            {
                next.IsEnabled = false;
            }
        }

        private void Button_Print_Click(object sender, RoutedEventArgs e)
        {
            var filePath = Tools.Gen(_valueList, genType, false);
            Tools.wordApp.Visible = true;
            Tools.wordApp.Documents.Add(filePath);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.coverGrid.Visibility = Visibility.Collapsed;
            Tools.wordApp.Visible = false;
        }
    }

}
