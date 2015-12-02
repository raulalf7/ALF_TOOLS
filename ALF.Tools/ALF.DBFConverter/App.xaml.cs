using System.Windows;
using System.Windows.Threading;

namespace ALF.DBFConverter
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException; 
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show(e.Exception.Message);
        }
    }
}
