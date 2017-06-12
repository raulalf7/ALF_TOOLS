using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DataReport
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        private void rectLoaded(object sender, RoutedEventArgs e)
        {
            var rect = sender as Rectangle;
            if (rect == null)
            {
                return;
            }
            var visual = rect.Fill as VisualBrush;
            if (visual == null)
            {
                return;
            }
            if (rect.Tag == null)
            {
                return;
            }
            if (rect.Tag.ToString() == "采集")
            {
                visual.Visual = Current.Resources["appbar_home_question"] as Visual;
            }
            if (rect.Tag.ToString() == "统计")
            {
                visual.Visual = Current.Resources["appbar_home"] as Visual;
            }
            if (rect.Tag.ToString() == "代管")
            {
                visual.Visual = Current.Resources["appbar_home_question"] as Visual;
            }
            rect.Fill = visual;
        }
    }
}
