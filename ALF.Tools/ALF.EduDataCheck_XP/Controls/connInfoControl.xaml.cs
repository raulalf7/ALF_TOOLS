using System;
using System.Windows;
using System.Windows.Controls;
using DataCheck_Pro.Data;
using DataCheck_Pro.Tools;

namespace DataCheck.Controls
{
    /// <summary>
    /// connInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class connInfoControl : UserControl
    {
        public connInfoControl()
        {
            InitializeComponent();
        }

        public event EventHandler onCancelClose;
        public event EventHandler onOKClose;

        public void load(connInfo connInfo)
        {
            dataGrid.DataContext = connInfo;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            onOKClose(this, EventArgs.Empty);

        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            onCancelClose(this, EventArgs.Empty);

        }
        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            sqlTools.connInfo = dataGrid.DataContext as connInfo;

            if (sqlTools.isDBOpen())
            {
                MessageBox.Show("连接成功");
                return;
            }
            MessageBox.Show("连接失败");
        }
    }
}
