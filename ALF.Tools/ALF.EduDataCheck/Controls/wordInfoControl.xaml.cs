using System.Windows.Controls;
using System.Windows.Input;
using DataCheck_Pro.Data;

namespace DataCheck.Controls
{
    /// <summary>
    /// wordInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class wordInfoControl : UserControl
    {
        public wordInfoControl()
        {
            InitializeComponent();
        }

        public void load(wordInfo wordInfo)
        {
            dataGrid.DataContext = wordInfo;
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {

        }
    }
}
