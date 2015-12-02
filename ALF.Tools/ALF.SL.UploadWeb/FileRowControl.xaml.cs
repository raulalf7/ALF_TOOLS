using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ALF.SL.UploadWeb.DataModel;

namespace ALF.SL.UploadWeb
{
    public partial class FileRowControl
    {
        public FileRowControl()
        {
            InitializeComponent();

            Loaded += FileRowControl_Loaded;
        }

        private UserFile UserFile
        {
            get
            {
                if (DataContext != null)
                    return ((UserFile) DataContext);
                return null;
            }
        }

        private void FileRowControl_Loaded(object sender, RoutedEventArgs e)
        {
            //定制UserFile的PropertyChanged 属性，如BytesUploaded，Percentage，IsDeleted
            UserFile.PropertyChanged += FileRowControl_PropertyChanged;
        }

        private void FileRowControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "State") return;
            //当前文件上传完毕后显示灰字
            if (UserFile.State == Constants.FileStates.Finished)
            {
                GreyOutText();
                ShowValidIcon();
            }

            //如上传失败显示错误信息
            if (UserFile.State == Constants.FileStates.Error)
            {
                ErrorMsgTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ShowValidIcon()
        {
            percentageProgress.Visibility = Visibility.Collapsed;
            validUploadIcon.Visibility = Visibility.Visible;
        }

        private void GreyOutText()
        {
            var grayBrush = new SolidColorBrush(Colors.Gray);

            fileNameTextBlock.Foreground = grayBrush;
            stateTextBlock.Foreground = grayBrush;
            fileSizeTextBlock.Foreground = grayBrush;
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var file = (UserFile) ((TextBlock) e.OriginalSource).DataContext;
            file.IsDeleted = true;

            Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var file = (UserFile) ((Button) e.OriginalSource).DataContext;
            file.IsDeleted = true;

            Visibility = Visibility.Collapsed;
        }
    }

}