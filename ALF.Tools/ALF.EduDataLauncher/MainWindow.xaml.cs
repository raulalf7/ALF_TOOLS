using System;
using System.Windows;

namespace ALF.EduDataLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var year = yearCombo.SelectedItem.ToString();
            var exeName = "EduClient.exe";
            if (year != "2012" && year!="2013")
            {
                exeName = string.Format("EduClient{0}.exe", year);
            }
            SYSTEM.WindowsTools.ExecCmd(string.Format(@"{2}\{0}\{1}", year, exeName,Environment.CurrentDirectory), connCombo.SelectedItem.ToString());
        }


    }
}
