using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ALF.EDU.DataModel;
using DataReport_XP;
using MahApps.Metro.Controls;
using System.Data;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace DataReport.Info
{
    /// <summary>
    /// TemplateInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateInfoControl
    {
        public TemplateInfoControl()
        {
            InitializeComponent();
        }

        private string _tmpReulst;
        bool _isEditable;
        List<ArgInfo> _argInfoList;
        private ArgInfo _selectedArgInfo;
        private Action<Task<MessageDialogResult>> _action;

        //public Action SelectAction;

        public void Load(TemplateInfo templateInfo, bool isEditable = false)
        {
            _isEditable = isEditable;
            mainGrid.DataContext = templateInfo;


            _argInfoList = Tools.GetArgInfoList(templateInfo, out _tmpReulst);
            IntialArgList();
            if (argTabControl.Items.Count != 0)
            {
                argTabControl.SelectedIndex = 0;
            }

            _action = o =>
            {
                if (o.Result == MessageDialogResult.Negative)
                {
                    return;
                }
                try
                {
                    foreach (var item in _argInfoList)
                    {
                        Tools.UpdateArgConfig(item);
                    }
                }
                catch (Exception e)
                {
                    WorkWindow.ShowError(e.Message);
                }
            };

        }

        private void IntialArgList()
        {
            if (_argInfoList == null || _argInfoList.Count == 0)
            {
                argTabControl.Items.Clear();
                return;
            }
            CreateArgTab();
        }

        private void CreateArgTab()
        {
            argTabControl.Items.Clear();
            foreach (var item in _argInfoList.Select(p => p.argType).Distinct())
            {
                var tabItem = new TabItem { Header = item, Background = new SolidColorBrush(Colors.Transparent) };
                CreateList(_argInfoList.Where(p => p.argType == item).ToList(), tabItem);
                argTabControl.Items.Add(tabItem);
            }
        }

        private void CreateList(IEnumerable<ArgInfo> argInfoTypeList, TabItem tabItem)
        {
            var listBox = new ListBox { ItemTemplate = Application.Current.Resources["ArgListTemplate"] as DataTemplate };
            listBox.SelectionChanged += listBox_SelectionChanged;
            foreach (var item in argInfoTypeList.OrderBy(p => p.argNo))
            {
                listBox.Items.Add(item);
            }
            tabItem.Content = listBox;
        }

        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
            {
                return;
            }
            _selectedArgInfo = listBox.SelectedItem as ArgInfo;
        }

        void editButton_Click(object sender, EventArgs e)
        {
            if (!_isEditable)
            {
                return;
            }
            if (_selectedArgInfo == null)
            {
                return;
            }

            DataReportTools.ShowArgInfoControl(_selectedArgInfo, this, mainGrid.DataContext as TemplateInfo);
        }

        private void countButton_Click(object sender, EventArgs e)
        {
            if (_argInfoList == null)
            {
                MessageBox.Show("请选择模板");
                return;
            }
            ShowResultWindow(GetCountDataView(""));
        }

        private IEnumerable<string> GetCountDataView(string regionA)
        {
            var regionCondition = "";
            if (regionA != "")
            {
                regionCondition = string.Format(" gatherRegionA = '{0}'", regionA);
            }
            var resultList = new List<string>();
            const string argInfoFormat = @"参数编号：{0}\t 查询结果：{1}\t参数名称：{2}";
            Console.WriteLine(regionA + @"全部个数查询开始");
            var allStart = DateTime.Now;
            foreach (var item in _argInfoList)
            {
                string sql = ALF.EDU.ReportOfficeTools.AddCondition(item, regionCondition);

                if (sql == "")
                {
                    continue;
                }
                sql = string.Format("select '{0} '+'{1} '+'{2} '+'{3} '+convert(nvarchar(50),count(1))+' {5} ' from ({4}) a", item.argNo, item.argBusinessGroup, item.argName, item.businessType, sql, regionA);

                string result;
                var tmp = ALF.MSSQL.Tools.GetSqlListString(sql, out result);
                if (result != "")
                {
                    resultList.Add(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t", item.argNo, item.argName, item.argBusinessGroup, item.businessType, result, regionA));
                    continue;
                }
                resultList.Add(tmp.Single());
                Console.WriteLine(argInfoFormat, item.argNo,tmp.Single(), item.argName);
            }
            Console.WriteLine(@"全部个数查询结束，用时：{0}", (DateTime.Now - allStart).ToString());

            return resultList;
        }

        private static void ShowResultWindow(IEnumerable<string> list)
        {
            var listbox = new ListBox {ItemsSource = list, SelectionMode = SelectionMode.Extended};
            listbox.SelectionChanged += (ss, ee) =>
            {
                var result = listbox.SelectedItems.Cast<object>().Aggregate("", (current, item) => current + (item.ToString() + "\r\n"));
                Clipboard.SetDataObject(result);
            };
            var w = new MetroWindow {Content = listbox , Width = 600, Height = 400,Title = "全部查询个数", EnableDWMDropShadow = true};
            w.Show();

        }

        private void syncButton_Click(object sender, EventArgs e)
        {
            string result;
            var dv = ALF.MSSQL.Tools.GetSqlDataView(string.Format("select * from checkArgInfo where templateName='{0}'", _argInfoList[0].templateName), out result);
            if (result != "" )
            {
                Tools.ShowError(0, string.Format("查询失败:【{0}】",result));
                return;
            }
            if(dv.Table.Rows.Count != _argInfoList.Count)
            {
                Tools.ShowError(0, string.Format("参数文件信息与数据库参数数据不匹配，数量分别为【{0}】,【{1}】",_argInfoList.Count, dv.Table.Rows.Count));
                return;
            }
            foreach (DataRow row in dv.Table.Rows)
            {
                try
                {
                    _argInfoList.Single(p => p.argName == row["argName"].ToString()).argDataSql = row["argDataSql"].ToString();
                }
                catch
                {
                    result += string.Format("【{0}】缺少对应数据\n", row["argName"]);
                }
            }
            if (result != "")
            {
                Tools.ShowError(0, result);
                return;
            }

            var test = WorkWindow.showDialog("同步完成", "参数已经同步完成，是否更新本地配置文件？");
            test.ContinueWith(_action);
        }
    }
}
