using System;
using System.Windows;
using DataCheck_Pro.Tools;
using DataCheck_Pro.Data;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace DataCheck.Controls
{
    /// <summary>
    /// ArgInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class ArgInfoControl
    {
        public ArgInfoControl()
        {
            InitializeComponent();
        }

        private ArgInfo _ArgInfo;


        public void load(ArgInfo ArgInfoA)
        {
            _ArgInfo = ArgInfoA;
            argGrid.DataContext = _ArgInfo;

        }

        public event EventHandler onOkClose;
        public event EventHandler onCancelClose;

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            onOkClose(this, EventArgs.Empty);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            onCancelClose(_ArgInfo, EventArgs.Empty);
        }

        private void findButton_Click(object sender, RoutedEventArgs e)
        {
            string result;
            resultDataGrid.ItemsSource = sqlTools.getSqlDataView(sqlTools.addCondition(_ArgInfo, ""), out result);
            stateTextBlock.Text = result;
        }

        private void findCountButton_Click(object sender, RoutedEventArgs e)
        {
            string result;

            string sql = sqlTools.addCondition(_ArgInfo, "");
            sql = "select count(1) as 数据量 from (" + sql + ") a";
            resultDataGrid.ItemsSource = sqlTools.getSqlDataView(sql, out result);
            stateTextBlock.Text = result;

        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog {FileName = "导出结果.csv", Filter = @"CSV文件|*.csv"};
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string sql = sqlTools.addCondition(_ArgInfo, "");

            string result=  sqlTools.exportExcel(sql, dialog.FileName);


            if (result == "")
            {
                MessageBox.Show("导出完成", "导出完成");
                return;
            }
            SystemTools.showError(402, result);
        }


    }
}
