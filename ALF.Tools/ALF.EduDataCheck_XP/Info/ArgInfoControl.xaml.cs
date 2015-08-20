using System;
using System.Windows.Forms;

using Alf7.Tools.Library;
using Alf7.Tools.Library.DataModel;
using Alf7.UI.Library;
using MessageBox = System.Windows.MessageBox;

namespace DataCheck_XP.Info
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

        private ArgInfo _argInfo;
        private ArgInfo _origArg;
        public event EventHandler onOkClose;
        public event EventHandler onCancelClose;
        
        public void load(ArgInfo argInfo)
        {
            _argInfo = argInfo;
            _origArg = new ArgInfo {upLimit = argInfo.upLimit, downLimit = argInfo.downLimit, argDataSql = argInfo.argDataSql,argName = argInfo.argName,argNo = argInfo.argNo};
            argPanel.DataContext = _argInfo;
            bindData();
        }
        
        private void cancelButton_Click(object sender, EventArgs e)
        {
            _argInfo.upLimit = _origArg.upLimit; 
            _argInfo.downLimit = _origArg.downLimit;
            _argInfo.argDataSql = _origArg.argDataSql;
            _argInfo.argName = _origArg.argName;
            _argInfo.argNo = _origArg.argNo;
            onCancelClose(_argInfo, EventArgs.Empty);
        }

        private void countButton_Click(object sender, EventArgs e)
        {
            string result;
            var sql = "select count(1) as 数据量 from (" + SqlTools.addCondition(_argInfo, "") + ") a";
            var view = SqlTools.getSqlDataView(sql, out result);
            if (result != "")
            {
                SystemTools.showError(999,result);
                return;
            }
            MessageBox.Show(view.Table.Rows[0][0].ToString());
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog {FileName = "导出结果.csv", Filter = @"CSV文件|*.csv"};
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string sql = SqlTools.addCondition(_argInfo, "");

            string result = SqlTools.exportExcel(sql, dialog.FileName);


            if (result == "")
            {
                MessageBox.Show("导出完成", "导出完成");
                return;
            }
            SystemTools.showError(999, result);
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            onOkClose(this, EventArgs.Empty);
        }

        private void bindData()
        {
            foreach (var child in argPanel.Children)
            {
                var label = child as TitleLabel;
                if (label != null)
                {
                    label.SetBinding();
                    continue;
                }

                var text = child as TitleText;
                if (text != null)
                {
                    text.SetBinding();
                }

            }
        }
    }
}
