using System;
using System.Windows.Forms;
using ALF.EDU.DataModel;
using ALF.UI.TitleControl;
using Label = System.Windows.Forms.Label;

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
        public event EventHandler OnOkClose;
        public event EventHandler OnCancelClose;
        
        public void Load(ArgInfo argInfo)
        {
            _argInfo = argInfo;
            _origArg = new ArgInfo {upLimit = argInfo.upLimit, downLimit = argInfo.downLimit, argDataSql = argInfo.argDataSql,argName = argInfo.argName,argNo = argInfo.argNo};
            argPanel.DataContext = _argInfo;
            BindData();
        }
        
        private void cancelButton_Click(object sender, EventArgs e)
        {
            _argInfo.upLimit = _origArg.upLimit; 
            _argInfo.downLimit = _origArg.downLimit;
            _argInfo.argDataSql = _origArg.argDataSql;
            _argInfo.argName = _origArg.argName;
            _argInfo.argNo = _origArg.argNo;
            if (OnCancelClose != null) OnCancelClose(_argInfo, EventArgs.Empty);
        }

        private void countButton_Click(object sender, EventArgs e)
        {
            string result;
            var sql = "select count(1) as 数据量 from (" + ALF.EDU.ReportOfficeTools.AddCondition(_argInfo, "") + ") a";
            var view = ALF.MSSQL.Tools.GetSqlDataView(sql, out result);
            if (result != "")
            {
                Tools.ShowError(999, result);
                return;
            }
            MessageBox.Show(view.Table.Rows[0][0].ToString());
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog { FileName = "导出结果.csv", Filter = @"CSV文件|*.csv" };
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string sql = ALF.EDU.ReportOfficeTools.AddCondition(_argInfo, "");

            string result = ALF.MSSQL.Tools.ExportCSV(sql, dialog.FileName);

            if (result == "")
            {
                MessageBox.Show(@"导出完成", @"导出完成");
                return;
            }
            Tools.ShowError(999, result);
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (OnOkClose != null) OnOkClose(this, EventArgs.Empty);
        }

        private void BindData()
        {
            foreach (var child in argPanel.Children)
            {
                var label = child as Label;
                if (label != null)
                {
                    //label.SetBinding();
                    continue;
                }

                var text = child as Text;
                if (text != null)
                {
                    text.SetBinding();
                }

            }
        }
    }
}
