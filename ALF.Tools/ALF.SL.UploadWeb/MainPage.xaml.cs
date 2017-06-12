using System;
using System.Windows;
using System.Windows.Controls;
using ALF.SILVERLIGHT;
using ALF.SILVERLIGHT.DataModel;
using Enum = ALF.SILVERLIGHT.DataModel.Enum;

namespace ALF.SL.UploadWeb
{
    public partial class MainPage
    {
        private readonly UploadFileCollection _files;
        private readonly string _fileFilter;
        private int _maxFileSize = int.MaxValue;

        public MainPage(string fileFilter)
        {
            _fileFilter = fileFilter;
            InitializeComponent();

            _files = new UploadFileCollection();
            fileList.ItemsSource = _files;
            filesCount.DataContext = _files;
            totalProgress.DataContext = _files;
            totalKb.DataContext = _files;
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
                var userFile = new UploadFile
                {
                    FilePhysicalName =  "cccc"+file.Name,
                    FileStream = file.OpenRead(),
                   // FilelName=file.Name
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
                foreach (var file in _files)
                {

                    if (!file.IsDeleted && file.State == Enum.UploadStates.等待上传)
                    {
                        var fileUploader = new UploadTools(file, "http://localhost:56696/SilverlightUploadService.svc");
                        //var fileUploader = new UploadTools(file, "http://172.21.28.50/Upload/SilverlightUploadService.svc");
                        fileUploader.UploadAdvanced();
                    }
                }
                //_files.UploadFiles();
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