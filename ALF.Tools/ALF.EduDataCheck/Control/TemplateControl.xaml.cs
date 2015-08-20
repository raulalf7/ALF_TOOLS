using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ALF.EDU;
using ALF.EDU.DataModel;
using ALF.SYSTEM;
using MahApps.Metro.Controls.Dialogs;

namespace DataCheck.Control
{
    /// <summary>
    ///     TemplateControl.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateControl
    {
        public TemplateControl()
        {
            InitializeComponent();
        }

        public Action<List<TemplateInfo>> SelectAction;
        private string _tmpResult;

        private Action<string> _analyzeFinished;
        private Action<Task<MessageDialogResult>> _action;
        private string _selectedFileName;
        private FileInfo _file;
        private TemplateInfo _templateInfo;


        public void Load()
        {
            fileInfoListBox.ItemsSource = Tools.GetLocalTemplateFileList(out _tmpResult);
            if (_tmpResult != "")
            {
                WorkWindow.ShowError(_tmpResult);
                return;
            }

            _action = o =>
            {
                if (o.Result == MessageDialogResult.Negative)
                {
                    return;
                }
                try
                {
                    File.Copy(_selectedFileName, _file.FullName, true);
                    AnalyzeFile();
                }
                catch (Exception e)
                {
                    WorkWindow.ShowError(e.Message);
                }
            };

            _analyzeFinished = s =>
            {
                WorkWindow.Cover.Visibility = Visibility.Collapsed;
                if (s != "")
                {
                    WorkWindow.ShowError(s);
                    return;
                }
                templateInfoControl.Load(_templateInfo, true);
                fileInfoListBox.ItemsSource = Tools.GetLocalTemplateFileList(out _tmpResult);
            };
        }

        private void fileInfoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            templateInfoControl.Load(fileInfoListBox.SelectedItem as TemplateInfo, true);
            var list =
                (from object selectedItem in fileInfoListBox.SelectedItems select selectedItem as TemplateInfo).ToList();


            if (SelectAction != null)
            {
                SelectAction(list);
            }
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog {Filter = @"word文件|*.docx"};
            dialog.ShowDialog();
            if (dialog.FileName == "")
            {
                return;
            }
            UploadFile(dialog.FileName);
        }

        private void UploadFile(string fileName)
        {
            _file =
                new FileInfo(string.Format(@"{0}\templateFiles\{1}", Environment.CurrentDirectory,
                    WindowsTools.GetBasicName(fileName)));
            _selectedFileName = fileName;
            WorkWindow.Cover.Visibility = Visibility.Visible;
            if (File.Exists(_file.FullName))
            {
                var test = WorkWindow.showDialog("文件已存在", "所选模板已经存在，是否替换？");
                test.ContinueWith(_action);
            }
            else
            {
                File.Copy(_selectedFileName, _file.FullName, true);
                AnalyzeFile();
            }
        }

        private void AnalyzeFile()
        {
            var task = new Thread(() =>
            {
                _templateInfo = new TemplateInfo
                {
                    templatePath = string.Format(@"{0}\templateFiles\{1}", Environment.CurrentDirectory, _file.Name),
                    templateID = Guid.NewGuid(),
                    templateSize = _file.Length.ToString(CultureInfo.InvariantCulture),
                    templateName = _file.Name,
                    updatetime = DateTime.Now,
                    rowid = Guid.NewGuid()
                };
                var argListTmpe = ReportOfficeTools.GetArgInfoListFromWord(_templateInfo, out _tmpResult);
                if (_tmpResult != "")
                {
                    WorkWindow.ShowError(_tmpResult);
                }
                else
                {
                    _tmpResult = Tools.updateArgConfig(argListTmpe);
                    if (_tmpResult == "")
                    {
                        _templateInfo.templatePath = string.Format(@".\templateFiles\{0}", _file.Name);
                        _tmpResult = Tools.UpdateTemplateConfig(_templateInfo);
                    }
                }
                Dispatcher.Invoke(_analyzeFinished, _tmpResult);
            });
            task.Start();
        }
    }
}
