using ALF.DocGen.ContentControl;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ALF.DocGen
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static Action Gen;
        public static Action<string> SecondChange;
        public static Grid coverGrid;

        private void typeCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (typeCombo.SelectedIndex == 0)
            {
                content.Content = new Type0Control();
                secondCombo.Visibility = Visibility.Visible;
                GenButton.Visibility = Visibility.Visible;
                GenButton.Content = "套打发文页";
            }

            if (typeCombo.SelectedIndex == 1)
            {
                content.Content = new Type1Control();
                secondCombo.Visibility = Visibility.Collapsed;
                GenButton.Visibility = Visibility.Visible;
                GenButton.Content = "套打签报页";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            typeCombo.Items = "发文稿纸,司局签报";
            secondCombo.Items = "部发文,部发函,厅发文,厅发函,司发文,司发函";
            secondCombo.SelectedIndex = 0;
            typeCombo.SelectedIndex = 0;
            coverGrid = cover;
        }

        private void GenButton_Click(object sender, RoutedEventArgs e)
        {
            cover.Visibility = Visibility.Visible;
            Gen?.Invoke();
        }

        private void secondCombo_SelectionChanged(object sender, EventArgs e)
        {
            SecondChange?.Invoke(secondCombo.SelectedItem.ToString());
        }

    }
}
