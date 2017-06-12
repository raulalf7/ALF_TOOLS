using System.Windows;

namespace ALF.DocGen
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Tools.StartWordApp();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Tools.CloseWordApp();
        }
    }
}
