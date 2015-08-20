using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Alf7.Tools.Library;
using Alf7.Tools.Library.DataModel;

namespace DataCheck_XP.Control
{
    /// <summary>
    /// TemplateControl.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateControl
    {
        public TemplateControl()
        {
            InitializeComponent();
        }

        public Action<List<TemplateInfo>> SelectAction;
        string _tmpResult;





        Action<string> _analyzeFinished;
        private string _selectedFileName;
        FileInfo _file;
        TemplateInfo _templateInfo;


        public void load()
        {
            fileInfoListBox.ItemsSource = Tools.getLocalTemplateFileList(out _tmpResult);
            if (_tmpResult != "")
            {
                WorkWindow.showError(_tmpResult);
                return;
            }


            _analyzeFinished = s =>
            {
                WorkWindow.Cover.Visibility = Visibility.Collapsed;
                if (s != "")
                {
                    WorkWindow.showError(s);
                    return;
                }
                templateInfoControl.load(_templateInfo, true);
                fileInfoListBox.ItemsSource = Tools.getLocalTemplateFileList(out _tmpResult);

            };


        }

        private void fileInfoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            templateInfoControl.load(fileInfoListBox.SelectedItem as TemplateInfo, true);
            var list  = (from object selectedItem in fileInfoListBox.SelectedItems select selectedItem as TemplateInfo).ToList();


            if (SelectAction != null)
            {
                SelectAction(list);
            }
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = @"word文件|*.docx" };
            dialog.ShowDialog();
            if (dialog.FileName == "")
            {
                return;
            }
            uploadFile(dialog.FileName);
        }

        private void uploadFile(string fileName)
        {
            _file = new FileInfo(string.Format(@"{0}\templateFiles\{1}", Environment.CurrentDirectory, SystemTools.getBasicName(fileName)));
            _selectedFileName = fileName;
            WorkWindow.Cover.Visibility = Visibility.Visible;
            if (File.Exists(_file.FullName))
            {
                var test = WorkWindow.showDialog("文件已存在", "所选模板已经存在，是否替换？");

                if (test == MessageBoxResult.Cancel)
                {
                    return;
                }
                try
                {
                    File.Copy(_selectedFileName, _file.FullName, true);
                    analyzeFile();
                }
                catch (Exception e)
                {
                    WorkWindow.showError(e.Message);
                }
            }
            else
            {
                File.Copy(_selectedFileName, _file.FullName, true);
                analyzeFile();
            }
        }

        private void analyzeFile()
        {
            var task = new Thread(() =>
            {
                _templateInfo = new TemplateInfo { templatePath = string.Format(@"{0}\templateFiles\{1}", Environment.CurrentDirectory,_file.Name), templateID = Guid.NewGuid(), templateSize = _file.Length.ToString(CultureInfo.InvariantCulture), templateName = _file.Name, updatetime = DateTime.Now, rowid = Guid.NewGuid() };
                var argListTmpe = ReportOfficeTools.getArgInfoListFromWord(_templateInfo, out _tmpResult);
                if (_tmpResult != "")
                {
                    WorkWindow.showError(_tmpResult);
                }
                else
                {
                    _tmpResult = Tools.updateArgConfig(argListTmpe);
                    if (_tmpResult == "")
                    {
                        _templateInfo.templatePath = string.Format(@".\templateFiles\{0}",_file.Name);
                        _tmpResult = Tools.updateTemplateConfig(_templateInfo);
                    }
                }
                Dispatcher.Invoke(_analyzeFinished, _tmpResult);
            });
            task.Start();
        }
    }
}
