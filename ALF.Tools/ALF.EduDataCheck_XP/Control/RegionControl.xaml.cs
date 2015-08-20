using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

using Alf7.Tools.Library;
using Alf7.Tools.Library.DataModel;
using Alf7.UI.Library;

namespace DataCheck_XP.Control
{
    /// <summary>
    /// fillReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegionControl
    {
        public RegionControl()
        {
            InitializeComponent();
        }

        public Action CreateAction;

        public void load(List<TemplateInfo> templateInfos)
        {
            if (templateInfos.Count == 0)
            {
                return;
            }

            _templateInfoList = templateInfos;
            fileNameTextBlock.Focus();
            var tmp = _templateInfoList.Aggregate("", (current, item) => current + (item.templateName + "，"));
            fileNameTextBlock.Text = tmp.Substring(0, tmp.Length - 1);
            intialRegionTree();
            _regionList = new RegionList();
            selectedRegion.DataContext = _regionList;

            _createFinished = result =>
            {
                WorkWindow.Cover.Visibility = Visibility.Collapsed;
                if (result != "")
                {
                    WorkWindow.showError(result);
                    return;
                }
                WorkWindow.showInfo("生成完成","报告生成完成");
                if (CreateAction != null)
                {
                    CreateAction();
                }
            };
        }

        Action<string> _createFinished;
        RegionTreeControl _regionTreeControl;
        private List<TemplateInfo> _templateInfoList;
        RegionList _regionList;

        private void intialRegionTree()
        {
            _regionTreeControl = new RegionTreeControl(SqlTools.sqlConnString) { appType = 1, analysisType = "采集" };
            _regionTreeControl.load(1, int.Parse(SqlTools.RecordYear), true);
            contentControl.Content = _regionTreeControl;
        }

        private void selectedRegion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tmp = selectedRegion.SelectedItem as return_getRegionTreeNodeList;
            _regionList.Remove(tmp);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (_templateInfoList == null||_templateInfoList.Count==0)
            {
                WorkWindow.showError("请先选择模板文件");
                return;
            }

            WorkWindow.Cover.Visibility = Visibility.Visible;
            var task = new Thread(() =>
            {
                string result;
                Tools.createReportFile(_templateInfoList, _regionList.ToList(), _regionTreeControl.appType, out result);
                Dispatcher.Invoke(_createFinished, result);
            });
            task.Start();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var tmp = _regionTreeControl.selectItem;

            if (_regionList.Count(p => p.nodeNo == tmp.nodeNo) != 0)
            {
                return;
            }
            _regionList.Add(tmp);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            selectedRegion_MouseDoubleClick(null, null);
        }
    }

    public class RegionList : ObservableCollection<return_getRegionTreeNodeList>
    {
    }
}
