using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ALF.SL.UploadWeb.DataModel;

namespace ALF.SL.UploadWeb
{
    public partial class MainPage
    {
        private readonly FileCollection _files;
        private string _customParams;
        private string _fileFilter;
        private int _maxFileSize = int.MaxValue;
        private int _maxUpload = 2;

        public MainPage(IDictionary<string, string> initParams)
        {
            InitializeComponent();

            LoadConfiguration(initParams);

            _files = new FileCollection(_customParams, _maxUpload);
            fileList.ItemsSource = _files;
            filesCount.DataContext = _files;
            totalProgress.DataContext = _files;
            totalKb.DataContext = _files;
        }

        /// <summary>
        ///     加载配置参数 then from .Config file
        /// </summary>
        /// <param name="initParams"></param>
        private void LoadConfiguration(IDictionary<string, string> initParams)
        {
            //加载定制配置信息串
            if (initParams.ContainsKey("CustomParam") && !string.IsNullOrEmpty(initParams["CustomParam"]))
                _customParams = initParams["CustomParam"];

            if (initParams.ContainsKey("MaxUploads") && !string.IsNullOrEmpty(initParams["MaxUploads"]))
            {
                int.TryParse(initParams["MaxUploads"], out _maxUpload);
            }

            if (initParams.ContainsKey("MaxFileSizeKB") && !string.IsNullOrEmpty(initParams["MaxFileSizeKB"]))
            {
                if (int.TryParse(initParams["MaxFileSizeKB"], out _maxFileSize))
                    _maxFileSize = _maxFileSize*1024;
            }

            if (initParams.ContainsKey("FileFilter") && !string.IsNullOrEmpty(initParams["FileFilter"]))
                _fileFilter = initParams["FileFilter"];

            _maxFileSize = 2000000*1024;
            _maxUpload = 5;

            ////从配置文件中获取相关信息
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MaxFileSizeKB"]))
            //{
            //    if (int.TryParse(ConfigurationManager.AppSettings["MaxFileSizeKB"], out _maxFileSize))
            //        _maxFileSize = _maxFileSize * 1024;
            //}


            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MaxUploads"]))
            //    int.TryParse(ConfigurationManager.AppSettings["MaxUploads"], out _maxUpload);

            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FileFilter"]))
            //    _fileFilter = ConfigurationManager.AppSettings["FileFilter"];
        }

        /// <summary>
        ///     选择文件对话框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog {Multiselect = true};

            try
            {
                if (!string.IsNullOrEmpty(_fileFilter))
                    ofd.Filter = _fileFilter;
            }
            catch (ArgumentException ex)
            {
                //User supplied a wrong configuration file
                throw new Exception("Wrong file filter configuration.", ex);
            }

            if (ofd.ShowDialog() != true) return;
            foreach (var file in ofd.Files)
            {
                var userFile = new UserFile
                {
                    FileName = file.Name,
                    FileStream = file.OpenRead()
                };


                if (userFile.FileStream.Length <= _maxFileSize)
                {
                    //向文件列表中添加文件信息
                    _files.Add(userFile);
                }
                else
                {
                    MessageBox.Show( "Maximum file size is: " + (_maxFileSize/1024) + "KB.");
                }
            }
        }

        /// <summary>
        ///     开始上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_files.Count == 0)
            {
                MessageBox.Show("No files to upload. Please select one or more files first.");
            }
            else
            {
                _files.UploadFiles();
            }
        }

        /// <summary>
        ///     清空上传文件列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _files.Clear();
        }
    }


}