using System;
using System.Globalization;
using System.Windows.Data;

namespace ALF.SL.UploadWeb.DataModel
{
    public class ByteConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = "0 KB";

            if (value != null)
            {
                var byteCount = (double) value;

                if (byteCount >= 1073741824)
                    size = string.Format("{0:##.##}", byteCount/1073741824) + " GB";
                else if (byteCount >= 1048576)
                    size = string.Format("{0:##.##}", byteCount/1048576) + " MB";
                else if (byteCount >= 1024)
                    size = string.Format("{0:##.##}", byteCount/1024) + " KB";
                else if (byteCount > 0 && byteCount < 1024)
                    size = "1 KB"; //Bytes are unimportant ;)            
            }

            return size;
        }

        //We only use one-way binding, so we don't implement this.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}