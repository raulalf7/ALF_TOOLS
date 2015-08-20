using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Alf7.Tools.Library;


namespace DataCheck_XP.Control
{
    /// <summary>
    /// WordControl.xaml 的交互逻辑
    /// </summary>
    public partial class WordControl
    {
        public WordControl()
        {
            InitializeComponent();
        }

        private FileInfo _selectedFile;


        public void load(bool isInitialAction)
        {
            var dir = new DirectoryInfo(@".\reportFiles");
            Action initial = () =>
            {
                docListbox.ItemsSource = null;
                docListbox.ItemsSource = dir.GetFiles("*.docx");
                docListbox.Focus();
            };
            Dispatcher.Invoke(initial);

        }

        private void delButton_Click(object sender, EventArgs e)
        {

            _selectedFile = docListbox.SelectedItem as FileInfo;
            if (_selectedFile == null)
            {
                return;
            }
            var test = WorkWindow.showDialog("删除文件", "是否确定删除所选文件？");

            if (test == MessageBoxResult.Cancel)
            {
                return;
            }
            try
            {
                _selectedFile.Delete();
            }
            catch (Exception ex)
            {
                SystemTools.showError(999, ex.Message);
            }
            load(false);
        }
        private void DocListbox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            _selectedFile = docListbox.SelectedItem as FileInfo;

        }

        private void openButton_Click(object sender, EventArgs e)
        {
            _selectedFile = docListbox.SelectedItem as FileInfo;
            if (_selectedFile == null)
            {
                return;
            }
            SystemTools.execCmd(_selectedFile.FullName, "");

        }
    }
}
