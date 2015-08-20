using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DataCheck.Control
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

        private Action<Task<MessageDialogResult>> _action;
        private FileInfo _selectedFile;


        public void Load(bool isInitialAction)
        {
            var dir = new DirectoryInfo(@".\reportFiles");
            Action initial = () =>
            {
                docListbox.ItemsSource = null;
                docListbox.ItemsSource = dir.GetFiles("*.docx");
                docListbox.Focus();
            };
            Dispatcher.Invoke(initial);

            if (isInitialAction)
            {
                _action = o =>
                    {
                        if (o.Result == MessageDialogResult.Negative)
                        {
                            return;
                        }
                        try
                        {
                            _selectedFile.Delete();
                        }
                        catch (Exception e)
                        {
                            Tools.showError(999,e.Message);
                        }
                        Load(false);
                    };
            }
        }

        private void delButton_Click(object sender, EventArgs e)
        {

            _selectedFile = docListbox.SelectedItem as FileInfo;
            if (_selectedFile == null)
            {
                return;
            }
            var test = WorkWindow.showDialog("删除文件", "是否确定删除所选文件？");
            if(_action!=null)
            {
                test.ContinueWith(_action);
            }
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
            ALF.SYSTEM.WindowsTools.ExecCmd(_selectedFile.FullName, "");

        }
    }
}
