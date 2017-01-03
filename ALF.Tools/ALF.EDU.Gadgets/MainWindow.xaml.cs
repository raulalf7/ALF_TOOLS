using System.Windows;
using System.Windows.Controls;
using ALF.EDU.Gadgets.UserControl;

namespace ALF.EDU.Gadgets
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;
            switch (btn.Content.ToString())
            {
                case "补充县基表实例和数据":
                    var obj1 = new CountyInstanceSupply();
                    mainContent.Content = obj1;
                    mainContent.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
