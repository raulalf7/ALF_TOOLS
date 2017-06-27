using System.IO;
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
            try
            {
                Directory.Delete(Tools.folder + @"\tmp",true);
                Directory.CreateDirectory(Tools.folder + @"\tmp");
            }
            catch
            { }
            if (OFFICE.WordTools.StartWordApp() != "") 
            {
                MessageBox.Show("请先关闭所有打开的Word文档再重新运行此工具。若仍出现此提示框请重新安装Word。");
                Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            OFFICE.WordTools.CloseWordApp();
        }
    }
}
